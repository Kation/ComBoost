using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Wodsoft.ComBoost.Wpf
{
    public class IsNullToBooleanValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter == null)
                return value == null ? true : false;
            if (parameter is bool)
                if ((bool)parameter)
                    return value == null ? true : false;
                else
                    return value != null ? true : false;
            else if (parameter is string)
                if ((string)parameter == "true")
                    return value == null ? true : false;
                else if ((string)parameter == "false")
                    return value != null ? true : false;
            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
