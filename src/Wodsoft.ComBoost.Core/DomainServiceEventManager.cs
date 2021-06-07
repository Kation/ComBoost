using System;
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

        private Dictionary<Type, Delegate> _Events;

        /// <summary>
        /// 实例化领域服务事件管理器。
        /// </summary>
        public DomainServiceEventManager()
        {
            _Events = new Dictionary<Type, Delegate>();
        }

        /// <summary>
        /// 实例化领域服务事件管理器。
        /// </summary>
        public DomainServiceEventManager(DomainServiceEventManagerOptions options)
        {
            _Events = options.GetEvents();
        }

        /// <summary>
        /// 获取事件路由委托。
        /// </summary>
        /// <typeparam name="T">事件参数类型。</typeparam>
        /// <returns>返回事件委托。</returns>
        protected virtual Delegate GetEventDelegate<T>()
        {
            _Events.TryGetValue(typeof(T), out var d);
            return d;
        }

        /// <summary>
        /// 设置事件路由委托。
        /// </summary>
        /// <typeparam name="T">事件参数类型。</typeparam>
        /// <param name="value">事件委托。</param>
        protected virtual void SetEventDelegate<T>(Delegate value)
        {
            _Events[typeof(T)] = value;
        }

        #region 关联事件

        /// <summary>
        /// 添加事件处理器。
        /// </summary>
        /// <typeparam name="T">事件参数类型。</typeparam>
        /// <param name="handler">事件处理器。</param>
        public virtual void AddEventHandler<T>(DomainServiceEventHandler<T> handler)
            where T : DomainServiceEventArgs
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            lock (_Events)
            {
                Delegate d = GetEventDelegate<T>();
                if (d == null)
                    SetEventDelegate<T>(handler);
                else
                    SetEventDelegate<T>(Delegate.Combine(d, handler));
            }
        }

        /// <summary>
        /// 移除事件处理器。
        /// </summary>
        /// <typeparam name="T">事件参数类型。</typeparam>
        /// <param name="handler">事件处理器。</param>
        public virtual void RemoveEventHandler<T>(DomainServiceEventHandler<T> handler)
            where T : DomainServiceEventArgs
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            lock (_Events)
            {
                Delegate d = GetEventDelegate<T>();
                if (d == null)
                    SetEventDelegate<T>(handler);
                else
                    SetEventDelegate<T>(Delegate.Remove(d, handler));
            }
        }

        #endregion

        #region 引发事件

        /// <summary>
        /// 引发异步事件。
        /// </summary>
        /// <typeparam name="T">事件参数类型。</typeparam>
        /// <param name="context">领域执行上下文。</param>
        /// <param name="eventArgs">事件参数。</param>
        public virtual async Task RaiseEvent<T>(IDomainExecutionContext context, T eventArgs)
            where T : DomainServiceEventArgs
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            Delegate d = GetEventDelegate<T>();
            if (d != null)
                foreach (var item in d.GetInvocationList().Cast<DomainServiceEventHandler<T>>())
                {
                    await item(context, eventArgs);
                    if (eventArgs.IsHandled)
                        break;
                }
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
