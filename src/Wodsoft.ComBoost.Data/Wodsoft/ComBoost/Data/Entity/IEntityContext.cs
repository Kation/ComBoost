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
        where T : IEntity
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
        /// 异步将查询结果转为数组。
        /// </summary>
        /// <param name="query">查询对象。</param>
        /// <returns>返回异步执行的数组查询结果。</returns>
        Task<T[]> ToArrayAsync(IQueryable<T> query);
        /// <summary>
        /// 异步将查询结果转为列表。
        /// </summary>
        /// <param name="query">查询对象。</param>
        /// <returns>返回异步执行的列表查询结果。</returns>
        Task<List<T>> ToListAsync(IQueryable<T> query);
        /// <summary>
        /// 异步查找出单个对象。
        /// </summary>
        /// <param name="query">查询对象。</param>
        /// <param name="expression">表达式。</param>
        /// <returns></returns>
        Task<T> SingleOrDefaultAsync(IQueryable<T> query, Expression<Func<T, bool>> expression);
        /// <summary>
        /// 异步查找出单个对象。
        /// </summary>
        /// <param name="query">查询对象。</param>
        /// <param name="expression">表达式。</param>
        /// <returns></returns>
        Task<T> SingleAsync(IQueryable<T> query, Expression<Func<T, bool>> expression);
        /// <summary>
        /// 异步查找出第一个对象。
        /// </summary>
        /// <param name="query">查询对象。</param>
        /// <param name="expression">表达式。</param>
        /// <returns></returns>
        Task<T> FirstOrDefaultAsync(IQueryable<T> query, Expression<Func<T, bool>> expression);
        /// <summary>
        /// 异步查找出第一个对象。
        /// </summary>
        /// <param name="query">查询对象。</param>
        /// <param name="expression">表达式。</param>
        /// <returns></returns>
        Task<T> FirstAsync(IQueryable<T> query, Expression<Func<T, bool>> expression);
        /// <summary>
        /// 异步查找出最后一个对象。
        /// </summary>
        /// <param name="query">查询对象。</param>
        /// <param name="expression">表达式。</param>
        /// <returns></returns>
        Task<T> LastOrDefaultAsync(IQueryable<T> query, Expression<Func<T, bool>> expression);
        /// <summary>
        /// 异步查找出最后一个对象。
        /// </summary>
        /// <param name="query">查询对象。</param>
        /// <param name="expression">表达式。</param>
        /// <returns></returns>
        Task<T> LastAsync(IQueryable<T> query, Expression<Func<T, bool>> expression);
        /// <summary>
        /// 异步统计查询结果的数量。
        /// </summary>
        /// <param name="query">查询对象。</param>
        /// <returns></returns>
        Task<int> CountAsync(IQueryable<T> query);
        /// <summary>
        /// 异步统计查询结果的数量。
        /// </summary>
        /// <param name="query">查询对象。</param>
        /// <param name="expression">表达式。</param>
        /// <returns></returns>
        Task<int> CountAsync(IQueryable<T> query, Expression<Func<T, bool>> expression);
    }
}
