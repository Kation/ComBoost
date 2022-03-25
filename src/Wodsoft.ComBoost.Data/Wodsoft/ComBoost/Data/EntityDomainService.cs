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

    public class EntityDomainService<TEntity, TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> : DomainService
        where TEntity : class, IEntity
        where TListDTO : class
        where TCreateDTO : class
        where TEditDTO : class
        where TRemoveDTO : class
    {
        #region List

        [EntityViewModelFilter]
        public virtual async Task<IViewModel<TListDTO>> List([FromService] IEntityContext<TEntity> entityContext, [FromService] IMapper mapper)
        {
            var queryable = entityContext.Query().AsNoTracking();
            var entityQueryEventArgs = new EntityQueryEventArgs<TEntity>(queryable);
            await RaiseEvent(entityQueryEventArgs);
            queryable = entityQueryEventArgs.Queryable;
            bool isOrdered = entityQueryEventArgs.IsOrdered;
            OnListQuery(ref queryable, ref isOrdered);
            var dtoQueryable = queryable.ProjectTo<TListDTO>(mapper.ConfigurationProvider);
            OnListQuery(ref dtoQueryable, ref isOrdered);
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

        public virtual async Task<IUpdateModel<TListDTO>> Create([FromService] IEntityContext<TEntity> entityContext, [FromService] IMapper mapper, [FromValue] TCreateDTO dto)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(dto, Context!.DomainContext, null);
            UpdateModel<TListDTO> model = new UpdateModel<TListDTO>();
            List<ValidationResult> results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(dto, validationContext, results, true))
            {
                model.IsSuccess = false;
                foreach (var result in results)
                    if (result.ErrorMessage != null)
                        model.ErrorMessage.Add(new KeyValuePair<string, string>(result.MemberNames.FirstOrDefault() ?? string.Empty, result.ErrorMessage));
                return model;
            }
            var entity = entityContext.Create();
            mapper.Map(dto, entity);
            await RaiseEvent(new EntityMappedEventArgs<TEntity, TCreateDTO>(entity, dto));
            entityContext.Add(entity);
            await RaiseEvent(new EntityPreCreateEventArgs<TEntity>(entity));
            await entityContext.Database.SaveAsync();
            await RaiseEvent(new EntityCreatedEventArgs<TEntity>(entity));
            model.IsSuccess = true;
            model.Item = mapper.Map<TListDTO>(entity);
            return model;
        }

        public virtual async Task<IUpdateRangeModel<TListDTO>> CreateRange([FromService] IEntityContext<TEntity> entityContext, [FromService] IMapper mapper, [FromValue] TCreateDTO[] dtos)
        {
            UpdateRangeModel<TListDTO> model = new UpdateRangeModel<TListDTO>();
            Dictionary<TListDTO, TEntity> entities = new Dictionary<TListDTO, TEntity>();
            foreach (var dto in dtos)
            {
                var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(dto, Context!.DomainContext, null);
                List<ValidationResult> results = new List<ValidationResult>();
                if (Validator.TryValidateObject(dto, validationContext, results, true))
                {
                    var entity = entityContext.Create();
                    mapper.Map(dto, entity);
                    await RaiseEvent(new EntityMappedEventArgs<TEntity, TCreateDTO>(entity, dto));
                    entityContext.Add(entity);
                    await RaiseEvent(new EntityPreCreateEventArgs<TEntity>(entity));
                    var listDto = mapper.Map<TListDTO>(entity);
                    entities.Add(listDto, entity);
                    model.AddItem(listDto);
                }
                else
                {
                    model.AddItem(null, results.Where(t => t.ErrorMessage != null).Select(t => new KeyValuePair<string, string>(t.MemberNames.FirstOrDefault() ?? string.Empty, t.ErrorMessage!)).ToList());
                }
            }
            if (!model.IsSuccess)
                return model;
            await entityContext.Database.SaveAsync();
            foreach (var entity in entities)
            {
                await RaiseEvent(new EntityCreatedEventArgs<TEntity>(entity.Value));
                mapper.Map(entity.Value, entity.Key);
            }
            return model;
        }

        #endregion

        #region Edit

        public virtual async Task<IUpdateModel<TListDTO>> Edit([FromService] IEntityContext<TEntity> entityContext, [FromService] IMapper mapper, [FromValue] TEditDTO dto)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(dto, Context!.DomainContext, null);
            UpdateModel<TListDTO> model = new UpdateModel<TListDTO>();
            List<ValidationResult> results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(dto, validationContext, results, true))
            {
                model.IsSuccess = false;
                foreach (var result in results)
                    if (result.ErrorMessage != null)
                        model.ErrorMessage.Add(new KeyValuePair<string, string>(result.MemberNames.FirstOrDefault() ?? string.Empty, result.ErrorMessage));
                return model;
            }
            var mappedEntity = mapper.Map<TEntity>(dto);
            var keyProperties = EntityDescriptor.GetMetadata<TEntity>().KeyProperties;
            var keys = new object[keyProperties.Count];
            for (int i = 0; i < keyProperties.Count; i++)
                keys[i] = keyProperties[i].GetValue(mappedEntity);
            var entity = await entityContext.GetAsync(keys);
            if (entity == null)
                throw new DomainServiceException(new ResourceNotFoundException("Entity does not exists."));
            await RaiseEvent(new EntityPreMapEventArgs<TEntity, TEditDTO>(entity, dto));
            mapper.Map(dto, entity);
            await RaiseEvent(new EntityMappedEventArgs<TEntity, TEditDTO>(entity, dto));
            entityContext.Update(entity);
            await RaiseEvent(new EntityPreEditEventArgs<TEntity>(entity));
            await entityContext.Database.SaveAsync();
            await RaiseEvent(new EntityEditedEventArgs<TEntity>(entity));
            model.IsSuccess = true;
            model.Item = mapper.Map<TListDTO>(entity);
            return model;
        }

        public virtual async Task<IUpdateRangeModel<TListDTO>> EditRange([FromService] IEntityContext<TEntity> entityContext, [FromService] IMapper mapper, [FromValue] TEditDTO[] dtos)
        {
            UpdateRangeModel<TListDTO> model = new UpdateRangeModel<TListDTO>();
            Dictionary<TListDTO, TEntity> entities = new Dictionary<TListDTO, TEntity>();
            foreach (var dto in dtos)
            {
                var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(dto, Context!.DomainContext, null);
                List<ValidationResult> results = new List<ValidationResult>();
                if (Validator.TryValidateObject(dto, validationContext, results, true))
                {
                    var mappedEntity = mapper.Map<TEntity>(dto);
                    var keyProperties = EntityDescriptor.GetMetadata<TEntity>().KeyProperties;
                    var keys = new object[keyProperties.Count];
                    for (int i = 0; i < keyProperties.Count; i++)
                        keys[i] = keyProperties[i].GetValue(mappedEntity);
                    var entity = await entityContext.GetAsync(keys);
                    if (entity == null)
                        throw new DomainServiceException(new ResourceNotFoundException("Entity does not exists."));
                    await RaiseEvent(new EntityPreMapEventArgs<TEntity, TEditDTO>(entity, dto));
                    mapper.Map(dto, entity);
                    await RaiseEvent(new EntityMappedEventArgs<TEntity, TEditDTO>(entity, dto));
                    entityContext.Update(entity);
                    await RaiseEvent(new EntityPreEditEventArgs<TEntity>(entity));
                    var listDto = mapper.Map<TListDTO>(entity);
                    entities.Add(listDto, entity);
                    model.AddItem(listDto);
                }
                else
                {
                    model.AddItem(null, results.Where(t => t.ErrorMessage != null).Select(t => new KeyValuePair<string, string>(t.MemberNames.FirstOrDefault() ?? string.Empty, t.ErrorMessage!)).ToList());
                }
            }
            if (!model.IsSuccess)
                return model;
            await entityContext.Database.SaveAsync();
            foreach (var entity in entities)
            {
                await RaiseEvent(new EntityEditedEventArgs<TEntity>(entity.Value));
                mapper.Map(entity.Value, entity.Key);
            }
            return model;
        }

        #endregion

        #region Remove

        public virtual async Task Remove([FromService] IEntityContext<TEntity> entityContext, [FromService] IMapper mapper, [FromValue] TRemoveDTO dto)
        {
            var mappedEntity = mapper.Map<TEntity>(dto);
            var keyProperties = EntityDescriptor.GetMetadata<TEntity>().KeyProperties;
            var keys = new object[keyProperties.Count];
            for (int i = 0; i < keyProperties.Count; i++)
                keys[i] = keyProperties[i].GetValue(mappedEntity);
            var entity = await entityContext.GetAsync(keys);
            if (entity == null)
                throw new DomainServiceException(new ResourceNotFoundException("Entity does not exists."));
            await RaiseEvent(new EntityPreRemoveEventArgs<TEntity>(entity));
            entityContext.Remove(entity);
            await entityContext.Database.SaveAsync();
            await RaiseEvent(new EntityRemovedEventArgs<TEntity>(entity));
        }

        public virtual async Task RemoveRange([FromService] IEntityContext<TEntity> entityContext, [FromService] IMapper mapper, [FromValue] TRemoveDTO[] dtos)
        {
            List<TEntity> entities = new List<TEntity>();
            foreach (var dto in dtos)
            {
                var mappedEntity = mapper.Map<TEntity>(dto);
                var keyProperties = EntityDescriptor.GetMetadata<TEntity>().KeyProperties;
                var keys = new object[keyProperties.Count];
                for (int i = 0; i < keyProperties.Count; i++)
                    keys[i] = keyProperties[i].GetValue(mappedEntity);
                var entity = await entityContext.GetAsync(keys);
                if (entity == null)
                    throw new DomainServiceException(new ResourceNotFoundException("Entity does not exists."));
                await RaiseEvent(new EntityPreRemoveEventArgs<TEntity>(entity));
                entityContext.Remove(entity);
                entities.Add(entity);
            }
            await entityContext.Database.SaveAsync();
            foreach (var entity in entities)
                await RaiseEvent(new EntityRemovedEventArgs<TEntity>(entity));
        }

        #endregion
    }
}
