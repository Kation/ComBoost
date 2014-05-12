using Company.Entity;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
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
using Wodsoft.ComBoost.Wpf;

namespace Company.Wpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private EntityPageController _Controller;

        public MainWindow()
        {
            InitializeComponent();

            EntityContextBuilder builder = new EntityContextBuilder(new DataContext());
            _Controller = new EntityPageController(builder);
        }
        
        private void EmployeeManager_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(_Controller.GetViewer<Employee>());
        }

        private void EmployeeGroup_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(_Controller.GetViewer<EmployeeGroup>());
        }
    }
}
