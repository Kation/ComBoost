using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Entity.Metadata
{
    /// <summary>
    /// Entity metadata interface.
    /// </summary>
    public interface IEntityMetadata
    {
        /// <summary>
        /// Get the system type of entity.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Get the system type of key of entity.
        /// </summary>
        Type KeyType { get; }

        /// <summary>
        /// Get the display name of entity.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get the display property of entity.
        /// </summary>
        IPropertyMetadata DisplayProperty { get; }

        /// <summary>
        /// Get the sort property of entity.
        /// </summary>
        IPropertyMetadata SortProperty { get; }

        /// <summary>
        /// Get the parent property of entity.
        /// </summary>
        IPropertyMetadata ParentProperty { get; }

        /// <summary>
        /// Get the sort mode of entity.
        /// </summary>
        bool SortDescending { get; }

        /// <summary>
        /// Get the properties of entity.
        /// </summary>
        IEnumerable<IPropertyMetadata> Properties { get; }

        /// <summary>
        /// Get the properties of entity in viewlist.
        /// </summary>
        IEnumerable<IPropertyMetadata> ViewProperties { get; }

        /// <summary>
        /// Get the properties of entity while create.
        /// </summary>
        IEnumerable<IPropertyMetadata> CreateProperties { get; }

        /// <summary>
        /// Get the properties of entity while edit.
        /// </summary>
        IEnumerable<IPropertyMetadata> EditProperties { get; }

        /// <summary>
        /// Get the properties of entity while search.
        /// </summary>
        IEnumerable<IPropertyMetadata> SearchProperties { get; }

        /// <summary>
        /// Get the properties of entity in detail.
        /// </summary>
        IEnumerable<IPropertyMetadata> DetailProperties { get; }

        /// <summary>
        /// Get is entity allow anonymous operate.
        /// </summary>
        bool AllowAnonymous { get; }

        /// <summary>
        /// Get the roles to view entity.
        /// </summary>
        IEnumerable<object> ViewRoles { get; }

        /// <summary>
        /// Get the roles to add entity.
        /// </summary>
        IEnumerable<object> AddRoles { get; }

        /// <summary>
        /// Get the roles to edit entity.
        /// </summary>
        IEnumerable<object> EditRoles { get; }

        /// <summary>
        /// Get the roles to remove entity.
        /// </summary>
        IEnumerable<object> RemoveRoles { get; }

        /// <summary>
        /// Get the authentication required mode.
        /// </summary>
        AuthenticationRequiredMode AuthenticationRequiredMode { get; }

        /// <summary>
        /// Get the property of entity.
        /// </summary>
        /// <param name="name">Name of property.</param>
        /// <returns>Return property metadata. Return null if property doesn't exists.</returns>
        IPropertyMetadata GetProperty(string name);
    }
}
