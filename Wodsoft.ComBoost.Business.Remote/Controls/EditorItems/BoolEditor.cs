using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Wodsoft.ComBoost.Business.Controls.EditorItems
{
    public class BoolEditor : EditorItem
    {
        public BoolEditor(WorkFrame frame)
            : base(frame)
        {
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;

            RadioButton yes = new RadioButton();
            yes.DataContext = this;
            yes.GroupName = "Bool";
            yes.Content = "是";
            panel.Children.Add(yes);

            RadioButton no = new RadioButton();
            no.GroupName = "Bool";
            no.Content = "否";
            panel.Children.Add(no);

            var binding = new Binding("Value");
            binding.Mode = BindingMode.TwoWay;
            yes.SetBinding(RadioButton.IsCheckedProperty, binding);

            Content = panel;
        }
    }
}
