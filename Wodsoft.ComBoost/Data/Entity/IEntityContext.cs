using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Entity
{
    /// <summary>
    /// Entity context.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    public interface IEntityContext<TEntity> where TEntity : class, IEntity, new()
    {
        /// <summary>
        /// Add an entity to database.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Return false if detect any error.</returns>
        bool Add(TEntity entity);

        /// <summary>
        /// Add an entity to database.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Return false if detect any error.</returns>
        Task<bool> AddAsync(TEntity entity);

        /// <summary>
        /// Add a lot of entity to database.
        /// </summary>
        /// <param name="entities">IEnumerable of entity.</param>
        /// <returns>Return true if success.</returns>
        bool AddRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Add a lot of entity to database.
        /// </summary>
        /// <param name="entities">IEnumerable of entity.</param>
        /// <returns>Return true if success.</returns>
        Task<bool> AddRangeAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Create an entity. Not added to database.
        /// </summary>
        /// <returns>Return created entity.</returns>
        TEntity Create();

        /// <summary>
        /// Remove an entity from database.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns>Return true if success.</returns>
        bool Remove(Guid id);

        /// <summary>
        /// Remove an entity from database.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns>Return true if success.</returns>
        Task<bool> RemoveAsync(Guid id);

        /// <summary>
        /// Remove a lot of entities from database.
        /// </summary>
        /// <param name="ids">IEnumerable of Guid of entities.</param>
        /// <returns>true if success.</returns>
        bool RemoveRange(IEnumerable<Guid> ids);

        /// <summary>
        /// Remove a lot of entities from database.
        /// </summary>
        /// <param name="ids">IEnumerable of Guid of entities.</param>
        /// <returns>true if success.</returns>
        Task<bool> RemoveRangeAsync(IEnumerable<Guid> ids);

        /// <summary>
        /// Edit an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Return true if success.</returns>
        bool Edit(TEntity entity);

        /// <summary>
        /// Edit an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Return true if success.</returns>
        Task<bool> EditAsync(TEntity entity);

        /// <summary>
        /// Get an entity by id.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns>Return entity. Return null if entity doesn't exists.</returns>
        TEntity GetEntity(Guid id);

        /// <summary>
        /// Get an entity by id.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns>Return entity. Return null if entity doesn't exists.</returns>
        Task<TEntity> GetEntityAsync(Guid id);

        /// <summary>
        /// Get entity queryable interface.
        /// </summary>
        /// <returns>Return queryable interface of entity.</returns>
        IQueryable<TEntity> Query();

        /// <summary>
        /// Get entities by sql query string.
        /// </summary>
        /// <param name="sql">Sql query string.</param>
        /// <param name="parameters">Query parameters.</param>
        /// <returns>Return IEnumerable of entity.</returns>
        IEnumerable<TEntity> Query(string sql, params object[] parameters);

        /// <summary>
        /// Get entitiy queryable in some parents.
        /// </summary>
        /// <param name="queryable">Queryable of entity.</param>
        /// <param name="parents">Parents id.</param>
        /// <returns>Return queryalble interface of entity.</returns>
        IQueryable<TEntity> InParent(IQueryable<TEntity> queryable, Guid[] parents);

        /// <summary>
        /// Get entitiy queryable in a parent.
        /// </summary>
        /// <param name="queryable">Queryable of entity.</param>
        /// <param name="path">Path that parent to entity. (Like User.Group, User is property of this entity. Group is property of type that User property.)</param>
        /// <param name="id">Parent id.</param>
        /// <returns>Return queryalble interface of entity.</returns>
        IQueryable<TEntity> InParent(IQueryable<TEntity> queryable, string path, Guid id);

        /// <summary>
        /// Get is the entity edit able.
        /// </summary>
        /// <returns>Return true if enabled.</returns>
        bool Editable();

        /// <summary>
        /// Get is the entity add able.
        /// </summary>
        /// <returns>Return true if enabled.</returns>
        bool Addable();

        /// <summary>
        /// Get is the entity remove able.
        /// </summary>
        /// <returns>Return true if enabled.</returns>
        bool Removeable();

        /// <summary>
        /// Get total entity count from database.
        /// </summary>
        /// <returns>Return total entity count number.</returns>
        int Count();

        /// <summary>
        /// Get total entity count from database.
        /// </summary>
        /// <returns>Return total entity count number.</returns>
        Task<int> CountAsync();

        /// <summary>
        /// Get an entity is added to database.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Return true if database contains this entity.</returns>
        bool Contains(TEntity entity);

        /// <summary>
        /// Get an entity is added to database.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Return true if database contains this entity.</returns>
        Task<bool> ContainsAsync(TEntity entity);

        /// <summary>
        /// Get an entity is added to database.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns>Return true if database contains this entity.</returns>
        bool Contains(Guid id);

        /// <summary>
        /// Get an entity is added to database.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns>Return true if database contains this entity.</returns>
        Task<bool> ContainsAsync(Guid id);

        /// <summary>
        /// Sort entity queryable.
        /// </summary>
        /// <param name="queryable">Entity queryable interface.</param>
        /// <returns>Return entity queryable interface.</returns>
        IOrderedQueryable<TEntity> OrderBy(IQueryable<TEntity> queryable);

        /// <summary>
        /// Sort entity queryable.
        /// </summary>
        /// <returns>Return entity queryable interface.</returns>
        IOrderedQueryable<TEntity> OrderBy();

        /// <summary>
        /// Get array from a queryable.
        /// </summary>
        /// <param name="queryable">Entity queryable interface.</param>
        /// <returns>Return array of entity.</returns>
        Task<TEntity[]> ToArrayAsync(IQueryable<TEntity> queryable);

        /// <summary>
        /// Get list from a queryable.
        /// </summary>
        /// <param name="queryable">Entity queryable interface.</param>
        /// <returns>Return list of entity.</returns>
        Task<List<TEntity>> ToListAsync(IQueryable<TEntity> queryable);
    }
}
