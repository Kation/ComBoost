using System;
using System.Collections.Generic;
using System.Data.Entity.Metadata;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Entity
{
    /// <summary>
    /// Entity context.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    public class EntityContext<TEntity> : IEntityContext<TEntity> where TEntity : class, IEntity, new()
    {
        /// <summary>
        /// Get the database context of entity framework.
        /// </summary>
        protected DbContext DbContext { get; private set; }

        /// <summary>
        /// Get the DbSet of entity.
        /// </summary>
        protected DbSet<TEntity> DbSet { get; private set; }

        /// <summary>
        /// Get the metadata of entity.
        /// </summary>
        protected IEntityMetadata Metadata { get; private set; }

        /// <summary>
        /// Initialize entity context.
        /// </summary>
        /// <param name="dbContext">Database context of entity framework.</param>
        public EntityContext(DbContext dbContext)
        {
            DbContext = dbContext;
            var dbset = DbContext.GetType().GetProperties().FirstOrDefault(t => t.PropertyType == typeof(DbSet<TEntity>));
            if (dbset == null)
                throw new ArgumentException("dbContext doesn't contains DbSet<" + typeof(TEntity).Name + ">");
            DbSet = (DbSet<TEntity>)dbset.GetValue(DbContext, null);
            Metadata = EntityAnalyzer.GetMetadata<TEntity>();
        }

        /// <summary>
        /// Add an entity to database.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Return false if detect any error.</returns>
        /// <exception cref="ArgumentException">Id of entity is already exists in database.</exception>
        public virtual bool Add(TEntity entity)
        {
            if (entity == null)
                return false;
            if (entity.Index == Guid.Empty)
                entity.Index = Guid.NewGuid();
            if (Contains(entity.Index))
                throw new ArgumentException("Index is already exists.");
            DbSet.Add(entity);
            try
            {
                DbContext.SaveChanges();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add an entity to database.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Return false if detect any error.</returns>
        /// <exception cref="ArgumentException">Id of entity is already exists in database.</exception>
        public virtual async Task<bool> AddAsync(TEntity entity)
        {
            if (entity == null)
                return false;
            if (entity.Index == Guid.Empty)
                entity.Index = Guid.NewGuid();
            if (Contains(entity.Index))
                throw new ArgumentException("Index is already exists.");
            DbSet.Add(entity);
            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add a lot of entity to database.
        /// </summary>
        /// <param name="entities">IEnumerable of entity.</param>
        /// <returns>Return true if success.</returns>
        public virtual bool AddRange(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
            try
            {
                DbContext.SaveChanges();
            }
            catch
            {
                DbSet.RemoveRange(entities);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add a lot of entity to database.
        /// </summary>
        /// <param name="entities">IEnumerable of entity.</param>
        /// <returns>Return true if success.</returns>
        public virtual async Task<bool> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch
            {
                DbSet.RemoveRange(entities);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Remove an entity from database.
        /// </summary>
        /// <param name="entityID">Entity id.</param>
        /// <returns>Return true if success.</returns>
        public virtual bool Remove(Guid entityID)
        {
            TEntity entity = DbSet.Find(entityID);
            if (entity == null)
                return false;
            if (!entity.IsRemoveAllowed)
                return false;
            DbSet.Remove(entity);
            try
            {
                DbContext.SaveChanges();
            }
            catch
            {
                DbContext.Entry<TEntity>(entity).State = EntityState.Unchanged;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Remove an entity from database.
        /// </summary>
        /// <param name="entityID">Entity id.</param>
        /// <returns>Return true if success.</returns>
        public virtual async Task<bool> RemoveAsync(Guid entityID)
        {
            TEntity entity = DbSet.Find(entityID);
            if (entity == null)
                return false;
            if (!entity.IsRemoveAllowed)
                return false;
            DbSet.Remove(entity);
            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch
            {
                DbContext.Entry<TEntity>(entity).State = EntityState.Unchanged;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Remove a lot of entities from database.
        /// </summary>
        /// <param name="ids">IEnumerable of Guid of entities.</param>
        /// <returns>Always return true.</returns>
        public virtual bool RemoveRange(IEnumerable<Guid> ids)
        {
            //Todo, use sql command to delete from database.
            //Guid[] list = ids.ToArray();
            //DbContext.Database.ExecuteSqlCommand("delete * from ?? where [Index] ")
            foreach (var id in ids)
                Remove(id);
            return true;
        }

        /// <summary>
        /// Remove a lot of entities from database.
        /// </summary>
        /// <param name="ids">IEnumerable of Guid of entities.</param>
        /// <returns>Always return true.</returns>
        public virtual async Task<bool> RemoveRangeAsync(IEnumerable<Guid> ids)
        {
            //Todo, use sql command to delete from database.
            //Guid[] list = ids.ToArray();
            //DbContext.Database.ExecuteSqlCommand("delete * from ?? where [Index] ")
            await Task.WhenAll(ids.Select(t => RemoveAsync(t)).ToArray());
            return true;
        }

        /// <summary>
        /// Edit an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Return true if success.</returns>
        public virtual bool Edit(TEntity entity)
        {
            TEntity item = DbSet.Find(entity.Index);
            if (item == null)
                return false;
            if (!item.IsEditAllowed)
                return false;
            if (item != entity)
                foreach (var property in typeof(TEntity).GetProperties().Where(t => t.CanRead && t.CanWrite))
                    property.SetValue(item, property.GetValue(entity, null), null);
            entity.OnEditCompleted();
            DbContext.SaveChanges();
            return true;
        }

        /// <summary>
        /// Edit an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Return true if success.</returns>
        public virtual async Task<bool> EditAsync(TEntity entity)
        {
            TEntity item = DbSet.Find(entity.Index);
            if (item == null)
                return false;
            if (!item.IsEditAllowed)
                return false;
            if (item != entity)
                foreach (var property in typeof(TEntity).GetProperties().Where(t => t.CanRead && t.CanWrite))
                    property.SetValue(item, property.GetValue(entity, null), null);
            entity.OnEditCompleted();
            await DbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Reload entity from database.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public virtual void Reload(TEntity entity)
        {
            var entry = DbContext.Entry<TEntity>(entity);
            if (entry.State == EntityState.Detached)
                throw new InvalidOperationException("This entity is not belong to this context.");
            entry.Reload();
        }

        /// <summary>
        /// Reload entity from database.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public virtual async Task ReloadAsync(TEntity entity)
        {
            var entry = DbContext.Entry<TEntity>(entity);
            if (entry.State == EntityState.Detached)
                throw new InvalidOperationException("This entity is not belong to this context.");
            await entry.ReloadAsync();
        }

        /// <summary>
        /// Get an entity by id.
        /// </summary>
        /// <param name="entityID">Entity id.</param>
        /// <returns>Return entity. Return null if entity doesn't exists.</returns>
        public virtual TEntity GetEntity(Guid entityID)
        {
            return DbSet.Find(entityID);
        }

        /// <summary>
        /// Get an entity by id.
        /// </summary>
        /// <param name="entityID">Entity id.</param>
        /// <returns>Return entity. Return null if entity doesn't exists.</returns>
        public virtual async Task<TEntity> GetEntityAsync(Guid entityID)
        {
            return await DbSet.FindAsync(entityID);
        }

        /// <summary>
        /// Get entity queryable interface.
        /// </summary>
        /// <returns>Return queryable interface of entity.</returns>
        public virtual IQueryable<TEntity> Query()
        {
            return DbSet;
        }

        /// <summary>
        /// Get is the entity edit able.
        /// </summary>
        /// <returns>Return true if enabled.</returns>
        public virtual bool Editable()
        {
            return true;
        }

        /// <summary>
        /// Get is the entity add able.
        /// </summary>
        /// <returns>Return true if enabled.</returns>
        public virtual bool Addable()
        {
            return true;
        }

        /// <summary>
        /// Get is the entity remove able.
        /// </summary>
        /// <returns>Return true if enabled.</returns>
        public virtual bool Removeable()
        {
            return true;
        }

        /// <summary>
        /// Get entitiy queryable in some parents.
        /// </summary>
        /// <param name="queryable">Queryable of entity.</param>
        /// <param name="parents">Parents id.</param>
        /// <returns>Return queryalble interface of entity.</returns>
        /// <exception cref="ArgumentNullException">queryable or parents is null.</exception>
        /// <exception cref="NotSupportedException">Entity doesn't contains parent property.</exception>
        public virtual IQueryable<TEntity> InParent(IQueryable<TEntity> queryable, Guid[] parents)
        {
            if (queryable == null)
                throw new ArgumentNullException("queryable");
            if (parents == null || parents.Length == 0)
                throw new ArgumentNullException("parents");
            if (Metadata.ParentProperty == null)
                throw new NotSupportedException("Entity doesn't contains parent property.");
            var parameter = Expression.Parameter(typeof(TEntity), "t");
            Expression equal = null;
            foreach (object parent in parents)
            {
                var item = Expression.Equal(Expression.Property(Expression.Property(parameter, Metadata.ParentProperty.ClrName), typeof(TEntity).GetProperty("Index")), Expression.Constant(parent));
                if (equal == null)
                    equal = item;
                else
                    equal = Expression.Or(equal, item);
            }
            var express = Expression.Lambda<Func<TEntity, bool>>(equal, parameter);
            return queryable.Where(express);
        }

        /// <summary>
        /// Get entitiy queryable in a parent.
        /// </summary>
        /// <param name="queryable">Queryable of entity.</param>
        /// <param name="path">Path that parent to entity. (Like User.Group, User is property of this entity. Group is property of type that User property.)</param>
        /// <param name="id">Parent id.</param>
        /// <returns>Return queryalble interface of entity.</returns>
        /// <exception cref="ArgumentNullException">queryable or path is null.</exception>
        /// <exception cref="NotSupportedException">Entity doesn't contains parent property.</exception>
        /// <exception cref="ArgumentException">Parent path invalid.</exception>
        public IQueryable<TEntity> InParent(IQueryable<TEntity> queryable, string path, Guid id)
        {
            if (queryable == null)
                throw new ArgumentNullException("queryable");
            if (path == null)
                throw new ArgumentNullException("path");
            if (Metadata.ParentProperty == null)
                throw new NotSupportedException("Entity doesn't contains parent property.");
            var parameter = Expression.Parameter(typeof(TEntity), "t");
            MemberExpression member = null;
            string[] properties = path.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            Type type = Metadata.Type;
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = type.GetProperty(properties[i]);
                if (property == null)
                    throw new ArgumentException("Parent path invalid.");
                if (member == null)
                    member = Expression.Property(parameter, property);
                else
                    member = Expression.Property(member, property);
                type = property.PropertyType;
            }
            Expression equal = Expression.Equal(Expression.Property(member, type.GetProperty("Index")), Expression.Constant(id));
            var express = Expression.Lambda<Func<TEntity, bool>>(equal, parameter);
            return queryable.Where(express);
        }

        /// <summary>
        /// Get total entity count from database.
        /// </summary>
        /// <returns>Return total entity count number.</returns>
        public virtual int Count()
        {
            return DbSet.Count();
        }

        /// <summary>
        /// Get total entity count from database.
        /// </summary>
        /// <returns>Return total entity count number.</returns>
        public virtual async Task<int> CountAsync()
        {
            return await DbSet.CountAsync();
        }

        /// <summary>
        /// Get an entity is added to database.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Return true if database contains this entity.</returns>
        public virtual bool Contains(TEntity entity)
        {
            return Contains(entity.Index);
        }

        /// <summary>
        /// Get an entity is added to database.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Return true if database contains this entity.</returns>
        public virtual async Task<bool> ContainsAsync(TEntity entity)
        {
            return await ContainsAsync(entity.Index);
        }

        /// <summary>
        /// Get an entity is added to database.
        /// </summary>
        /// <param name="id">Entity.</param>
        /// <returns>Return true if database contains this entity.</returns>
        public virtual bool Contains(Guid id)
        {
            return DbSet.Count(t => t.Index == id) > 0;
        }

        /// <summary>
        /// Get an entity is added to database.
        /// </summary>
        /// <param name="id">Entity.</param>
        /// <returns>Return true if database contains this entity.</returns>
        public virtual async Task<bool> ContainsAsync(Guid id)
        {
            return await DbSet.CountAsync(t => t.Index == id) > 0;
        }

        /// <summary>
        /// Create an entity. Not added to database.
        /// </summary>
        /// <returns>Return created entity.</returns>
        public virtual TEntity Create()
        {
            TEntity item = DbSet.Create();
            item.CreateDate = DateTime.Now;
            item.OnCreateCompleted();
            return item;
        }

        /// <summary>
        /// Sort entity queryable.
        /// </summary>
        /// <param name="queryable">Entity queryable interface.</param>
        /// <returns>Return entity queryable interface.</returns>
        /// <exception cref="ArgumentNullException">queryable is null.</exception>
        public virtual IOrderedQueryable<TEntity> OrderBy(IQueryable<TEntity> queryable)
        {
            if (queryable == null)
                throw new ArgumentNullException("queryable");
            if (Metadata.SortProperty == null)
                return queryable.OrderByDescending(t => t.CreateDate);
            var parameter = Expression.Parameter(typeof(TEntity), "t");
            var express = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TEntity), Metadata.SortProperty.ClrType), Expression.Property(parameter, Metadata.SortProperty.ClrName), parameter);
            if (Metadata.SortDescending)
            {
                var method = typeof(Queryable).GetMethods().Where(t => t.Name == "OrderByDescending").ElementAt(0).MakeGenericMethod(typeof(TEntity), Metadata.SortProperty.ClrType);
                return (IOrderedQueryable<TEntity>)method.Invoke(null, new object[] { queryable, express });
            }
            else
            {
                var method = typeof(Queryable).GetMethods().Where(t => t.Name == "OrderBy").ElementAt(0).MakeGenericMethod(typeof(TEntity), Metadata.SortProperty.ClrType);
                return (IOrderedQueryable<TEntity>)method.Invoke(null, new object[] { queryable, express });
            }
        }

        /// <summary>
        /// Sort entity queryable.
        /// </summary>
        /// <returns>Return entity queryable interface.</returns>
        public IOrderedQueryable<TEntity> OrderBy()
        {
            return OrderBy(DbSet);
        }

        /// <summary>
        /// Get data by sql query string.
        /// </summary>
        /// <param name="sql">Sql query string.</param>
        /// <param name="parameters">Query parameters.</param>
        /// <returns>A System.Data.Entity.Infrastructure.DbRawSqlQuery object that will execute the query when it is enumerated.</returns>
        public IEnumerable<TEntity> Query(string sql, params object[] parameters)
        {
            return DbContext.Database.SqlQuery<TEntity>(sql, parameters);
        }

        /// <summary>
        /// Get array from a queryable.
        /// </summary>
        /// <param name="queryable">Entity queryable interface.</param>
        /// <returns>Return array of entity.</returns>
        public async Task<TEntity[]> ToArrayAsync(IQueryable<TEntity> queryable)
        {
            return await queryable.ToArrayAsync();
        }

        /// <summary>
        /// Get list from a queryable.
        /// </summary>
        /// <param name="queryable">Entity queryable interface.</param>
        /// <returns>Return list of entity.</returns>
        public async Task<List<TEntity>> ToListAsync(IQueryable<TEntity> queryable)
        {
            return await queryable.ToListAsync();
        }
    }
}
