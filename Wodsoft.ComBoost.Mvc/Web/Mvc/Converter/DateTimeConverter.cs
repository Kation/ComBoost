using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc.Converter
{
    /// <summary>
    /// DateTime converter.
    /// </summary>
    public class DateTimeConverter : TypeConverter
    {        
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="sourceType">A System.Type that represents the type you want to convert from.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="destinationType">A System.Type that represents the type you want to convert to.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="culture">The System.Globalization.CultureInfo to use as the current culture.</param>
        /// <param name="value">The System.Object to convert.</param>
        /// <returns>An System.Object that represents the converted value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, Globalization.CultureInfo culture, object value)
        {
            if (string.IsNullOrEmpty((string)value))
                return null;
            return DateTime.Parse((string)value);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="culture">A System.Globalization.CultureInfo. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The System.Object to convert.</param>
        /// <param name="destinationType">The System.Type to convert the value parameter to.</param>
        /// <returns>An System.Object that represents the converted value.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
                return "";
            return ((DateTime)value).ToLongDateString() + " " + ((DateTime)value).ToLongTimeString();
        }
    }
}
