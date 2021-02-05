using EventCoordinator.ProcessManager.Interface;
using EventCoordinator.ProcessManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCoordinator.ProcessManager.Impl
{
    /// <summary>
    /// 所有参与流程的事件处理器实现必须继承该抽象类型
    /// </summary>
    public abstract class EventProcessHandlerBase
    {
        public EventProcessPiplineStateModel processCache;
        /// <summary>
        /// 事件处理器执行成功，通知管理器发布下一个事件
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CallNext<TEvent>(TEvent input) where TEvent : EventProcessBase
        {
            processCache.ProcessState = ProcessStateEnum.Normal;
            processCache.NextEvent = input;
            await Task.CompletedTask;
        }
        /// <summary>
        /// 事件处理器执行失败，通知管理器开始回滚
        /// </summary>
        /// <returns></returns>
        public async Task RollBack()
        {
            //若事件调用回滚，则通知管理器触发回滚
            if (processCache.ProcessState == ProcessStateEnum.Normal)
                processCache.ProcessState = ProcessStateEnum.RollBack;
            //若补偿调用回滚，则通知管理器回滚失败，流程结束
            else if (processCache.ProcessState == ProcessStateEnum.RollBack)
                processCache.ProcessState = ProcessStateEnum.RollBackError;
            processCache.NextEvent = null;
            await Task.CompletedTask;
        }
    }
}
