using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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

        public bool TrackEntity { get; set; }

        public DatabaseContext(DbContext context)
        {
            TrackEntity = false;
            AsyncQueryableExtensions.Context = AsyncQueryable.Default;
            DatabaseContextAccessor.Context = this;
            _CachedEntityContext = new Dictionary<Type, object>();
            InnerContext = context;
            context.Configuration.AutoDetectChangesEnabled = false;
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
            var entry = InnerContext.ChangeTracker.Entries<TSource>().FirstOrDefault(t => t.Entity.Index.Equals(entity.Index));
            if (entry != null && entry.Entity != entity)
                entry.CurrentValues.SetValues(entity);
            else
                entry = InnerContext.Entry(entity);
            var reference = entry.Reference(propertyName);
            if (!reference.IsLoaded)
            {
                if (entry.State == EntityState.Detached)
                    entry.State = EntityState.Unchanged;
                await reference.LoadAsync();
            }
            return (TResult)reference.CurrentValue;
        }

        async Task<IQueryableCollection<TResult>> IDatabaseContext.LoadAsync<TSource, TResult>(TSource entity, Expression<Func<TSource, ICollection<TResult>>> expression)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            string propertyName = GetPropertyName(expression);
            var entry = InnerContext.ChangeTracker.Entries<TSource>().FirstOrDefault(t => t.Entity.Index.Equals(entity.Index));
            if (entry != null && entry.Entity != entity)
                entry.CurrentValues.SetValues(entity);
            else
                entry = InnerContext.Entry(entity);
            var property = entry.Collection(propertyName);
            if (entry.State == EntityState.Detached)
                entry.State = EntityState.Unchanged;
            IQueryable<TResult> queryable = (IQueryable<TResult>)property.Query();
            var context = this.GetWrappedContext<TResult>();
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
            return InnerContext.Database.ExecuteSqlCommandAsync(sql, parameters);
        }

        public Task<TValue> ExecuteScalarAsync<TValue>(string sql, params object[] parameters)
        {
            return InnerContext.Database.SqlQuery<TValue>(sql, parameters).FirstOrDefaultAsync();
        }
    }
}
