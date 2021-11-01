using Grpc.Core;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.Aggregation;
using Wodsoft.ComBoost.Aggregation.Data;
using Wodsoft.ComBoost.Data;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Grpc.Client;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainAggregationDataDependencyInjectionExtensions
    {
        public static IComBoostGrpcServiceBuilder AddAggregatorService<T, TKey>(this IComBoostGrpcServiceBuilder builder, CallOptions callOptions = default(CallOptions))
        {
            builder.UseTemplate<IDomainAggregatorService<T, TKey>>(callOptions);
            return builder;
        }

        public static IComBoostLocalServiceBuilder<DomainAggregatorEntityService<TKey, TEntity, TDto>> AddAggregatorService<TKey, TEntity, TDto>(this IComBoostLocalBuilder builder)
            where TEntity : class, IEntity<TKey>
            where TDto : class
        {
            return builder.AddService<DomainAggregatorEntityService<TKey, TEntity, TDto>>().UseTemplate<IDomainAggregatorService<TDto, TKey>>();
        }
    }
}
