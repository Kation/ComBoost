using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Aggregation
{
    public interface IDomainAggregatorService<T, TKey> : IDomainTemplate
    {
        Task<T> GetAsync(TKey id);
    }
}
