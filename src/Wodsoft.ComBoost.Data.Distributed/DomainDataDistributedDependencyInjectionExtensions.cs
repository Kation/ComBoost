using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.Data;
using Wodsoft.ComBoost.Data.Distributed;
using Wodsoft.ComBoost.Data.Entity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainDataDistributedDependencyInjectionExtensions
    {
        public static IComBoostLocalServiceBuilder<EntityDomainService<TEntity, TListDto, TCreateDto, TEditDto, TRemoveDto>> UseDistributedEvent<TEntity, TListDto, TCreateDto, TEditDto, TRemoveDto>(this IComBoostLocalServiceBuilder<EntityDomainService<TEntity, TListDto, TCreateDto, TEditDto, TRemoveDto>> builder)
            where TEntity : class, IEntity
            where TListDto : class
            where TCreateDto : class
            where TEditDto : class
            where TRemoveDto : class
        {
            builder.UseEventHandler<EntityDistributedHandler<TEntity, TListDto>, EntityEditedEventArgs<TEntity>>();
            builder.UseEventHandler<EntityDistributedHandler<TEntity, TListDto>, EntityRemovedEventArgs<TEntity>>();
            builder.LocalBuilder.AddEventHandler<DomainDistributedEventPublisher<ObjectChangedEventArgs<TListDto>>, ObjectChangedEventArgs<TListDto>>();
            return builder;
        }
    }
}
