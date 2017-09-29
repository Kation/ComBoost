using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public static class AsyncQueryableExtensions
    {
        public static IAsyncQueryable Context
        {
            get { return RequestScope.Current.Get<IAsyncQueryable>("__IAsyncQueryable"); }
            set
            {
                RequestScope.Current["__IAsyncQueryable"] = value;
            }
        }

        public static Task<bool> AllAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AllAsync(source, predicate, cancellationToken);
        }
        public static Task<bool> AnyAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AnyAsync(source, cancellationToken);
        }
        public static Task<bool> AnyAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AnyAsync(source, predicate, cancellationToken);
        }
        public static Task<double> AverageAsync(this IQueryable<int> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AverageAsync(source, cancellationToken);
        }
        public static Task<double?> AverageAsync(this IQueryable<int?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AverageAsync(source, cancellationToken);
        }
        public static Task<double> AverageAsync(this IQueryable<long> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AverageAsync(source, cancellationToken);
        }
        public static Task<double?> AverageAsync(this IQueryable<long?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AverageAsync(source, cancellationToken);
        }
        public static Task<double> AverageAsync(this IQueryable<double> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AverageAsync(source, cancellationToken);
        }
        public static Task<double?> AverageAsync(this IQueryable<double?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AverageAsync(source, cancellationToken);
        }
        public static Task<decimal?> AverageAsync(this IQueryable<decimal?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AverageAsync(source, cancellationToken);
        }
        public static Task<float?> AverageAsync(this IQueryable<float?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AverageAsync(source, cancellationToken);
        }
        public static Task<float> AverageAsync(this IQueryable<float> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AverageAsync(source, cancellationToken);
        }
        public static Task<decimal> AverageAsync(this IQueryable<decimal> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AverageAsync(source, cancellationToken);
        }
        public static Task<decimal> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AverageAsync(source, selector, cancellationToken);
        }
        public static Task<decimal?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AverageAsync(source, selector, cancellationToken);
        }
        public static Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AverageAsync(source, selector, cancellationToken);
        }
        public static Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AverageAsync(source, selector, cancellationToken);
        }
        public static Task<float> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AverageAsync(source, selector, cancellationToken);
        }
        public static Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AverageAsync(source, selector, cancellationToken);
        }
        public static Task<float?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AverageAsync(source, selector, cancellationToken);
        }
        public static Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AverageAsync(source, selector, cancellationToken);
        }
        public static Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AverageAsync(source, selector, cancellationToken);
        }
        public static Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.AverageAsync(source, selector, cancellationToken);
        }
        public static Task<bool> ContainsAsync<TSource>(this IQueryable<TSource> source, TSource item, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.ContainsAsync(source, item, cancellationToken);
        }
        public static Task<int> CountAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.CountAsync(source, cancellationToken);
        }
        public static Task<int> CountAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.CountAsync(source, predicate, cancellationToken);
        }
        public static Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.FirstAsync(source, cancellationToken);
        }
        public static Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.FirstAsync(source, predicate, cancellationToken);
        }
        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.FirstOrDefaultAsync(source, cancellationToken);
        }
        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.FirstOrDefaultAsync(source, predicate, cancellationToken);
        }
        public static IQueryable<TEntity> Include<TEntity, TProperty>(this IQueryable<TEntity> source, Expression<Func<TEntity, TProperty>> navigationPropertyPath) where TEntity : class
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            source = Context.Include(source, navigationPropertyPath);
            if (isWrapped)
            {
                IQueryProvider provider = (IQueryProvider)Activator.CreateInstance(typeof(WrappedQueryableProvider<,>).MakeGenericType(args), source.Provider);
                source = provider.CreateQuery<TEntity>(source.Expression);
            }
            return source;
        }
        public static Task<TSource> LastAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.LastAsync(source, cancellationToken);
        }
        public static Task<TSource> LastAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.LastAsync(source, predicate, cancellationToken);
        }
        public static Task<TSource> LastOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.LastOrDefaultAsync(source, cancellationToken);
        }
        public static Task<TSource> LastOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.LastOrDefaultAsync(source, predicate, cancellationToken);
        }
        public static Task<long> LongCountAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.LongCountAsync(source, cancellationToken);
        }
        public static Task<long> LongCountAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.LongCountAsync(source, predicate, cancellationToken);
        }
        public static Task<TSource> MaxAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.MaxAsync(source, cancellationToken);
        }
        public static Task<TResult> MaxAsync<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.MaxAsync(source, selector, cancellationToken);
        }
        public static Task<TSource> MinAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.MinAsync(source, cancellationToken);
        }
        public static Task<TResult> MinAsync<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.MinAsync(source, selector, cancellationToken);
        }
        public static Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SingleAsync(source, cancellationToken);
        }
        public static Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SingleAsync(source, predicate, cancellationToken);
        }
        public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SingleOrDefaultAsync(source, cancellationToken);
        }
        public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SingleOrDefaultAsync(source, predicate, cancellationToken);
        }
        public static Task<long?> SumAsync(this IQueryable<long?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SumAsync(source, cancellationToken);
        }
        public static Task<float> SumAsync(this IQueryable<float> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SumAsync(source, cancellationToken);
        }
        public static Task<decimal?> SumAsync(this IQueryable<decimal?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SumAsync(source, cancellationToken);
        }
        public static Task<double> SumAsync(this IQueryable<double> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SumAsync(source, cancellationToken);
        }
        public static Task<int> SumAsync(this IQueryable<int> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SumAsync(source, cancellationToken);
        }
        public static Task<int?> SumAsync(this IQueryable<int?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SumAsync(source, cancellationToken);
        }
        public static Task<double?> SumAsync(this IQueryable<double?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SumAsync(source, cancellationToken);
        }
        public static Task<long> SumAsync(this IQueryable<long> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SumAsync(source, cancellationToken);
        }
        public static Task<decimal> SumAsync(this IQueryable<decimal> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SumAsync(source, cancellationToken);
        }
        public static Task<float?> SumAsync(this IQueryable<float?> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SumAsync(source, cancellationToken);
        }
        public static Task<long?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SumAsync(source, selector, cancellationToken);
        }
        public static Task<double> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SumAsync(source, selector, cancellationToken);
        }
        public static Task<double?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SumAsync(source, selector, cancellationToken);
        }
        public static Task<long> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SumAsync(source, selector, cancellationToken);
        }
        public static Task<float> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SumAsync(source, selector, cancellationToken);
        }
        public static Task<float?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SumAsync(source, selector, cancellationToken);
        }
        public static Task<int?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SumAsync(source, selector, cancellationToken);
        }
        public static Task<int> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SumAsync(source, selector, cancellationToken);
        }
        public static Task<decimal?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SumAsync(source, selector, cancellationToken);
        }
        public static Task<decimal> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.SumAsync(source, selector, cancellationToken);
        }
        public static Task<TSource[]> ToArrayAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.ToArrayAsync(source, cancellationToken);
        }
        public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.ToDictionaryAsync(source, keySelector, cancellationToken);
        }
        public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.ToDictionaryAsync(source, keySelector, comparer, cancellationToken);
        }
        public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.ToDictionaryAsync(source, keySelector, elementSelector, cancellationToken);
        }
        public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.ToDictionaryAsync(source, keySelector, elementSelector, comparer, cancellationToken);
        }
        public static Task<List<TSource>> ToListAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            source = Unwrap(source, out bool isWrapped, out Type[] args);
            return Context.ToListAsync(source, cancellationToken);
        }

        private static readonly MethodInfo UnwrapMethod = typeof(QueryableExtensions).GetMethod("Unwrap");
        private static IQueryable<TSource> Unwrap<TSource>(IQueryable<TSource> source, out bool isWrapped, out Type[] args)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            Type type = source.GetType();
            if (source is IWrappedQueryable)
            {
                isWrapped = true;
                args = type.GetGenericArguments();
                var method = UnwrapMethod.MakeGenericMethod(args);
                return (IQueryable<TSource>)method.Invoke(null, new object[] { source });
            }
            isWrapped = false;
            args = null;
            return source;
        }
    }
}
