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
    [TemplatePart(Name = "PART_PasswordBox", Type = typeof(TextBox))]
    public class PasswordEditor : EditorBase
    {
        static PasswordEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PasswordEditor), new FrameworkPropertyMetadata(typeof(PasswordEditor)));
        }

        protected PasswordBox PasswordBox { get; private set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PasswordBox = (PasswordBox)GetTemplateChild("PART_PasswordBox");
            PasswordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CurrentValue = PasswordBox.Password;
            IsChanged = true;
        }
    }
}
