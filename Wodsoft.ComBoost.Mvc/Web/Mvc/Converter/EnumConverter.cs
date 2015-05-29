using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc.Converter
{
    /// <summary>
    /// Enum converter.
    /// </summary>
    public class EnumConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="sourceType">A System.Type that represents the type you want to convert from.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || sourceType == typeof(int);
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
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="culture">A System.Globalization.CultureInfo. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The System.Object to convert.</param>
        /// <param name="destinationType">The System.Type to convert the value parameter to.</param>
        /// <returns>An System.Object that represents the converted value.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, Globalization.CultureInfo culture, object value, Type destinationType)
        {
            Type type = value.GetType();
            if (type.GetCustomAttribute<FlagsAttribute>() != null)
            {
                Enum v = (Enum)value;
                Array values = Enum.GetValues(type);
                List<string> target = new List<string>();
                foreach (Enum item in values)
                {
                    if (!v.HasFlag(item))
                        continue;
                    string name = Enum.GetName(type, item);
                    FieldInfo field = type.GetField(name, Reflection.BindingFlags.Public | Reflection.BindingFlags.Static);
                    DescriptionAttribute description = field.GetCustomAttribute<DescriptionAttribute>();
                    if (description != null)
                    {
                        target.Add(description.Description);
                        continue;
                    }
                    DisplayAttribute display = field.GetCustomAttribute<DisplayAttribute>();
                    if (display != null)
                        target.Add(display.Name);
                    else
                        target.Add(name);
                }
                return string.Join(", ", target);
            }
            else
            {
                string name = Enum.GetName(type, value);
                FieldInfo field = type.GetField(name, Reflection.BindingFlags.Public | Reflection.BindingFlags.Static);
                DescriptionAttribute description = field.GetCustomAttribute<DescriptionAttribute>();
                if (description != null)
                    return description.Description;
                DisplayAttribute display = field.GetCustomAttribute<DisplayAttribute>();
                if (display != null)
                    return display.Name;
                return name;
            }
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
            Type type = ((EntityValueConverterContext)context).Property.ClrType;
            if (value is int)
                return Enum.ToObject(type, (int)value);
            return Enum.Parse(type, (string)value);
        }
    }
}
