using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Transactions;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics.CodeAnalysis;

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
                if (reference.CurrentValue != null && reference.TargetEntry!.State == EntityState.Detached)
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
                return new WrappedAsyncQueryable<T>(DbSet);
            else
                return new WrappedAsyncQueryable<T>(DbSet.AsNoTracking());
        }

        public IQueryable<TChildren> QueryChildren<TChildren>(T item, Expression<Func<T, ICollection<TChildren>>> childrenSelector)
            where TChildren : class
        {
#pragma warning disable EF1001 // Internal EF Core API usage.
            var entry = new EntityEntry<T>(((IDbContextDependencies)Database.InnerContext).StateManager.GetOrCreateEntry(item));
#pragma warning restore EF1001 // Internal EF Core API usage.
            var state = entry.State;
            if (state == EntityState.Detached)
                entry.State = EntityState.Unchanged;
            var query = (IQueryable<TChildren>)entry.Collection(((MemberExpression)childrenSelector.Body).Member.Name).Query();
            if (Database.TrackEntity)
                query = new WrappedAsyncQueryable<TChildren>(query);
            else
                query = new WrappedAsyncQueryable<TChildren>(query.AsNoTracking());
            if (state == EntityState.Detached)
                entry.State = EntityState.Detached;
            return query;
        }

        public async Task LoadPropertyAsync<TProperty>(T item, Expression<Func<T, TProperty?>> propertySelector)
            where TProperty : class
        {
#pragma warning disable EF1001 // Internal EF Core API usage.
            var entry = new EntityEntry<T>(((IDbContextDependencies)Database.InnerContext).StateManager.GetOrCreateEntry(item));
#pragma warning restore EF1001 // Internal EF Core API usage.
            var state = entry.State;
            if (state == EntityState.Detached)
                entry.State = EntityState.Unchanged;
            var reference = entry.Reference(propertySelector);
            TProperty? propertyValue;
            if (Database.TrackEntity)
                propertyValue = await reference.Query().FirstOrDefaultAsync();
            else
                propertyValue = await reference.Query().AsNoTracking().FirstOrDefaultAsync();
            if (state == EntityState.Detached)
                entry.State = EntityState.Detached;
            Metadata.GetProperty(reference.Metadata.Name)!.SetValue(item, propertyValue);
        }

        public void Remove(T item)
        {
            DbSet.Remove(item);
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            DbSet.RemoveRange(items);
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

        public Task<T> GetAsync(params object[] keys)
        {
#pragma warning disable CS8619 // 值中的引用类型的为 Null 性与目标类型不匹配。
            if (Database.TrackEntity)
                return DbSet.FindAsync(keys).AsTask();
            else
            {
                ParameterExpression parameter = Expression.Parameter(typeof(T));
                Expression? expression = null;
                if (Metadata.KeyProperties.Count != keys.Length)
                    throw new InvalidOperationException("Length of keys is difference to entity.");
                for (int i = 0; i < Metadata.KeyProperties.Count; i++)
                {
                    var equal = Expression.Equal(Expression.Property(parameter, Metadata.KeyProperties[i].ClrName), Expression.Constant(keys[i]));
                    if (expression == null)
                        expression = equal;
                    else
                        expression = Expression.AndAlso(expression, equal);
                }
                var lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
                return DbSet.AsNoTracking().Where(lambda).FirstOrDefaultAsync();
            }
#pragma warning restore CS8619 // 值中的引用类型的为 Null 性与目标类型不匹配。
        }
    }
}
