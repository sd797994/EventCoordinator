using System;
using System.Collections.Generic;
using System.Text;

namespace EventCoordinator.Event.Model
{
    /// <summary>
    /// 事件处理器都需要注解该特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EventHandlerAttribute: Attribute
    {
        /// <summary>
        /// 订阅主题
        /// </summary>
        public string Topic { get; set; }
        public EventHandlerAttribute(string topic)
        {
            this.Topic = topic;
        }
    }
}
