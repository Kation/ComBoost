using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Data.Entity
{
    /// <summary>
    /// 实体仓储上下文。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    public interface IEntityContext<T> : IEntityQueryContext<T>
        where T : class, IEntity
    {
        /// <summary>
        /// 将实体添加到数据库。
        /// </summary>
        /// <param name="item">要插入的实体。</param>
        void Add(T item);
        /// <summary>
        /// 将多个实体添加到数据库。
        /// </summary>
        /// <param name="items">要插入的实体。</param>
        void AddRange(IEnumerable<T> items);
        /// <summary>
        /// 创建新实体。
        /// </summary>
        /// <returns>返回实体实例。</returns>
        T Create();
        /// <summary>
        /// 更新实体数据到数据库。
        /// </summary>
        /// <param name="item">要更新的实体。</param>
        void Update(T item);
        /// <summary>
        /// 更新实体数据到数据库。
        /// </summary>
        /// <param name="items">要更新的实体。</param>
        void UpdateRange(IEnumerable<T> items);
        /// <summary>
        /// 从数据库删除实体。
        /// </summary>
        /// <param name="item">要删除的实体。</param>
        void Remove(T item);
        /// <summary>
        /// 从数据库删除实体。
        /// </summary>
        /// <param name="items">要删除的实体。</param>
        void RemoveRange(IEnumerable<T> items);
        /// <summary>
        /// 从上下文分离实体。
        /// </summary>
        /// <param name="item">要分离的实体。</param>
        void Detach(T item);
        /// <summary>
        /// 从上下文分离实体。
        /// </summary>
        /// <param name="items">要分离的实体。</param>
        void DetachRange(IEnumerable<T> items);
    }
}
