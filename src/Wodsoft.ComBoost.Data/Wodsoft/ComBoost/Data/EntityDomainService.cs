using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using Wodsoft.ComBoost.Data.Entity.Metadata;
using Wodsoft.ComBoost.Security;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Wodsoft.ComBoost.Data.Linq;
using AutoMapper.QueryableExtensions;

namespace Wodsoft.ComBoost.Data
{

    //public class EntityDomainService<TKey, TEntity, TEntityDTO> : EntityDomainService<TKey, TEntity, TEntityDTO, TEntityDTO, TEntityDTO>
    //    where TEntity : class, IEntity<TKey>
    //    where TEntityDTO : class
    //{
    //}

    public class EntityDomainService<TKey, TEntity, TListDTO, TCreateDTO, TEditDTO> : DomainService
        where TEntity : class, IEntity<TKey>
        where TListDTO : class, IEntityDTO<TKey>
        where TCreateDTO : class, IEntityDTO<TKey>
        where TEditDTO : class, IEntityDTO<TKey>
    {
        #region List

        [EntityViewModelFilter]
        public virtual async Task<IViewModel<TListDTO>> List([FromService] IEntityContext<TEntity> entityContext, [FromService] IMapper mapper)
        {
            var queryable = entityContext.Query();
            var entityQueryEventArgs = new EntityQueryEventArgs<TEntity>(queryable);
            await RaiseEvent(entityQueryEventArgs);
            queryable = entityQueryEventArgs.Queryable;
            bool isOrdered = entityQueryEventArgs.IsOrdered;
            OnListQuery(ref queryable, ref isOrdered);
            if (!isOrdered)
                queryable = queryable.OrderByDescending(t => t.CreationDate);
            var dtoQueryable = queryable.ProjectTo<TListDTO>(mapper.ConfigurationProvider);
            OnListQuery(ref dtoQueryable, ref isOrdered);
            if (!isOrdered)
                queryable = queryable.OrderByDescending(t => t.CreationDate);
            ViewModel<TListDTO> model = new ViewModel<TListDTO>(dtoQueryable);
            await RaiseEvent(new EntityQueryModelCreatedEventArgs<TListDTO>(model));
            return model;
        }

        protected virtual void OnListQuery(ref IQueryable<TEntity> queryable, ref bool isOrdered)
        {

        }

        protected virtual void OnListQuery(ref IQueryable<TListDTO> queryable, ref bool isOrdered)
        {

        }

        #endregion

        #region Create

        public virtual async Task<IUpdateModel<TCreateDTO>> Create([FromService] IEntityContext<TEntity> entityContext, [FromService] IMapper mapper, [FromValue] TCreateDTO dto)
        {
            var entity = entityContext.Create();
            mapper.Map(dto, entity);
            entityContext.Add(entity);
            await RaiseEvent(new EntityPreCreateEventArgs<TEntity>(entity));
            await entityContext.Database.SaveAsync();
            await RaiseEvent(new EntityCreatedEventArgs<TEntity>(entity));
            UpdateModel<TCreateDTO> model = new UpdateModel<TCreateDTO>();
            model.Result = dto;
            model.IsSuccess = true;
            mapper.Map(entity, dto);
            return model;
        }

        protected virtual void OnCreateModelCreated(EntityEditModel<TCreateDTO> model)
        {

        }

        #endregion

        #region Edit

        public virtual async Task<IUpdateModel<TEditDTO>> Edit([FromService] IEntityContext<TEntity> entityContext, [FromService] IMapper mapper, [FromValue] TEditDTO dto)
        {
            var entity = await entityContext.Query().Where(t => t.Id.Equals(dto.Id)).FirstOrDefaultAsync();
            if (entity == null)
                throw new DomainServiceException(new ResourceNotFoundException("Entity does not exists."));
            await RaiseEvent(new EntityEditMapEventArgs<TEntity, TEditDTO>(entity, dto));
            mapper.Map(dto, entity);
            entityContext.Update(entity);
            await RaiseEvent(new EntityPreEditEventArgs<TEntity>(entity));
            await entityContext.Database.SaveAsync();
            await RaiseEvent(new EntityEditedEventArgs<TEntity>(entity));
            UpdateModel<TEditDTO> model = new UpdateModel<TEditDTO>();
            model.Result = dto;
            model.IsSuccess = true;
            mapper.Map(entity, dto);
            return model;
        }

        protected virtual void OnEditModelCreated(EntityEditModel<TEditDTO> model)
        {

        }

        #endregion

        #region Remove

        public virtual async Task Remove([FromService] IEntityContext<TEntity> entityContext, [FromValue] TKey id)
        {
            var entity = await entityContext.Query().Where(t => t.Id.Equals(id)).FirstOrDefaultAsync();
            if (entity == null)
                throw new DomainServiceException(new ResourceNotFoundException("Entity does not exists."));
            await RaiseEvent(new EntityPreRemoveEventArgs<TEntity>(entity));
            entityContext.Remove(entity);
            await entityContext.Database.SaveAsync();
            await RaiseEvent(new EntityRemovedEventArgs<TEntity>(entity));
        }

        #endregion
    }
}
