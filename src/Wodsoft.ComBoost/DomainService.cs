using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
#if NET451
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
#endif

namespace Wodsoft.ComBoost
{
    public class DomainService : IDomainService
    {
#if NET451
        private static readonly string LogicalDataKey = "__IDomainExecutionContext_Current__" + AppDomain.CurrentDomain.Id;

        public IDomainExecutionContext Context
        {
            get
            {
                var handle = CallContext.LogicalGetData(LogicalDataKey) as ObjectHandle;
                return handle?.Unwrap() as IDomainExecutionContext;
            }
            private set
            {
                CallContext.LogicalSetData(LogicalDataKey, new ObjectHandle(value));
            }
        }
#else
        private AsyncLocal<IDomainExecutionContext> _ExecutionContext;

        public DomainService()
        {
            _ExecutionContext = new AsyncLocal<IDomainExecutionContext>();
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
#endif
        private ConcurrentDictionary<MethodInfo, IDomainServiceFilter[]> _FilterCache = new ConcurrentDictionary<MethodInfo, IDomainServiceFilter[]>();

        public event DomainExecuteEvent Executed;
        public event DomainExecuteEvent Executing;

        public async Task ExecuteAsync(IDomainContext domainContext, MethodInfo method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            if (method.DeclaringType != GetType())
                throw new ArgumentException("该方法不是此领域服务的方法。");
            IDomainServiceFilter[] filters = _FilterCache.GetOrAdd(method, t => t.GetCustomAttributes<DomainServiceFilterAttribute>().ToArray());
            var accessor = domainContext.GetRequiredService<IDomainServiceAccessor>();
            var context = new DomainExecutionContext(this, domainContext, method);
            Context = context;
            try
            {
                accessor.DomainService = this;
                await Task.WhenAll(filters.Select(t => t.OnExecutingAsync(context)));
                if (Executing != null)
                    await Executing(context);
                await (Task)method.Invoke(this, context.ParameterValues);
                if (Executed != null)
                    await Executed(context);
                await Task.WhenAll(filters.Select(t => t.OnExecutedAsync(context)));
                accessor.DomainService = null;
            }
            catch (Exception ex)
            {
                await Task.WhenAll(filters.Select(t => t.OnExceptionThrowingAsync(context, ex)));
                throw ex;
            }
        }

        public async Task<T> ExecuteAsync<T>(IDomainContext domainContext, MethodInfo method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            if (method.DeclaringType != GetType())
                throw new ArgumentException("该方法不是此领域服务的方法。");
            IDomainServiceFilter[] filters = _FilterCache.GetOrAdd(method, t => t.GetCustomAttributes<DomainServiceFilterAttribute>().ToArray());
            var accessor = domainContext.GetRequiredService<IDomainServiceAccessor>();
            var context = new DomainExecutionContext(this, domainContext, method);
            var executionContext = ExecutionContext.Capture();
            Context = context;
            try
            {
                accessor.DomainService = this;
                await Task.WhenAll(filters.Select(t => t.OnExecutingAsync(context)));
                if (Executing != null)
                    await Executing(context);
                var result = await (Task<T>)method.Invoke(this, context.ParameterValues);
                context.Result = result;
                if (Executed != null)
                    await Executed(context);
                await Task.WhenAll(filters.Select(t => t.OnExecutedAsync(context)));
                accessor.DomainService = null;
                return result;
            }
            catch (Exception ex)
            {
                await Task.WhenAll(filters.Select(t => t.OnExceptionThrowingAsync(context, ex)));
                throw ex;
            }
        }
    }
}
