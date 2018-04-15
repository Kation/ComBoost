using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public abstract class DomainExtension : IDomainExtension
    {
        public IDomainService DomainService { get; private set; }

        public virtual Task OnExecutedAsync(IDomainExecutionContext context, DomainServiceEventArgs e)
        {
            DomainService = context.DomainService;
            return Task.FromResult(0);
        }

        public virtual Task OnExecutingAsync(IDomainExecutionContext context, DomainServiceEventArgs e)
        {
            return Task.FromResult(0);
        }

        public virtual void OnInitialize(IServiceProvider serviceProvider, IDomainService domainService) { }
    }
}
