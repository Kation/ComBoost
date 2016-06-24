using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public static class EntityExtensions
    {

        public static Task<T> LazyLoadEntityAsync<TSource, T>(this TSource source, Expression<Func<TSource, T>> expression)
            where TSource : IEntity
            where T : class, IEntity, new()
        {
            return source.EntityContext.LazyLoadEntityAsync(source, expression);
        }

        public static Task<IQueryableCollection<T>> LazyLoadCollectionAsync<TSource, T>(this TSource source, Expression<Func<TSource, ICollection<T>>> expression)
            where TSource : IEntity
            where T : class, IEntity, new()
        {
            return source.EntityContext.LazyLoadCollectionAsync(source, expression);
        }
    }
}
