using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;
using System.Linq.Expressions;

namespace Wodsoft.ComBoost.EntityFramework
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

        public IDatabaseContext Database { get; private set; }

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
            item.OnCreating();
            return item;
        }

        public IQueryable<T> Query()
        {
            return DbSet;
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
            DbSet.Update(item);
        }

        public void UpdateRange(IEnumerable<T> items)
        {
            foreach (var item in items)
                item.OnEditCompleted();
            DbSet.UpdateRange(items);
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

        public Task<T> LastOrDefaultAsync(IQueryable<T> query, Expression<Func<T, bool>> expression)
        {
            return query.LastOrDefaultAsync(expression);
        }

        public Task<T> LastAsync(IQueryable<T> query, Expression<Func<T, bool>> expression)
        {
            return query.LastAsync(expression);
        }

        public Task<int> CountAsync(IQueryable<T> query)
        {
            return query.CountAsync();
        }

        public Task<int> CountAsync(IQueryable<T> query, Expression<Func<T, bool>> expression)
        {
            return query.CountAsync(expression);
        }
    }
}
