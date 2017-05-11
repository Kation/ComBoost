using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class DatabaseContext : IDatabaseContext
    {
        private static ConcurrentDictionary<Type, IEnumerable<Type>> _CachedSupportTypes;
        private Dictionary<Type, object> _CachedEntityContext;

        static DatabaseContext()
        {
            _CachedSupportTypes = new ConcurrentDictionary<Type, IEnumerable<Type>>();
        }

        public DbContext InnerContext { get; private set; }

        public IEnumerable<Type> SupportTypes { get; private set; }

        public DatabaseContext(DbContext context)
        {
            AsyncQueryableExtensions.Context = AsyncQueryable.Default;
            context.GetService<CurrentDatabaseContext>().Context = this;
            _CachedEntityContext = new Dictionary<Type, object>();
            InnerContext = context;
            context.ChangeTracker.AutoDetectChangesEnabled = false;
            SupportTypes = _CachedSupportTypes.GetOrAdd(context.GetType(), type =>
            {
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(t => t.CanRead && t.CanWrite && t.PropertyType.IsConstructedGenericType && t.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                    .Select(t => t.PropertyType.GetGenericArguments()[0]).ToList();
                return new System.Collections.ObjectModel.ReadOnlyCollection<Type>(properties);
            });
        }

        public Task<int> SaveAsync()
        {
            return InnerContext.SaveChangesAsync();
        }

        public IEntityContext<T> GetContext<T>() where T : class, IEntity, new()
        {
            if (_CachedEntityContext.ContainsKey(typeof(T)))
                return (EntityContext<T>)_CachedEntityContext[typeof(T)];
            var context = new EntityContext<T>(this, InnerContext.Set<T>());
            _CachedEntityContext.Add(typeof(T), context);
            return context;
        }
        
        Task<TResult> IDatabaseContext.LoadAsync<TSource, TResult>(TSource entity, Expression<Func<TSource, TResult>> expression)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            TResult result = expression.Compile()(entity);
            if (result != null)
                return Task.FromResult(result);
            string propertyName = GetPropertyName(expression);
            var entry = InnerContext.Entry((object)entity);
            var property = entry.Metadata.FindNavigation(propertyName).ForeignKey.Properties[0];
            var infrastructure = entry.GetInfrastructure();
            var key = infrastructure.GetCurrentValue(property);
            if (key == null)
                return null;
            var context = this.GetWrappedContext<TResult>();
            return context.GetAsync(key);
        }

        async Task<IQueryableCollection<TResult>> IDatabaseContext.LoadAsync<TSource, TResult>(TSource entity, Expression<Func<TSource, ICollection<TResult>>> expression)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            string propertyName = GetPropertyName(expression);
            var entry = InnerContext.Entry((object)entity);
            var property = entry.Metadata.FindNavigation(propertyName);
            var inverseProperty = property.FindInverse();
            var parameterExpression = Expression.Parameter(typeof(TResult));
            Expression propertyExpression = Expression.Property(parameterExpression, inverseProperty.Name);
            if (inverseProperty.IsCollection())
            {
                var whereP = Expression.Parameter(typeof(TSource));
                var equalE = Expression.Equal(Expression.Property(whereP, "Index"), Expression.Constant(entity.Index));
                var lambdaE = Expression.Lambda<Func<TSource, bool>>(equalE, whereP);
                propertyExpression = Expression.Call(typeof(Queryable).GetMethod("Any", new Type[] { typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, bool>>) }), propertyExpression, lambdaE);
            }
            else
                propertyExpression = Expression.Property(propertyExpression, "Index");
            var equalExpression = Expression.Equal(propertyExpression, Expression.Constant(entity.Index));
            var lambdaExpression = Expression.Lambda<Func<TResult, bool>>(equalExpression, parameterExpression);

            //entry.GetInfrastructure().AddToCollectionSnapshot()

            var context = this.GetWrappedContext<TResult>();
            var queryable = context.Query().Where(lambdaExpression);
            var count = await context.CountAsync(queryable);
            return new ComBoostEntityCollection<TResult>(entry, context, property, queryable, count);
        }

        private static string GetPropertyName<TSource, TResult>(Expression<Func<TSource, TResult>> expression)
        {
            MemberExpression memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new NotSupportedException("不支持的路径。");
            var parameter = memberExpression.Expression as ParameterExpression;
            if (parameter == null)
                throw new NotSupportedException("不支持的路径。");
            return memberExpression.Member.Name;
        }
        
        public Task<int> ExecuteNonQueryAsync(string sql, params object[] parameters)
        {
            throw new NotSupportedException();
        }

        public Task<TValue> ExecuteScalarAsync<TValue>(string sql, params object[] parameters)
        {
            throw new NotSupportedException();
        }
    }
}
