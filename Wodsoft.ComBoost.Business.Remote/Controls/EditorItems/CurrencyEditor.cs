using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Wodsoft.ComBoost.Business.Controls.EditorItems
{
    public class CurrencyEditor : EditorItem, IValueConverter
    {
        public CurrencyEditor(WorkFrame frame)
            : base(frame)
        {
            TextBox textBox = new TextBox();
            textBox.DataContext = this;
            var binding = new Binding("Value");
            binding.Mode = BindingMode.TwoWay;
            binding.Converter = this;
            textBox.SetBinding(TextBox.TextProperty, binding);
            Content = textBox;
        }
        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                value = default(decimal);
            System.Globalization.NumberFormatInfo info = (System.Globalization.NumberFormatInfo)System.Globalization.CultureInfo.CurrentCulture.NumberFormat.Clone();
            info.CurrencyDecimalDigits = 2;
            return ((decimal)value).ToString("c", info);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal result;
            string val = value.ToString();
            if (!decimal.TryParse(value.ToString(), System.Globalization.NumberStyles.Currency, System.Globalization.CultureInfo.CurrentCulture, out result))
                return default(decimal);
            return result;
        }
    }
}
