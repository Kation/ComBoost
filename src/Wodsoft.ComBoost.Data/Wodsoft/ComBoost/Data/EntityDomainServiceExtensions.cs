using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Data
{
    /// <summary>
    /// 实体领域服务扩展方法。
    /// </summary>
    public static class EntityDomainServiceExtensions
    {
        /// <summary>
        /// 执行列表。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="domain">实体领域服务。</param>
        /// <param name="context">领域上下文。</param>
        /// <returns>返回实体视图模型。</returns>
        public static Task<IEntityViewModel<T>> ExecuteList<T>(this EntityDomainService<T> domain, IDomainContext context)
            where T : class, IEntity, new()
        {
            return domain.ExecuteAsync<IDatabaseContext, IAuthenticationProvider, EntityDomainAuthorizeOption, IEntityViewModel<T>>(context, domain.List);
        }

        public static Task<IEntityViewModel<T>> ExecuteList<T>(this EntityDomainService<T> domain, IDomainContext context, Expression<Func<T, bool>> whereExpression)
            where T : class, IEntity, new()
        {
            context.EventManager.AddAsyncEventHandler<EntityQueryEventArgs<T>>(EntityDomainService<T>.EntityQueryEvent, (d, e) =>
            {
                e.Queryable = e.Queryable.Where(whereExpression);
                return Task.CompletedTask;
            });
            return domain.ExecuteAsync<IDatabaseContext, IAuthenticationProvider, EntityDomainAuthorizeOption, IEntityViewModel<T>>(context, domain.List);
        }

        public static Task<IViewModel<TModel>> ExecuteListViewModel<T, TModel>(this EntityDomainService<T> domain, IDomainContext context, Expression<Func<T, TModel>> selectExpression)
            where T : class, IEntity, new()
            where TModel : class
        {
            context.Options.SetOption(new EntityQuerySelectOption<T, TModel>(selectExpression));
            return domain.ExecuteAsync<IDatabaseContext, IAuthenticationProvider, EntityDomainAuthorizeOption, EntityQuerySelectOption<T, TModel>, IViewModel<TModel>>(context, domain.ListViewModel);
        }

        public static Task<IViewModel<TModel>> ExecuteListViewModel<T, TModel>(this EntityDomainService<T> domain, IDomainContext context, Expression<Func<T, bool>> whereExpression, Expression<Func<T, TModel>> selectExpression)
            where T : class, IEntity, new()
            where TModel : class
        {
            context.EventManager.AddAsyncEventHandler<EntityQueryEventArgs<T>>(EntityDomainService<T>.EntityQueryEvent, (d, e) =>
            {
                e.Queryable = e.Queryable.Where(whereExpression);
                return Task.CompletedTask;
            });
            context.Options.SetOption(new EntityQuerySelectOption<T, TModel>(selectExpression));
            return domain.ExecuteAsync<IDatabaseContext, IAuthenticationProvider, EntityDomainAuthorizeOption, EntityQuerySelectOption<T, TModel>, IViewModel<TModel>>(context, domain.ListViewModel);
        }

        public static Task<IEntityEditModel<T>> ExecuteCreate<T>(this EntityDomainService<T> domain, IDomainContext context)
            where T : class, IEntity, new()
        {
            return domain.ExecuteAsync<IDatabaseContext, IAuthenticationProvider, EntityDomainAuthorizeOption, IEntityEditModel<T>>(context, domain.Create);
        }

        public static Task<IEntityEditModel<T>> ExecuteEdit<T>(this EntityDomainService<T> domain, IDomainContext context)
            where T : class, IEntity, new()
        {
            return domain.ExecuteAsync<IDatabaseContext, IAuthenticationProvider, IValueProvider, EntityDomainAuthorizeOption, IEntityEditModel<T>>(context, domain.Edit);
        }

        public static Task<IEntityUpdateModel<T>> ExecuteUpdate<T>(this EntityDomainService<T> domain, IDomainContext context)
            where T : class, IEntity, new()
        {
            return domain.ExecuteAsync<IDatabaseContext, IAuthenticationProvider, IValueProvider, EntityDomainAuthorizeOption, IEntityUpdateModel<T>>(context, domain.Update);
        }

        public static Task ExecuteRemove<T>(this EntityDomainService<T> domain, IDomainContext context)
            where T : class, IEntity, new()
        {
            return domain.ExecuteAsync<IDatabaseContext, IAuthenticationProvider, IValueProvider, EntityDomainAuthorizeOption>(context, domain.Remove);
        }

        public static Task<IEntityEditModel<T>> ExecuteDetail<T>(this EntityDomainService<T> domain, IDomainContext context)
            where T : class, IEntity, new()
        {
            return domain.ExecuteAsync<IDatabaseContext, IAuthenticationProvider, IValueProvider, EntityDomainAuthorizeOption, IEntityEditModel<T>>(context, domain.Detail);
        }
    }
}
