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
        /// <summary>
        /// 获取排序后的实体查询。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="context">实体上下文。</param>
        /// <returns>返回排序后的实体查询。</returns>
        public static IOrderedQueryable<T> Order<T>(this IEntityContext<T> context)
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
        public static IOrderedQueryable<T> Order<T>(this IEntityContext<T> context, IQueryable<T> query)
            where T : class, IEntity
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            var parameter = Expression.Parameter(typeof(T));
            IPropertyMetadata sortProperty = context.Metadata.SortProperty ?? context.Metadata.KeyProperties.FirstOrDefault();
            if (sortProperty == null)
                throw new InvalidOperationException($"实体“{typeof(T).FullName}”找不到排序字段。");
            dynamic express = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(T), sortProperty.ClrType), Expression.Property(parameter, sortProperty.ClrName), parameter);
            if (context.Metadata.IsSortDescending)
                return Queryable.OrderByDescending(query, express);
            else
                return Queryable.OrderBy(query, express);
        }

        public static IQueryable<T> InParent<T>(this IEntityContext<T> context, IQueryable<T> query, string path, object value)
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
                if (propertyMetadata.KeyProperties.Count > 0)
                    throw new InvalidOperationException("不支持多主键的父级实体类型。");
                equal = Expression.Equal(Expression.Property(member, type.GetProperty(propertyMetadata.KeyProperties[0].ClrName)), Expression.Constant(value));
            }
            var express = Expression.Lambda<Func<T, bool>>(equal, parameter);
            return query.Where(express);
        }

        public static IQueryable<T> InParent<T>(this IEntityContext<T> context, IQueryable<T> query, object[] parentIds)
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
    }
}
