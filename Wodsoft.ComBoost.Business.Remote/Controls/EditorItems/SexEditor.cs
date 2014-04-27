using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Wodsoft.ComBoost.Business.Controls.EditorItems
{
    public class SexEditor : EditorItem
    {
        public SexEditor(WorkFrame frame)
            : base(frame)
        {
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;

            RadioButton girl = new RadioButton();
            girl.GroupName = "Sex";
            girl.Content = "女";
            panel.Children.Add(girl);

            RadioButton boy = new RadioButton();
            boy.DataContext = this;
            boy.GroupName = "Sex";
            boy.Content = "男";
            panel.Children.Add(boy);

            var binding = new Binding("Value");
            binding.Mode = BindingMode.TwoWay;
            boy.SetBinding(RadioButton.IsCheckedProperty, binding);

            Content = panel;
        }
    }
}
