using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    [AutoTemplate]
    public partial class LifetimeService : DomainService
    {
        public Task<int> Increase([FromService] LifetimeCounter counter)
        {
            return Task.FromResult(++counter.Count);
        }

        [DomainLifetimeStrategy(DomainLifetimeStrategy.Transient)]
        public Task<int> TransientIncrease([FromService] LifetimeCounter counter)
        {
            return Task.FromResult(++counter.Count);
        }

        [DomainLifetimeStrategy(DomainLifetimeStrategy.Transient)]
        public Task DomainContextAccessor([FromService] ILifetimeService lifetimeService)
        {
            Debug.Assert(lifetimeService.Context is TransientDomainContext);
            return Task.CompletedTask;
        }
    }
}
