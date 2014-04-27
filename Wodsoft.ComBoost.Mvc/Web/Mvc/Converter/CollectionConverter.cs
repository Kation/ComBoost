using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc.Converter
{
    public class CollectionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, Globalization.CultureInfo culture, object value)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            string[] ids = ((string)value).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<object> items = new List<object>();
            dynamic queryable = context.GetService(((EntityValueConverterContext)context).Property.Property.PropertyType.GetGenericArguments()[0]);
            for (int i = 0; i < ids.Length;i++ )
            {
                Guid id;
                if (!Guid.TryParse(ids[i], out id))
                    continue;
                object item = queryable.GetEntity(id);
                if (item != null)
                    items.Add(item);
            }
            return items.ToArray();
        }
    }
}
