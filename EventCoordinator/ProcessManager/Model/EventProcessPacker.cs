using EventCoordinator.ProcessManager.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCoordinator.ProcessManager.Model
{
    /// <summary>
    /// 流程管理器事件包装器
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public class EventProcessPacker<TEvent> where TEvent : EventProcessBase
    {
        public EventProcessPiplineStateModel processCache { get; set; }
        public TEvent Input { get; set; }
    }
}
