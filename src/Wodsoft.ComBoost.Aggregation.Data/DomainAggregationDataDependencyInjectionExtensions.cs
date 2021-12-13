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
        public static IComBoostGrpcServiceBuilder AddAggregatorService<T>(this IComBoostGrpcServiceBuilder builder, CallOptions callOptions = default(CallOptions))
        {
            builder.UseTemplate<IDomainAggregatorService<T>>(callOptions);
            return builder;
        }

        public static IComBoostLocalServiceBuilder<DomainAggregatorEntityService<TEntity, TDto>> AddAggregatorService<TEntity, TDto>(this IComBoostLocalBuilder builder)
            where TEntity : class, IEntity
            where TDto : class
        {
            return builder.AddService<DomainAggregatorEntityService<TEntity, TDto>>().UseTemplate<IDomainAggregatorService<TDto>>();
        }
    }
}
