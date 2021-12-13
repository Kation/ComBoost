using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Aggregation
{
    public class DomainAggregatorProvider<T> : IDomainAggregatorProvider<T>
    {
        private IServiceProvider _services;

        public DomainAggregatorProvider(IServiceProvider services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public async Task<T> GetAsync(object[] keys, DomainAggregatorExecutionPipeline<T> next)
        {
            var scope = _services.CreateScope();
            try
            {
                var aggregatorService = scope.ServiceProvider.GetRequiredService<IDomainAggregatorService<T>>();
                var value = await aggregatorService.GetAsync(keys.Select(t=>t.ToString()).ToArray());
                if (value != null || next == null)
                    return value;
                return await next();
            }
            finally
            {
                scope.Dispose();
            }
        }
    }
}
