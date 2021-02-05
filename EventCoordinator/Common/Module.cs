using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using EventCoordinator.Event.Impl;
using EventCoordinator.Event.Interface;
using EventCoordinator.ProcessManager.Impl;
using EventCoordinator.ProcessManager.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCoordinator.Common
{
    public class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EventBus>().As<IEventBus>().InstancePerLifetimeScope();
            builder.RegisterType<SubscribeProxyFactory>().As<ISubscribeProxyFactory>().InstancePerLifetimeScope();
            builder.RegisterType<EventProcessManager>().As<IProcessManager>().InstancePerLifetimeScope();
        }
    }
}
