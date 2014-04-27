using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Wodsoft.ComBoost.Business.Controls.EditorItems
{
    public class ComboBoxEditor : EditorItem
    {
        public ComboBoxEditor(WorkFrame frame, object[] items)
            : base(frame)
        {
            ComboBox comboBox = new ComboBox();
            comboBox.DataContext = this;
            comboBox.ItemsSource = items;
            var binding = new Binding("Value");
            binding.Mode = BindingMode.TwoWay;
            comboBox.SetBinding(ComboBox.SelectedItemProperty, binding);

            Content = comboBox;
        }
    }
}
