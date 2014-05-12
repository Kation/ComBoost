using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Windows.Media;
using Wodsoft.ComBoost.Business.Controls;
using System.Data.Entity;
using System.Security;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows;

namespace Wodsoft.ComBoost.Business
{
    public class BussinessApplication
    {
        public static BussinessApplication Current { get; private set; }

        private MainWindow window;

        public BussinessApplication()
        {
            Current = this;
            Background = Brushes.White;
            Buttons = new ObservableCollection<Button>();
            if (Application.Current.Resources.MergedDictionaries.Count(t => t.Source.OriginalString == "pack://application:,,,/MahApps.Metro;component/Styles/Colours.xaml") == 0)
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Colours.xaml") });
            if (Application.Current.Resources.MergedDictionaries.Count(t => t.Source.OriginalString == "pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml") == 0)
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml") });
            if (Application.Current.Resources.MergedDictionaries.Count(t => t.Source.OriginalString == "pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml") == 0)
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml") });
            if (Application.Current.Resources.MergedDictionaries.Count(t => t.Source.OriginalString == "pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml") == 0)
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml") });
            if (Application.Current.Resources.MergedDictionaries.Count(t => t.Source.OriginalString == "pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml") == 0)
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml") });
            if (Application.Current.Resources.MergedDictionaries.Count(t => t.Source.OriginalString == "pack://application:,,,/MahApps.Metro;component/Styles/FlatButton.xaml") == 0)
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/FlatButton.xaml") });
        }

        public string Title { get; set; }

        public IEntityContextBuilder ContextBuilder { get; set; }

        public ISecurity Security { get; set; }

        public ImageSource Icon { get; set; }

        public Brush Background { get; set; }

        public WorkFlow[] WorkFlows { get; set; }

        public void Load()
        {
            if (window != null)
                return;
            window = new MainWindow();
            window.Title = Title;
            window.Icon = Icon;
            window.Flows = WorkFlows;
            window.Background = Background;
            window.Show();
        }

        public void UnLoad()
        {
            if (window == null)
                return;
            window.Close();
            window = null;
        }

        public void SwitchFrame(WorkFrame frame)
        {
            if (window == null)
                throw new InvalidOperationException("未载入窗体。");
            if (frame == null)
                throw new ArgumentNullException("frame");
            window.SwitchFrame(frame);
        }

        public ObservableCollection<Button> Buttons { get; private set; }
    }
}
