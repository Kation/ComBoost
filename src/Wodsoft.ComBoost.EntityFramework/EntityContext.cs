using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.Entity;
using System.ComponentModel;

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

        public Task<T> SingleOrDefaultAsync(IQueryable<T> query)
        {
            return query.SingleOrDefaultAsync();
        }

        public Task<T> SingleAsync(IQueryable<T> query)
        {
            return query.SingleAsync();
        }

        public Task<T> FirstOrDefaultAsync(IQueryable<T> query)
        {
            return query.FirstOrDefaultAsync();
        }

        public Task<T> FirstAsync(IQueryable<T> query)
        {
            return query.FirstAsync();
        }

        public Task<int> CountAsync(IQueryable<T> query)
        {
            return query.CountAsync();
        }

        public IQueryable<T> Include<TProperty>(IQueryable<T> query, Expression<Func<T, TProperty>> expression)
        {
            return query.Include(expression);
        }

        public Task<T> GetAsync(object key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (key.GetType() != Metadata.KeyType)
                key = TypeDescriptor.GetConverter(Metadata.KeyType).ConvertFrom(key);
            return DbSet.FindAsync(key);
        }
    }
}
