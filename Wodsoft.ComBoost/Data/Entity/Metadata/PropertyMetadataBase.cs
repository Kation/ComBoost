using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.ObjectModel;

namespace System.Data.Entity.Metadata
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
        public string Name { get; protected set; }

        /// <summary>
        /// Get the property runtime name.
        /// </summary>
        public string ClrName { get; private set; }

        /// <summary>
        /// Get the short name of property.
        /// </summary>
        public string ShortName { get; protected set; }

        /// <summary>
        /// Get the description of property.
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// Get the property is distinct.
        /// </summary>
        public bool IsDistinct { get; protected set; }

        /// <summary>
        /// Get the property is expended.
        /// </summary>
        public bool IsExpended { get; protected set; }

        /// <summary>
        /// Get the runtime type of property.
        /// </summary>
        public Type ClrType { get; private set; }

        /// <summary>
        /// Get the type of property.
        /// </summary>
        public CustomDataType Type { get; protected set; }

        /// <summary>
        /// Get the custom data type of property.
        /// </summary>
        public string CustomType { get; protected set; }

        /// <summary>
        /// Get the property is base on upload file.
        /// </summary>
        public bool IsFileUpload { get; protected set; }

        /// <summary>
        /// Get is the property must has data.
        /// </summary>
        public bool IsRequired { get; protected set; }

        /// <summary>
        /// Get is the property hidden while creating.
        /// </summary>
        public bool IsHiddenOnCreate { get; protected set; }

        /// <summary>
        /// Get is the property hidden while edit.
        /// </summary>
        public bool IsHiddenOnEdit { get; protected set; }

        /// <summary>
        /// Get is the property hidden in viewlist.
        /// </summary>
        public bool IsHiddenOnView { get; protected set; }

        /// <summary>
        /// Get is the property hidden in detail.
        /// </summary>
        public bool IsHiddenOnDetail { get; protected set; }

        /// <summary>
        /// Get the order of property.
        /// </summary>
        public int Order { get; protected set; }

        /// <summary>
        /// Get is the property search able.
        /// </summary>
        public bool Searchable { get; protected set; }

        /// <summary>
        /// Get is property allow anonymous operate.
        /// </summary>
        public bool AllowAnonymous { get; protected set; }

        /// <summary>
        /// Get the roles to view property.
        /// </summary>
        public IEnumerable<object> ViewRoles { get; protected set; }

        /// <summary>
        /// Get the roles to edit property.
        /// </summary>
        public IEnumerable<object> EditRoles { get; protected set; }

        /// <summary>
        /// Get the authentication required mode.
        /// </summary>
        public AuthenticationRequiredMode AuthenticationRequiredMode { get; protected set; }

        /// <summary>
        /// Get is the property can get value.
        /// </summary>
        public bool CanGet { get; protected set; }

        /// <summary>
        /// Get is the property can set value.
        /// </summary>
        public bool CanSet { get; protected set; }

        /// <summary>
        /// Get attribute from metadata.
        /// </summary>
        /// <typeparam name="T">Attribute type.</typeparam>
        /// <returns></returns>
        public abstract T GetAttribute<T>() where T : Attribute;

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
        public abstract PropertyInfo TryGetPropertyInfo();

        /// <summary>
        /// Set the metadata of display.
        /// </summary>
        /// <param name="display">Display attribute.</param>
        protected virtual void SetDisplay(DisplayAttribute display)
        {
            if (display == null)
                throw new ArgumentNullException("display");
            if (display.Name == null)
                Name = ClrName;
            else
                Name = display.Name;
            ShortName = display.ShortName == null ? Name : display.ShortName;
            Description = display.Description;
            Order = display.GetOrder().HasValue ? display.Order : 0;
        }

        /// <summary>
        /// Set the metadata of hide.
        /// </summary>
        /// <param name="hide"></param>
        protected virtual void SetHide(HideAttribute hide)
        {
            if (hide == null)
                throw new ArgumentNullException("hide");
            IsHiddenOnCreate = hide.IsHiddenOnCreate;
            IsHiddenOnDetail = hide.IsHiddenOnDetail;
            IsHiddenOnEdit = hide.IsHiddenOnEdit;
            IsHiddenOnView = hide.IsHiddenOnView;
        }

        /// <summary>
        /// Set the metadata of authentication.
        /// </summary>
        /// <param name="authentication"></param>
        protected virtual void SetAuthentication(PropertyAuthenticationAttribute authentication)
        {
            if (authentication == null)
                throw new ArgumentNullException("authentication");
            AllowAnonymous = authentication.AllowAnonymous;
            EditRoles = new ReadOnlyCollection<object>(authentication.EditRolesRequired);
            ViewRoles = new ReadOnlyCollection<object>(authentication.ViewRolesRequired);
            AuthenticationRequiredMode = authentication.Mode;
        }

        /// <summary>
        /// Set the metadata of data type.
        /// </summary>
        /// <param name="dataType"></param>
        protected virtual void SetDataType(CustomDataTypeAttribute dataType)
        {
            if (dataType == null)
                throw new ArgumentNullException("dataType");
            Type = dataType.Type;
            CustomType = dataType.Custom;
            IsFileUpload = dataType.IsFileUpload;
        }

        /// <summary>
        /// Set the metadata automatic.
        /// </summary>
        protected virtual void SetMetadata()
        {
            var display = GetAttribute<DisplayAttribute>();
            if (display != null)
                SetDisplay(display);
            else
                Name = ClrName;

            var hide = GetAttribute<HideAttribute>();
            if (hide != null)
                SetHide(hide);

            var authentication = GetAttribute<PropertyAuthenticationAttribute>();
            if (authentication != null)
                SetAuthentication(authentication);
            else
            {
                ViewRoles = new string[0];
                EditRoles = new string[0];
                AllowAnonymous = true;
            }

            var dataType = GetAttribute<CustomDataTypeAttribute>();
            if (dataType != null)
                SetDataType(dataType);

            IsRequired = GetAttribute<RequiredAttribute>() != null || (ClrType.IsValueType && !ClrType.IsGenericType);
            Searchable = GetAttribute<SearchableAttribute>() != null;
            IsDistinct = GetAttribute<DistinctAttribute>() != null;
            IsExpended = GetAttribute<ExpendEntityAttribute>() != null || ClrType.GetCustomAttribute<ExpendEntityAttribute>() != null;

        }
    }
}
