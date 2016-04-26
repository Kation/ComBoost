using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Entity.Metadata
{
    /// <summary>
    /// Property metadata interface.
    /// </summary>
    public interface IPropertyMetadata
    {
        /// <summary>
        /// Get the property name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get the property runtime name.
        /// </summary>
        string ClrName { get; }

        /// <summary>
        /// Get the short name of property.
        /// </summary>
        string ShortName { get; }

        /// <summary>
        /// Get the description of property.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Get the property is distinct.
        /// </summary>
        bool IsDistinct { get; }

        /// <summary>
        /// Get the property is expended.
        /// </summary>
        bool IsExpended { get; }

        /// <summary>
        /// Get the runtime type of property.
        /// </summary>
        Type ClrType { get; }

        /// <summary>
        /// Get the type of property.
        /// </summary>
        CustomDataType Type { get; }

        /// <summary>
        /// Get the custom data type of property.
        /// </summary>
        string CustomType { get; }

        /// <summary>
        /// Get the property is base on upload file.
        /// </summary>
        bool IsFileUpload { get; }

        /// <summary>
        /// Get is the property must has data.
        /// </summary>
        bool IsRequired { get; }
        
        /// <summary>
        /// Get is the property hidden while creating.
        /// </summary>
        bool IsHiddenOnCreate { get; }

        /// <summary>
        /// Get is the property hidden while edit.
        /// </summary>
        bool IsHiddenOnEdit { get; }

        /// <summary>
        /// Get is the property hidden in viewlist.
        /// </summary>
        bool IsHiddenOnView { get; }

        /// <summary>
        /// Get is the property hidden in detail.
        /// </summary>
        bool IsHiddenOnDetail { get; }

        /// <summary>
        /// Get the order of property.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Get is the property search able.
        /// </summary>
        bool Searchable { get; }

        /// <summary>
        /// Get is property allow anonymous operate.
        /// </summary>
        bool AllowAnonymous { get; }

        /// <summary>
        /// Get the roles to view property.
        /// </summary>
        IEnumerable<object> ViewRoles { get; }

        /// <summary>
        /// Get the roles to edit property.
        /// </summary>
        IEnumerable<object> EditRoles { get; }
        
        /// <summary>
        /// Get the authentication required mode.
        /// </summary>
        AuthenticationRequiredMode AuthenticationRequiredMode { get; }

        /// <summary>
        /// Get is the property can get value.
        /// </summary>
        bool CanGet { get; }

        /// <summary>
        /// Get is the property can set value.
        /// </summary>
        bool CanSet { get; }

        /// <summary>
        /// Get attribute from metadata.
        /// </summary>
        /// <typeparam name="T">Attribute type.</typeparam>
        /// <returns></returns>
        T GetAttribute<T>() where T : Attribute;

        /// <summary>
        /// Get attributes from metadata.
        /// </summary>
        /// <typeparam name="T">Attribute type.</typeparam>
        /// <returns></returns>
        T[] GetAttributes<T>() where T : Attribute;

        /// <summary>
        /// Get property value from an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns></returns>
        object GetValue(object entity);

        /// <summary>
        /// Set property value to an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="value">Value.</param>
        void SetValue(object entity, object value);

        /// <summary>
        /// Try get PropertyInfo from metadata.
        /// </summary>
        /// <returns>Return value if there can be a PropertyInfo.</returns>
        PropertyInfo TryGetPropertyInfo();
    }
}
