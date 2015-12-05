using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Wodsoft.ComBoost.Wpf
{
    [TemplatePart(Name = "PART_Loading", Type = typeof(UIElement))]
    public class EntityWindow : Window
    {
        private UIElement _Loading;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _Loading = GetTemplateChild("PART_Loading") as UIElement;
        }

        public static DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(EntityWindow), new PropertyMetadata(false));
        public bool IsLoading { get { return (bool)GetValue(IsLoadingProperty); } set { SetValue(IsLoadingProperty, value); } }
    }
}
