using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Wodsoft.ComBoost.Business.Controls
{
    [ContentProperty("Content")]
    [TemplatePart(Name = "Loading", Type = typeof(Grid))]
    public class WorkPage : Control
    {
        static WorkPage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WorkPage), new FrameworkPropertyMetadata(typeof(WorkPage)));
        }

        public WorkPage()
        {
            Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/Wodsoft.ComBoost.Business.Remote;component/Resources/Icons.xaml") });
        }

        public WorkFrame Frame { get; internal set; }

        public static DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(WorkPage));
        public object Content { get { return GetValue(ContentProperty); } set { SetValue(ContentProperty, value); } }

        public static DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(WorkPage), new PropertyMetadata(""));
        public string Title { get { return (string)GetValue(TitleProperty); } set { SetValue(TitleProperty, value); } }

        public event NavigatedEventHandler NavigatedTo;
        protected virtual void OnNavigatedTo(WorkPage page)
        {
            if (NavigatedTo != null)
                NavigatedTo(page, this);
        }
        public event NavigatedEventHandler NavigatedFrom;
        protected virtual void OnNavigatedFrom(WorkPage page)
        {
            if (NavigatedFrom != null)
                NavigatedFrom(this, page);
        }
        internal void BaseNavigateTo(WorkPage page)
        {
            OnNavigatedTo(page);
        }
        internal void BaseNavigateFrom(WorkPage page)
        {
            OnNavigatedFrom(page);
        }
    }
}
