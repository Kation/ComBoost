using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Wodsoft.ComBoost.Business.Controls.EditorItems
{
    public class TimeEditor : EditorItem, IValueConverter
    {
        public TimeEditor(WorkFrame frame)
            : base(frame)
        {
            var time = new TextBox();
            time.DataContext = this;
            var timeBinding = new Binding("Value");
            timeBinding.Mode = BindingMode.TwoWay;
            timeBinding.Converter = this;
            time.SetBinding(TextBox.TextProperty, timeBinding);
            Content = time;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan time;
            if (!TimeSpan.TryParse((string)value, out time))
            {
                return TimeSpan.Zero;
            }
            return time;
        }
    }
}
