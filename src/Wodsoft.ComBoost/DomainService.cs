using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class DomainService : IDomainService
    {
        private DomainExecutionContext _Context;
        public IDomainExecutionContext ExecutionContext { get { return _Context; } }

        public event DomainExecuteEvent Executed;
        public event DomainExecuteEvent Executing;

        public async Task ExecuteAsync(IDomainContext domainContext, Delegate method)
        {
            if (domainContext == null)
                throw new ArgumentNullException(nameof(domainContext));
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            await OnExecuting(domainContext, method.GetMethodInfo());
            await (Task)method.DynamicInvoke(this, _Context.ParameterValues);
            await OnExecuted();
        }

        private async Task OnExecuting(IDomainContext domainContext, MethodInfo method)
        {
            _Context = new DomainExecutionContext(this, domainContext, method);
            if (Executing != null)
                await Executing(_Context);
        }
        private async Task OnExecuted()
        {
            if (Executed != null)
                await Executed(_Context);
            _Context = null;
        }
    }
}
