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

namespace Wodsoft.ComBoost.Data
{
    /// <summary>
    /// 实体领域服务。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    public class EntityDomainService<T> : DomainService
        where T : class, IEntity, new()
    {
        /// <summary>
        /// 获取实体元数据。
        /// </summary>
        public IEntityMetadata Metadata { get; private set; }

        /// <summary>
        /// 实例化实体领域服务。
        /// </summary>
        public EntityDomainService()
        {
            Metadata = EntityDescriptor.GetMetadata<T>();
        }

        public virtual async Task<IEntityViewModel<T>> List([FromService] IDatabaseContext database, [FromService] IAuthenticationProvider authenticationProvider, [FromOptions]EntityDomainAuthorizeOption authorizeOption)
        {
            var auth = authenticationProvider.GetAuthentication();
            if (authorizeOption == null)
                authorizeOption = EntityDomainAuthorizeOption.View;
            authorizeOption.Validate(Metadata, auth);
            var context = database.GetContext<T>();
            var queryable = context.Query();
            foreach (var propertyMetadata in Metadata.Properties.Where(t => t.CustomType == "Entity"))
            {
                queryable = context.Include(queryable, propertyMetadata.ClrName);
            }
            var e = new EntityQueryEventArgs<T>(queryable);
            await RaiseAsyncEvent(EntityQueryEvent, e);
            queryable = e.Queryable;
            if (!e.IsOrdered)
                queryable = context.Order(queryable);
            EntityViewModel<T> model = new EntityViewModel<T>(queryable);
            model.Properties = authorizeOption.GetProperties(Metadata, auth);
            EntityPagerOption pagerOption = Context.DomainContext.Options.GetOption<EntityPagerOption>();
            if (pagerOption != null)
            {
                model.CurrentSize = pagerOption.CurrentSize;
                model.CurrentPage = pagerOption.CurrentPage;
                model.PageSizeOption = pagerOption.PageSizeOption;
            }
            await model.UpdateTotalPageAsync();
            await model.UpdateItemsAsync();
            return model;
        }

        public static readonly DomainServiceEventRoute EntityQueryEvent = DomainServiceEventRoute.RegisterAsyncEvent<EntityQueryEventArgs<T>>("EntityQuery", typeof(EntityDomainService<T>));
        public event DomainServiceAsyncEventHandler<EntityQueryEventArgs<T>> EntityQuery { add { AddAsyncEventHandler(EntityQueryEvent, value); } remove { RemoveAsyncEventHandler(EntityQueryEvent, value); } }

        public virtual async Task<IEntityViewModel<TViewModel>> ListViewModel<TViewModel>([FromService] IDatabaseContext database, [FromService] IAuthenticationProvider authenticationProvider, [FromOptions]EntityDomainAuthorizeOption authorizeOption, [FromOptions(true)]EntityQuerySelectOption<T, TViewModel> selectOption)
        {
            var auth = authenticationProvider.GetAuthentication();
            if (authorizeOption == null)
                authorizeOption = EntityDomainAuthorizeOption.View;
            authorizeOption.Validate(Metadata, auth);
            var context = database.GetContext<T>();
            var queryable = context.Query();
            var e = new EntityQueryEventArgs<T>(queryable);
            await RaiseAsyncEvent(EntityQueryEvent, e);
            queryable = e.Queryable;
            var convertQueryable = selectOption.Select(queryable);
            EntityViewModel<TViewModel> model = new EntityViewModel<TViewModel>(convertQueryable);
            EntityPagerOption pagerOption = Context.DomainContext.Options.GetOption<EntityPagerOption>();
            if (pagerOption != null)
            {
                model.CurrentSize = pagerOption.CurrentSize;
                model.CurrentPage = pagerOption.CurrentPage;
                model.PageSizeOption = pagerOption.PageSizeOption;
            }
            await model.UpdateTotalPageAsync();
            await model.UpdateItemsAsync();
            return model;
        }

        public virtual async Task<IEntityEditModel<T>> Create([FromService] IDatabaseContext database, [FromService] IAuthenticationProvider authenticationProvider, [FromOptions]EntityDomainAuthorizeOption authorizeOption)
        {
            var auth = authenticationProvider.GetAuthentication();
            if (authorizeOption == null)
                authorizeOption = EntityDomainAuthorizeOption.Create;
            authorizeOption.Validate(Metadata, auth);
            var context = database.GetContext<T>();
            var item = context.Create();
            item.OnCreating();
            EntityEditModel<T> model = new EntityEditModel<T>(item);
            model.Properties = authorizeOption.GetProperties(Metadata, auth);
            EntityModelCreatedEventArgs<T> arg = new EntityModelCreatedEventArgs<T>(model);
            await RaiseAsyncEvent(EntityCreateModelCreatedEvent, arg);
            model = arg.Model;
            return model;
        }

        public static readonly DomainServiceEventRoute EntityCreateModelCreatedEvent = DomainServiceEventRoute.RegisterAsyncEvent<EntityModelCreatedEventArgs<T>>("EntityCreateModelCreated", typeof(EntityDomainService<T>));
        public event DomainServiceAsyncEventHandler<EntityModelCreatedEventArgs<T>> EntityCreateModelCreated { add { AddAsyncEventHandler(EntityCreateModelCreatedEvent, value); } remove { RemoveAsyncEventHandler(EntityCreateModelCreatedEvent, value); } }

        public virtual async Task<IEntityEditModel<T>> Edit([FromService] IDatabaseContext database, [FromService] IAuthenticationProvider authenticationProvider, [FromService] IValueProvider valueProvider, [FromOptions]EntityDomainAuthorizeOption authorizeOption)
        {
            object index = valueProvider.GetRequiredValue("id", Metadata.KeyProperty.ClrType);
            var auth = authenticationProvider.GetAuthentication();
            if (authorizeOption == null)
                authorizeOption = EntityDomainAuthorizeOption.Edit;
            authorizeOption.Validate(Metadata, auth);
            var context = database.GetContext<T>();
            var queryable = context.Query();
            foreach (var propertyMetadata in Metadata.Properties.Where(t => t.CustomType == "Entity"))
            {
                queryable = context.Include(queryable, propertyMetadata.ClrName);
            }
            T entity = await context.GetAsync(queryable, index);
            if (entity == null)
                throw new DomainServiceException(new EntityNotFoundException(typeof(T), index));
            entity.OnEditing();
            var model = new EntityEditModel<T>(entity);
            model.Properties = authorizeOption.GetProperties(Metadata, auth);
            EntityModelCreatedEventArgs<T> arg = new EntityModelCreatedEventArgs<T>(model);
            await RaiseAsyncEvent(EntityEditModelCreatedEvent, arg);
            model = arg.Model;
            return model;
        }

        public static readonly DomainServiceEventRoute EntityEditModelCreatedEvent = DomainServiceEventRoute.RegisterAsyncEvent<EntityModelCreatedEventArgs<T>>("EntityEditModelCreated", typeof(EntityDomainService<T>));
        public event DomainServiceAsyncEventHandler<EntityModelCreatedEventArgs<T>> EntityEditModelCreated { add { AddAsyncEventHandler(EntityEditModelCreatedEvent, value); } remove { RemoveAsyncEventHandler(EntityEditModelCreatedEvent, value); } }

        public virtual async Task<IEntityUpdateModel<T>> Update([FromService] IDatabaseContext database, [FromService] IAuthenticationProvider authenticationProvider, [FromService] IValueProvider valueProvider, [FromOptions]EntityDomainAuthorizeOption authorizeOption)
        {
            object index = valueProvider.GetValue("id", Metadata.KeyProperty.ClrType);
            var context = database.GetContext<T>();
            bool isNew = index == null || (Metadata.KeyProperty.ClrType.GetTypeInfo().IsValueType ? index.Equals(Activator.CreateInstance(Metadata.KeyProperty.ClrType)) : false);
            var auth = authenticationProvider.GetAuthentication();
            if (authorizeOption == null)
                if (isNew)
                    authorizeOption = EntityDomainAuthorizeOption.Create;
                else
                    authorizeOption = EntityDomainAuthorizeOption.Edit;
            authorizeOption.Validate(Metadata, auth);
            T entity;
            if (isNew)
            {
                entity = context.Create();
                entity.OnCreating();
            }
            else
            {
                entity = await context.GetAsync(index);
                if (entity == null)
                    throw new DomainServiceException(new EntityNotFoundException(typeof(T), index));
            }
            var result = await UpdateCore(valueProvider, auth, entity, authorizeOption.GetProperties(Metadata, auth).ToArray());
            if (result.IsSuccess)
            {
                if (isNew)
                    context.Add(entity);
                else
                    context.Update(entity);
                await database.SaveAsync();
            }
            return result;
        }

        protected virtual async Task<EntityUpdateModel<T>> UpdateCore(IValueProvider valueProvider, IAuthentication authentication, T entity, IPropertyMetadata[] properties)
        {
            var model = new EntityUpdateModel<T>();
            {
                var arg = new EntityUpdateEventArgs<T>(entity, valueProvider, properties);
                await RaiseAsyncEvent(EntityPreUpdateEvent, arg);
                properties = arg.Properties;
            }
            foreach (var property in properties)
            {
                try
                {
                    await UpdateProperty(valueProvider, entity, property);
                }
                catch (ArgumentException ex)
                {
                    model.ErrorMessage.Add(property, ex.Message);
                }
            }
            if (model.ErrorMessage.Count == 0)
            {
                var arg = new EntityUpdateEventArgs<T>(entity, valueProvider, properties);
                await RaiseAsyncEvent(EntityUpdatedEvent, arg);
            }
            model.IsSuccess = model.ErrorMessage.Count == 0;
            model.Result = entity;
            return model;
        }

        public static readonly DomainServiceEventRoute EntityPreUpdateEvent = DomainServiceEventRoute.RegisterAsyncEvent<EntityUpdateEventArgs<T>>("EntityPreUpdate", typeof(EntityDomainService<T>));
        public static readonly DomainServiceEventRoute EntityUpdatedEvent = DomainServiceEventRoute.RegisterAsyncEvent<EntityUpdateEventArgs<T>>("EntityUpdated", typeof(EntityDomainService<T>));
        public event DomainServiceAsyncEventHandler<EntityUpdateEventArgs<T>> EntityPreUpdate { add { AddAsyncEventHandler(EntityPreUpdateEvent, value); } remove { RemoveAsyncEventHandler(EntityPreUpdateEvent, value); } }
        public event DomainServiceAsyncEventHandler<EntityUpdateEventArgs<T>> EntityUpdated { add { AddAsyncEventHandler(EntityUpdatedEvent, value); } remove { RemoveAsyncEventHandler(EntityUpdatedEvent, value); } }

        protected virtual async Task UpdateProperty(IValueProvider valueProvider, T entity, IPropertyMetadata property)
        {
            bool handled = false;
            bool hasValue = valueProvider.ContainsKey(property.ClrName);
            object value;
            if (hasValue)
            {
                if (property.IsFileUpload)
                {
                    value = valueProvider.GetValue<ISelectedFile>(property.ClrName);
                }
                else if (property.Type == CustomDataType.Password)
                {
                    value = valueProvider.GetValue<string>(property.ClrName);
                }
                else
                {
                    value = valueProvider.GetValue(property.ClrName, property.ClrType);
                }
                var arg = new EntityPropertyUpdateEventArgs<T>(entity, valueProvider, property, value);
                await RaiseAsyncEvent(EntityPropertyUpdateEvent, arg);
                handled = arg.IsHandled;
            }
            else
                value = property.GetValue(entity);
            if (!handled)
            {
                if (value != null && !property.ClrType.IsAssignableFrom(value.GetType()))
                    throw new NotImplementedException("未处理的属性“" + property.Name + "”。");
                ValidationContext validationContext = new ValidationContext(entity, Context.DomainContext, null);
                validationContext.MemberName = property.ClrName;
                validationContext.DisplayName = property.Name;
                var error = property.GetAttributes<ValidationAttribute>().Select(t => t.GetValidationResult(value, validationContext)).Where(t => t != ValidationResult.Success).ToArray();
                if (error.Length > 0)
                    throw new DomainServiceException(new ArgumentException(string.Join("，", error.Select(t => t.ErrorMessage))));
                if (hasValue)
                    property.SetValue(entity, value);
            }
        }

        public static readonly DomainServiceEventRoute EntityPropertyUpdateEvent = DomainServiceEventRoute.RegisterAsyncEvent<EntityPropertyUpdateEventArgs<T>>("EntityPropertyUpdate", typeof(EntityDomainService<T>));
        public event DomainServiceAsyncEventHandler<EntityPropertyUpdateEventArgs<T>> EntityPropertyUpdate { add { AddAsyncEventHandler(EntityPropertyUpdateEvent, value); } remove { RemoveAsyncEventHandler(EntityPropertyUpdateEvent, value); } }

        public virtual async Task Remove([FromService] IDatabaseContext database, [FromService]IAuthenticationProvider authenticationProvider, [FromService]IValueProvider valueProvider, [FromOptions]EntityDomainAuthorizeOption authorizeOption)
        {
            object index = valueProvider.GetRequiredValue("id", Metadata.KeyProperty.ClrType);
            var auth = authenticationProvider.GetAuthentication();
            if (authorizeOption == null)
                authorizeOption = EntityDomainAuthorizeOption.Remove;
            authorizeOption.Validate(Metadata, auth);
            var context = database.GetContext<T>();
            T entity = await context.GetAsync(index);
            if (entity == null)
                throw new DomainServiceException(new EntityNotFoundException(typeof(T), index));
            var e = new EntityRemoveEventArgs<T>(entity);
            await RaiseAsyncEvent(EntityRemoveEvent, e);
            if (e.IsCanceled)
                return;
            context.Remove(entity);
            await database.SaveAsync();
        }

        public static readonly DomainServiceEventRoute EntityRemoveEvent = DomainServiceEventRoute.RegisterAsyncEvent<EntityRemoveEventArgs<T>>("EntityRemove", typeof(EntityDomainService<T>));
        public event DomainServiceAsyncEventHandler<EntityRemoveEventArgs<T>> EntityRemove { add { AddAsyncEventHandler(EntityRemoveEvent, value); } remove { RemoveAsyncEventHandler(EntityRemoveEvent, value); } }

        public virtual async Task<IEntityEditModel<T>> Detail([FromService] IDatabaseContext database, [FromService] IAuthenticationProvider authenticationProvider, [FromService]IValueProvider valueProvider, [FromOptions]EntityDomainAuthorizeOption authorizeOption)
        {
            object index = valueProvider.GetRequiredValue("id", Metadata.KeyProperty.ClrType);
            var auth = authenticationProvider.GetAuthentication();
            if (authorizeOption == null)
                authorizeOption = EntityDomainAuthorizeOption.Detail;
            authorizeOption.Validate(Metadata, auth);
            var context = database.GetContext<T>();
            var queryable = context.Query();
            foreach (var propertyMetadata in Metadata.Properties.Where(t => t.CustomType == "Entity"))
            {
                queryable = context.Include(queryable, propertyMetadata.ClrName);
            }
            T entity = await context.GetAsync(queryable, index);
            if (entity == null)
                throw new DomainServiceException(new EntityNotFoundException(typeof(T), index));
            var model = new EntityEditModel<T>(entity);
            model.Properties = authorizeOption.GetProperties(Metadata, auth);
            var e = new EntityModelCreatedEventArgs<T>(model);
            await RaiseAsyncEvent(EntityDetailModelCreatedEvent, e);
            return model;
        }

        public static readonly DomainServiceEventRoute EntityDetailModelCreatedEvent = DomainServiceEventRoute.RegisterAsyncEvent<EntityModelCreatedEventArgs<T>>("EntityDetailModelCreated", typeof(EntityDomainService<T>));
        public event DomainServiceAsyncEventHandler<EntityModelCreatedEventArgs<T>> EntityDetailModelCreated { add { AddAsyncEventHandler(EntityDetailModelCreatedEvent, value); } remove { RemoveAsyncEventHandler(EntityDetailModelCreatedEvent, value); } }
    }
}
