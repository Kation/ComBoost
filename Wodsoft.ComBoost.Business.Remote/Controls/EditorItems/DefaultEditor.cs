using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Wodsoft.ComBoost.Business.Controls.EditorItems
{
    public class DefaultEditor : EditorItem
    {
        public DefaultEditor(WorkFrame frame)
            : base(frame)
        {
            IsAllowdEmpty = false;
            TextBox textBox = new TextBox();
            textBox.DataContext = this;
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
