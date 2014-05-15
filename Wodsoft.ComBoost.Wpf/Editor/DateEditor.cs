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
    [TemplatePart(Name = "PART_DatePicker", Type = typeof(DatePicker))]
    public class DateEditor : EditorBase
    {
        static DateEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateEditor), new FrameworkPropertyMetadata(typeof(DateEditor)));
        }

        protected DatePicker DatePicker { get; private set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            DatePicker = (DatePicker)GetTemplateChild("PART_DatePicker");
            DatePicker.SetBinding(DatePicker.SelectedDateProperty, new Binding { Source = this, Path = new PropertyPath(CurrentValueProperty), Mode = BindingMode.TwoWay });
            DatePicker.SelectedDateChanged += DatePicker_SelectedDateChanged;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            IsChanged = true;
        }
    }
}
