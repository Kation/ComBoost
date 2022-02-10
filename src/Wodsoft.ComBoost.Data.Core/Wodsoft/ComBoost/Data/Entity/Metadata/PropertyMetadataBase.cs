using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Data.Entity.Metadata
{
    /// <summary>
    /// Property metadata base class.
    /// </summary>
    public abstract class PropertyMetadataBase : IPropertyMetadata
    {
        /// <summary>
        /// Initialize property metadata.
        /// </summary>
        /// <param name="clrName">Clr name of property.</param>
        /// <param name="clrType">Clr type of property.</param>
        protected PropertyMetadataBase(string clrName, Type clrType)
        {
            if (clrName == null)
                throw new ArgumentNullException("clrName");
            if (clrType == null)
                throw new ArgumentNullException("clrType");
            ClrName = clrName;
            ClrType = clrType;
        }

        /// <summary>
        /// Get the property display name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Get the property runtime name.
        /// </summary>
        public string ClrName { get; private set; }

        /// <summary>
        /// Get the short name of property.
        /// </summary>
        public abstract string ShortName { get; }

        /// <summary>
        /// Get the description of property.
        /// </summary>
        public abstract string? Description { get; }

        /// <summary>
        /// Get the property is distinct.
        /// </summary>
        public abstract bool IsDistinct { get; }

        /// <summary>
        /// Get the property is expended.
        /// </summary>
        public abstract bool IsExpended { get; }

        /// <summary>
        /// Get the runtime type of property.
        /// </summary>
        public Type ClrType { get; private set; }

        /// <summary>
        /// Get the type of property.
        /// </summary>
        public abstract CustomDataType Type { get; }

        /// <summary>
        /// Get the custom data type of property.
        /// </summary>
        public abstract string? CustomType { get; }

        /// <summary>
        /// 获取或设置类型转换器。
        /// </summary>
        public abstract TypeConverter Converter { get; }

        ///// <summary>
        ///// Get the property is base on upload file.
        ///// </summary>
        //public bool IsFileUpload { get; protected set; }

        /// <summary>
        /// Get is the property must has data.
        /// </summary>
        public abstract bool IsRequired { get; }

        public abstract bool IsKey { get; }

        ///// <summary>
        ///// Get is the property hidden while creating.
        ///// </summary>
        //public bool IsHiddenOnCreate { get; protected set; }

        ///// <summary>
        ///// Get is the property hidden while edit.
        ///// </summary>
        //public bool IsHiddenOnEdit { get; protected set; }

        ///// <summary>
        ///// Get is the property hidden in viewlist.
        ///// </summary>
        //public bool IsHiddenOnView { get; protected set; }

        ///// <summary>
        ///// Get is the property hidden in detail.
        ///// </summary>
        //public bool IsHiddenOnDetail { get; protected set; }

        /// <summary>
        /// Get the order of property.
        /// </summary>
        public int Order { get; protected set; }

        ///// <summary>
        ///// Get is the property search able.
        ///// </summary>
        //public bool Searchable { get; protected set; }

        ///// <summary>
        ///// Get is property allow anonymous operate.
        ///// </summary>
        //public bool AllowAnonymous { get; protected set; }

        ///// <summary>
        ///// Get the roles to view property.
        ///// </summary>
        //public IEnumerable<object> ViewRoles { get; protected set; }

        ///// <summary>
        ///// Get the roles to edit property.
        ///// </summary>
        //public IEnumerable<object> AddRoles { get; protected set; }

        ///// <summary>
        ///// Get the roles to edit property.
        ///// </summary>
        //public IEnumerable<object> EditRoles { get; protected set; }

        /// <summary>
        /// Get is the property can get value.
        /// </summary>
        public abstract bool CanGet { get; }

        /// <summary>
        /// Get is the property can set value.
        /// </summary>
        public abstract bool CanSet { get; }

        /// <summary>
        /// Get attribute from metadata.
        /// </summary>
        /// <typeparam name="T">Attribute type.</typeparam>
        /// <returns></returns>
        public abstract T? GetAttribute<T>() where T : Attribute;

        /// <summary>
        /// Get attributes from metadata.
        /// </summary>
        /// <typeparam name="T">Attribute type.</typeparam>
        /// <returns></returns>
        public abstract T[] GetAttributes<T>() where T : Attribute;

        /// <summary>
        /// Get property value from an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns></returns>
        public abstract object GetValue(object entity);

        /// <summary>
        /// Set property value to an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="value">Value.</param>
        public abstract void SetValue(object entity, object value);

        /// <summary>
        /// Try get PropertyInfo from metadata.
        /// </summary>
        /// <returns>Return value if there can be a PropertyInfo.</returns>
        public abstract PropertyInfo? TryGetPropertyInfo();
    }
}
