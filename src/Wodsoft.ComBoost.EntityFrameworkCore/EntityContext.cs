using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Extensions;
using System.Reflection;
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

        protected void SetUnchangeProperties(T item)
        {
            foreach (var reference in Database.InnerContext.Entry(item).References)
            {
                if (reference.CurrentValue != null && reference.TargetEntry.State == EntityState.Detached)
                    reference.TargetEntry.State = EntityState.Unchanged;
            }
        }

        public void Add(T item)
        {
            item.OnCreateCompleted();
            SetUnchangeProperties(item);
            DbSet.Add(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                item.OnCreateCompleted();
                SetUnchangeProperties(item);
            }
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
            if (Database.TrackEntity)
                return DbSet;
            else
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
            SetUnchangeProperties(item);
            DbSet.Update(item);
        }

        public void UpdateRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                item.OnEditCompleted();
                SetUnchangeProperties(item);
            }
            DbSet.UpdateRange(items);
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

        public Task ReloadAsync(T item)
        {
            return Database.InnerContext.Entry(item).ReloadAsync();
        }

        public IQueryable<T> ExecuteQuery(string sql, params object[] parameters)
        {
            return DbSet.FromSql(sql, parameters).AsNoTracking();
        }
    }
}
