using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Aggregation
{
    public class DomainAggregatorProvider<T, TKey> : IDomainAggregatorProvider<T, TKey>
    {
        private IServiceProvider _services;

        public DomainAggregatorProvider(IServiceProvider services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public async Task<T> GetAsync(TKey key, DomainAggregatorExecutionPipeline<T> next)
        {
            var scope = _services.CreateScope();
            try
            {
                var aggregatorService = scope.ServiceProvider.GetRequiredService<IDomainAggregatorService<T, TKey>>();
                var value = await aggregatorService.GetAsync(key);
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
