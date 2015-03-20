using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Data.Entity.Metadata
{
    /// <summary>
    /// Property metadata.
    /// </summary>
    public class PropertyMetadata
    {
        /// <summary>
        /// Initialize property metadata.
        /// </summary>
        /// <param name="propertyInfo">Property info.</param>
        public PropertyMetadata(PropertyInfo propertyInfo)
        {
            Property = propertyInfo;

            DisplayAttribute display = propertyInfo.GetCustomAttribute<DisplayAttribute>();
            if (display != null)
            {
                Name = display.Name == null ? propertyInfo.Name : display.Name;
                if (display.Description != null)
                    Description = display.Description;
                Order = display.GetOrder().HasValue ? display.Order : 0;
                ShortName = display.ShortName == null ? Name : display.ShortName;
            }
            else
            {
                Name = propertyInfo.Name;
            }

            RequiredAttribute required = propertyInfo.GetCustomAttribute<RequiredAttribute>();
            IsRequired = required != null;

            HideAttribute hide = propertyInfo.GetCustomAttribute<HideAttribute>();
            if (hide != null)
            {
                IsHiddenOnEdit = hide.IsHiddenOnEdit;
                IsHiddenOnView = hide.IsHiddenOnView;
                IsHiddenOnDetail = hide.IsHiddenOnDetail;
            }
            else
            {
                if (propertyInfo.PropertyType.IsGenericType)
                    IsHiddenOnView = true;
            }

            var customDataType = propertyInfo.GetCustomAttribute<CustomDataTypeAttribute>();
            if (customDataType != null)
            {
                Type = customDataType.Type;
                CustomType = customDataType.Custom;
                IsFileUpload = customDataType.IsFileUpload;
            }
            else
            {
                Type type = propertyInfo.PropertyType;
                ValueFilterAttribute filter = propertyInfo.GetCustomAttribute<ValueFilterAttribute>();
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    type = type.GetGenericArguments()[0];
                if (filter != null)
                {
                    Type = CustomDataType.Other;
                    CustomType = "ValueFilter";
                }
                else if (type == typeof(DateTime))
                    Type = CustomDataType.Date;
                else if (type == typeof(TimeSpan))
                    Type = CustomDataType.Time;
                else if (type == typeof(bool))
                    Type = CustomDataType.Boolean;
                else if (type == typeof(short) || type == typeof(int) || type == typeof(long))
                    Type = CustomDataType.Integer;
                else if (type == typeof(float) || type == typeof(double))
                    Type = CustomDataType.Number;
                else if (type == typeof(decimal))
                    Type = CustomDataType.Currency;
                else if (type == typeof(byte[]))
                {
                    Type = CustomDataType.File;
                    IsFileUpload = true;
                }
                else if (type.IsEnum)
                {
                    Type = CustomDataType.Other;
                    CustomType = "Enum";
                }
                else if (type.IsGenericType)
                {
                    Type = CustomDataType.Other;
                    CustomType = "Collection";
                }
                else if (typeof(IEntity).IsAssignableFrom(type))
                {
                    Type = CustomDataType.Other;
                    CustomType = "Entity";
                }
            }

            if (Type != CustomDataType.Image && Type != CustomDataType.Password && Type != CustomDataType.Time)
            {
                if (CustomType == null || CustomType == "Entity" || CustomType == "Enum")
                {
                    SearchableAttribute searchable = propertyInfo.GetCustomAttribute<SearchableAttribute>();
                    Searchable = searchable != null;
                }
            }

            IsDistinct = propertyInfo.GetCustomAttribute<DistinctAttribute>() != null;

            IsExpended = propertyInfo.GetCustomAttribute<ExpendEntityAttribute>() != null || propertyInfo.PropertyType.GetCustomAttribute<ExpendEntityAttribute>() != null;
        }

        /// <summary>
        /// Get the property info.
        /// </summary>
        public PropertyInfo Property { get; private set; }

        /// <summary>
        /// Get the property name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Get the short name of property.
        /// </summary>
        public string ShortName { get; private set; }

        /// <summary>
        /// Get the description of property.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Get the property is distinct.
        /// </summary>
        public bool IsDistinct { get; private set; }

        /// <summary>
        /// Get the property is expended.
        /// </summary>
        public bool IsExpended { get; private set; }

        /// <summary>
        /// Get the type of property.
        /// </summary>
        public CustomDataType Type { get; private set; }

        /// <summary>
        /// Get the custom data type of property.
        /// </summary>
        public string CustomType { get; private set; }

        /// <summary>
        /// Get the property is base on upload file.
        /// </summary>
        public bool IsFileUpload { get; private set; }

        /// <summary>
        /// Get is the property must has data.
        /// </summary>
        public bool IsRequired { get; private set; }

        /// <summary>
        /// Get is the property hidden while edit.
        /// </summary>
        public bool IsHiddenOnEdit { get; private set; }

        /// <summary>
        /// Get is the property hidden in viewlist.
        /// </summary>
        public bool IsHiddenOnView { get; private set; }
        
        /// <summary>
        /// Get is the property hidden in detail.
        /// </summary>
        public bool IsHiddenOnDetail { get; private set; }

        /// <summary>
        /// Get the order of property.
        /// </summary>
        public int Order { get; private set; }

        private int? _MaxLength;
        /// <summary>
        /// Get the maximum length of property.
        /// </summary>
        public int MaxLength
        {
            get
            {
                if (!_MaxLength.HasValue)
                {
                    var maxLength = Property.GetCustomAttribute<MaxLengthAttribute>();
                    if (maxLength == null)
                        _MaxLength = int.MaxValue;
                    else
                        _MaxLength = maxLength.Length;
                }
                return _MaxLength.Value;
            }
        }

        private int? _MinLength;
        /// <summary>
        /// Get the minimum length of property.
        /// </summary>
        public int MinLength
        {
            get
            {
                if (!_MinLength.HasValue)
                {
                    var minLength = Property.GetCustomAttribute<MinLengthAttribute>();
                    if (minLength == null)
                        _MinLength = int.MaxValue;
                    else
                        _MinLength = minLength.Length;
                }
                return _MinLength.Value;
            }
        }

        //public string[] Roles { get; private set; }

        /// <summary>
        /// Get is the property search able.
        /// </summary>
        public bool Searchable { get; private set; }

        /// <summary>
        /// Get property display name.
        /// </summary>
        /// <returns>Default return name of display + "-" + name of property.</returns>
        public override string ToString()
        {
            return Name + "-" + Property.Name;
        }
    }
}
