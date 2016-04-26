using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Linq.Expressions;
using System.Data.Entity.Metadata;
using System.Threading.Tasks;

namespace System.Data.Entity
{
    /// <summary>
    /// Entity queryable context.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    [Obsolete("Please use EntityContext to replace EntityQueryable. EntityQueryable will be removed in next major version.")]
    public class EntityQueryable<TEntity> : EntityContext<TEntity>, IEntityQueryable<TEntity> where TEntity : class, IEntity, new()
    {
        /// <summary>
        /// Initialize entity queryable context.
        /// </summary>
        /// <param name="dbContext">Database context of entity framework.</param>
        public EntityQueryable(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
