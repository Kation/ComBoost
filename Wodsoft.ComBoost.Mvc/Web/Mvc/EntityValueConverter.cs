using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc.Converter;

namespace System.Web.Mvc
{
    /// <summary>
    /// Entity value converter.
    /// </summary>
    public static class EntityValueConverter
    {
        private static Dictionary<CustomDataType, TypeConverter> _DefaultItems;
        private static Dictionary<string, TypeConverter> _CustomItems;

        static EntityValueConverter()
        {
            _DefaultItems = new Dictionary<CustomDataType, TypeConverter>();
            _CustomItems = new Dictionary<string, TypeConverter>();

            //AddConverter(CustomDataType.Boolean, new BooleanConverter());
            //AddConverter(CustomDataType.Currency, new DecimalConverter());
            //AddConverter(CustomDataType.Date, new DateConverter());
            AddConverter(CustomDataType.DateTime, new System.Web.Mvc.Converter.DateTimeConverter());
            //AddConverter(CustomDataType.Default, new StringConverter());
            //AddConverter(CustomDataType.EmailAddress, new StringConverter());
            //AddConverter(CustomDataType.File, new StringConverter());
            //AddConverter(CustomDataType.Html, new StringConverter());
            //AddConverter(CustomDataType.Image, new StringConverter());
            //AddConverter(CustomDataType.ImageUrl, new StringConverter());
            //AddConverter(CustomDataType.Integer, new Int32Converter());
            //AddConverter(CustomDataType.MultilineText, new StringConverter());
            //AddConverter(CustomDataType.Number, new DoubleConverter());
            //AddConverter(CustomDataType.Password, new StringConverter());
            //AddConverter(CustomDataType.PhoneNumber, new StringConverter());
            AddConverter(CustomDataType.Sex, new SexConverter());
            //AddConverter(CustomDataType.Text, new StringConverter());
            //AddConverter(CustomDataType.Time, new TimeSpanConverter());
            //AddConverter(CustomDataType.Url, new StringConverter());

            AddConverter("Enum", new System.Web.Mvc.Converter.EnumConverter());
            AddConverter("Entity", new EntityConverter());
            AddConverter("Collection", new System.Web.Mvc.Converter.CollectionConverter());
            AddConverter("ValueFilter", new StringConverter());
        }

        /// <summary>
        /// Add a converter for a type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="converter">Converter.</param>
        public static void AddConverter(CustomDataType type, TypeConverter converter)
        {
            if (type == CustomDataType.Other)
                throw new InvalidOperationException("Not support Other type.");
            if (converter == null)
                throw new ArgumentNullException("converter");
            if (_DefaultItems.ContainsKey(type))
                _DefaultItems[type] = converter;
            else
                _DefaultItems.Add(type, converter);
        }

        /// <summary>
        /// Add a converter for a type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="converter">Converter.</param>
        public static void AddConverter(string type, TypeConverter converter)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (converter == null)
                throw new ArgumentNullException("converter");
            if (_CustomItems.ContainsKey(type))
                _CustomItems[type] = converter;
            else
                _CustomItems.Add(type, converter);
        }

        /// <summary>
        /// Remove converter for a type.
        /// </summary>
        /// <param name="type">Defined type.</param>
        public static void RemoveConverter(CustomDataType type)
        {
            if (_DefaultItems.ContainsKey(type))
                _DefaultItems.Remove(type);
        }

        /// <summary>
        /// Remove converter for a type.
        /// </summary>
        /// <param name="type">Custom type.</param>
        public static void RemoveConverter(string type)
        {
            if (_CustomItems.ContainsKey(type))
                _CustomItems.Remove(type);
        }

        /// <summary>
        /// Get converter for a type.
        /// </summary>
        /// <param name="metadata">Property metadata.</param>
        /// <returns></returns>
        public static TypeConverter GetConverter(IPropertyMetadata metadata)
        {
            if (metadata == null)
                throw new ArgumentNullException("metadata");
            TypeConverter converter;
            if (metadata.Type == CustomDataType.Other)
                converter = GetConverter(metadata.CustomType);
            else
                converter = GetConverter(metadata.Type);
            if (converter == null)
                converter = TypeDescriptor.GetConverter(metadata.ClrType);
            return converter;
        }

        /// <summary>
        /// Get converter for a type.
        /// </summary>
        /// <param name="type">Defined type.</param>
        /// <returns></returns>
        public static TypeConverter GetConverter(CustomDataType type)
        {
            TypeConverter converter;
            _DefaultItems.TryGetValue(type, out converter);
            return converter;
        }

        /// <summary>
        /// Get converter for a type.
        /// </summary>
        /// <param name="type">Custom type.</param>
        /// <returns></returns>
        public static TypeConverter GetConverter(string type)
        {
            TypeConverter converter;
            _CustomItems.TryGetValue(type, out converter);
            return converter;
        }
    }
}
