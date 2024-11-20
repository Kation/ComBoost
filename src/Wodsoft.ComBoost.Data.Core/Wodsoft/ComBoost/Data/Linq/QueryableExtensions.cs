using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Data.Linq
{
    public static class QueryableExtensions
    {
        #region Wrap

        public static IQueryable<T> Wrap<T, M>(this IQueryable<M> queryable)
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

        public static IQueryable<M> Unwrap<T, M>(this IQueryable<T> queryable)
            where T : IEntity
            where M : IEntity, T
        {
            WrappedQueryable<T, M>? wrapped = queryable as WrappedQueryable<T, M>;
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

        #endregion

        #region All

        private static readonly MethodInfo _AllAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethod(nameof(AllAsync));
        public static Task<bool> AllAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<bool>(Expression.Call(
                            instance: null,
                            method: _AllAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(predicate), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        #endregion

        #region Any

        private static readonly MethodInfo _AnyAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(AnyAsync))
                .First(t => t.GetParameters().Length == 2);
        public static Task<bool> AnyAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<bool>(Expression.Call(
                            instance: null,
                            method: _AnyAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _AnyPredicateAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(AnyAsync))
                .First(t => t.GetParameters().Length == 3);
        public static Task<bool> AnyAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<bool>(Expression.Call(
                            instance: null,
                            method: _AnyPredicateAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(predicate), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        #endregion

        #region Average

        private static readonly MethodInfo _AverageLongToDoubleNullableAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(AverageAsync))
                .First(t => t.IsGenericMethod && t.ReturnType == typeof(Task<double?>) && t.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments()[1] == typeof(long?));
        public static Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<double?>(Expression.Call(
                            instance: null,
                            method: _AverageLongToDoubleNullableAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _AverageLongToDoubleAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(AverageAsync))
                .First(t => t.IsGenericMethod && t.ReturnType == typeof(Task<double>) && t.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments()[1] == typeof(long));
        public static Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<double>(Expression.Call(
                            instance: null,
                            method: _AverageLongToDoubleAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _AverageFloatNullableAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(AverageAsync))
                .First(t => t.IsGenericMethod && t.ReturnType == typeof(Task<float?>));
        public static Task<float?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<float?>(Expression.Call(
                            instance: null,
                            method: _AverageFloatNullableAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _AverageFloatAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(AverageAsync))
                .First(t => t.IsGenericMethod && t.ReturnType == typeof(Task<float>));
        public static Task<float> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<float>(Expression.Call(
                            instance: null,
                            method: _AverageFloatAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _AverageFloatNullableNonSelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(AverageAsync))
                .First(t => !t.IsGenericMethod && t.ReturnType == typeof(Task<float?>));
        public static Task<float?> AverageAsync(this IQueryable<float?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<float?>(Expression.Call(
                            instance: null,
                            method: _AverageFloatNullableNonSelectAsyncMethodInfo,
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _AverageFloatNonSelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(AverageAsync))
                .First(t => !t.IsGenericMethod && t.ReturnType == typeof(Task<float>));
        public static Task<float> AverageAsync(this IQueryable<float> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<float>(Expression.Call(
                            instance: null,
                            method: _AverageFloatNonSelectAsyncMethodInfo,
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _AverageDoubleNullableAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(AverageAsync))
                .First(t => t.IsGenericMethod && t.ReturnType == typeof(Task<double?>) && t.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments()[1] == typeof(double?));
        public static Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<double?>(Expression.Call(
                            instance: null,
                            method: _AverageDoubleNullableAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _AverageDoubleAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(AverageAsync))
                .First(t => t.IsGenericMethod && t.ReturnType == typeof(Task<double>) && t.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments()[1] == typeof(double));
        public static Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<double>(Expression.Call(
                            instance: null,
                            method: _AverageDoubleAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _AverageDoubleNullableNonSelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(AverageAsync))
                .First(t => !t.IsGenericMethod && t.ReturnType == typeof(Task<double?>) && t.GetParameters()[0].ParameterType.GetGenericArguments()[0] == typeof(double?));
        public static Task<double?> AverageAsync(this IQueryable<double?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<double?>(Expression.Call(
                            instance: null,
                            method: _AverageDoubleNullableNonSelectAsyncMethodInfo,
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _AverageDecimalNonSelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(AverageAsync))
                .First(t => !t.IsGenericMethod && t.ReturnType == typeof(Task<decimal>));
        public static Task<decimal> AverageAsync(this IQueryable<decimal> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<decimal>(Expression.Call(
                            instance: null,
                            method: _AverageDecimalNonSelectAsyncMethodInfo,
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _AverageDoubleNonSelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(AverageAsync))
                .First(t => !t.IsGenericMethod && t.ReturnType == typeof(Task<double>) && t.GetParameters()[0].ParameterType.GetGenericArguments()[0] == typeof(double));
        public static Task<double> AverageAsync(this IQueryable<double> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<double>(Expression.Call(
                            instance: null,
                            method: _AverageDoubleNonSelectAsyncMethodInfo,
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _AverageDecimalAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(AverageAsync))
                .First(t => t.IsGenericMethod && t.ReturnType == typeof(Task<decimal>));
        public static Task<decimal> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<decimal>(Expression.Call(
                            instance: null,
                            method: _AverageDecimalAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _AverageDecimalNullableAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(AverageAsync))
                .First(t => t.IsGenericMethod && t.ReturnType == typeof(Task<decimal?>));
        public static Task<decimal?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<decimal?>(Expression.Call(
                            instance: null,
                            method: _AverageDecimalNullableAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _AverageIntToDoubleNonSelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(AverageAsync))
                .First(t => !t.IsGenericMethod && t.ReturnType == typeof(Task<double>) && t.GetParameters()[0].ParameterType.GetGenericArguments()[0] == typeof(int));
        public static Task<double> AverageAsync(this IQueryable<int> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<double>(Expression.Call(
                            instance: null,
                            method: _AverageIntToDoubleNonSelectAsyncMethodInfo,
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _AverageIntToDoubleNullableNonSelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(AverageAsync))
                .First(t => !t.IsGenericMethod && t.ReturnType == typeof(Task<double?>) && t.GetParameters()[0].ParameterType.GetGenericArguments()[0] == typeof(int?));
        public static Task<double?> AverageAsync(this IQueryable<int?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<double?>(Expression.Call(
                            instance: null,
                            method: _AverageIntToDoubleNullableNonSelectAsyncMethodInfo,
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _AverageIntToDoubleAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(AverageAsync))
                .First(t => t.IsGenericMethod && t.ReturnType == typeof(Task<double>) && t.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments()[1] == typeof(int));
        public static Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<double>(Expression.Call(
                            instance: null,
                            method: _AverageIntToDoubleAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _AverageIntToDoubleNullableAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(AverageAsync))
                .First(t => t.IsGenericMethod && t.ReturnType == typeof(Task<double?>) && t.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments()[1] == typeof(int?));
        public static Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<double?>(Expression.Call(
                            instance: null,
                            method: _AverageIntToDoubleNullableAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _AverageLongToDoubleNonSelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(AverageAsync))
                .First(t => !t.IsGenericMethod && t.ReturnType == typeof(Task<double>) && t.GetParameters()[0].ParameterType.GetGenericArguments()[0] == typeof(long));
        public static Task<double> AverageAsync(this IQueryable<long> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<double>(Expression.Call(
                            instance: null,
                            method: _AverageLongToDoubleNonSelectAsyncMethodInfo,
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _AverageLongToDoubleNullableNonSelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(AverageAsync))
                .First(t => !t.IsGenericMethod && t.ReturnType == typeof(Task<double?>) && t.GetParameters()[0].ParameterType.GetGenericArguments()[0] == typeof(int?));
        public static Task<double?> AverageAsync(this IQueryable<long?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<double?>(Expression.Call(
                            instance: null,
                            method: _AverageLongToDoubleNullableNonSelectAsyncMethodInfo,
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _AverageDecimalNullableNonSelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(AverageAsync))
                .First(t => !t.IsGenericMethod && t.ReturnType == typeof(Task<decimal?>));
        public static Task<decimal?> AverageAsync(this IQueryable<decimal?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<decimal?>(Expression.Call(
                            instance: null,
                            method: _AverageDecimalNullableNonSelectAsyncMethodInfo,
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        #endregion

        #region Count

        private static readonly MethodInfo _CountAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(CountAsync))
                .First(t => t.GetParameters().Length == 2);
        public static Task<int> CountAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<int>(Expression.Call(
                            instance: null,
                            method: _CountAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _CountPredicateAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(CountAsync))
                .First(t => t.GetParameters().Length == 3);
        public static Task<int> CountAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<int>(Expression.Call(
                            instance: null,
                            method: _CountPredicateAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(predicate), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _LongCountAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(LongCountAsync))
                .First(t => t.GetParameters().Length == 2);
        public static Task<long> LongCountAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<long>(Expression.Call(
                            instance: null,
                            method: _LongCountAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _LongCountPredicateAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(LongCountAsync))
                .First(t => t.GetParameters().Length == 3);
        public static Task<long> LongCountAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<long>(Expression.Call(
                            instance: null,
                            method: _LongCountPredicateAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(predicate), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }


        #endregion

        #region First

        private static readonly MethodInfo _FirstAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(FirstAsync))
                .First(t => t.GetParameters().Length == 2);
        public static Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<TSource>(Expression.Call(
                            instance: null,
                            method: _FirstAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _FirstPredicateAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(FirstAsync))
                .First(t => t.GetParameters().Length == 3);
        public static Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<TSource>(Expression.Call(
                            instance: null,
                            method: _FirstPredicateAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(predicate), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _FirstOrDefaultAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(FirstOrDefaultAsync))
                .First(t => t.GetParameters().Length == 2);
        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<TSource>(Expression.Call(
                            instance: null,
                            method: _FirstOrDefaultAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _FirstOrDefaultPredicateAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(FirstOrDefaultAsync))
                .First(t => t.GetParameters().Length == 3);
        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<TSource>(Expression.Call(
                            instance: null,
                            method: _FirstOrDefaultPredicateAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(predicate), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        #endregion

        #region Last

        private static readonly MethodInfo _LastAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(LastAsync))
                .First(t => t.GetParameters().Length == 2);
        public static Task<TSource> LastAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<TSource>(Expression.Call(
                            instance: null,
                            method: _LastAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _LastPredicateAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(LastAsync))
                .First(t => t.GetParameters().Length == 3);
        public static Task<TSource> LastAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<TSource>(Expression.Call(
                            instance: null,
                            method: _LastPredicateAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(predicate), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _LastOrDefaultAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(LastOrDefaultAsync))
                .First(t => t.GetParameters().Length == 2);
        public static Task<TSource> LastOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<TSource>(Expression.Call(
                            instance: null,
                            method: _LastOrDefaultAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _LastOrDefaultPredicateAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(LastOrDefaultAsync))
                .First(t => t.GetParameters().Length == 3);
        public static Task<TSource> LastOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<TSource>(Expression.Call(
                            instance: null,
                            method: _LastOrDefaultPredicateAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(predicate), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        #endregion

        #region Max

        private static readonly MethodInfo _MaxNonSelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(MaxAsync))
                .First(t => t.GetParameters().Length == 2);
        public static Task<TSource> MaxAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<TSource>(Expression.Call(
                            instance: null,
                            method: _MaxNonSelectAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _MaxAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(MaxAsync))
                .First(t => t.GetParameters().Length == 3);
        public static Task<TResult> MaxAsync<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<TResult>(Expression.Call(
                            instance: null,
                            method: _MaxAsyncMethodInfo.MakeGenericMethod(typeof(TSource), typeof(TResult)),
                            arguments: new[] { source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        #endregion

        #region Min

        private static readonly MethodInfo _MinNonSelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(MinAsync))
                .First(t => t.GetParameters().Length == 2);
        public static Task<TSource> MinAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<TSource>(Expression.Call(
                            instance: null,
                            method: _MinNonSelectAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _MinAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(MinAsync))
                .First(t => t.GetParameters().Length == 3);
        public static Task<TResult> MinAsync<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<TResult>(Expression.Call(
                            instance: null,
                            method: _MinAsyncMethodInfo.MakeGenericMethod(typeof(TSource), typeof(TResult)),
                            arguments: new[] { source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        #endregion

        #region Single

        private static readonly MethodInfo _SingleAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SingleAsync))
                .First(t => t.GetParameters().Length == 2);
        public static Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<TSource>(Expression.Call(
                            instance: null,
                            method: _SingleAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _SinglePredicateAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SingleAsync))
                .First(t => t.GetParameters().Length == 3);
        public static Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<TSource>(Expression.Call(
                            instance: null,
                            method: _SinglePredicateAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(predicate), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _SingleOrDefaultAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SingleOrDefaultAsync))
                .First(t => t.GetParameters().Length == 2);
        public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<TSource>(Expression.Call(
                            instance: null,
                            method: _SingleOrDefaultAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _SingleOrDefaultPredicateAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SingleOrDefaultAsync))
                .First(t => t.GetParameters().Length == 3);
        public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<TSource>(Expression.Call(
                            instance: null,
                            method: _SingleOrDefaultPredicateAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(predicate), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        #endregion

        #region Sum

        private static readonly MethodInfo _SumFloatAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SumAsync))
                .First(t => t.IsGenericMethod && t.ReturnType == typeof(Task<float>));
        public static Task<float> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<float>(Expression.Call(
                            instance: null,
                            method: _SumFloatAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _SumFloatNullableNonSelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SumAsync))
                .First(t => !t.IsGenericMethod && t.ReturnType == typeof(Task<float?>));
        public static Task<float?> SumAsync(this IQueryable<float?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<float?>(Expression.Call(
                            instance: null,
                            method: _SumFloatNullableNonSelectAsyncMethodInfo,
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _SumDecimalNonSelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SumAsync))
                .First(t => !t.IsGenericMethod && t.ReturnType == typeof(Task<decimal>));
        public static Task<decimal> SumAsync(this IQueryable<decimal> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<decimal>(Expression.Call(
                            instance: null,
                            method: _SumDecimalNonSelectAsyncMethodInfo,
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _SumDecimalNullableNonSelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SumAsync))
                .First(t => !t.IsGenericMethod && t.ReturnType == typeof(Task<decimal?>));
        public static Task<decimal?> SumAsync(this IQueryable<decimal?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<decimal?>(Expression.Call(
                            instance: null,
                            method: _SumDecimalNullableNonSelectAsyncMethodInfo,
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _SumDecimalAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SumAsync))
                .First(t => t.IsGenericMethod && t.ReturnType == typeof(Task<decimal>));
        public static Task<decimal> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<decimal>(Expression.Call(
                            instance: null,
                            method: _SumDecimalAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _SumDecimalNullableAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SumAsync))
                .First(t => t.IsGenericMethod && t.ReturnType == typeof(Task<decimal?>));
        public static Task<decimal?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<decimal?>(Expression.Call(
                            instance: null,
                            method: _SumDecimalNullableAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _SumIntNonSelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SumAsync))
                .First(t => !t.IsGenericMethod && t.ReturnType == typeof(Task<int>));
        public static Task<int> SumAsync(this IQueryable<int> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<int>(Expression.Call(
                            instance: null,
                            method: _SumIntNonSelectAsyncMethodInfo,
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _SumIntNullableNonSelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SumAsync))
                .First(t => !t.IsGenericMethod && t.ReturnType == typeof(Task<int?>));
        public static Task<int?> SumAsync(this IQueryable<int?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<int?>(Expression.Call(
                            instance: null,
                            method: _SumIntNullableNonSelectAsyncMethodInfo,
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _SumIntAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SumAsync))
                .First(t => t.IsGenericMethod && t.ReturnType == typeof(Task<int>));
        public static Task<int> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<int>(Expression.Call(
                            instance: null,
                            method: _SumIntAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _SumFloatNullableAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SumAsync))
                .First(t => t.IsGenericMethod && t.ReturnType == typeof(Task<float?>));
        public static Task<float?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<float?>(Expression.Call(
                            instance: null,
                            method: _SumFloatNullableAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _SumLongNonSelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SumAsync))
                .First(t => !t.IsGenericMethod && t.ReturnType == typeof(Task<long>));
        public static Task<long> SumAsync(this IQueryable<long> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<long>(Expression.Call(
                            instance: null,
                            method: _SumLongNonSelectAsyncMethodInfo,
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _SumIntNullableAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SumAsync))
                .First(t => t.IsGenericMethod && t.ReturnType == typeof(Task<int?>));
        public static Task<int?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<int?>(Expression.Call(
                            instance: null,
                            method: _SumIntNullableAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _SumDoubleAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SumAsync))
                .First(t => t.IsGenericMethod && t.ReturnType == typeof(Task<double>) && t.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments()[1] == typeof(double));
        public static Task<double> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<double>(Expression.Call(
                            instance: null,
                            method: _SumDoubleAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _SumFloatNonSelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SumAsync))
                .First(t => !t.IsGenericMethod && t.ReturnType == typeof(Task<float>));
        public static Task<float> SumAsync(this IQueryable<float> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<float>(Expression.Call(
                            instance: null,
                            method: _SumFloatNonSelectAsyncMethodInfo,
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _SumDoubleNullableAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SumAsync))
                .First(t => t.IsGenericMethod && t.ReturnType == typeof(Task<double?>) && t.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments()[1] == typeof(double?));
        public static Task<double?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<double?>(Expression.Call(
                            instance: null,
                            method: _SumDoubleNullableAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _SumLongAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SumAsync))
                .First(t => t.IsGenericMethod && t.ReturnType == typeof(Task<long>) && t.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments()[1] == typeof(long));
        public static Task<long> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<long>(Expression.Call(
                            instance: null,
                            method: _SumLongAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _SumLongNullableAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SumAsync))
                .First(t => t.IsGenericMethod && t.ReturnType == typeof(Task<long?>) && t.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments()[1] == typeof(long?));
        public static Task<long?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<long?>(Expression.Call(
                            instance: null,
                            method: _SumLongNullableAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _SumDoubleNonSelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SumAsync))
                .First(t => !t.IsGenericMethod && t.ReturnType == typeof(Task<double>) && t.GetParameters()[0].ParameterType.GetGenericArguments()[0] == typeof(double));
        public static Task<double> SumAsync(this IQueryable<double> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<double>(Expression.Call(
                            instance: null,
                            method: _SumDoubleNonSelectAsyncMethodInfo,
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _SumDoubleNullableNonSelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SumAsync))
                .First(t => !t.IsGenericMethod && t.ReturnType == typeof(Task<double?>) && t.GetParameters()[0].ParameterType.GetGenericArguments()[0] == typeof(double?));
        public static Task<double?> SumAsync(this IQueryable<double?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<double?>(Expression.Call(
                            instance: null,
                            method: _SumDoubleNullableNonSelectAsyncMethodInfo,
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _SumLongNullableNonSelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(SumAsync))
                .First(t => !t.IsGenericMethod && t.ReturnType == typeof(Task<long?>) && t.GetParameters()[0].ParameterType.GetGenericArguments()[0] == typeof(long?));
        public static Task<long?> SumAsync(this IQueryable<long?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<long?>(Expression.Call(
                            instance: null,
                            method: _SumLongNullableNonSelectAsyncMethodInfo,
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        #endregion

        #region ToArray

        private static readonly MethodInfo _ToArrayAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethod(nameof(ToArrayAsync));
        public static Task<TSource[]> ToArrayAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<TSource[]>(Expression.Call(
                            instance: null,
                            method: _ToArrayAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        #endregion

        #region ToDictionary

        private static readonly MethodInfo _ToDictionaryAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(ToDictionaryAsync))
                .First(t => t.GetGenericArguments().Length == 2 && t.GetParameters().Length == 3);
        public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<Dictionary<TKey, TSource>>(Expression.Call(
                            instance: null,
                            method: _ToDictionaryAsyncMethodInfo.MakeGenericMethod(typeof(TSource), typeof(TKey)),
                            arguments: new[] { source.Expression, Expression.Constant(keySelector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _ToDictionaryWithComparerAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(ToDictionaryAsync))
                .First(t => t.GetGenericArguments().Length == 2 && t.GetParameters().Length == 4);
        public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<Dictionary<TKey, TSource>>(Expression.Call(
                            instance: null,
                            method: _ToDictionaryWithComparerAsyncMethodInfo.MakeGenericMethod(typeof(TSource), typeof(TKey)),
                            arguments: new[] { source.Expression, Expression.Constant(keySelector), Expression.Constant(comparer), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _ToDictionarySelectAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(ToDictionaryAsync))
                .First(t => t.GetGenericArguments().Length == 3 && t.GetParameters().Length == 4);
        public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<Dictionary<TKey, TElement>>(Expression.Call(
                            instance: null,
                            method: _ToDictionarySelectAsyncMethodInfo.MakeGenericMethod(typeof(TSource), typeof(TKey), typeof(TElement)),
                            arguments: new[] { source.Expression, Expression.Constant(keySelector), Expression.Constant(elementSelector), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _ToDictionarySelectWithComparerAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(ToDictionaryAsync))
                .First(t => t.GetGenericArguments().Length == 3 && t.GetParameters().Length == 5);
        public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<Dictionary<TKey, TElement>>(Expression.Call(
                            instance: null,
                            method: _ToDictionarySelectAsyncMethodInfo.MakeGenericMethod(typeof(TSource), typeof(TKey), typeof(TElement)),
                            arguments: new[] { source.Expression, Expression.Constant(keySelector), Expression.Constant(elementSelector), Expression.Constant(comparer), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }


        #endregion

        #region ToList

        private static readonly MethodInfo _ToListAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethod(nameof(ToListAsync));
        public static Task<List<TSource>> ToListAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<List<TSource>>(Expression.Call(
                            instance: null,
                            method: _ToListAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        #endregion

        #region Include

        private static readonly MethodInfo _IncludeMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(Include))
                .Single(
                    mi =>
                        mi.GetGenericArguments().Count() == 2
                        && mi.GetParameters().Any(
                            pi => pi.Name == "navigationPropertyPath" && pi.ParameterType != typeof(string)));

        public static IIncludableQueryable<TEntity, TProperty> Include<TEntity, TProperty>(this IQueryable<TEntity> source, Expression<Func<TEntity, TProperty>> navigationPropertyPath)
            where TEntity : class
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (navigationPropertyPath == null)
                throw new ArgumentNullException(nameof(navigationPropertyPath));
            return new AsyncIncludableQueryableWrapper<TEntity, TProperty>(
                source.Provider.CreateQuery<TEntity>(
                        Expression.Call(
                            instance: null,
                            method: _IncludeMethodInfo.MakeGenericMethod(typeof(TEntity), typeof(TProperty)),
                            arguments: new[] { source.Expression, Expression.Quote(navigationPropertyPath) })));
        }

        private static readonly MethodInfo _StringIncludeMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(Include))
                .Single(
                    mi => mi.GetParameters().Any(
                        pi => pi.Name == "navigationPropertyPath" && pi.ParameterType == typeof(string)));
        public static IQueryable<TEntity> Include<TEntity>(this IQueryable<TEntity> source, string navigationPropertyPath)
            where TEntity : class
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (navigationPropertyPath == null)
                throw new ArgumentNullException(nameof(navigationPropertyPath));
            return new AsyncQueryableWrapper<TEntity>(
                source.Provider.CreateQuery<TEntity>(
                        Expression.Call(
                            instance: null,
                            method: _StringIncludeMethodInfo.MakeGenericMethod(typeof(TEntity)),
                            arg0: source.Expression,
                            arg1: Expression.Constant(navigationPropertyPath))));
        }

        private static readonly MethodInfo _ThenIncludeAfterEnumerableMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(ThenInclude))
                .Where(mi => mi.GetGenericArguments().Count() == 3)
                .Single(
                    mi =>
                    {
                        var typeInfo = mi.GetParameters()[0].ParameterType.GenericTypeArguments[1];
                        return typeInfo.IsGenericType
                            && typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>);
                    });

        private static readonly MethodInfo _ThenIncludeAfterReferenceMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(ThenInclude))
                .Single(
                    mi => mi.GetGenericArguments().Count() == 3
                        && mi.GetParameters()[0].ParameterType.GenericTypeArguments[1].IsGenericParameter);

        public static IIncludableQueryable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(
            this IIncludableQueryable<TEntity, IEnumerable<TPreviousProperty>> source,
            Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
            where TEntity : class
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (navigationPropertyPath == null)
                throw new ArgumentNullException(nameof(navigationPropertyPath));
            return new AsyncIncludableQueryableWrapper<TEntity, TProperty>(
                source.Provider.CreateQuery<TEntity>(
                    Expression.Call(
                        instance: null,
                        method: _ThenIncludeAfterEnumerableMethodInfo.MakeGenericMethod(
                            typeof(TEntity), typeof(TPreviousProperty), typeof(TProperty)),
                        arguments: new[] { source.Expression, Expression.Quote(navigationPropertyPath) })));
        }

        public static IIncludableQueryable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(
            this IIncludableQueryable<TEntity, TPreviousProperty> source,
            Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
            where TEntity : class
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (navigationPropertyPath == null)
                throw new ArgumentNullException(nameof(navigationPropertyPath));
            return new AsyncIncludableQueryableWrapper<TEntity, TProperty>(
                source.Provider.CreateQuery<TEntity>(
                    Expression.Call(
                        instance: null,
                        method: _ThenIncludeAfterReferenceMethodInfo.MakeGenericMethod(
                            typeof(TEntity), typeof(TPreviousProperty), typeof(TProperty)),
                        arguments: new[] { source.Expression, Expression.Quote(navigationPropertyPath) })));
        }

        private class AsyncIncludableQueryableWrapper<TEntity, TProperty> : IIncludableQueryable<TEntity, TProperty>
        {
            private readonly IQueryable<TEntity> _queryable;

            public AsyncIncludableQueryableWrapper(IQueryable<TEntity> queryable)
            {
                _queryable = queryable;
            }

            public Type ElementType => _queryable.ElementType;

            public Expression Expression => _queryable.Expression;

            public IQueryProvider Provider => _queryable.Provider;

            public IEnumerator<TEntity> GetEnumerator()
            {
                return _queryable.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class AsyncQueryableWrapper<T> : IQueryable<T>
        {
            private readonly IQueryable<T> _queryable;

            public AsyncQueryableWrapper(IQueryable<T> queryable)
            {
                _queryable = queryable;
            }

            public Type ElementType => _queryable.ElementType;

            public Expression Expression => _queryable.Expression;

            public IQueryProvider Provider => _queryable.Provider;

            public IEnumerator<T> GetEnumerator()
            {
                return _queryable.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        #endregion

        #region Track

        private static readonly MethodInfo _AsTrackingMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethod(nameof(AsTracking))!;
        public static IQueryable<TSource> AsTracking<TSource>(this IQueryable<TSource> source)
        {
            return source.Provider.CreateQuery<TSource>(
                        Expression.Call(
                            instance: null,
                            method: _AsTrackingMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression }));
        }

        private static readonly MethodInfo _AsNoTrackingMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethod(nameof(AsNoTracking))!;
        public static IQueryable<TSource> AsNoTracking<TSource>(this IQueryable<TSource> source)
        {
            return source.Provider.CreateQuery<TSource>(
                        Expression.Call(
                            instance: null,
                            method: _AsNoTrackingMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression }));
        }

        #endregion

        #region Execute

        private static readonly MethodInfo _DeleteAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethod(nameof(DeleteAsync));
        public static Task<int> DeleteAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<int>(Expression.Call(
                            instance: null,
                            method: _DeleteAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        private static readonly MethodInfo _UpdateAsyncMethodInfo
            = typeof(QueryableExtensions)
                .GetTypeInfo().GetDeclaredMethod(nameof(UpdateAsync));
        public static Task<int> UpdateAsync<TSource>(this IQueryable<TSource> source, Expression<Func<UpdateCaller<TSource>, UpdateCaller<TSource>>> updater, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (updater == null)
                throw new ArgumentNullException(nameof(updater));
            if (source.Provider is IWrappedAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<int>(Expression.Call(
                            instance: null,
                            method: _UpdateAsyncMethodInfo.MakeGenericMethod(typeof(TSource)),
                            arguments: new[] { source.Expression, Expression.Constant(updater), Expression.Constant(cancellationToken) }), cancellationToken);
            }
            throw new NotSupportedException("该查询体不支持此操作。");
        }

        #endregion
    }
}
