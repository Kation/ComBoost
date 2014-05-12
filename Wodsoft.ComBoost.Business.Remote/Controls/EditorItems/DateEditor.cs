using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Wodsoft.ComBoost.Business.Controls.EditorItems
{
    public class DateEditor : EditorItem
    {
        public DateEditor(WorkFrame frame)
            : base(frame)
        {
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;

            DatePicker date = new DatePicker();
            date.Width = 120;
            date.DataContext = this;
            var binding = new Binding("Value");
            binding.Mode = BindingMode.TwoWay;
            date.SetBinding(DatePicker.SelectedDateProperty, binding);

            panel.Children.Add(date);

            Content = panel;
        }
    }
}
