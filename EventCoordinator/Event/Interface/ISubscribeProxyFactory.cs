using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventCoordinator.Event.Interface
{
    /// <summary>
    /// 订阅代理工厂接口
    /// </summary>
    public interface ISubscribeProxyFactory
    {
        /// <summary>
        /// 将委托和topic注册一个订阅代理实现，并存储到本地字典中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="subscribeHandler"></param>
        /// <returns></returns>
        bool Register<T>(string topic, Func<T, Task> subscribeHandler);
        /// <summary>
        /// 取消topic注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        bool UnRegister<T>(string topic);
    }
}
