using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Entity
{
    /// <summary>
    /// Entity context builder.
    /// </summary>
    public interface IEntityContextBuilder : IDisposable
    {
        /// <summary>
        /// Get entity context.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity.</typeparam>
        /// <returns>Return entity context.</returns>
        /// <exception cref="ArgumentException">Type of entity doesn't support.</exception>
        IEntityContext<TEntity> GetContext<TEntity>() where TEntity : class, IEntity, new();

        /// <summary>
        /// Get entity context.
        /// </summary>
        /// <param name="entityType">Type of entity.</param>
        /// <returns>Return entity context.</returns>
        /// <exception cref="ArgumentException">Type of entity doesn't support.</exception>
        object GetContext(Type entityType);

        /// <summary>
        /// Get the descriptor context of builder.
        /// </summary>
        EntityDescriptorContext DescriptorContext { get; }

        /// <summary>
        /// Get data by sql query.
        /// </summary>
        /// <typeparam name="T">Type of result.</typeparam>
        /// <param name="sql">Sql query.</param>
        /// <param name="parameters">Query parameters.</param>
        /// <returns>Return enumerable data.</returns>
        IEnumerable<T> Query<T>(string sql, params object[] parameters);

        ///// <summary>
        ///// Get data by sql query.
        ///// </summary>
        ///// <typeparam name="T">Type of result.</typeparam>
        ///// <param name="sql">Sql query.</param>
        ///// <param name="parameters">Query parameters.</param>
        ///// <returns>Return enumerable data.</returns>
        //T Execute<T>(string sql, params object[] parameters);

        /// <summary>
        /// Get support entity types array.
        /// </summary>
        Type[] EntityTypes { get; }
    }
}
