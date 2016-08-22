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
    public class EntityDomainService<T> : DomainService
        where T : class, IEntity, new()
    {
        public IEntityMetadata Metadata { get; private set; }

        public EntityDomainService()
        {
            Metadata = EntityDescriptor.GetMetadata<T>();
        }

        public virtual async Task<IEntityViewModel<T>> List([FromService] IDatabaseContext database, [FromService] IAuthenticationProvider authenticationProvider)
        {
            var auth = authenticationProvider.GetAuthentication();
            if (!Metadata.AllowAnonymous)
            {
                if (auth == null)
                    throw new NotSupportedException("不能从当前权限提供器获取权限。");
                if (Metadata.AuthenticationRequiredMode == AuthenticationRequiredMode.All)
                {
                    if (Metadata.ViewRoles.Any(t => !auth.IsInRole(t)))
                        throw new UnauthorizedAccessException("查看权限不足。");
                }
                else
                {
                    if (Metadata.ViewRoles.All(t => !auth.IsInRole(t)))
                        throw new UnauthorizedAccessException("查看权限不足。");
                }
            }
            var context = database.GetContext<T>();
            var queryable = context.Query();
            if (EntityQuery != null)
            {
                var e = new EntityQueryEventArgs<T>(queryable);
                EntityQuery(Context, e);
                queryable = e.Queryable;
            }
            EntityViewModel<T> model = new EntityViewModel<T>(queryable);
            model.Items = await context.ToArrayAsync(queryable);
            return model;
        }

        public event DomainServiceEvent<EntityQueryEventArgs<T>> EntityQuery;

        public virtual Task<IEntityEditModel<T>> Create([FromService] IDatabaseContext database, [FromService] IAuthenticationProvider authenticationProvider)
        {
            var auth = authenticationProvider.GetAuthentication();
            if (!Metadata.AllowAnonymous)
            {
                if (auth == null)
                    throw new NotSupportedException("不能从当前权限提供器获取权限。");
                if (Metadata.AuthenticationRequiredMode == AuthenticationRequiredMode.All)
                {
                    if (Metadata.AddRoles.Any(t => !auth.IsInRole(t)))
                        throw new UnauthorizedAccessException("创建权限不足。");
                }
                else
                {
                    if (Metadata.AddRoles.All(t => !auth.IsInRole(t)))
                        throw new UnauthorizedAccessException("创建权限不足。");
                }
            }
            var context = database.GetContext<T>();
            var item = context.Create();
            item.OnCreating();
            EntityEditModel<T> model = new EntityEditModel<T>(item);
            model.Properties = Metadata.CreateProperties.Where(t =>
            {
                if (!t.AllowAnonymous && auth == null)
                    return false;
                if (t.AuthenticationRequiredMode == AuthenticationRequiredMode.All)
                    return t.AddRoles.All(r => auth.IsInRole(r));
                else
                    return t.AddRoles.Any(r => auth.IsInRole(r));
            }).ToArray();
            if (EntityCreateModelCreated != null)
            {
                EntityModelCreatedEventArgs<T> arg = new EntityModelCreatedEventArgs<T>(model);
                EntityCreateModelCreated(Context, arg);
                model = arg.Model;
            }
            return Task.FromResult((IEntityEditModel<T>)model);
        }

        public event DomainServiceEvent<EntityModelCreatedEventArgs<T>> EntityCreateModelCreated;

        public virtual async Task<IEntityEditModel<T>> Edit([FromService] IDatabaseContext database, [FromService] IAuthenticationProvider authenticationProvider, [FromValue] object index)
        {
            var auth = authenticationProvider.GetAuthentication();
            if (!Metadata.AllowAnonymous)
            {
                if (auth == null)
                    throw new NotSupportedException("不能从当前权限提供器获取权限。");
                if (Metadata.AuthenticationRequiredMode == AuthenticationRequiredMode.All)
                {
                    if (Metadata.EditRoles.Any(t => !auth.IsInRole(t)))
                        throw new UnauthorizedAccessException("编辑权限不足。");
                }
                else
                {
                    if (Metadata.EditRoles.All(t => !auth.IsInRole(t)))
                        throw new UnauthorizedAccessException("编辑权限不足。");
                }
            }
            var context = database.GetContext<T>();
            T entity = await context.GetAsync(index);
            if (entity == null)
                throw new EntityNotFoundException(typeof(T), index);
            var model = new EntityEditModel<T>(entity);
            model.Properties = Metadata.EditProperties.Where(t =>
            {
                if (!t.AllowAnonymous && auth == null)
                    return false;
                if (t.AuthenticationRequiredMode == AuthenticationRequiredMode.All)
                    return t.EditRoles.All(r => auth.IsInRole(r));
                else
                    return t.EditRoles.Any(r => auth.IsInRole(r));
            }).ToArray();
            if (EntityEditModelCreated != null)
            {
                EntityModelCreatedEventArgs<T> arg = new EntityModelCreatedEventArgs<T>(model);
                EntityEditModelCreated(Context, arg);
                model = arg.Model;
            }
            return model;
        }

        public event DomainServiceEvent<EntityModelCreatedEventArgs<T>> EntityEditModelCreated;

        public virtual async Task<EntityUpdateModel> Update([FromService] IDatabaseContext database, [FromService] IAuthenticationProvider authenticationProvider, [FromService] IValueProvider valueProvider)
        {
            var context = database.GetContext<T>();
            object index = valueProvider.GetRequiredValue(Metadata.KeyProperty.ClrName);
            if (index.GetType() != Metadata.KeyProperty.ClrType)
            {
                var converter = TypeDescriptor.GetConverter(Metadata.KeyProperty.ClrType);
                index = converter.ConvertFrom(index);
            }
            bool isNew = index == Activator.CreateInstance(Metadata.KeyProperty.ClrType);
            var auth = authenticationProvider.GetAuthentication();
            if (!Metadata.AllowAnonymous)
            {
                if (auth == null)
                    throw new NotSupportedException("不能从当前权限提供器获取权限。");
                if (Metadata.AuthenticationRequiredMode == AuthenticationRequiredMode.All)
                {
                    if (isNew)
                    {
                        if (Metadata.AddRoles.Any(t => !auth.IsInRole(t)))
                            throw new UnauthorizedAccessException("创建权限不足。");
                    }
                    else
                    {
                        if (Metadata.EditRoles.Any(t => !auth.IsInRole(t)))
                            throw new UnauthorizedAccessException("编辑权限不足。");
                    }
                }
                else
                {
                    if (isNew)
                    {
                        if (Metadata.AddRoles.All(t => !auth.IsInRole(t)))
                            throw new UnauthorizedAccessException("创建权限不足。");
                    }
                    else
                    {
                        if (Metadata.EditRoles.All(t => !auth.IsInRole(t)))
                            throw new UnauthorizedAccessException("编辑权限不足。");
                    }
                }
            }
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
            var result = UpdateCore(valueProvider, auth, entity, isNew);
            await database.SaveAsync();
            return result;
        }

        protected virtual EntityUpdateModel UpdateCore(IValueProvider valueProvider, IAuthentication authentication, T entity, bool isNew)
        {
            var properties = isNew ? Metadata.CreateProperties.Where(t =>
            {
                if (!t.AllowAnonymous && authentication == null)
                    return false;
                if (t.AuthenticationRequiredMode == AuthenticationRequiredMode.All)
                    return t.AddRoles.All(r => authentication.IsInRole(r));
                else
                    return t.AddRoles.Any(r => authentication.IsInRole(r));
            }).ToArray() :
            Metadata.EditProperties.Where(t =>
            {
                if (!t.AllowAnonymous && authentication == null)
                    return false;
                if (t.AuthenticationRequiredMode == AuthenticationRequiredMode.All)
                    return t.EditRoles.All(r => authentication.IsInRole(r));
                else
                    return t.EditRoles.Any(r => authentication.IsInRole(r));
            }).ToArray();
            var model = new EntityUpdateModel();
            if (EntityPreUpdate != null)
            {
                var arg = new EntityUpdateEventArgs<T>(entity, valueProvider, properties);
                EntityPreUpdate(Context, arg);
                properties = arg.Properties;
            }
            foreach (var property in properties)
            {
                UpdateProperty(valueProvider, entity, property);
            }
            if (EntityUpdated != null)
            {
                var arg = new EntityUpdateEventArgs<T>(entity, valueProvider, properties);
                EntityUpdated(Context, arg);
            }
            model.IsSuccess = model.ErrorMessage.Count == 0;
            return model;
        }

        public event DomainServiceEvent<EntityUpdateEventArgs<T>> EntityPreUpdate;
        public event DomainServiceEvent<EntityUpdateEventArgs<T>> EntityUpdated;

        protected virtual void UpdateProperty(IValueProvider valueProvider, T entity, IPropertyMetadata property)
        {
            object value = valueProvider.GetValue(property.ClrName, property.ClrType);
            bool handled = false; ;
            if (EntityPropertyUpdate != null)
            {
                var arg = new EntityPropertyUpdateEventArgs<T>(entity, valueProvider, property, value);
                EntityPropertyUpdate(Context, arg);
                handled = arg.IsHandled;
            }
            if (!handled)
            {
                if (value == null)
                    return;
                property.SetValue(entity, value);
            }
        }

        public event DomainServiceEvent<EntityPropertyUpdateEventArgs<T>> EntityPropertyUpdate;

        public virtual async Task Remove([FromService] IDatabaseContext database, [FromService] IAuthenticationProvider authenticationProvider, [FromValue] object index)
        {
            var auth = authenticationProvider.GetAuthentication();
            if (!Metadata.AllowAnonymous)
            {
                if (auth == null)
                    throw new NotSupportedException("不能从当前权限提供器获取权限。");
                if (Metadata.AuthenticationRequiredMode == AuthenticationRequiredMode.All)
                {
                    if (Metadata.EditRoles.Any(t => !auth.IsInRole(t)))
                        throw new UnauthorizedAccessException("创建权限不足。");
                }
                else
                {
                    if (Metadata.EditRoles.All(t => !auth.IsInRole(t)))
                        throw new UnauthorizedAccessException("创建权限不足。");
                }
            }
            var context = database.GetContext<T>();
            T entity = await context.GetAsync(index);
            if (entity == null)
                throw new EntityNotFoundException(typeof(T), index);
            if (EntityRemove != null)
            {
                var e = new EntityRemoveEventArgs<T>(entity);
                EntityRemove(Context, e);
                if (e.IsCanceled)
                    return;
            }
            context.Remove(entity);
            await database.SaveAsync();
        }

        public event DomainServiceEvent<EntityRemoveEventArgs<T>> EntityRemove;

        public virtual async Task<IEntityEditModel<T>> Detail([FromService] IDatabaseContext database, [FromService] IAuthenticationProvider authenticationProvider, [FromValue] object index)
        {
            var auth = authenticationProvider.GetAuthentication();
            if (!Metadata.AllowAnonymous)
            {
                if (auth == null)
                    throw new NotSupportedException("不能从当前权限提供器获取权限。");
                if (Metadata.AuthenticationRequiredMode == AuthenticationRequiredMode.All)
                {
                    if (Metadata.ViewRoles.Any(t => !auth.IsInRole(t)))
                        throw new UnauthorizedAccessException("查看权限不足。");
                }
                else
                {
                    if (Metadata.ViewRoles.All(t => !auth.IsInRole(t)))
                        throw new UnauthorizedAccessException("查看权限不足。");
                }
            }
            var context = database.GetContext<T>();
            T entity = await context.GetAsync(index);
            var model = new EntityEditModel<T>(entity);
            model.Properties = Metadata.DetailProperties.Where(t =>
            {
                if (!t.AllowAnonymous && auth == null)
                    return false;
                if (t.AuthenticationRequiredMode == AuthenticationRequiredMode.All)
                    return t.ViewRoles.All(r => auth.IsInRole(r));
                else
                    return t.ViewRoles.Any(r => auth.IsInRole(r));
            }).ToArray();
            if (EntityDetail != null)
            {
                var e = new EntityDetailEventArgs<T>(entity, (IPropertyMetadata[])model.Properties);
                EntityDetail(Context, e);
                model.Properties = e.Properties;
            }
            return model;
        }

        public event DomainServiceEvent<EntityDetailEventArgs<T>> EntityDetail;
    }
}
