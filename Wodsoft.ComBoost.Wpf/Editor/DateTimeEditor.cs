using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wodsoft.ComBoost.Wpf.Editor
{
    [TemplatePart(Name = "PART_Time", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_Date", Type = typeof(DatePicker))]
    public class DateTimeEditor : EditorBase
    {
        static DateTimeEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimeEditor), new FrameworkPropertyMetadata(typeof(DateTimeEditor)));
        }

        protected DatePicker DatePicker { get; private set; }
        protected TextBox TextBox { get; private set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            DatePicker = (DatePicker)GetTemplateChild("PART_Date");
            DatePicker.SetBinding(DatePicker.SelectedDateProperty, new Binding { Source = this, Path = new PropertyPath(CurrentValueProperty), Mode = BindingMode.OneWay });
            DatePicker.SelectedDateChanged += DatePicker_SelectedDateChanged;
            TextBox = (TextBox)GetTemplateChild("PART_Time");
            TextBox.SetBinding(DatePicker.SelectedDateProperty, new Binding { Source = this, Path = new PropertyPath(CurrentValueProperty), Mode = BindingMode.OneWay, Converter = new DateToTimespanValueConverter() });
            TextBox.TextChanged += TextBox_TextChanged;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TimeSpan time;
            TimeSpan.TryParse(TextBox.Text, out time);
            CurrentValue = ((DateTime)CurrentValue).Date.Add(time);
            IsChanged = true;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentValue = DatePicker.SelectedDate.Value.Add(((DateTime)CurrentValue).TimeOfDay);
            IsChanged = true;
        }
    }
}
