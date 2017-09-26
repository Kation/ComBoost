using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace System.ComponentModel
{
    /// <summary>
    /// Entity edit model.
    /// </summary>
    /// <typeparam name="TEntity">Type of Entity.</typeparam>
    public class EntityEditModel<TEntity> : NotifyBase, IEntityEditModel<TEntity>, IEntityEditModel
        where TEntity : IEntity
    {
        /// <summary>
        /// Initialize entity edit model.
        /// </summary>
        /// <param name="entity">Entity to edit.</param>
        public EntityEditModel(TEntity entity)
        {
            Item = entity;
            Metadata = EntityDescriptor.GetMetadata<TEntity>();
            //Let repository library to set Properties value.
            //Don't: Properties = Metadata.EditProperties;
        }

        /// <summary>
        /// Get or set the properties to edit.
        /// </summary>
        public IEnumerable<IPropertyMetadata> Properties { get; set; }

        /// <summary>
        /// Get or set the metadata of entity.
        /// </summary>
        public IEntityMetadata Metadata { get; set; }

        /// <summary>
        /// Get or set the properties to edit.
        /// </summary>
        public TEntity Item { get { return (TEntity)GetValue(); } set { SetValue(value); } }

        /// <summary>
        /// Get or set the item to edit.
        /// </summary>
        IEntity IEntityEditModel.Item { get { return Item; } }
    }
}
