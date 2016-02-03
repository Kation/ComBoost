using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Entity
{
    /// <summary>
    /// Entity context builder.
    /// </summary>
    public class EntityContextBuilder : IEntityContextBuilder
    {
        private Dictionary<Type, object> cache;
        private bool disposed;

        /// <summary>
        /// Initialize entity context builder.
        /// </summary>
        /// <param name="context">Entity framework database context.</param>
        public EntityContextBuilder(DbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            DbContext = context;
            context.Configuration.ValidateOnSaveEnabled = false;
            List<Type> types = new List<Type>();
            foreach (var property in context.GetType().GetProperties())
            {
                if (!property.PropertyType.IsGenericType)
                    continue;
                if (property.PropertyType.GetGenericTypeDefinition() != typeof(DbSet<>))
                    continue;
                types.Add(property.PropertyType.GetGenericArguments()[0]);
            }
            EntityTypes = types.ToArray();
            cache = new Dictionary<Type, object>();
            DescriptorContext = new EntityDescriptorContext(this);
        }

        /// <summary>
        /// Get entity framework database context.
        /// </summary>
        public DbContext DbContext { get; private set; }

        /// <summary>
        /// Get the descriptor context of builder.
        /// </summary>
        public EntityDescriptorContext DescriptorContext { get; private set; }

        /// <summary>
        /// Get entity context.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity.</typeparam>
        /// <returns>Return entity context.</returns>
        /// <exception cref="ArgumentException">Type of entity doesn't support.</exception>
        public IEntityContext<TEntity> GetContext<TEntity>() where TEntity : class, IEntity, new()
        {
            if (disposed)
                throw new ObjectDisposedException("EntityContextBuilder");
            if (!EntityTypes.Contains(typeof(TEntity)))
                throw new ArgumentException(typeof(TEntity).Name + " doesn't belong to this context。");
            Type type = typeof(TEntity);
            if (!cache.ContainsKey(type))
            {
                IEntityContext<TEntity> result = new EntityContext<TEntity>(DbContext);
                cache.Add(type, result);
                return result;
            }
            return (IEntityContext<TEntity>)cache[type];
        }

        /// <summary>
        /// Get support entity types array.
        /// </summary>
        public Type[] EntityTypes { get; private set; }

        /// <summary>
        /// Get entity context.
        /// </summary>
        /// <param name="entityType">Type of entity.</param>
        /// <returns>Return entity context.</returns>
        /// <exception cref="ArgumentException">Type of entity doesn't support.</exception>
        public object GetContext(Type entityType)
        {
            if (disposed)
                throw new ObjectDisposedException("EntityContextBuilder");
            if (!EntityTypes.Contains(entityType))
                throw new ArgumentException(entityType.Name + " doesn't belong to this context.");
            if (!cache.ContainsKey(entityType))
            {
                object result = Activator.CreateInstance(typeof(EntityContext<>).MakeGenericType(entityType), DbContext);
                cache.Add(entityType, result);
                return result;
            }
            return cache[entityType];
        }

        /// <summary>
        /// Dispose entity context builder.
        /// </summary>
        public void Dispose()
        {
            if (disposed)
                return;
            lock (this)
            {
                disposed = true;
                cache.Clear();
                DbContext.Dispose();
                DbContext = null;
                cache = null;
                DescriptorContext = null;
            }
        }


        /// <summary>
        /// Get data by sql query string.
        /// </summary>
        /// <typeparam name="T">Type of result.</typeparam>
        /// <param name="sql">Sql query string.</param>
        /// <param name="parameters">Query parameters.</param>
        /// <returns>A System.Data.Entity.Infrastructure.DbRawSqlQuery object that will execute the query when it is enumerated.</returns>
        public IEnumerable<T> Query<T>(string sql, params object[] parameters)
        {
            if (disposed)
                throw new ObjectDisposedException("EntityContextBuilder");
            return DbContext.Database.SqlQuery<T>(sql, parameters);
        }
    }
}
