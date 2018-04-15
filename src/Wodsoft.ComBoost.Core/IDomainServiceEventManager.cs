using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域服务区事件管理器接口。
    /// </summary>
    public interface IDomainServiceEventManager
    {
        #region 关联事件

        /// <summary>
        /// 添加事件处理器。
        /// </summary>
        /// <typeparam name="T">事件参数类型。</typeparam>
        /// <param name="route">事件路由。</param>
        /// <param name="handler">事件处理器。</param>
        void AddEventHandler<T>(DomainServiceEventRoute route, DomainServiceEventHandler<T> handler) where T : DomainServiceEventArgs;

        /// <summary>
        /// 添加事件处理器。
        /// </summary>
        /// <typeparam name="T">事件参数类型。</typeparam>
        /// <param name="route">事件路由。</param>
        /// <param name="handler">事件处理器。</param>
        void AddAsyncEventHandler<T>(DomainServiceEventRoute route, DomainServiceAsyncEventHandler<T> handler) where T : DomainServiceEventArgs;

        /// <summary>
        /// 移除事件处理器。
        /// </summary>
        /// <typeparam name="T">事件参数类型。</typeparam>
        /// <param name="route">事件路由。</param>
        /// <param name="handler">事件处理器。</param>
        void RemoveEventHandler<T>(DomainServiceEventRoute route, DomainServiceEventHandler<T> handler) where T : DomainServiceEventArgs;

        /// <summary>
        /// 移除事件处理器。
        /// </summary>
        /// <typeparam name="T">事件参数类型。</typeparam>
        /// <param name="route">事件路由。</param>
        /// <param name="handler">事件处理器。</param>
        void RemoveAsyncEventHandler<T>(DomainServiceEventRoute route, DomainServiceAsyncEventHandler<T> handler) where T : DomainServiceEventArgs;

        #endregion

        #region 引发事件

        /// <summary>
        /// 引发事件。
        /// </summary>
        /// <typeparam name="T">事件参数类型。</typeparam>
        /// <param name="route">事件路由。</param>
        /// <param name="context">领域执行上下文。</param>
        /// <param name="eventArgs">事件参数。</param>
        void RaiseEvent<T>(DomainServiceEventRoute route, IDomainExecutionContext context, T eventArgs) where T : DomainServiceEventArgs;

        /// <summary>
        /// 引发异步事件。
        /// </summary>
        /// <typeparam name="T">事件参数类型。</typeparam>
        /// <param name="route">事件路由。</param>
        /// <param name="context">领域执行上下文。</param>
        /// <param name="eventArgs">事件参数。</param>
        Task RaiseAsyncEvent<T>(DomainServiceEventRoute route, IDomainExecutionContext context, T eventArgs) where T : DomainServiceEventArgs;

        #endregion
    }
}
