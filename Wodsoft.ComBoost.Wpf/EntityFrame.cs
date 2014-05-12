using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Wodsoft.ComBoost.Wpf
{
    [TemplatePart(Name = "PART_LoadingPanel", Type = typeof(Panel))]
    public class EntityFrame : Frame
    {
        static EntityFrame()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EntityFrame), new FrameworkPropertyMetadata(typeof(EntityFrame)));
        }

        private Panel _LoadingPanel;

        public EntityFrame()
        {
            Navigating += EntityNavigationWindow_Navigating;
            LoadCompleted += EntityNavigationWindow_LoadCompleted;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _LoadingPanel = GetTemplateChild("LoadingPanel") as Panel;
        }

        private void EntityNavigationWindow_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (_LoadingPanel != null)
                _LoadingPanel.Visibility = System.Windows.Visibility.Hidden;
        }

        private void EntityNavigationWindow_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (_LoadingPanel != null)
                _LoadingPanel.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
