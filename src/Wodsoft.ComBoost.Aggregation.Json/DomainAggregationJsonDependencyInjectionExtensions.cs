using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.Aggregation;
using Wodsoft.ComBoost.Aggregation.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainAggregationJsonDependencyInjectionExtensions
    {
        public static IComBoostAggregationBuilder AddJsonExtension(this IComBoostAggregationBuilder builder)
        {
            DomainAggregationsBuilder.AddExtension<DomainAggregationBuilderJsonExtension>();
            return new ComBoostAggregationBuilder(builder.Services);
        }
    }
}
