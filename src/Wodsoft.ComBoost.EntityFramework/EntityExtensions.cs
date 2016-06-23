using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Wodsoft.ComBoost.Data.Entity
{
    public static class EntityExtensions
    {
        public static Task<T> LazyLoadEntityAsync<TSource, T>(this TSource source, Expression<Func<TSource, T>> expression, IEntityContext<T> context)
            where TSource : IEntity
            where T : class, IEntity, new()
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            EntityContext<T> entityContext = context as EntityContext<T>;
            if (entityContext == null)
                throw new NotSupportedException("不支持的实体上下文类型。");
            DatabaseContext databaseContext = (DatabaseContext)entityContext.Database;
            string propertyName = GetPropertyName(expression);
            var entry = databaseContext.InnerContext.Entry((object)source);
            var property = entry.Metadata.FindNavigation(propertyName);
            var infrastructure = entry.GetInfrastructure();
            var key = infrastructure.GetCurrentValue(property);
            if (key == null)
                return null;
            return context.GetAsync(key);
        }

        public static IQueryable<T> LazyLoadQuery<TSource, T>(this TSource source, Expression<Func<TSource, ICollection<T>>> expression, IEntityContext<T> context)
            where TSource : IEntity
            where T : class, IEntity, new()
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            EntityContext<T> entityContext = context as EntityContext<T>;
            if (entityContext == null)
                throw new NotSupportedException("不支持的实体上下文类型。");
            DatabaseContext databaseContext = (DatabaseContext)entityContext.Database;
            string propertyName = GetPropertyName(expression);
            var entry = databaseContext.InnerContext.Entry((object)source);
            var property = entry.Metadata.FindNavigation(propertyName);
            var inverseProperty = property.FindInverse();

            var parameterExpression = Expression.Parameter(typeof(T));
            var propertyExpression = Expression.Property(parameterExpression, inverseProperty.Name);
            propertyExpression = Expression.Property(propertyExpression, "Index");
            var equalExpression = Expression.Equal(propertyExpression, Expression.Constant(source.Index));
            var lambdaExpression = Expression.Lambda<Func<T, bool>>(equalExpression, parameterExpression);
            return context.Query().Where(lambdaExpression);
        }

        private static string GetPropertyName<TSource, T>(Expression<Func<TSource, T>> expression)
        {
            MemberExpression memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new NotSupportedException("不支持的路径。");
            var parameter = memberExpression.Expression as ParameterExpression;
            if (parameter == null)
                throw new NotSupportedException("不支持的路径。");
            return memberExpression.Member.Name;
        }
    }
}
