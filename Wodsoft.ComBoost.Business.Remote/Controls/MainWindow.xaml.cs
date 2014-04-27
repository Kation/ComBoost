using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace Wodsoft.ComBoost.Business.Controls
{
    /// <summary>
    /// MetroWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private StartMenu StartMenu;
        private Dictionary<WorkItem, WorkFrame> WindowList;

        public MainWindow()
        {
            InitializeComponent();
            WindowList = new Dictionary<WorkItem, WorkFrame>();
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            for (int i = 2; i < WindowMenu.Items.Count; i++)
            {
                MenuItem item = (MenuItem)WindowMenu.Items[i];
                WorkFrame frame = (WorkFrame)item.Tag;
                frame.NavigationService.NavigateTo(null);
            }
            foreach (var button in BussinessApplication.Current.Buttons)
                TitleButton.Items.Remove(button);
        }

        public WorkFlow[] Flows { get; set; }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var button in BussinessApplication.Current.Buttons)
                TitleButton.Items.Add(button);
            BussinessApplication.Current.Buttons.CollectionChanged += Buttons_CollectionChanged;

            StartMenu = new StartMenu();
            StartMenuItem.Tag = StartMenu;
            foreach (var flow in Flows)
            {
                StartMenuGroup group = new StartMenuGroup();
                Label name = new Label();
                name.FontSize = 48;
                name.Content = flow.Name;
                name.Foreground = Brushes.Black;
                group.Header = name;

                foreach (var item in flow.Items.OrderBy(t => t.Order))
                {
                    Tile tile = new Tile();
                    tile.Title = item.Name;
                    tile.Background = Brushes.Green;
                    if (item.SmallSize)
                        tile.Width = 120;
                    else
                        tile.Width = 248;
                    tile.Height = 120;
                    tile.Click += Tile_Click;
                    tile.Tag = item;
                    if (item.Icon != null)
                        tile.Content = new Rectangle { Fill = new VisualBrush(item.Icon) { Stretch = Stretch.Uniform }, Width = item.IconSize.Width, Height = item.IconSize.Height };
                    tile.Margin = new Thickness(4);

                    group.Items.Add(tile);
                }

                StartMenu.Items.Add(group);
            }
            ContentControl.Content = StartMenu;
        }

        private void Buttons_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (var obj in e.NewItems)
                    TitleButton.Items.Add(obj);
            if (e.OldItems != null)
                foreach (var obj in e.NewItems)
                    TitleButton.Items.Remove(obj);
        }

        private void WindowButton_Click(object sender, RoutedEventArgs e)
        {
            WindowMenu.IsOpen = true;
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            Tile tile = (Tile)sender;
            WorkItem item = (WorkItem)tile.Tag;
            if (WindowList.ContainsKey(item))
            {
                WindowList[item].Close += frame_Close;
                ContentControl.Content = WindowList[item];
                return;
            }
            WorkFrame frame = new WorkFrame();
            frame.MainTitle = item.Name;
            WorkPage content = item.GetWorkPage();
            WindowList.Add(item, frame);
            MenuItem menu = new MenuItem();
            menu.Tag = frame;
            menu.Click += Menu_Click;
            menu.SetBinding(MenuItem.HeaderProperty, new Binding { Source = frame, Mode = BindingMode.OneWay, Path = new PropertyPath("Content.Title") });
            WindowMenu.Items.Add(menu);
            frame.Close += frame_Close;
            frame.Close += (s, ee) =>
                {
                    WindowList.Remove(item);
                    WindowMenu.Items.Remove(menu);
                    ContentControl.Content = StartMenu;
                };
            ContentControl.Content = frame;
            frame.NavigationService.NavigateTo(content);
        }

        public void SwitchFrame(WorkFrame frame)
        {
            frame.Close += frame_Close;
            ContentControl.Content = frame;
        }

        private void frame_Close(object sender, EventArgs e)
        {
            ContentControl.Content = StartMenu;
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            if (ContentControl.Content == ((MenuItem)sender).Tag)
                return;
            if (ContentControl.Content != StartMenu)
                ((WorkFrame)ContentControl.Content).Close -= frame_Close;
            ContentControl.Content = ((MenuItem)sender).Tag;
        }
    }
}
