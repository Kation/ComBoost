using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Data.Entity.Metadata
{
    /// <summary>
    /// Entity metadata base class.
    /// </summary>
    public abstract class EntityMetadataBase : IEntityMetadata
    {
        /// <summary>
        /// Initialize entity metadata.
        /// </summary>
        /// <param name="entityType"></param>
        protected EntityMetadataBase(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType");
            Type = entityType;
            DataBag = new EntityMetadataDataBag();
        }

        public dynamic DataBag { get; }

        /// <summary>
        /// Get the system type of entity.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Get the properties of key of entity.
        /// </summary>
        public abstract IReadOnlyList<IPropertyMetadata> KeyProperties { get; }

        /// <summary>
        /// Get the display name of entity.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Get the display property of entity.
        /// </summary>
        public IPropertyMetadata? DisplayProperty { get; protected set; }

        /// <summary>
        /// Get the sort property of entity.
        /// </summary>
        public abstract IPropertyMetadata SortProperty { get; }

        /// <summary>
        /// Get the sort mode of entity.
        /// </summary>
        public bool IsSortDescending { get; protected set; }

        /// <summary>
        /// Get the properties of entity.
        /// </summary>
        public abstract IReadOnlyList<IPropertyMetadata> Properties { get; }

        public abstract IPropertyMetadata? ParentProperty { get; }

        ///// <summary>
        ///// Get the properties of entity in viewlist.
        ///// </summary>
        //public IReadOnlyList<IPropertyMetadata> ViewProperties { get; private set; }

        ///// <summary>
        ///// Get the properties of entity while create.
        ///// </summary>
        //public IReadOnlyList<IPropertyMetadata> CreateProperties { get; private set; }

        ///// <summary>
        ///// Get the properties of entity while edit.
        ///// </summary>
        //public IReadOnlyList<IPropertyMetadata> EditProperties { get; private set; }

        ///// <summary>
        ///// Get the properties of entity while search.
        ///// </summary>
        //public IReadOnlyList<IPropertyMetadata> SearchProperties { get; private set; }

        ///// <summary>
        ///// Get the properties of entity in detail.
        ///// </summary>
        //public IReadOnlyList<IPropertyMetadata> DetailProperties { get; private set; }

        ///// <summary>
        ///// Get is entity allow anonymous operate.
        ///// </summary>
        //public bool AllowAnonymous { get; protected set; }

        ///// <summary>
        ///// Get roles to view entity.
        ///// </summary>
        //public IEnumerable<object> ViewRoles { get; protected set; }

        ///// <summary>
        ///// Get roles to add entity.
        ///// </summary>
        //public IEnumerable<object> AddRoles { get; protected set; }

        ///// <summary>
        ///// Get roles to edit entity.
        ///// </summary>
        //public IEnumerable<object> EditRoles { get; protected set; }

        ///// <summary>
        ///// Get roles to remove entity.
        ///// </summary>
        //public IEnumerable<object> RemoveRoles { get; protected set; }

        /// <summary>
        /// Get the property of entity.
        /// </summary>
        /// <param name="name">Name of property.</param>
        /// <returns>Return property metadata. Return null if property doesn't exists.</returns>
        public abstract IPropertyMetadata? GetProperty(string name);
    }
}
