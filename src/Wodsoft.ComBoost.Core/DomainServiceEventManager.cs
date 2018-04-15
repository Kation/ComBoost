﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域服务区事件管理器。
    /// </summary>
    public class DomainServiceEventManager : IDomainServiceEventManager, IDisposable
    {
        ///// <summary>
        ///// 获取全局领域服务事件管理器。
        ///// </summary>
        //public static DomainServiceEventManager GlobalEventManager { get; }

        //static DomainServiceEventManager()
        //{
        //    GlobalEventManager = new DomainServiceEventManager();
        //}

        private Dictionary<DomainServiceEventRoute, Delegate> _Events;

        /// <summary>
        /// 实例化领域服务事件管理器。
        /// </summary>
        public DomainServiceEventManager()
        {
            _Events = new Dictionary<DomainServiceEventRoute, Delegate>();
        }

        /// <summary>
        /// 获取事件路由委托。
        /// </summary>
        /// <param name="route">事件路由。</param>
        /// <returns>返回事件委托。</returns>
        protected virtual Delegate GetEventRouteDelegate(DomainServiceEventRoute route)
        {
            _Events.TryGetValue(route, out var d);
            return d;
        }

        /// <summary>
        /// 设置事件路由委托。
        /// </summary>
        /// <param name="route">事件路由。</param>
        /// <param name="value">事件委托。</param>
        protected virtual void SetEventRouteDelegate(DomainServiceEventRoute route, Delegate value)
        {
            _Events[route] = value;
        }

        #region 关联事件

        /// <summary>
        /// 添加事件处理器。
        /// </summary>
        /// <param name="route">事件路由。</param>
        /// <param name="handler">事件处理器。</param>
        public virtual void AddEventHandler<T>(DomainServiceEventRoute route, DomainServiceEventHandler<T> handler)
            where T : DomainServiceEventArgs
        {
            if (route == null)
                throw new ArgumentNullException(nameof(route));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            if (route.HandlerType != typeof(DomainServiceEventHandler<T>))
                throw new InvalidCastException("事件路由处理器类型与传入的类型不符。");
            lock (_Events)
            {
                Delegate d = GetEventRouteDelegate(route);
                if (d == null)
                    SetEventRouteDelegate(route, handler);
                else
                    SetEventRouteDelegate(route, Delegate.Combine(d, handler));
            }
        }

        /// <summary>
        /// 添加事件处理器。
        /// </summary>
        /// <param name="route">事件路由。</param>
        /// <param name="handler">事件处理器。</param>
        public virtual void AddAsyncEventHandler<T>(DomainServiceEventRoute route, DomainServiceAsyncEventHandler<T> handler)
            where T : DomainServiceEventArgs
        {
            if (route == null)
                throw new ArgumentNullException(nameof(route));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            if (route.HandlerType != typeof(DomainServiceAsyncEventHandler<T>))
                throw new InvalidCastException("事件路由处理器类型与传入的类型不符。");
            lock (_Events)
            {
                Delegate d = GetEventRouteDelegate(route);
                if (d == null)
                    SetEventRouteDelegate(route, handler);
                else
                    SetEventRouteDelegate(route, Delegate.Combine(d, handler));
            }
        }

        /// <summary>
        /// 移除事件处理器。
        /// </summary>
        /// <param name="route">事件路由。</param>
        /// <param name="handler">事件处理器。</param>
        public virtual void RemoveEventHandler<T>(DomainServiceEventRoute route, DomainServiceEventHandler<T> handler)
            where T : DomainServiceEventArgs
        {
            if (route == null)
                throw new ArgumentNullException(nameof(route));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            if (route.HandlerType != typeof(DomainServiceEventHandler<T>))
                throw new InvalidCastException("事件路由处理器类型与传入的类型不符。");
            lock (_Events)
            {
                Delegate d = GetEventRouteDelegate(route);
                if (d == null)
                    SetEventRouteDelegate(route, handler);
                else
                    SetEventRouteDelegate(route, Delegate.Remove(d, handler));
            }
        }

        /// <summary>
        /// 移除事件处理器。
        /// </summary>
        /// <param name="route">事件路由。</param>
        /// <param name="handler">事件处理器。</param>
        public virtual void RemoveAsyncEventHandler<T>(DomainServiceEventRoute route, DomainServiceAsyncEventHandler<T> handler)
            where T : DomainServiceEventArgs
        {
            if (route == null)
                throw new ArgumentNullException(nameof(route));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            if (route.HandlerType != typeof(DomainServiceAsyncEventHandler<T>))
                throw new InvalidCastException("事件路由处理器类型与传入的类型不符。");
            lock (_Events)
            {
                Delegate d = GetEventRouteDelegate(route);
                if (d == null)
                    SetEventRouteDelegate(route, handler);
                else
                    SetEventRouteDelegate(route, Delegate.Remove(d, handler));
            }
        }

        #endregion

        #region 引发事件

        /// <summary>
        /// 引发事件。
        /// </summary>
        /// <param name="route">事件路由。</param>
        /// <param name="context">领域执行上下文。</param>
        /// <param name="eventArgs">事件参数。</param>
        public virtual void RaiseEvent<T>(DomainServiceEventRoute route, IDomainExecutionContext context, T eventArgs)
            where T : DomainServiceEventArgs
        {
            if (route == null)
                throw new ArgumentNullException(nameof(route));
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (!route.HandlerType.IsAssignableFrom(typeof(DomainServiceEventHandler<T>)))
                throw new InvalidCastException("事件路由处理器类型不符。");
            Delegate d = GetEventRouteDelegate(route);
            if (d != null)
                foreach (var item in d.GetInvocationList().Cast<DomainServiceEventHandler<T>>())
                {
                    item(context, eventArgs);
                    if (eventArgs.IsHandled)
                        break;
                }
            if (!eventArgs.IsHandled && route.ParentRoute != null)
                RaiseEvent(route.ParentRoute, context, eventArgs);
        }

        /// <summary>
        /// 引发异步事件。
        /// </summary>
        /// <param name="route">事件路由。</param>
        /// <param name="context">领域执行上下文。</param>
        /// <param name="eventArgs">事件参数。</param>
        public virtual async Task RaiseAsyncEvent<T>(DomainServiceEventRoute route, IDomainExecutionContext context, T eventArgs)
            where T : DomainServiceEventArgs
        {
            if (route == null)
                throw new ArgumentNullException(nameof(route));
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (!route.HandlerType.IsAssignableFrom(typeof(DomainServiceAsyncEventHandler<T>)))
                throw new InvalidCastException("事件路由处理器类型不符。");
            Delegate d = GetEventRouteDelegate(route);
            if (d != null)
                foreach (var item in d.GetInvocationList().Cast<DomainServiceAsyncEventHandler<T>>())
                {
                    await item(context, eventArgs);
                    if (eventArgs.IsHandled)
                        break;
                }
            if (route.ParentRoute != null)
                await RaiseAsyncEvent(route.ParentRoute, context, eventArgs);
        }

        public void Dispose()
        {
            Dispose(_Disposed);
        }

        private volatile bool _Disposed;
        protected void Dispose(bool disposed)
        {
            if (disposed)
                return;
            _Disposed = true;
            _Events.Clear();
            _Events = null;
        }

        #endregion
    }
}
