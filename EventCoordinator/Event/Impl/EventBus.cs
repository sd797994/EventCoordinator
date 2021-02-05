using EventCoordinator.Event.Model;
using EventCoordinator.Event.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventCoordinator.Event.Impl
{
    /// <summary>
    /// 事件总线实现
    /// </summary>
    public class EventBus : IEventBus
    {
        /// <summary>
        /// 发送事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public async Task Send<T>(string topic, T @event)
        {
            //获取代理工厂的channel实例，发送事件
            if (SubscribeProxyFactory.PiplineList.TryGetValue(topic, out SubscribeProxyBase pipline))
            {
                await (pipline as SubscribeProxy<T>).Pipline.Writer.WriteAsync(@event);
            }
            else
            {
                throw new Exception($"{topic}没有注册过!");
            }
        }
    }
}
