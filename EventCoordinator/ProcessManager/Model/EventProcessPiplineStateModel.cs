using EventCoordinator.ProcessManager.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventCoordinator.ProcessManager.Model
{
    /// <summary>
    /// 事件处理器管道状态类
    /// </summary>
    public class EventProcessPiplineStateModel
    {
        public EventProcessPiplineStateModel()
        {
            AutoResetEvent = new AutoResetEvent(false);
            ProcessState = ProcessStateEnum.Normal;
            RollbackParams = new ConcurrentStack<EventProcessBase>();
        }
        public AutoResetEvent AutoResetEvent { get; set; }
        public ProcessStateEnum ProcessState { get; set; }
        public EventProcessBase NextEvent { get; set; }
        public ConcurrentStack<EventProcessBase> RollbackParams { get; set; }

        public void SetRollbackParams(EventProcessBase input)
        {
            RollbackParams.Push(input);
        }
        public EventProcessBase PopRollBackParam()
        {
            if (RollbackParams.TryPop(out EventProcessBase result))
            {
                return result;
            }
            return default;
        }
    }
    /// <summary>
    /// 状态枚举
    /// </summary>
    public enum ProcessStateEnum
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 回滚
        /// </summary>
        RollBack = 1,
        /// <summary>
        /// 正常结束
        /// </summary>
        End = 2,
        /// <summary>
        /// 回滚结束
        /// </summary>
        RollBackEnd = 3,
        /// <summary>
        /// 回滚异常
        /// </summary>
        RollBackError = -1,
    }
}
