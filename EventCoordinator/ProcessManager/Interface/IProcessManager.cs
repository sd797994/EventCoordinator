using EventCoordinator.ProcessManager.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventCoordinator.ProcessManager.Interface
{
    /// <summary>
    /// 事务协调流程管理器
    /// </summary>
    public interface IProcessManager
    {
        /// <summary>
        /// 注册一个流程管理器实例
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IProcessManager ResgiterProcess(string name);
        /// <summary>
        /// 为该流程管理器实例注册事件处理委托
        /// </summary>
        /// <typeparam name="Tservice"></typeparam>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventhandle"></param>
        /// <returns></returns>
        IProcessManager ResgiterPipline<Tservice, TEvent>(Func<TEvent, Task> eventhandle) where Tservice : class where TEvent : EventProcessBase;
        /// <summary>
        /// 为该流程管理器实例注册事件处理委托和委托对应的补偿
        /// </summary>
        /// <typeparam name="Tservice"></typeparam>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventhandle"></param>
        /// <param name="callbackeventhandle"></param>
        /// <returns></returns>
        IProcessManager ResgiterPipline<Tservice, TEvent>(Func<TEvent, Task> eventhandle, Func<TEvent, Task> callbackeventhandle) where Tservice : class where TEvent : EventProcessBase;
        /// <summary>
        /// 将事务协调流程管理器写入静态字典
        /// </summary>
        void BuildProcess();
        /// <summary>
        /// 获取并开始执行一个流程
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="name"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task GetAndRun<TEvent>(string name, TEvent input) where TEvent : EventProcessBase;
    }
}
