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

        public static IComBoostLocalServiceBuilder<EntityDTODomainService<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>> AddEntityDtoService<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>(this IComBoostLocalBuilder builder)
            where TListDTO : class, IEntityDTO
            where TCreateDTO : class, IEntityDTO
            where TEditDTO : class, IEntityDTO
            where TRemoveDTO : class, IEntityDTO
        {
            var serviceBuilder = builder.AddService<EntityDTODomainService<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>>();
            serviceBuilder.UseTemplate<IEntityDTODomainTemplate<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>>();
            return serviceBuilder;
        }

        public static IComBoostLocalServiceBuilder<EntityDTODomainService<TDTO, TDTO, TDTO, TDTO>> AddEntityDtoService<TDTO>(this IComBoostLocalBuilder builder)
            where TDTO : class, IEntityDTO
        {
            return AddEntityDtoService<TDTO, TDTO, TDTO, TDTO>(builder);
        }

        public static IServiceCollection AddEntityDtoContext<TEntity, TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>(this IServiceCollection services)
            where TEntity : class, IEntity
        {
            return services.AddScoped<IDTOContext<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>, EntityDtoContext<TEntity, TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>>();
        }

        public static IServiceCollection AddEntityDtoContext<TEntity, TDto>(this IServiceCollection services)
            where TEntity : class, IEntity
        {
            return AddEntityDtoContext<TEntity, TDto, TDto, TDto, TDto>(services);
        }
    }
}
