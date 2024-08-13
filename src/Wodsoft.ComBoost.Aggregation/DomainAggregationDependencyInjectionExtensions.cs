using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.Aggregation;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainAggregationDependencyInjectionExtensions
    {
        public static IComBoostAggregationBuilder AddAggregation(this IComBoostLocalBuilder builder)
        {
            builder.Services.AddSingleton<IDomainAggregator, DomainAggregator>();
            return new ComBoostAggregationBuilder(builder.Services);
        }

        public static IComBoostAggregationBuilder UseAggregatorService(this IComBoostAggregationBuilder builder)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IDomainAggregatorProvider<>), typeof(DomainAggregatorProvider<>)));
            return new ComBoostAggregationBuilder(builder.Services);
        }
    }
}
