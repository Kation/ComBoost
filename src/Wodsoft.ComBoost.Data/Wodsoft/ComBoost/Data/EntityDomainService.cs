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
            queryable = context.Order(queryable);
            if (EntityQuery != null)
            {
                var e = new EntityQueryEventArgs<T>(queryable);
                await RaiseAsyncEvent(EntityQuery, e);
                queryable = e.Queryable;
            }
            EntityViewModel<T> model = new EntityViewModel<T>(queryable);
            model.Properties = authorizeOption.GetProperties(Metadata, auth);
            model.Items = await queryable.ToArrayAsync();
            return model;
        }

        public event DomainServiceAsyncEvent<EntityQueryEventArgs<T>> EntityQuery;

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
            if (EntityCreateModelCreated != null)
            {
                EntityModelCreatedEventArgs<T> arg = new EntityModelCreatedEventArgs<T>(model);
                await RaiseAsyncEvent(EntityCreateModelCreated, arg);
                model = arg.Model;
            }
            return model;
        }

        public event DomainServiceAsyncEvent<EntityModelCreatedEventArgs<T>> EntityCreateModelCreated;

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
                throw new EntityNotFoundException(typeof(T), index);
            entity.OnEditing();
            var model = new EntityEditModel<T>(entity);
            model.Properties = authorizeOption.GetProperties(Metadata, auth);
            if (EntityEditModelCreated != null)
            {
                EntityModelCreatedEventArgs<T> arg = new EntityModelCreatedEventArgs<T>(model);
                await RaiseAsyncEvent(EntityEditModelCreated, arg);
                model = arg.Model;
            }
            return model;
        }

        public event DomainServiceAsyncEvent<EntityModelCreatedEventArgs<T>> EntityEditModelCreated;

        public virtual async Task<IEntityUpdateModel<T>> Update([FromService] IDatabaseContext database, [FromService] IAuthenticationProvider authenticationProvider, [FromService] IValueProvider valueProvider, [FromOptions]EntityDomainAuthorizeOption authorizeOption)
        {
            object index = valueProvider.GetValue("id", Metadata.KeyProperty.ClrType);
            var context = database.GetContext<T>();
            bool isNew = index == null || Metadata.KeyProperty.ClrType.GetTypeInfo().IsValueType ? index.Equals(Activator.CreateInstance(Metadata.KeyProperty.ClrType)) : false;
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
                    throw new EntityNotFoundException(typeof(T), index);
            }
            var result = await UpdateCore(valueProvider, auth, entity, authorizeOption.GetProperties(Metadata, auth).ToArray());
            if (isNew)
                context.Add(entity);
            else
                context.Update(entity);
            await database.SaveAsync();
            return result;
        }

        protected virtual async Task<EntityUpdateModel<T>> UpdateCore(IValueProvider valueProvider, IAuthentication authentication, T entity, IPropertyMetadata[] properties)
        {
            var model = new EntityUpdateModel<T>();
            if (EntityPreUpdate != null)
            {
                var arg = new EntityUpdateEventArgs<T>(entity, valueProvider, properties);
                await RaiseAsyncEvent(EntityPreUpdate, arg);
                properties = arg.Properties;
            }
            foreach (var property in properties)
            {
                await UpdateProperty(valueProvider, entity, property);
            }
            if (EntityUpdated != null)
            {
                var arg = new EntityUpdateEventArgs<T>(entity, valueProvider, properties);
                await RaiseAsyncEvent(EntityUpdated, arg);
            }
            model.IsSuccess = model.ErrorMessage.Count == 0;
            model.Result = entity;
            return model;
        }

        public event DomainServiceAsyncEvent<EntityUpdateEventArgs<T>> EntityPreUpdate;
        public event DomainServiceAsyncEvent<EntityUpdateEventArgs<T>> EntityUpdated;

        protected virtual async Task UpdateProperty(IValueProvider valueProvider, T entity, IPropertyMetadata property)
        {
            bool handled = false;
            bool hasValue = valueProvider.Keys.Contains(property.ClrName);
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
                if (EntityPropertyUpdate != null)
                {
                    var arg = new EntityPropertyUpdateEventArgs<T>(entity, valueProvider, property, value);
                    await RaiseAsyncEvent(EntityPropertyUpdate, arg);
                    handled = arg.IsHandled;
                }
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
                    throw new ArgumentException(string.Join("，", error.Select(t => t.ErrorMessage)));
                if (hasValue)
                    property.SetValue(entity, value);
            }
        }

        public event DomainServiceAsyncEvent<EntityPropertyUpdateEventArgs<T>> EntityPropertyUpdate;

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
                throw new EntityNotFoundException(typeof(T), index);
            if (EntityRemove != null)
            {
                var e = new EntityRemoveEventArgs<T>(entity);
                await RaiseAsyncEvent(EntityRemove, e);
                if (e.IsCanceled)
                    return;
            }
            context.Remove(entity);
            await database.SaveAsync();
        }

        public event DomainServiceAsyncEvent<EntityRemoveEventArgs<T>> EntityRemove;

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
                throw new EntityNotFoundException(typeof(T), index);
            var model = new EntityEditModel<T>(entity);
            model.Properties = authorizeOption.GetProperties(Metadata, auth);
            if (EntityDetailModelCreated != null)
            {
                var e = new EntityDetailEventArgs<T>(entity, (IPropertyMetadata[])model.Properties);
                await RaiseAsyncEvent(EntityDetailModelCreated, e);
                model.Properties = e.Properties;
            }
            return model;
        }

        public event DomainServiceAsyncEvent<EntityDetailEventArgs<T>> EntityDetailModelCreated;
    }
}
