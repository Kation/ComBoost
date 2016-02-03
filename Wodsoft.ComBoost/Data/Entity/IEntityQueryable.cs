using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Entity
{
    /// <summary>
    /// Entity queryable context.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    [Obsolete("Please use IEntityContext to replace IEntityQueryable. IEntityQueryable will be removed in next major version.")]
    public interface IEntityQueryable<TEntity> : IEntityContext<TEntity> where TEntity : class, IEntity, new()
    {
    }
}
