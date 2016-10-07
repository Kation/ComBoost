using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class DomainService : IDomainService
    {
        private Dictionary<ExecutionContext, IDomainExecutionContext> _ExecutionContext;

        public DomainService()
        {
            _ExecutionContext = new Dictionary<System.Threading.ExecutionContext, IDomainExecutionContext>();
        }

        public IDomainExecutionContext Context
        {
            get
            {
                var context = ExecutionContext.Capture();
                IDomainExecutionContext value;
                if (_ExecutionContext.TryGetValue(context, out value))
                    return value;
                else
                    return null;
            }
        }

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
            var context = new DomainExecutionContext(this, domainContext, method);
            var executionContext = ExecutionContext.Capture();
            _ExecutionContext.Add(executionContext, context);
            try
            {
                if (Executing != null)
                    await Executing(context);
                await (Task)method.Invoke(this, context.ParameterValues);
                if (Executed != null)
                    await Executed(context);
            }
            finally
            {
                _ExecutionContext.Remove(executionContext);
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
            var context = new DomainExecutionContext(this, domainContext, method);
            var executionContext = ExecutionContext.Capture();
            _ExecutionContext.Add(executionContext, context);
            try
            {
                if (Executing != null)
                    await Executing(context);
                var result = await (Task<T>)method.Invoke(this, context.ParameterValues);
                context.Result = result;
                if (Executed != null)
                    await Executed(context);
                return result;
            }
            finally
            {
                _ExecutionContext.Remove(executionContext);
            }
        }
    }
}
