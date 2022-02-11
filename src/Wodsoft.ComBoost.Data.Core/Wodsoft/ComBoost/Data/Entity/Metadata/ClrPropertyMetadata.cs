﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wodsoft.ComBoost.Data.Entity.Metadata
{
    /// <summary>
    /// Clr Property metadata.
    /// </summary>
    public class ClrPropertyMetadata : PropertyMetadataBase
    {
        /// <summary>
        /// Initialize property metadata.
        /// </summary>
        /// <param name="propertyInfo">Property info.</param>
        public ClrPropertyMetadata(PropertyInfo propertyInfo)
            : base(propertyInfo.Name, propertyInfo.PropertyType)
        {
            Property = propertyInfo;
            CanGet = propertyInfo.GetGetMethod() != null;
            CanSet = propertyInfo.GetSetMethod() != null;
            if (CanGet)
                _GetValue = propertyInfo.GetGetMethodDelegate();
            if (CanSet)
                _SetValue = propertyInfo.GetSetMethodDelegate();


            var display = GetAttribute<DisplayAttribute>();
            if (display != null)
            {
                if (display.Name == null)
                    Name = ClrName;
                else
                    Name = display.Name;
                ShortName = display.ShortName == null ? Name : display.ShortName;
                Description = display.Description;
                Order = display.GetOrder().HasValue ? display.Order : 0;
            }
            else
            {
                Name = ClrName;
                ShortName = Name;
            }


            //HideAttribute hide = propertyInfo.GetCustomAttribute<HideAttribute>();
            //if (hide == null)
            //    if (!propertyInfo.PropertyType.GetTypeInfo().IsValueType && propertyInfo.PropertyType.GetTypeInfo().IsGenericType)
            //        IsHiddenOnView = true;

            string? customType;
            bool isFileUpload;
            Type = propertyInfo.GetCustomDataType(out customType, out isFileUpload);
            CustomType = customType;
            Converter = TypeDescriptor.GetConverter(ClrType);

            IsKey = GetAttribute<KeyAttribute>() != null;
            IsRequired = GetAttribute<RequiredAttribute>() != null || (ClrType.GetTypeInfo().IsValueType && !ClrType.GetTypeInfo().IsGenericType);
            IsDistinct = GetAttribute<DistinctAttribute>() != null;
        }


        /// <summary>
        /// Get the property info.
        /// </summary>
        public PropertyInfo Property { get; private set; }

        public override string Name { get; }

        public override string ShortName { get; }

        public override string? Description { get; }

        public override TypeConverter Converter { get; }

        public override bool IsDistinct { get; }

        public override bool IsExpended { get; }

        public override CustomDataType Type { get; }

        public override string? CustomType { get; }

        public override bool IsRequired { get; }

        public override bool IsKey { get; }

        public override bool CanGet { get; }

        public override bool CanSet { get; }

        /// <summary>
        /// Get property display name.
        /// </summary>
        /// <returns>Default return name of display + "-" + name of property.</returns>
        public override string ToString()
        {
            return Name + "-" + Property.Name;
        }

        /// <summary>
        /// Get attribute from metadata.
        /// </summary>
        /// <typeparam name="T">Attribute type.</typeparam>
        /// <returns></returns>
        public override T GetAttribute<T>()
        {
            return Property.GetCustomAttribute<T>(true);
        }

        /// <summary>
        /// Get attributes from metadata.
        /// </summary>
        /// <typeparam name="T">Attribute type.</typeparam>
        /// <returns></returns>
        public override T[] GetAttributes<T>()
        {
            return Property.GetCustomAttributes<T>(true).ToArray();
        }

        private Func<object, object>? _GetValue;
        /// <summary>
        /// Get property value from an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns></returns>
        public override object GetValue(object entity)
        {
            if (!CanGet)
                throw new NotSupportedException("Property doen't support get method.");
            return _GetValue!(entity);
        }

        private Action<object, object?>? _SetValue;
        /// <summary>
        /// Set property value to an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="value">Value.</param>
        public override void SetValue(object entity, object? value)
        {
            if (!CanSet)
                throw new NotSupportedException("Property doen't support set method.");
            _SetValue!(entity, value);
        }

        /// <summary>
        /// Try get PropertyInfo from metadata.
        /// </summary>
        /// <returns>Return value if there can be a PropertyInfo.</returns>
        public override PropertyInfo TryGetPropertyInfo()
        {
            return Property;
        }
    }
}
