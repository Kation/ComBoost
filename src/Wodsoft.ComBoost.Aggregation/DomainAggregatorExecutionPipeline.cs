using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Aggregation
{
    public delegate Task<T?> DomainAggregatorExecutionPipeline<T>();
}
