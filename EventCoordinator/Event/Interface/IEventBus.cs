using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventCoordinator.Event.Interface
{
    /// <summary>
    /// 事件总线
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// 发送事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        Task Send<T>(string topic, T @event);
    }
}
