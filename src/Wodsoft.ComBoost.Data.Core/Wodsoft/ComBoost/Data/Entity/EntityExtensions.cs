using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public static class EntityExtensions
    {
        ///// <summary>
        ///// 加载导航实体。
        ///// </summary>
        ///// <typeparam name="TSource">当前实体类型。</typeparam>
        ///// <typeparam name="T">目标实体类型。</typeparam>
        ///// <param name="source">当前实体。</param>
        ///// <param name="expression">导航属性表达式。</param>
        ///// <returns>返回导航至的实体。</returns>
        //public static Task<T> LoadAsync<TSource, T>(this TSource source, Expression<Func<TSource, T>> expression)
        //    where TSource : class, IEntity
        //    where T : class, IEntity
        //{
        //    return DatabaseContextAccessor.Context.LoadAsync(source, expression);
        //}

        ///// <summary>
        ///// 加载导航集合。
        ///// </summary>
        ///// <typeparam name="TSource">当前实体类型。</typeparam>
        ///// <typeparam name="T">目标实体类型。</typeparam>
        ///// <param name="source">当前实体。</param>
        ///// <param name="expression">导航属性表达式。</param>
        ///// <returns>返回导航至的集合。</returns>
        //public static Task<IQueryableCollection<T>> LoadAsync<TSource, T>(this TSource source, Expression<Func<TSource, ICollection<T>>> expression)
        //    where TSource : class, IEntity
        //    where T : class, IEntity
        //{
        //    return DatabaseContextAccessor.Context.LoadAsync(source, expression);
        //}
    }
}
