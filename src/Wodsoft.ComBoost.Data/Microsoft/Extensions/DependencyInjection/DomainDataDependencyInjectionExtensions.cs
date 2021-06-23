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

        public static IComBoostLocalServiceBuilder<EntityDTODomainService<TKey, TListDTO, TCreateDTO, TEditDTO>> AddEntityDtoService<TKey, TListDTO, TCreateDTO, TEditDTO>(this IComBoostLocalBuilder builder)
            where TListDTO : class, IEntityDTO<TKey>
            where TCreateDTO : class, IEntityDTO<TKey>
            where TEditDTO : class, IEntityDTO<TKey>
        {
            var serviceBuilder = builder.AddService<EntityDTODomainService<TKey, TListDTO, TCreateDTO, TEditDTO>>();
            serviceBuilder.UseTemplate<IEntityDTODomainTemplate<TKey, TListDTO, TCreateDTO, TEditDTO>>();
            return serviceBuilder;
        }

        public static IComBoostLocalServiceBuilder<EntityDTODomainService<TKey, TDTO, TDTO, TDTO>> AddEntityDtoService<TKey, TDTO>(this IComBoostLocalBuilder builder)
            where TDTO : class, IEntityDTO<TKey>
        {
            return AddEntityDtoService<TKey, TDTO, TDTO, TDTO>(builder);
        }

        public static IServiceCollection AddEntityDtoContext<TKey, TEntity, TListDTO, TCreateDTO, TEditDTO>(this IServiceCollection services)
            where TEntity : class, IEntity<TKey>
            where TListDTO : class, IEntityDTO<TKey>
            where TCreateDTO : class, IEntityDTO<TKey>
            where TEditDTO : class, IEntityDTO<TKey>
        {
            return services.AddScoped<IDTOContext<TKey, TListDTO, TCreateDTO, TEditDTO>, EntityDtoContext<TKey, TEntity, TListDTO, TCreateDTO, TEditDTO>>();
        }

        public static IServiceCollection AddEntityDtoContext<TKey, TEntity, TDto>(this IServiceCollection services)
            where TEntity : class, IEntity<TKey> 
            where TDto : class, IEntityDTO<TKey>
        {
            return AddEntityDtoContext<TKey, TEntity, TDto, TDto, TDto>(services);
        }
    }
}
