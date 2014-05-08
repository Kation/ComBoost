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
    /// Entity edit model.
    /// </summary>
    public class EntityEditModel : NotifyBase
    {
        /// <summary>
        /// Get or set the item to edit.
        /// </summary>
        public IEntity Item { get { return (IEntity)GetValue("Item"); } set { SetValue("Item", value); } }

        /// <summary>
        /// Get or set the properties to edit.
        /// </summary>
        public PropertyMetadata[] Properties { get { return (PropertyMetadata[])GetValue("Properties"); } set { SetValue("Properties", value); } }

        /// <summary>
        /// Get or set the metadata of entity.
        /// </summary>
        public EntityMetadata Metadata { get; set; }
    }

    /// <summary>
    /// Entity edit model.
    /// </summary>
    /// <typeparam name="TEntity">Type of Entity.</typeparam>
    public class EntityEditModel<TEntity> : EntityEditModel where TEntity : IEntity
    {
        /// <summary>
        /// Initialize entity edit model.
        /// </summary>
        /// <param name="entity">Entity to edit.</param>
        public EntityEditModel(TEntity entity)
        {
            Item = entity;
            Metadata = EntityAnalyzer.GetMetadata<TEntity>();
            Properties = Metadata.EditProperties;
        }

        /// <summary>
        /// Get or set the properties to edit.
        /// </summary>
        public new TEntity Item { get { return (TEntity)base.Item; } set { base.Item = value; } }
    }

}
