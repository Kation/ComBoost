using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Data.Entity
{
    /// <summary>
    /// 实体上下文扩展。
    /// </summary>
    public static class EntityContextExtensions
    {
        ///// <summary>
        ///// 根据主键获取实体。
        ///// </summary>
        ///// <typeparam name="T">实体类型。</typeparam>
        ///// <param name="context">实体上下文。</param>
        ///// <param name="key">主键。</param>
        ///// <returns>返回实体。找不到实体时返回空。</returns>
        //public static Task<T> GetAsync<T>(this IEntityContext<T> context, object key)
        //    where T : IEntity
        //{
        //    if (context == null)
        //        throw new ArgumentNullException(nameof(context));
        //    if (key == null)
        //        throw new ArgumentNullException(nameof(key));
        //    return GetAsync(context, context.Query(), key);
        //}


        /// <summary>
        /// 根据主键获取实体。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="context">实体上下文。</param>
        /// <param name="query">查询器。</param>
        /// <param name="key">主键。</param>
        /// <returns>返回实体。找不到实体时返回空。</returns>
        public static async Task<T> GetAsync<T>(this IEntityContext<T> context, IAsyncQueryable<T> query, object key)
            where T : class, IEntity
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            Type type = typeof(T);
            string keyName = context.Metadata.KeyProperty.ClrName;
            var parameter = Expression.Parameter(type);
            MemberExpression property;
            PropertyInfo propertyInfo = context.Metadata.Type.GetProperty(context.Metadata.KeyProperty.ClrName);
            if (propertyInfo.PropertyType != key.GetType())
                key = TypeDescriptor.GetConverter(propertyInfo.PropertyType).ConvertFrom(key);
            if (type.GetTypeInfo().IsInterface)
            {
                List<Type> list = new List<Type>() { type };
                while (list.Count > 0)
                {
                    var target = list[0];
                    propertyInfo = target.GetProperty(keyName);
                    if (propertyInfo != null)
                        break;
                    list.RemoveAt(0);
                    list.AddRange(target.GetInterfaces());
                }
            }
            else
                propertyInfo = type.GetProperty(context.Metadata.KeyProperty.ClrName);
            property = Expression.Property(parameter, propertyInfo);
            Expression expression = Expression.Equal(property, Expression.Constant(key, propertyInfo.PropertyType));
            var lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
            return await query.Where(lambda).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 异步统计查询结果的数量。
        /// </summary>
        /// <param name="context">实体上下文。</param>
        /// <returns></returns>
        public static ValueTask<int> CountAsync<T>(this IEntityContext<T> context)
            where T : class, IEntity
        {
            return context.Query().CountAsync();
        }

        /// <summary>
        /// 获取排序后的实体查询。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="context">实体上下文。</param>
        /// <returns>返回排序后的实体查询。</returns>
        public static IOrderedAsyncQueryable<T> Order<T>(this IEntityContext<T> context)
            where T : class, IEntity
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            return Order(context, context.Query());
        }

        /// <summary>
        /// 获取排序后的实体查询。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="context">实体上下文。</param>
        /// <param name="query">实体查询。</param>
        /// <returns>返回排序后的实体查询。</returns>
        public static IOrderedAsyncQueryable<T> Order<T>(this IEntityContext<T> context, IAsyncQueryable<T> query)
            where T : class, IEntity
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            var parameter = Expression.Parameter(typeof(T));
            IPropertyMetadata sortProperty = context.Metadata.SortProperty ?? context.Metadata.KeyProperty;
            dynamic express = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(T), sortProperty.ClrType), Expression.Property(parameter, sortProperty.ClrName), parameter);
            if (context.Metadata.IsSortDescending)
                return Queryable.OrderByDescending(query, express);
            else
                return Queryable.OrderBy(query, express);
        }

        public static IAsyncQueryable<T> InParent<T>(this IEntityContext<T> context, IAsyncQueryable<T> query, string path, object value)
            where T : class, IEntity
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            var parameter = Expression.Parameter(typeof(T));
            MemberExpression member = null;
            string[] properties = path.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            Type type = context.Metadata.Type;
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = type.GetProperty(properties[i]);
                if (property == null)
                    throw new ArgumentException("父级路径错误。");
                if (member == null)
                    member = Expression.Property(parameter, property);
                else
                    member = Expression.Property(member, property);
                type = property.PropertyType;
            }
            Expression equal;
            if (type.GetTypeInfo().IsValueType)
                equal = Expression.Equal(member, Expression.Constant(value));
            else
            {
                var propertyMetadata = EntityDescriptor.GetMetadata(type);
                equal = Expression.Equal(Expression.Property(member, type.GetProperty(propertyMetadata.KeyProperty.ClrName)), Expression.Constant(value));
            }
            var express = Expression.Lambda<Func<T, bool>>(equal, parameter);
            return query.Where(express);
        }

        public static IAsyncQueryable<T> InParent<T>(this IEntityContext<T> context, IAsyncQueryable<T> query, object[] parentIds)
            where T : class, IEntity
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (parentIds == null || parentIds.Length == 0)
                throw new ArgumentNullException(nameof(parentIds));
            if (context.Metadata.ParentProperty == null)
                throw new NotSupportedException("该实体不支持父级元素。");
            var parameter = Expression.Parameter(typeof(T));
            Expression equal = null;
            foreach (object parent in parentIds)
            {
                var item = Expression.Equal(Expression.Property(Expression.Property(parameter, context.Metadata.ParentProperty.ClrName), typeof(T).GetProperty("Index")), Expression.Constant(parent));
                if (equal == null)
                    equal = item;
                else
                    equal = Expression.Or(equal, item);
            }
            var express = Expression.Lambda<Func<T, bool>>(equal, parameter);
            return query.Where(express);

        }


        private static readonly MethodInfo IncludeMethodInfo
            = typeof(EntityContextExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(Include))
                .Single(
                    mi =>
                        mi.GetGenericArguments().Count() == 2
                        && mi.GetParameters().Any(
                            pi => pi.Name == "navigationPropertyPath" && pi.ParameterType != typeof(string)));
        public static IAsyncIncludableQueryable<TEntity, TProperty> Include<TEntity, TProperty>(this IAsyncQueryable<TEntity> source, Expression<Func<TEntity, TProperty>> navigationPropertyPath)
            where TEntity : class
        {
            return new AsyncIncludableQueryableWrapper<TEntity, TProperty>(
                source.Provider.CreateQuery<TEntity>(
                        Expression.Call(
                            instance: null,
                            method: IncludeMethodInfo.MakeGenericMethod(typeof(TEntity), typeof(TProperty)),
                            arguments: new[] { source.Expression, Expression.Quote(navigationPropertyPath) })));
        }

        private static readonly MethodInfo StringIncludeMethodInfo
            = typeof(EntityContextExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(Include))
                .Single(
                    mi => mi.GetParameters().Any(
                        pi => pi.Name == "navigationPropertyPath" && pi.ParameterType == typeof(string)));
        public static IAsyncQueryable<TEntity> Include<TEntity>(this IAsyncQueryable<TEntity> source, string navigationPropertyPath)
            where TEntity : class
        {
            return new AsyncQueryableWrapper<TEntity>(
                source.Provider.CreateQuery<TEntity>(
                        Expression.Call(
                            instance: null,
                            method: StringIncludeMethodInfo.MakeGenericMethod(typeof(TEntity)),
                            arg0: source.Expression,
                            arg1: Expression.Constant(navigationPropertyPath))));
        }


        private static readonly MethodInfo ThenIncludeAfterEnumerableMethodInfo
            = typeof(EntityContextExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(ThenInclude))
                .Where(mi => mi.GetGenericArguments().Count() == 3)
                .Single(
                    mi =>
                    {
                        var typeInfo = mi.GetParameters()[0].ParameterType.GenericTypeArguments[1];
                        return typeInfo.IsGenericType
                            && typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>);
                    });

        private static readonly MethodInfo ThenIncludeAfterReferenceMethodInfo
            = typeof(EntityContextExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(ThenInclude))
                .Single(
                    mi => mi.GetGenericArguments().Count() == 3
                        && mi.GetParameters()[0].ParameterType.GenericTypeArguments[1].IsGenericParameter);

        public static IAsyncIncludableQueryable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(
            this IAsyncIncludableQueryable<TEntity, IEnumerable<TPreviousProperty>> source,
            Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
            where TEntity : class
            => new AsyncIncludableQueryableWrapper<TEntity, TProperty>(
                source.Provider.CreateQuery<TEntity>(
                    Expression.Call(
                        instance: null,
                        method: ThenIncludeAfterEnumerableMethodInfo.MakeGenericMethod(
                            typeof(TEntity), typeof(TPreviousProperty), typeof(TProperty)),
                        arguments: new[] { source.Expression, Expression.Quote(navigationPropertyPath) })));

        public static IAsyncIncludableQueryable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(
            this IAsyncIncludableQueryable<TEntity, TPreviousProperty> source,
            Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
            where TEntity : class
            => new AsyncIncludableQueryableWrapper<TEntity, TProperty>(
                source.Provider.CreateQuery<TEntity>(
                    Expression.Call(
                        instance: null,
                        method: ThenIncludeAfterReferenceMethodInfo.MakeGenericMethod(
                            typeof(TEntity), typeof(TPreviousProperty), typeof(TProperty)),
                        arguments: new[] { source.Expression, Expression.Quote(navigationPropertyPath) })));

        private class AsyncIncludableQueryableWrapper<TEntity, TProperty> : IAsyncIncludableQueryable<TEntity, TProperty>
        {
            private readonly IAsyncQueryable<TEntity> _queryable;

            public AsyncIncludableQueryableWrapper(IAsyncQueryable<TEntity> queryable)
            {
                _queryable = queryable;
            }

            public Type ElementType => _queryable.ElementType;

            public Expression Expression => _queryable.Expression;

            public IAsyncQueryProvider Provider => _queryable.Provider;

            public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return _queryable.GetAsyncEnumerator(cancellationToken);
            }
        }

        private class AsyncQueryableWrapper<T> : IAsyncQueryable<T>
        {
            private readonly IAsyncQueryable<T> _queryable;

            public AsyncQueryableWrapper(IAsyncQueryable<T> queryable)
            {
                _queryable = queryable;
            }

            public Type ElementType => _queryable.ElementType;

            public Expression Expression => _queryable.Expression;

            public IAsyncQueryProvider Provider => _queryable.Provider;

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return _queryable.GetAsyncEnumerator(cancellationToken);
            }
        }
    }
}
