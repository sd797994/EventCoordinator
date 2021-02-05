using Autofac;
using EventCoordinator.Event.Interface;
using EventCoordinator.Event.Model;
using EventCoordinator.ProcessManager.Interface;
using EventCoordinator.ProcessManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EventCoordinator.ProcessManager.Impl
{
    /// <summary>
    /// 管道配置构造器
    /// </summary>
    public static class ProcessConcifgureBuilder
    {
        private static Type ProcessConfigureType = typeof(ProcessConfigure<,,>);
        /// <summary>
        /// 构造一个管道配置
        /// </summary>
        /// <typeparam name="Tservice"></typeparam>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="lifetimeScope"></param>
        /// <param name="subscribeFactory"></param>
        /// <param name="eventBus"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static ProcessConfigureBase Build<Tservice, TEvent>(ILifetimeScope lifetimeScope, ISubscribeProxyFactory subscribeFactory, IEventBus eventBus, Func<TEvent, Task> method) where Tservice : class where TEvent : EventProcessBase
        {
            //获取委托订阅器的订阅主题
            var topic = method.Method.GetCustomAttribute<EventHandlerAttribute>().Topic;
            //构造配置类型的泛型
            var processconfigure = ProcessConfigureType.MakeGenericType(typeof(Tservice), method.Method.DeclaringType, typeof(TEvent));
            //创建配置实例
            var instance = (ProcessConfigureBase)Activator.CreateInstance(processconfigure, lifetimeScope, topic, eventBus, method);
            //将实例的代理委托注册到订阅器
            subscribeFactory.Register<EventProcessPacker<TEvent>>(topic, instance.HandleInvoke);
            return instance;
        }
    }
}
