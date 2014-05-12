using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Wodsoft.ComBoost.Business.Controls.EditorItems
{
    public class PhoneNumberEditor : EditorItem
    {
        public PhoneNumberEditor(WorkFrame frame)
            : base(frame)
        {
            TextBox textBox = new TextBox();
            textBox.DataContext = this;
            textBox.PreviewKeyDown += textBox_PreviewKeyDown;
            var binding = new Binding("Value");
            binding.Mode = BindingMode.TwoWay;
            textBox.SetBinding(TextBox.TextProperty, binding);

            Content = textBox;
        }

        private void textBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!(e.Key >= System.Windows.Input.Key.D0 && e.Key <= System.Windows.Input.Key.D9 || e.Key >= System.Windows.Input.Key.NumPad0 && e.Key <= System.Windows.Input.Key.NumPad9 || e.Key == System.Windows.Input.Key.Subtract || e.Key == System.Windows.Input.Key.Delete || e.Key == System.Windows.Input.Key.Back))
                e.Handled = true;
        }
    }
}
