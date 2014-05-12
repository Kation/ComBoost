using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc.Converter
{
    public class SexConverter : System.ComponentModel.BooleanConverter
    {
        public override object ConvertTo(ComponentModel.ITypeDescriptorContext context, Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return (bool)value ? "Male" : "Female";
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
