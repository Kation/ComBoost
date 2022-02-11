using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Aggregation
{
    public interface IDomainAggregatorProvider<T>
    {
        Task<T?> GetAsync(object[] keys, DomainAggregatorExecutionPipeline<T>? next);
    }
}
