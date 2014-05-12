using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wodsoft.ComBoost.Business.Input;

namespace Wodsoft.ComBoost.Business.Controls
{
    public class AppButtonPanel : ItemsControl
    {
        static AppButtonPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AppButtonPanel), new FrameworkPropertyMetadata(typeof(AppButtonPanel)));
        }

        public static DependencyProperty ButtonsProperty = DependencyProperty.Register("Buttons", typeof(ObservableCollection<AppButton>), typeof(AppButtonPanel), new PropertyMetadata(new ObservableCollection<AppButton>()));
        public ObservableCollection<AppButton> Buttons { get { return GetValue(ButtonsProperty) as ObservableCollection<AppButton>; } set { SetValue(ButtonsProperty, value); } }
        
        public void UpdateCommand()
        {            
            foreach (var item in Items)
            {
                if (item is AppButton)
                {
                    AppButton button = (AppButton)item;
                    if (button.Command is CustomCommand)
                        ((CustomCommand)button.Command).Update();
                }
            }
        }
    }
}
