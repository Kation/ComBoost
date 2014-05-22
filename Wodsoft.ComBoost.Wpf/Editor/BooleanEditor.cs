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
    [TemplatePart(Name = "PART_True", Type = typeof(RadioButton))]
    [TemplatePart(Name = "PART_False", Type = typeof(RadioButton))]
    public class BooleanEditor : EditorBase
    {
        static BooleanEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BooleanEditor), new FrameworkPropertyMetadata(typeof(BooleanEditor)));
        }

        protected RadioButton True { get; private set; }
        protected RadioButton False { get; private set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            True = (RadioButton)GetTemplateChild("PART_True");
            True.SetBinding(RadioButton.IsCheckedProperty, new Binding { Source = this, Path = new PropertyPath(CurrentValueProperty), Mode = BindingMode.TwoWay });
            True.Checked += Checked;
            False = (RadioButton)GetTemplateChild("PART_False");
            False.SetBinding(RadioButton.IsCheckedProperty, new Binding { Source = this, Path = new PropertyPath(CurrentValueProperty), Mode = BindingMode.TwoWay, Converter = new BooleanReverseValueConverter() });
            False.Checked += Checked;
        }

        private void Checked(object sender, RoutedEventArgs e)
        {
            IsChanged = true;
        }
    }
}
