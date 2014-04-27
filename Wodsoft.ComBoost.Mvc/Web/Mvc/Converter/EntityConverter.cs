using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc.Converter
{
    public class EntityConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, Globalization.CultureInfo culture, object value)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            Guid id;
            if (!Guid.TryParse((string)value, out id))
                return null;
            dynamic queryable = context.GetService(((EntityValueConverterContext)context).Property.Property.PropertyType);
            return queryable.GetEntity(id);
        }

        public override object ConvertTo(ITypeDescriptorContext context, Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (!(value is EntityBase))
                return null;
            return ((EntityBase)value).ToString();
        }
    }
}
