using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace EventCoordinator.Event.Model
{
    /// <summary>
    /// 订阅代理基类
    /// </summary>
    public abstract class SubscribeProxyBase
    {
        public virtual Channel<object> Pipline { get; set; }
        public virtual Func<object, Task> SubscribeHandler { get; set; }
    }
}
