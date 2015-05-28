using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    /// <summary>
    /// Entity search model.
    /// </summary>
    public class EntitySearchModel
    {
        /// <summary>
        /// Get or set the properties able to search.
        /// </summary>
        public IEnumerable<IPropertyMetadata> Properties { get; set; }

        /// <summary>
        /// Get or set the metadata of entity.
        /// </summary>
        public IEntityMetadata Metadata { get; set; }
    }

    /// <summary>
    /// Entity search model.
    /// </summary>
    /// <typeparam name="TEntity">Type of Entity.</typeparam>
    public class EntitySearchModel<TEntity> : EntitySearchModel
    {
        /// <summary>
        /// Initialize entity search model.
        /// </summary>
        public EntitySearchModel()
        {
            Metadata = EntityAnalyzer.GetMetadata<TEntity>();
            Properties = Metadata.SearchProperties;
        }
    }
}
