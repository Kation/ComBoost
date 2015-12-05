using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Wodsoft.ComBoost.Wpf
{
    public class BooleanToVisibilityValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            if (value is bool)
            {
                var result = (bool)value;
                if (parameter is bool)
                    if ((bool)parameter)
                        return result ? Visibility.Visible : Visibility.Collapsed;
                    else
                        return !result ? Visibility.Visible : Visibility.Collapsed;
                else if (parameter is string)
                {
                    if ((string)parameter == "true")
                        return result ? Visibility.Visible : Visibility.Collapsed;
                    else if ((string)parameter == "false")
                        return !result ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                    return result ? Visibility.Visible : Visibility.Collapsed;

            }
            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
