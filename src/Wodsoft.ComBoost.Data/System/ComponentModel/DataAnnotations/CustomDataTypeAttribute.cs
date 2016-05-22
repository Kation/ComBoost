using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Property custom data type attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CustomDataTypeAttribute : Attribute
    {
        /// <summary>
        /// Initialize attribute.
        /// </summary>
        /// <param name="type">Type of property.</param>
        public CustomDataTypeAttribute(CustomDataType type)
        {
            Type = type;
            if (type == CustomDataType.File || type == CustomDataType.Image)
                IsFileUpload = true;
        }

        /// <summary>
        /// Initialize attribute.
        /// </summary>
        /// <param name="custom">Custom type of property.</param>
        /// <param name="isFileUpload">Is property base on upload file.</param>
        public CustomDataTypeAttribute(string custom, bool isFileUpload = false)
        {
            if (custom == null)
                throw new ArgumentNullException("custom");
            Type = CustomDataType.Other;
            Custom = custom;
        }

        /// <summary>
        /// Get the type of property.
        /// </summary>
        public CustomDataType Type { get; private set; }

        /// <summary>
        /// Get the custom type of property.
        /// </summary>
        public string Custom { get; private set; }

        /// <summary>
        /// Get the property is base on upload file.
        /// </summary>
        public bool IsFileUpload { get; private set; }
    }
}
