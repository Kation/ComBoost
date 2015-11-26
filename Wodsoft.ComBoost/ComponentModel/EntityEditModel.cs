using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    ///// <summary>
    ///// Entity edit model.
    ///// </summary>
    //public class EntityEditModel : NotifyBase
    //{
    //    /// <summary>
    //    /// Get or set the item to edit.
    //    /// </summary>
    //    public IEntity Item { get { return (IEntity)GetValue(); } set { SetValue(value); } }

    //    /// <summary>
    //    /// Get or set the properties to edit.
    //    /// </summary>
    //    public IEnumerable<IPropertyMetadata> Properties { get { return (IEnumerable<IPropertyMetadata>)GetValue(); } set { SetValue(value); } }

    //    /// <summary>
    //    /// Get or set the metadata of entity.
    //    /// </summary>
    //    public IEntityMetadata Metadata { get; set; }
    //}

    /// <summary>
    /// Entity edit model.
    /// </summary>
    /// <typeparam name="TEntity">Type of Entity.</typeparam>
    public class EntityEditModel<TEntity> : NotifyBase, IEntityEditModel<TEntity>, IEntityEditModel where TEntity : IEntity
    {
        /// <summary>
        /// Initialize entity edit model.
        /// </summary>
        /// <param name="entity">Entity to edit.</param>
        public EntityEditModel(TEntity entity)
        {
            Item = entity;
            Metadata = EntityAnalyzer.GetMetadata<TEntity>();
            //Let repository library to set Properties value.
            //Don't: Properties = Metadata.EditProperties;
        }

        /// <summary>
        /// Get or set the properties to edit.
        /// </summary>
        public IEnumerable<IPropertyMetadata> Properties { get { return (IEnumerable<IPropertyMetadata>)GetValue(); } set { SetValue(value); } }

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
        IEntity IEntityEditModel.Item { get { return (IEntity)GetValue(); } }
    }

}
