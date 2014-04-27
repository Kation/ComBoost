using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Wodsoft.ComBoost.Business.Controls.EditorItems
{
    public class UrlEditor : EditorItem
    {
        public UrlEditor(WorkFrame frame)
            : base(frame)
        {
            TextBox textBox = new TextBox();
            textBox.DataContext = this;
            var binding = new Binding("Value");
            binding.Mode = BindingMode.TwoWay;
            textBox.SetBinding(TextBox.TextProperty, binding);
            Content = textBox;
        }
    }
}
