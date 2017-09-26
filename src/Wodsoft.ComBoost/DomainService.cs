using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.ExceptionServices;

namespace Wodsoft.ComBoost
{
    public class DomainService : IDomainService
    {
        private AsyncLocal<IDomainExecutionContext> _ExecutionContext;

        public DomainService()
        {
            _ExecutionContext = new AsyncLocal<IDomainExecutionContext>();
            Filters = new List<IDomainServiceFilter>();
            foreach (var filter in GetType().GetTypeInfo().GetCustomAttributes<DomainServiceFilterAttribute>())
                Filters.Add(filter);
        }

        public IDomainExecutionContext Context
        {
            get
            {
                return _ExecutionContext.Value;
            }
            private set
            {
                _ExecutionContext.Value = value;
            }
        }

        public IList<IDomainServiceFilter> Filters { get; private set; }

        private ConcurrentDictionary<MethodInfo, IDomainServiceFilter[]> _FilterCache = new ConcurrentDictionary<MethodInfo, IDomainServiceFilter[]>();

        public static readonly DomainServiceEventRoute ExecutedEvent = DomainServiceEventRoute.RegisterAsyncEvent("Executed", typeof(DomainService));
        public static readonly DomainServiceEventRoute ExecutingEvent = DomainServiceEventRoute.RegisterAsyncEvent("Executing", typeof(DomainService));
        public event DomainServiceAsyncEventHandler Executed { add { AddAsyncEventHandler(ExecutedEvent, value); } remove { RemoveAsyncEventHandler(ExecutedEvent, value); } }
        public event DomainServiceAsyncEventHandler Executing { add { AddAsyncEventHandler(ExecutingEvent, value); } remove { RemoveAsyncEventHandler(ExecutingEvent, value); } }

        public async Task ExecuteAsync(IDomainContext domainContext, MethodInfo method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            if (!method.DeclaringType.IsAssignableFrom(GetType()))
                throw new ArgumentException("该方法不是此领域服务的方法。");
            IDomainServiceFilter[] methodFilters = _FilterCache.GetOrAdd(method, t => t.GetCustomAttributes<DomainServiceFilterAttribute>().ToArray()).ToArray();
            var accessor = domainContext.GetRequiredService<IDomainServiceAccessor>();
            var context = new DomainExecutionContext(this, domainContext, method);
            Context = context;
            accessor.DomainService = this;
            try
            {
                await OnFilterExecuting(context, methodFilters);
                if (context.IsCompleted)
                    return;
                await RaiseAsyncEvent(ExecutingEvent);
                if (context.IsCompleted)
                    return;
                var synchronizationContext = SynchronizationContext.Current;
                await (Task)method.Invoke(this, context.ParameterValues);
                SynchronizationContext.SetSynchronizationContext(synchronizationContext);
                await RaiseAsyncEvent(ExecutedEvent);
                if (context.IsCompleted)
                    return;
                await OnFilterExecuted(context, methodFilters);
            }
            catch (Exception ex)
            {
                await OnFilterThrowing(context, methodFilters, ex);
                if (context.IsCompleted)
                    return;
                ExceptionDispatchInfo.Capture(ex).Throw();
                throw;
            }
            finally
            {
                accessor.DomainService = null;
                Context = null;
            }
        }

        public async Task<T> ExecuteAsync<T>(IDomainContext domainContext, MethodInfo method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            if (!method.DeclaringType.IsAssignableFrom(GetType()))
                throw new ArgumentException("该方法不是此领域服务的方法。");
            IDomainServiceFilter[] methodFilters = _FilterCache.GetOrAdd(method, t => t.GetCustomAttributes<DomainServiceFilterAttribute>().ToArray());
            var accessor = domainContext.GetRequiredService<IDomainServiceAccessor>();
            var context = new DomainExecutionContext(this, domainContext, method);
            var executionContext = ExecutionContext.Capture();
            Context = context;
            accessor.DomainService = this;
            try
            {
                await OnFilterExecuting(context, methodFilters);
                if (context.IsCompleted)
                    return (T)context.Result;
                await RaiseAsyncEvent(ExecutingEvent);
                if (context.IsCompleted)
                    return (T)context.Result;
                var synchronizationContext = SynchronizationContext.Current;
                var result = await (Task<T>)method.Invoke(this, context.ParameterValues);
                SynchronizationContext.SetSynchronizationContext(synchronizationContext);
                context.Result = result;
                await RaiseAsyncEvent(ExecutedEvent);
                if (context.IsCompleted)
                    return (T)context.Result;
                await OnFilterExecuted(context, methodFilters);
                return result;
            }
            catch (Exception ex)
            {
                await OnFilterThrowing(context, methodFilters, ex);
                if (context.IsCompleted)
                    return (T)context.Result;
                ExceptionDispatchInfo.Capture(ex).Throw();
                throw;
            }
            finally
            {
                accessor.DomainService = null;
                Context = null;
            }
        }

        #region 筛选器

        protected virtual async Task OnFilterExecuting(IDomainExecutionContext context, IDomainServiceFilter[] methodFilters)
        {
            foreach (var filter in Filters)
                await filter.OnExecutingAsync(context);
            foreach (var filter in methodFilters)
                await filter.OnExecutingAsync(context);
            foreach (var filter in context.DomainContext.Filter)
                await filter.OnExecutingAsync(context);
        }

        protected virtual async Task OnFilterExecuted(IDomainExecutionContext context, IDomainServiceFilter[] methodFilters)
        {
            foreach (var filter in context.DomainContext.Filter)
                await filter.OnExecutedAsync(context);
            foreach (var filter in methodFilters)
                await filter.OnExecutedAsync(context);
            foreach (var filter in Filters)
                await filter.OnExecutedAsync(context);
        }

        protected virtual async Task OnFilterThrowing(IDomainExecutionContext context, IDomainServiceFilter[] methodFilters, Exception exception)
        {
            foreach (var filter in Filters)
                await filter.OnExceptionThrowingAsync(context, exception);
            foreach (var filter in methodFilters)
                await filter.OnExceptionThrowingAsync(context, exception);
            foreach (var filter in context.DomainContext.Filter)
                await filter.OnExceptionThrowingAsync(context, exception);
        }

        #endregion

        #region 关联事件

        /// <summary>
        /// 添加事件处理器。
        /// </summary>
        /// <param name="route">事件路由。</param>
        /// <param name="handler">事件处理器。</param>
        protected virtual void AddEventHandler(DomainServiceEventRoute route, DomainServiceEventHandler handler)
        {
            DomainServiceEventManager.GlobalEventManager.AddEventHandler(route, handler);
        }

        /// <summary>
        /// 添加事件处理器。
        /// </summary>
        /// <param name="route">事件路由。</param>
        /// <param name="handler">事件处理器。</param>
        protected virtual void AddEventHandler<T>(DomainServiceEventRoute route, DomainServiceEventHandler<T> handler)
            where T : EventArgs
        {
            DomainServiceEventManager.GlobalEventManager.AddEventHandler(route, handler);
        }

        /// <summary>
        /// 添加事件处理器。
        /// </summary>
        /// <param name="route">事件路由。</param>
        /// <param name="handler">事件处理器。</param>
        protected virtual void AddAsyncEventHandler(DomainServiceEventRoute route, DomainServiceAsyncEventHandler handler)
        {
            DomainServiceEventManager.GlobalEventManager.AddAsyncEventHandler(route, handler);
        }

        /// <summary>
        /// 添加事件处理器。
        /// </summary>
        /// <param name="route">事件路由。</param>
        /// <param name="handler">事件处理器。</param>
        protected virtual void AddAsyncEventHandler<T>(DomainServiceEventRoute route, DomainServiceAsyncEventHandler<T> handler)
            where T : EventArgs
        {
            DomainServiceEventManager.GlobalEventManager.AddAsyncEventHandler(route, handler);
        }

        /// <summary>
        /// 移除事件处理器。
        /// </summary>
        /// <param name="route">事件路由。</param>
        /// <param name="handler">事件处理器。</param>
        protected virtual void RemoveEventHandler(DomainServiceEventRoute route, DomainServiceEventHandler handler)
        {
            DomainServiceEventManager.GlobalEventManager.RemoveEventHandler(route, handler);
        }

        /// <summary>
        /// 移除事件处理器。
        /// </summary>
        /// <param name="route">事件路由。</param>
        /// <param name="handler">事件处理器。</param>
        protected virtual void RemoveEventHandler<T>(DomainServiceEventRoute route, DomainServiceEventHandler<T> handler)
            where T : EventArgs
        {
            DomainServiceEventManager.GlobalEventManager.RemoveEventHandler(route, handler);
        }

        /// <summary>
        /// 移除事件处理器。
        /// </summary>
        /// <param name="route">事件路由。</param>
        /// <param name="handler">事件处理器。</param>
        protected virtual void RemoveAsyncEventHandler(DomainServiceEventRoute route, DomainServiceAsyncEventHandler handler)
        {
            DomainServiceEventManager.GlobalEventManager.RemoveAsyncEventHandler(route, handler);
        }

        /// <summary>
        /// 移除事件处理器。
        /// </summary>
        /// <param name="route">事件路由。</param>
        /// <param name="handler">事件处理器。</param>
        protected virtual void RemoveAsyncEventHandler<T>(DomainServiceEventRoute route, DomainServiceAsyncEventHandler<T> handler)
            where T : EventArgs
        {
            DomainServiceEventManager.GlobalEventManager.RemoveAsyncEventHandler(route, handler);
        }

        #endregion

        #region 引发事件

        protected virtual void RaiseEvent(DomainServiceEventRoute eventRoute)
        {
            Context.DomainContext.EventManager.RaiseEvent(eventRoute, Context);
        }

        protected virtual void RaiseEvent<TArgs>(DomainServiceEventRoute eventRoute, TArgs e)
            where TArgs : EventArgs
        {
            Context.DomainContext.EventManager.RaiseEvent(eventRoute, Context, e);
        }

        protected virtual Task RaiseAsyncEvent(DomainServiceEventRoute eventRoute)
        {
            return Context.DomainContext.EventManager.RaiseAsyncEvent(eventRoute, Context);
        }

        protected virtual Task RaiseAsyncEvent<TArgs>(DomainServiceEventRoute eventRoute, TArgs e)
            where TArgs : EventArgs
        {
            return Context.DomainContext.EventManager.RaiseAsyncEvent(eventRoute, Context, e);
        }

        #endregion
    }
}
