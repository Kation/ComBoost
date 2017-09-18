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
using Microsoft.EntityFrameworkCore.Metadata;

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
            DatabaseContextAccessor.Context = this;
            AsyncQueryableExtensions.Context = AsyncQueryable.Default;
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

        async Task<TResult> IDatabaseContext.LoadAsync<TSource, TResult>(TSource entity, Expression<Func<TSource, TResult>> expression)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            string propertyName = GetPropertyName(expression);
            var entry = InnerContext.Entry((object)entity);
            var reference = entry.Reference(propertyName);
            if (!reference.IsLoaded)
                await reference.LoadAsync();
            TResult result = expression.Compile()(entity);
            return result;
        }

        async Task<IQueryableCollection<TResult>> IDatabaseContext.LoadAsync<TSource, TResult>(TSource entity, Expression<Func<TSource, ICollection<TResult>>> expression)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            string propertyName = GetPropertyName(expression);
            var entry = InnerContext.Entry((object)entity);
            var reference = entry.Collection(propertyName);
            var property = entry.Metadata.FindNavigation(propertyName);
            var context = this.GetWrappedContext<TResult>();
            if (entry.State == EntityState.Detached)
                entry.State = EntityState.Unchanged;
            var queryable = (IQueryable<TResult>)reference.Query();
            var count = await queryable.CountAsync();
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
