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
    public class DefaultEditor : EditorBase
    {
        static DefaultEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DefaultEditor), new FrameworkPropertyMetadata(typeof(DefaultEditor)));
        }

        protected TextBox TextBox { get; private set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            TextBox = (TextBox)GetTemplateChild("PART_TextBox");
            TextBox.SetBinding(TextBox.TextProperty, new Binding { Source = this, Path = new PropertyPath(CurrentValueProperty), Mode = BindingMode.TwoWay });
            //TextBox.TextChanged += TextBox_TextChanged;
        }
    }
}
