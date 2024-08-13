using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Aggregation
{
    public interface IDomainAggregator
    {
        Task<T> AggregateAsync<T>(T value);

        Task<object> AggregateAsync(object value, Type valueType);

        Task<T?> GetAggregationAsync<T>(params object[] keys);
    }
}
