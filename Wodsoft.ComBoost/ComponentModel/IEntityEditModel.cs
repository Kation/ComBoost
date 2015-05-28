using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    public interface IEntityEditModel
    {
        /// <summary>
        /// Get or set the item to edit.
        /// </summary>
        IEntity Item { get; }

        /// <summary>
        /// Get or set the properties to edit.
        /// </summary>
        IEnumerable<IPropertyMetadata> Properties { get; }

        /// <summary>
        /// Get or set the metadata of entity.
        /// </summary>
        IEntityMetadata Metadata { get; }
    }

    public interface IEntityEditModel<TEntity> : IEntityEditModel where TEntity : IEntity
    {

        /// <summary>
        /// Get or set the properties to edit.
        /// </summary>
        TEntity Item { get; }
    }
}
