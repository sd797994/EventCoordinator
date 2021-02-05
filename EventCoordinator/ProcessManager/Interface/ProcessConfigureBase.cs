using EventCoordinator.ProcessManager.Impl;
using EventCoordinator.ProcessManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCoordinator.ProcessManager.Interface
{
    /// <summary>
    /// 管道配置基类
    /// </summary>
    public abstract class ProcessConfigureBase
    {
        public string Key { get; set; }
        public abstract Task Excute<TEvent>(EventProcessPiplineStateModel processCache, TEvent input) where TEvent : EventProcessBase;
        public abstract Task HandleInvoke<T>(EventProcessPacker<T> data) where T : EventProcessBase;
    }
}