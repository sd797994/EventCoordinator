using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using EventCoordinator.ProcessManager.Model;
using Microsoft.Extensions.Logging;
using EventCoordinator.ProcessManager.Interface;

namespace EventCoordinator.ProcessManager.Impl
{
    /// <summary>
    /// 事件流程管理器管道
    /// </summary>
    public class EventProcessManagerPipline
    {
        /// <summary>
        /// 管道事件
        /// </summary>
        private ConcurrentQueue<ProcessConfigureBase> Processes { get; set; } = new ConcurrentQueue<ProcessConfigureBase>();
        /// <summary>
        /// 管道补偿
        /// </summary>
        private ConcurrentDictionary<string, ConcurrentStack<ProcessConfigureBase>> CallbackList { get; set; } = new ConcurrentDictionary<string, ConcurrentStack<ProcessConfigureBase>>();
        private readonly ILogger logger;
        public string Key { get; set; }
        private string lastProcessesKey = "";
        /// <summary>
        /// 管道状态临时存储
        /// </summary>
        private EventProcessPiplineStateModel processCache;
        public EventProcessManagerPipline(ILogger logger) { this.logger = logger; }
        public EventProcessManagerPipline(ILogger logger, EventProcessManagerPipline pipline)
        {
            this.logger = logger;
            this.Key = pipline.Key;
            this.Processes = new ConcurrentQueue<ProcessConfigureBase>();
            foreach (var item in pipline.GetProcessee())
            {
                Processes.Enqueue(item);
            };
            this.CallbackList = new ConcurrentDictionary<string, ConcurrentStack<ProcessConfigureBase>>();
            foreach (var item in pipline.GetCallbackList())
            {
                var callbacklist = new List<ProcessConfigureBase>();
                var callbackstack = new ConcurrentStack<ProcessConfigureBase>();
                foreach (var child in item.Value)
                {
                    callbacklist.Add(child);
                }
                callbacklist.Reverse();//由于是从栈复制到栈，所以这里需要进行一次反转
                callbacklist.ForEach(x => callbackstack.Push(x));
                CallbackList.TryAdd(item.Key, callbackstack);
            }
        }
        /// <summary>
        /// 创建一个流程配置，并注册到管道事件
        /// </summary>
        /// <param name="process"></param>
        /// <param name="callback"></param>
        public void SetProcess(ProcessConfigureBase process, ProcessConfigureBase callback = null)
        {
            process.Key = Guid.NewGuid().ToString();
            Processes.Enqueue(process);
            var thiscallback = new ConcurrentStack<ProcessConfigureBase>();
            if (CallbackList.TryGetValue(lastProcessesKey, out ConcurrentStack<ProcessConfigureBase> lastcallback))
            {
                foreach (var item in lastcallback)
                {
                    thiscallback.Push(item);
                }
            }
            lastProcessesKey = process.Key;
            if (callback != null)
            {
                thiscallback.Push(callback);
            }
            CallbackList.TryAdd(lastProcessesKey, thiscallback);
        }
        /// <summary>
        /// 从管道事件中弹出下一个事件
        /// </summary>
        /// <returns></returns>
        public ConcurrentQueue<ProcessConfigureBase> GetProcessee()
        {
            return Processes;
        }
        /// <summary>
        /// 获取所有补偿事件
        /// </summary>
        /// <returns></returns>
        public ConcurrentDictionary<string, ConcurrentStack<ProcessConfigureBase>> GetCallbackList()
        {
            return CallbackList;
        }
        /// <summary>
        /// 根据流程配置获取其对应的所有补偿事件
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public ConcurrentStack<ProcessConfigureBase> GetProcessCallbackList(ProcessConfigureBase process)
        {
            if (process != null && CallbackList.TryGetValue(process.Key, out ConcurrentStack<ProcessConfigureBase> thisCallbackList))
                return thisCallbackList;
            else
                return new ConcurrentStack<ProcessConfigureBase>();
        }
        /// <summary>
        /// 启动当前管道
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task Start<TEvent>(TEvent input) where TEvent : EventProcessBase
        {
            //初始化状态
            processCache = new EventProcessPiplineStateModel();
            //当前处理器
            ProcessConfigureBase currentProcess = default;
            //当前事件
            dynamic currentEvent = input;
            while (true)
            {
                switch (processCache.ProcessState)
                {
                    case ProcessStateEnum.Normal:
                        if (GetProcessee().TryDequeue(out ProcessConfigureBase process))
                        {
                            await process.Excute(processCache, currentEvent);
                            processCache.AutoResetEvent.WaitOne();
                            //若未进行任何操作且调用链并未执行完毕，则会自动触发RollBack。
                            if (processCache.NextEvent == null)
                            {
                                if (processCache.ProcessState != ProcessStateEnum.RollBack)
                                {
                                    if (GetProcessee().TryPeek(out _))
                                    {
                                        processCache.ProcessState = ProcessStateEnum.RollBack;
                                    }
                                    else
                                    {
                                        processCache.ProcessState = ProcessStateEnum.End;
                                    }
                                }
                                else
                                {
                                    _ = processCache.PopRollBackParam();//由于本次执行失败，强制弹出本次插入的数据
                                }
                            }
                            else
                            {
                                currentEvent = processCache.NextEvent;
                                currentProcess = process;
                                processCache.SetRollbackParams(currentEvent);
                            }
                        }
                        else
                        {
                            processCache.ProcessState = ProcessStateEnum.End;
                        }
                        break;
                    case ProcessStateEnum.RollBack:
                        if (GetProcessCallbackList(currentProcess).TryPop(out ProcessConfigureBase callback))
                        {
                            currentEvent = processCache.PopRollBackParam();
                            await callback.Excute(processCache, currentEvent);
                            processCache.AutoResetEvent.WaitOne();
                            //只要没有触发RollBackError且回滚列表未回滚完毕，则继续执行RollBack
                            if (processCache.ProcessState != ProcessStateEnum.RollBackError)
                            {
                                processCache.ProcessState = ProcessStateEnum.RollBack;
                            }
                        }
                        else
                        {
                            processCache.ProcessState = ProcessStateEnum.RollBackEnd;
                        }
                        break;
                    case ProcessStateEnum.End:
                        //流程结束
                        logger.LogInformation("流程执行成功，打印结束日志!");
                        return;
                    case ProcessStateEnum.RollBackEnd:
                        //流程结束
                        logger.LogWarning("流程执行失败，回滚全部成功，打印结束日志!");
                        return;
                    case ProcessStateEnum.RollBackError:
                        //流程结束
                        logger.LogError("流程执行失败，回滚异常，打印结束日志!");
                        return;
                }
            }
        }
    }
}
