using Autofac;
using Autofac.Core.Lifetime;
using EventCoordinator.Common;
using EventCoordinator.Event.Model;
using EventCoordinator.Event.Interface;
using EventCoordinator.ProcessManager.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EventCoordinator.ProcessManager.Interface;

namespace EventCoordinator.ProcessManager.Impl
{
    /// <summary>
    /// 管道配置
    /// </summary>
    /// <typeparam name="Tservice"></typeparam>
    /// <typeparam name="Timpl"></typeparam>
    /// <typeparam name="TEvent"></typeparam>
    public class ProcessConfigure<Tservice, Timpl, TEvent> : ProcessConfigureBase where Timpl : class where TEvent : EventProcessBase
    {
        private readonly IEventBus eventBus;
        private readonly string topic;
        private readonly ILifetimeScope lifetimeScope;
        private readonly Func<Timpl, TEvent, Task> method;
        public ProcessConfigure(ILifetimeScope lifetimeScope, string topic, IEventBus eventBus, Func<TEvent, Task> method)
        {
            this.topic = topic;
            this.lifetimeScope = lifetimeScope;
            //重新构造委托的动态代理方法，避免依赖注入生命周期不一致
            this.method = DelegateHelper.CreateMethodDelegate<Timpl, TEvent, Task>(method.Method);
            this.eventBus = eventBus;
        }
        /// <summary>
        /// 发送事件代理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="processCache"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task Excute<T>(EventProcessPiplineStateModel processCache, T input)
        {
            await eventBus.Send(topic, new EventProcessPacker<T>() { processCache = processCache, Input = input });
        }
        /// <summary>
        /// 接受事件代理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public override async Task HandleInvoke<T>(EventProcessPacker<T> data)
        {
            //构造一个临时的IOC生命周期
            using var scope = lifetimeScope.BeginLifetimeScope();
            //通过Tservice构造委托对应的服务实例
            var serviceInstance = scope.Resolve<Tservice>() as EventProcessHandlerBase;
            //传递管道状态给该实例
            serviceInstance.processCache = data.processCache;
            //传递实例及事件给动态代理，执行其委托
            await method(serviceInstance as Timpl, data.Input as TEvent);
            //通知流程管理器管道可以执行下一步
            serviceInstance.processCache.AutoResetEvent.Set();
        }
    }
}
