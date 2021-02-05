using Autofac;
using EventCoordinator.Common;
using EventCoordinator.Event.Interface;
using EventCoordinator.ProcessManager.Interface;
using EventCoordinator.ProcessManager.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace EventCoordinator.ProcessManager.Impl
{
    /// <summary>
    /// 流程管理器实现
    /// </summary>
    public class EventProcessManager : IProcessManager
    {
        /*
        * 流程管理器工作逻辑如下
        * 流程管理器包含多个注册的事件流程处理管道，每个管道包含所有事件处理方法的委托。
        * 流程启动时，管理器初始化一个管道实例，并调用其start方法启动一个流程管道实例
        * 管道实例会启动一个while循环来轮询发布事件/发布补偿
        * 事件处理器逻辑：
        * 管道实例发布事件(异步等待)——事件的处理委托收到事件进行消费——根据处理结果调用base.CallNext() 或者base.RollBack()
        * 若调用CallNext则通知管道事件处理成功，可以执行下一步
        * 若调用RollBack则通知管道事件处理失败，可以执行补偿
        * 若未调用以上两者且管道并未执行完毕则会通知管道事件执行补偿
        * 若所有管道事件，则整个流程成功结束打印结束日志。
        * 若所有管道补偿事件执行完毕，则整个流程失败补偿成功打印结束日志。
        * 补偿事件委托消费时若主动调用RollBack，则整个流程失败补偿失败打印结束日志。
        */
        /// <summary>
        /// 注册到管理器的所有管道
        /// </summary>
        public static List<EventProcessManagerPipline> piplines = new List<EventProcessManagerPipline>();

        private readonly ILifetimeScope lifetimeScope;
        /// <summary>
        /// 当前正在进行注册的管道
        /// </summary>
        private EventProcessManagerPipline currentPipline;
        /// <summary>
        /// 事件相关类型，用于流程管道订阅/发布事件
        /// </summary>
        private readonly IEventBus eventBus;
        private readonly ISubscribeProxyFactory subscribeFactory;
        private readonly ILogger<EventProcessManager> logger;
        public EventProcessManager(ILogger<EventProcessManager> logger, ILifetimeScope lifetimeScope, IEventBus eventBus, ISubscribeProxyFactory subscribeFactory)
        {
            this.logger = logger;
            this.lifetimeScope = lifetimeScope;
            this.eventBus = eventBus;
            this.subscribeFactory = subscribeFactory;
        }
        /// <summary>
        /// 注册一个流程管理器实例
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IProcessManager ResgiterProcess(string name)
        {
            if (string.IsNullOrEmpty(name) || piplines.Any(x => name == x.Key))
            {
                throw new EventCoordinatorException($"流程{name}注册失败");
            }
            else
            {
                currentPipline = new EventProcessManagerPipline(logger);
                currentPipline.Key = name;
            }
            return this;
        }
        /// <summary>
        /// 将事务协调流程管理器写入静态字典
        /// </summary>
        public void BuildProcess()
        {
            if (currentPipline == null)
                throw new EventCoordinatorException("配置流程失败!");
            else if (!currentPipline.GetProcessee().Any())
                throw new EventCoordinatorException("配置流程失败!");
            else
                piplines.Add(currentPipline);
        }
        /// <summary>
        /// 为该流程管理器实例注册事件处理委托
        /// </summary>
        /// <typeparam name="Tservice"></typeparam>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventhandle"></param>
        /// <returns></returns>
        public IProcessManager ResgiterPipline<Tservice, TEvent>(Func<TEvent, Task> eventhandle) where Tservice : class where TEvent : EventProcessBase
        {
            if (currentPipline != null)
            {
                currentPipline.SetProcess(ProcessConcifgureBuilder.Build<Tservice, TEvent>(lifetimeScope, subscribeFactory, eventBus, eventhandle));
            }
            return this;
        }
        /// <summary>
        /// 为该流程管理器实例注册事件处理委托和委托对应的补偿
        /// </summary>
        /// <typeparam name="Tservice"></typeparam>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventhandle"></param>
        /// <param name="callbackEventhandle"></param>
        /// <returns></returns>
        public IProcessManager ResgiterPipline<Tservice, TEvent>(Func<TEvent, Task> eventhandle, Func<TEvent, Task> callbackEventhandle) where Tservice : class where TEvent : EventProcessBase
        {
            if (currentPipline != null)
            {
                currentPipline.SetProcess(ProcessConcifgureBuilder.Build<Tservice, TEvent>(lifetimeScope, subscribeFactory, eventBus, eventhandle),
                    ProcessConcifgureBuilder.Build<Tservice, TEvent>(lifetimeScope, subscribeFactory, eventBus, callbackEventhandle));
            }
            return this;
        }
        /// <summary>
        /// 获取并开始执行一个流程
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="name"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task GetAndRun<TEvent>(string name, TEvent input) where TEvent : EventProcessBase
        {
            var config = piplines.FirstOrDefault(x => x.Key == name);
            if (config != null)
            {
                var instance = new EventProcessManagerPipline(logger, config);
                await instance.Start(input);
            }
            else
            {
                throw new EventCoordinatorException($"没有查询到流程{name}!");
            }
        }
    }
}
