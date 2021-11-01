using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Aggregation
{
    public interface IComBoostAggregationBuilder
    {
        IServiceCollection Services { get; }
    }
}
