using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.Entity;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class EntityContext<T> : IEntityContext<T>
        where T : class, IEntity, new()
    {
        public EntityContext(DatabaseContext database, DbSet<T> dbset)
        {
            if (database == null)
                throw new ArgumentNullException(nameof(database));
            if (dbset == null)
                throw new ArgumentNullException(nameof(dbset));
            Database = database;
            DbSet = dbset;
            Metadata = EntityDescriptor.GetMetadata<T>();
        }

        public DbSet<T> DbSet { get; private set; }

        public DatabaseContext Database { get; private set; }

        IDatabaseContext IEntityQueryContext<T>.Database { get { return Database; } }

        public IEntityMetadata Metadata { get; private set; }

        public void Add(T item)
        {
            item.OnCreateCompleted();
            DbSet.Add(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
                item.OnCreateCompleted();
            DbSet.AddRange(items);
        }

        public T Create()
        {
            var item = new T();
            item.EntityContext = this;
            item.OnCreating();
            return item;
        }

        public IQueryable<T> Query()
        {
            return DbSet.AsNoTracking();
        }

        public void Remove(T item)
        {
            DbSet.Remove(item);
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            DbSet.RemoveRange(items);
        }

        public Task<T[]> ToArrayAsync(IQueryable<T> query)
        {
            return query.ToArrayAsync();
        }

        public Task<List<T>> ToListAsync(IQueryable<T> query)
        {
            return query.ToListAsync();
        }

        public void Update(T item)
        {
            item.OnEditCompleted();
            DbSet.Attach(item);
        }

        public void UpdateRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                item.OnEditCompleted();
                DbSet.Attach(item);
            }
        }

        public Task<T> SingleOrDefaultAsync(IQueryable<T> query, Expression<Func<T, bool>> expression)
        {
            return query.SingleOrDefaultAsync(expression);
        }

        public Task<T> SingleAsync(IQueryable<T> query, Expression<Func<T, bool>> expression)
        {
            return query.SingleAsync(expression);
        }

        public Task<T> FirstOrDefaultAsync(IQueryable<T> query, Expression<Func<T, bool>> expression)
        {
            return query.FirstOrDefaultAsync(expression);
        }

        public Task<T> FirstAsync(IQueryable<T> query, Expression<Func<T, bool>> expression)
        {
            return query.FirstAsync(expression);
        }

        public Task<int> CountAsync(IQueryable<T> query)
        {
            return query.CountAsync();
        }

        public Task<int> CountAsync(IQueryable<T> query, Expression<Func<T, bool>> expression)
        {
            return query.CountAsync(expression);
        }

        async Task<TResult> IEntityQueryContext<T>.LazyLoadEntityAsync<TSource, TResult>(TSource entity, Expression<Func<TSource, TResult>> expression)
        {
            if (typeof(TSource) != typeof(T))
                throw new InvalidOperationException("实体不属于该上下文。");
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            //TResult result = expression.Compile()(entity);
            //if (result != null)
            //    return result;
            string propertyName = GetPropertyName(expression);
            var entry = Database.InnerContext.Entry((object)entity);
            var property = entry.Reference(propertyName);
            if (property.IsLoaded)
                return (TResult)property.CurrentValue;
            //var infrastructure = entry.GetInfrastructure();
            await property.LoadAsync();
            return (TResult)property.CurrentValue;
        }

        async Task<IQueryableCollection<TResult>> IEntityQueryContext<T>.LazyLoadCollectionAsync<TSource, TResult>(TSource entity, Expression<Func<TSource, ICollection<TResult>>> expression)
        {
            if (typeof(TSource) != typeof(T))
                throw new InvalidOperationException("实体不属于该上下文。");
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            string propertyName = GetPropertyName(expression);
            var entry = Database.InnerContext.Entry((object)entity);
            var property = entry.Collection(propertyName);
            IQueryable<TResult> queryable = (IQueryable<TResult>)property.Query();

            var context = Database.GetWrappedContext<TResult>();
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

        public IQueryable<T> Include<TProperty>(IQueryable<T> query, Expression<Func<T, TProperty>> expression)
        {
            return query.Include(expression);
        }
    }
}
