using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using System.Windows.Markup;

namespace Wodsoft.ComBoost.Business.Controls
{
    [ContentProperty("Content")]
    public class WorkFrame : Control
    {
        public WorkFrame()
        {
            NavigationService = new NavigationService(this);
        }

        static WorkFrame()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WorkFrame), new FrameworkPropertyMetadata(typeof(WorkFrame)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Loading = (Grid)GetTemplateChild("Loading");
            ContentControl = (TransitioningContentControl)GetTemplateChild("Content");
            ContentControl.Content = Content;
        }

        private Grid Loading;
        private TransitioningContentControl ContentControl;

        public void ShowLoading()
        {
            Loading.Visibility = System.Windows.Visibility.Visible;
        }

        public void HideLoading()
        {
            Loading.Visibility = System.Windows.Visibility.Collapsed;
        }

        public event EventHandler Close;

        public static DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(WorkPage), typeof(WorkFrame), new PropertyMetadata(OnContentChange));
        public WorkPage Content { get { return (WorkPage)GetValue(ContentProperty); } set { SetValue(ContentProperty, value); } }
        private static void OnContentChange(object sender, DependencyPropertyChangedEventArgs e)
        {
            WorkFrame frame =(WorkFrame)sender;
            if (e.OldValue != null)
                ((WorkPage)e.OldValue).Frame = null;
            if (e.NewValue != null)
            {
                ((WorkPage)e.NewValue).Frame = (WorkFrame)sender;
                if (frame.ContentControl != null)
                    frame.ContentControl.Content = e.NewValue;
            }
            else
            {
                if (e.OldValue != null && frame.Close != null)
                    frame.Close(frame, null);
            }
        }

        public NavigationService NavigationService { get; private set; }

        public static DependencyProperty MainTitleProperty = DependencyProperty.Register("MainTitle", typeof(string), typeof(WorkFrame), new PropertyMetadata(string.Empty));
        public string MainTitle { get { return (string)GetValue(MainTitleProperty); } set { SetValue(MainTitleProperty, value); } }
    }
}
