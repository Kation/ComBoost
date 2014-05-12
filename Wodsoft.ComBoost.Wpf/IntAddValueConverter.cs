using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Wodsoft.ComBoost.Wpf
{
    public class IntAddValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is int))
                throw new NotSupportedException();
            int add;
            if (parameter is int)
                add = (int)parameter;
            else if (parameter is string)
                add = int.Parse((string)parameter);
            else
                throw new NotSupportedException();
            return (int)value + add;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
