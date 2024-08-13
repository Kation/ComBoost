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
        public static IComBoostLocalServiceBuilder<EntityDomainService<TEntity, TDto, TDto, TDto, TDto>> AddEntityService<TEntity, TDto>(this IComBoostLocalBuilder builder)
            where TEntity : class, IEntity
            where TDto : class
        {
            var serviceBuilder = builder.AddEntityService<TEntity, TDto, TDto, TDto, TDto>();
            serviceBuilder.UseTemplate<IEntityDomainTemplate<TDto>>();
            return serviceBuilder;
        }

        public static IComBoostLocalServiceBuilder<EntityDomainService<TEntity, TListDto, TCreateDto, TEditDto, TRemoveDto>> AddEntityService<TEntity, TListDto, TCreateDto, TEditDto, TRemoveDto>(this IComBoostLocalBuilder builder)
            where TEntity : class, IEntity
            where TListDto : class
            where TCreateDto : class
            where TEditDto : class
            where TRemoveDto : class
        {
            var serviceBuilder = builder.AddService<EntityDomainService<TEntity, TListDto, TCreateDto, TEditDto, TRemoveDto>>();
            serviceBuilder.UseTemplate<IEntityDomainTemplate<TListDto, TCreateDto, TEditDto, TRemoveDto>>();
            return serviceBuilder;
        }

        public static IComBoostLocalServiceBuilder<EntityDTODomainService<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>> AddEntityDtoService<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>(this IComBoostLocalBuilder builder)
            where TListDTO : class
            where TCreateDTO : class
            where TEditDTO : class
            where TRemoveDTO : class
        {
            var serviceBuilder = builder.AddService<EntityDTODomainService<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>>();
            serviceBuilder.UseTemplate<IEntityDTODomainTemplate<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>>();
            return serviceBuilder;
        }

        public static IComBoostLocalServiceBuilder<EntityDTODomainService<TDTO, TDTO, TDTO, TDTO>> AddEntityDtoService<TDTO>(this IComBoostLocalBuilder builder)
            where TDTO : class
        {
            return AddEntityDtoService<TDTO, TDTO, TDTO, TDTO>(builder);
        }

        public static IServiceCollection AddEntityDtoContext<TEntity, TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>(this IServiceCollection services, Action<EntityDtoContextOptions<TEntity, TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>>? optionConfigue = null)
            where TEntity : class, IEntity
            where TListDTO : class
            where TCreateDTO : class
            where TEditDTO : class
            where TRemoveDTO : class
        {
            if (optionConfigue != null)
                services.PostConfigure(optionConfigue);
            return services.AddScoped<IDTOContext<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>, EntityDtoContext<TEntity, TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>>();
        }

        public static IServiceCollection AddEntityDtoContext<TEntity, TDto>(this IServiceCollection services, Action<EntityDtoContextOptions<TEntity, TDto, TDto, TDto, TDto>>? optionConfigue = null)
            where TEntity : class, IEntity
            where TDto : class
        {
            return AddEntityDtoContext<TEntity, TDto, TDto, TDto, TDto>(services, optionConfigue);
        }
    }
}
