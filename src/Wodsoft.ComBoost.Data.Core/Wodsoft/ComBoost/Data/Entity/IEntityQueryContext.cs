using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Data.Entity
{
    /// <summary>
    /// 实体查询上下文。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    public interface IEntityQueryContext<T>
        where T : IEntity
    {
        /// <summary>
        /// 获取相关数据库上下文。
        /// </summary>
        IDatabaseContext Database { get; }

        /// <summary>
        /// 获取实体元数据。
        /// </summary>
        IEntityMetadata Metadata { get; }

        /// <summary>
        /// 获取查询接口。
        /// </summary>
        /// <returns>返回查询对象。</returns>
        IQueryable<T> Query();

        /// <summary>
        /// 获取实体。
        /// </summary>
        /// <param name="keys">主键。</param>
        /// <returns>返回实体对象，可能为空。</returns>
        Task<T> GetAsync(params object[] keys);

        IQueryable<TChildren> QueryChildren<TChildren>(T item, Expression<Func<T, ICollection<TChildren>>> childrenSelector)
            where TChildren : class;

        Task LoadPropertyAsync<TProperty>(T item, Expression<Func<T, TProperty>> propertySelector)
            where TProperty : class;
    }
}
