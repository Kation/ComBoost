using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    /// <summary>
    /// 数据库上下文。
    /// </summary>
    public interface IDatabaseContext
    {
        /// <summary>
        /// 获取支持的实体类型。
        /// </summary>
        IEnumerable<Type> SupportTypes { get; }

        /// <summary>
        /// 获取实体上下文。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <returns>返回实体上下文。</returns>
        IEntityContext<T> GetContext<T>() where T : class, IEntity, new();

        /// <summary>
        /// 保存更改。
        /// </summary>
        /// <returns>返回受影响的行数。</returns>
        Task<int> SaveAsync();

        /// <summary>
        /// 加载实体对象。
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="entity"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<TResult> LoadAsync<TSource, TResult>(TSource entity, Expression<Func<TSource, TResult>> expression)
            where TSource : class, IEntity
            where TResult : class, IEntity;

        /// <summary>
        /// 加载实体集合。
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="entity"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<IQueryableCollection<TResult>> LoadAsync<TSource, TResult>(TSource entity, Expression<Func<TSource, ICollection<TResult>>> expression)
            where TSource : class, IEntity
            where TResult : class, IEntity;
        
        /// <summary>
        /// 执行非查询Sql语句。
        /// </summary>
        /// <param name="sql">Sql语句。</param>
        /// <param name="parameters">Sql参数。</param>
        /// <returns></returns>
        Task<int> ExecuteNonQueryAsync(string sql, params object[] parameters);

        /// <summary>
        /// 执行Sql语句并返回标量。
        /// </summary>
        /// <typeparam name="TValue">返回类型。</typeparam>
        /// <param name="sql">Sql语句。</param>
        /// <param name="parameters">Sql参数。</param>
        /// <returns></returns>
        Task<TValue> ExecuteScalarAsync<TValue>(string sql, params object[] parameters);
    }
}
