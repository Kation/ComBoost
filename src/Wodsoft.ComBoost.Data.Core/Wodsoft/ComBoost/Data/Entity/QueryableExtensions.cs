using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using Wodsoft.ComBoost.Data.Entity.Metadata;
using System.Reflection;

namespace Wodsoft.ComBoost.Data.Entity
{
    public static class QueryableExtensions
    {
        public static IAsyncQueryable<T> Wrap<T, M>(this IAsyncQueryable<M> queryable)
            where T : IEntity
            where M : IEntity, T
        {
            WrappedQueryableProvider<T, M> provider = new WrappedQueryableProvider<T, M>(queryable.Provider);
            return provider.CreateQuery<T>(queryable.Expression);
        }

        public static object Wrap(this object value)
        {
            return value;
        }

        public static IAsyncQueryable<M> Unwrap<T, M>(this IAsyncQueryable<T> queryable)
            where T : IEntity
            where M : IEntity, T
        {
            WrappedQueryable<T, M> wrapped = queryable as WrappedQueryable<T, M>;
            if (wrapped == null)
                throw new NotSupportedException("不支持的类型。");
            WrappedQueryableProvider<T, M> provider = wrapped.Provider;
            var visitor = new ExpressionWrapper<T, M>();
            var expression = visitor.Visit(wrapped.Expression);
            return provider.InnerQueryProvider.CreateQuery<M>(expression);
        }

        public static bool IsWrapped(this IQueryable queryable)
        {
            return queryable is IWrappedQueryable;
        }

        public static IAsyncQueryable<TResult> SelectMany<TSource, TResult>(this IAsyncQueryable<TSource> source, Expression<Func<TSource, int, IEnumerable<TResult>>> selector)
        {
            return source.Provider.CreateQuery<TResult>(Expression.Call(((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(TSource), typeof(TResult)), source.Expression, Expression.Quote(selector)));
        }

        public static IAsyncQueryable<TResult> SelectMany<TSource, TResult>(this IAsyncQueryable<TSource> source, Expression<Func<TSource, IEnumerable<TResult>>> selector)
        {
            return source.Provider.CreateQuery<TResult>(Expression.Call(((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(TSource), typeof(TResult)), source.Expression, Expression.Quote(selector)));
        }

        public static IAsyncQueryable<TResult> SelectMany<TSource, TCollection, TResult>(this IAsyncQueryable<TSource> source, Expression<Func<TSource, IEnumerable<TCollection>>> collectionSelector, Expression<Func<TSource, TCollection, TResult>> resultSelector)
        {
            return source.Provider.CreateQuery<TResult>(Expression.Call(((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(TSource), typeof(TCollection), typeof(TResult)), source.Expression, Expression.Quote(collectionSelector), Expression.Quote(resultSelector)));
        }

        public static IAsyncQueryable<TResult> SelectMany<TSource, TCollection, TResult>(this IAsyncQueryable<TSource> source, Expression<Func<TSource, int, IEnumerable<TCollection>>> collectionSelector, Expression<Func<TSource, TCollection, TResult>> resultSelector)
        {
            return source.Provider.CreateQuery<TResult>(Expression.Call(((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(TSource), typeof(TCollection), typeof(TResult)), source.Expression, Expression.Quote(collectionSelector), Expression.Quote(resultSelector)));
        }
    }
}
