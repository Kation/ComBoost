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

        public virtual async Task List([FromService] IDatabaseContext database, [FromService] IAuthenticationProvider authenticationProvider)
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
            if (ListQuery != null)
                queryable = ListQuery(queryable);
            EntityViewModel<T> model = new EntityViewModel<T>(queryable);
            model.Items = await context.ToArrayAsync(queryable);
            ExecutionContext.DomainContext.Result = model;
        }

        public event Func<IQueryable<T>, IQueryable<T>> ListQuery;


        public virtual Task Create([FromService] IDatabaseContext database, [FromService] IAuthenticationProvider authenticationProvider)
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
            ExecutionContext.DomainContext.Result = model;
            return Task.FromResult(0);
        }

        public virtual async Task Edit([FromService] IDatabaseContext database, [FromService] IAuthenticationProvider authenticationProvider, [FromValue] object index)
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
            ExecutionContext.DomainContext.Result = model;
        }

        public virtual async Task Update([FromService] IDatabaseContext database, [FromService] IAuthenticationProvider authenticationProvider, [FromService] IValueProvider valueProvider)
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
            UpdateCore(valueProvider, auth, entity, isNew);
        }

        protected virtual void UpdateCore(IValueProvider valueProvider, IAuthentication authentication, T entity, bool isNew)
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
            foreach (var property in properties)
            {
                UpdateProperty(valueProvider, entity, property);
            }
            model.IsSuccess = model.ErrorMessage.Count == 0;
        }

        protected virtual void UpdateProperty(IValueProvider valueProvider, T entity, IPropertyMetadata property)
        {
            object value = valueProvider.GetValue(property.ClrName);
            if (value == null)
                return;
            if (!property.ClrType.GetType().IsAssignableFrom(value.GetType()))
                value = property.Converter.ConvertFrom(value);
            property.SetValue(entity, value);
        }

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
            context.Remove(entity);
            await database.SaveAsync();
        }

        public virtual async Task Detail([FromService] IDatabaseContext database, [FromService] IAuthenticationProvider authenticationProvider, [FromValue] object index)
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
            ExecutionContext.DomainContext.Result = model;
        }
    }
}
