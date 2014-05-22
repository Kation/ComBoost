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
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    public class NumberEditor : EditorBase
    {
        static NumberEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberEditor), new FrameworkPropertyMetadata(typeof(NumberEditor)));
        }
        protected TextBox TextBox { get; private set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            TextBox = (TextBox)GetTemplateChild("PART_TextBox");
            TextBox.SetBinding(TextBox.TextProperty, new Binding { Source = this, Path = new PropertyPath(CurrentValueProperty), Mode = BindingMode.OneWay });
            TextBox.TextChanged += TextBox_TextChanged;
        }

        void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            double value;
            double.TryParse(TextBox.Text, out value);
            CurrentValue = value;
            IsChanged = true;
        }
    }
}
