using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Wodsoft.ComBoost.Business.Controls.EditorItems
{
    public class MultilineTextEditor : EditorItem
    {
        public MultilineTextEditor(WorkFrame frame)
            : base(frame)
        {
            TextBox textBox = new TextBox();
            textBox.MaxHeight = 320;
            textBox.DataContext = this;
            textBox.AcceptsReturn = true;
            textBox.AcceptsTab = true;
            textBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            textBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            textBox.TextWrapping = System.Windows.TextWrapping.NoWrap;
            var binding = new Binding("Value");
            binding.Mode = BindingMode.TwoWay;
            textBox.SetBinding(TextBox.TextProperty, binding);

            Content = textBox;
        }

        public bool IsAllowdEmpty { get; set; }

        public override bool ValidateData()
        {
            if (!base.ValidateData())
                return false;
            if (IsRequired && !IsAllowdEmpty && string.IsNullOrEmpty((string)Value))
                return false;
            return true;
        }
    }
}
