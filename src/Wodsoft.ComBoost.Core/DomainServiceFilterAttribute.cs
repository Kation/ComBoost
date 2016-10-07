using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class DomainServiceFilterAttribute : Attribute
    {
        public virtual Task OnActionExecuting(IDomainExecutionContext context)
        {
            return Task.FromResult(0);
        }

        public virtual Task OnActionExecuted(IDomainExecutionContext context)
        {
            return Task.FromResult(0);
        }
    }
}
