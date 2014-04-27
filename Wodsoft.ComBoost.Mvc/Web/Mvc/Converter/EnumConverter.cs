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
    public class EnumConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || sourceType == typeof(int);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, Globalization.CultureInfo culture, object value, Type destinationType)
        {
            Type type = value.GetType();
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

        public override object ConvertFrom(ITypeDescriptorContext context, Globalization.CultureInfo culture, object value)
        {
            Type type = ((EntityValueConverterContext)context).Property.Property.PropertyType;
            if (value is int)
                return Enum.ToObject(type, (int)value);
            return Enum.Parse(type, (string)value);
        }
    }
}
