using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Wodsoft.ComBoost.Business.Controls.EditorItems
{
    public class DateTimeEditor : EditorItem, IValueConverter
    {
        DatePicker date;
        TextBox time;
        
        public DateTimeEditor(WorkFrame frame)
            : base(frame)
        {
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;
                        
            time = new TextBox();
            time.DataContext = this;
            time.Width = 64;
            var timeBinding = new Binding("Value");
            timeBinding.Mode = BindingMode.TwoWay;
            timeBinding.Converter = this;
            time.SetBinding(TextBox.TextProperty, timeBinding);
            
            date = new DatePicker();
            date.DataContext = this;
            var dateBinding = new Binding("Value");
            dateBinding.Converter = new DateConvert(time);
            dateBinding.Mode = BindingMode.TwoWay;
            date.SetBinding(DatePicker.SelectedDateProperty, dateBinding);

            Button now = new Button();
            now.Content = "当前时间";
            now.Click += (s, e) => Value = DateTime.Now;
            
            panel.Children.Add(date);
            panel.Children.Add(time);
            panel.Children.Add(now);

            Content = panel;
        }
        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is DateTime))
                return "0:0:0";
            DateTime date = (DateTime)value;
            TimeSpan time = date.TimeOfDay;
            return time.Hours + ":" + time.Minutes + ":" + time.Seconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan time;
            if (!TimeSpan.TryParse((string)value, out time))
            {
                this.time.Text = "0:0:0";
                return date.SelectedDate.Value;
            }
            date.SelectedDate = date.SelectedDate.Value.Date.Add(time);
            return date.SelectedDate.Value;
        }

        public class DateConvert : IValueConverter
        {
            private TextBox Time;

            public DateConvert(TextBox time)
            {
                Time = time;
            }

            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (value == null)
                    return DateTime.Now;
                return value;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                DateTime date = (DateTime)value;
                TimeSpan time;
                if (TimeSpan.TryParse(Time.Text, out time))
                {
                    date = date.Date.Add(time);
                }
                return date;
            }
        }
    }
}
