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
    [TemplatePart(Name = "PART_Male", Type = typeof(RadioButton))]
    public class SexEditor : EditorBase
    {
        static SexEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SexEditor), new FrameworkPropertyMetadata(typeof(SexEditor)));
        }

        protected RadioButton Male { get; private set; }
        protected RadioButton Female { get; private set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Male = (RadioButton)GetTemplateChild("PART_Male");
            Male.SetBinding(RadioButton.IsCheckedProperty, new Binding { Source = this, Path = new PropertyPath(CurrentValueProperty), Mode = BindingMode.TwoWay });
            Male.Checked += Checked;
            Female = (RadioButton)GetTemplateChild("PART_Female");
            Female.SetBinding(RadioButton.IsCheckedProperty, new Binding { Source = this, Path = new PropertyPath(CurrentValueProperty), Mode = BindingMode.TwoWay, Converter = new BooleanReverseValueConverter() });
            Female.Checked += Checked;
        }

        private void Checked(object sender, RoutedEventArgs e)
        {
            IsChanged = true;
        }
    }
}
