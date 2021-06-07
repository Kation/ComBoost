using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.Data;
using Wodsoft.ComBoost.Data.Entity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainDataDependencyInjectionExtensions
    {
        public static IComBoostLocalServiceBuilder<EntityDomainService<TKey, TEntity, TDto, TDto, TDto>> AddEntityService<TKey, TEntity, TDto>(this IComBoostLocalBuilder builder)
            where TEntity : class, IEntity<TKey>
            where TDto : class, IEntityDTO<TKey>
        {
            var serviceBuilder = builder.AddEntityService<TKey, TEntity, TDto, TDto, TDto>();
            serviceBuilder.UseTemplate<IEntityDomainTemplate<TKey, TDto>>();
            return serviceBuilder;
        }

        public static IComBoostLocalServiceBuilder<EntityDomainService<TKey, TEntity, TListDto, TCreateDto, TEditDto>> AddEntityService<TKey, TEntity, TListDto, TCreateDto, TEditDto>(this IComBoostLocalBuilder builder)
            where TEntity : class, IEntity<TKey>
            where TListDto : class, IEntityDTO<TKey>
            where TCreateDto : class, IEntityDTO<TKey>
            where TEditDto : class, IEntityDTO<TKey>
        {
            var serviceBuilder = builder.AddService<EntityDomainService<TKey, TEntity, TListDto, TCreateDto, TEditDto>>();
            serviceBuilder.UseTemplate<IEntityDomainTemplate<TKey, TListDto, TCreateDto, TEditDto>>();
            return serviceBuilder;
        }


    }
}
