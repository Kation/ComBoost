using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Aggregation
{
    public interface IDomainAggregatorProvider<T, TKey>
    {
        Task<T> GetAsync(TKey key, DomainAggregatorExecutionPipeline<T> next);
    }
}
