using EventCoordinator.Event.Model;
using EventCoordinator.Event.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCoordinator.Event.Impl
{
    /// <summary>
    /// 订阅代理工厂实现
    /// </summary>
    public class SubscribeProxyFactory : ISubscribeProxyFactory
    {
        public static ConcurrentDictionary<string, SubscribeProxyBase> PiplineList = new ConcurrentDictionary<string, SubscribeProxyBase>();
        /// <summary>
        /// 将委托和topic注册一个订阅代理实现，并存储到本地字典中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="topic"></param>
        /// <param name="subscribeHandler"></param>
        /// <returns></returns>
        public bool Register<T>(string topic, Func<T, Task> subscribeHandler)
        {
            if (PiplineList.Keys.Any(x => x == topic))
                throw new Exception($"topic:{topic}已经注册过了!");
            else
                return PiplineList.TryAdd(topic, new SubscribeProxy<T>(subscribeHandler));
        }
        /// <summary>
        /// 取消topic注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="topic"></param>
        /// <returns></returns>
        public bool UnRegister<T>(string topic)
        {
            return PiplineList.TryRemove(topic, out _);
        }
    }
}
