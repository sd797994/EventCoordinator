using EventCoordinator.Event.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace EventCoordinator.Event.Impl
{
    /// <summary>
    /// 订阅代理实现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SubscribeProxy<T> : SubscribeProxyBase
    {
        public SubscribeProxy() { }
        /// <summary>
        /// 初始化一个订阅代理
        /// </summary>
        /// <param name="subscribeHandler"></param>
        public SubscribeProxy(Func<T, Task> subscribeHandler)
        {
            this.Pipline = Channel.CreateUnbounded<T>();//注册一个无限的Channel实例用于发送和接受事件
            this.SubscribeHandler = subscribeHandler;//传递订阅委托
            _ = EventInvoker();//异步启动事件处理器, 等待接受消息
        }
        /// <summary>
        /// 事件处理器
        /// </summary>
        /// <returns></returns>
        async Task EventInvoker()
        {
            await foreach (var @event in Pipline.Reader.ReadAllAsync())
            {
                //收到事件后调用委托进行具体的业务处理
                await SubscribeHandler(@event);
            }
        }
        public new Channel<T> Pipline { get; set; }
        public new Func<T, Task> SubscribeHandler { get; set; }
    }
}
