using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Wodsoft.ComBoost.Wpf
{
    [TemplatePart(Name = "PART_GridView", Type = typeof(GridView))]
    public class EntityViewer : EntityPage
    {
        static EntityViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EntityViewer), new FrameworkPropertyMetadata(typeof(EntityViewer)));
        }

        public EntityViewer(EntityController controller)
            : base(controller)
        {
            Loaded += EntityViewer_Loaded;
        }

        private void EntityViewer_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationService == null)
                throw new NotSupportedException("EntityViewer must place in a navigator.");
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            GridView view = (GridView)GetTemplateChild("PART_GridView");
            foreach (var header in Model.Headers)
            {
                GridViewColumn column = new GridViewColumn();
                column.Header = header.ShortName ?? header.Name;
                column.DisplayMemberBinding = new Binding { Path = new PropertyPath(header.Property.Name) };
                view.Columns.Add(column);
            }
        }

        public EntityViewModel Model { get { return (EntityViewModel)GetValue(ModelProperty); } set { SetValue(ModelProperty, value); } }
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(EntityViewModel), typeof(EntityViewer), new PropertyMetadata(ModelChanged));
        private static void ModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EntityViewer viewer = (EntityViewer)d;
            if (e.NewValue == null)
            {
                viewer.NavigatePageCommand = null;
                viewer.ChangePageSizeCommand = null;
            }
            else
            {
                viewer.NavigatePageCommand = new NavigatePageCommand((EntityViewModel)e.NewValue);
                viewer.ChangePageSizeCommand = new ChangePageSizeCommand((EntityViewModel)e.NewValue);
            }
        }

        public NavigatePageCommand NavigatePageCommand { get; private set; }

        public ChangePageSizeCommand ChangePageSizeCommand { get; private set; }
    }

    public class NavigatePageCommand : ICommand
    {
        public NavigatePageCommand(EntityViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            Model = model;
        }

        public EntityViewModel Model { get; private set; }

        public bool CanExecute(object parameter)
        {
            if (!(parameter is int))
                return false;
            int page = (int)parameter;
            if (page < 1)
                return false;
            return page <= Model.TotalPage;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            int page = (int)parameter;
            Model.SetPage(page);
            Model.UpdateItems();
        }
    }

    public class ChangePageSizeCommand : ICommand
    {
        public ChangePageSizeCommand(EntityViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            Model = model;
        }

        public EntityViewModel Model { get; private set; }

        public bool CanExecute(object parameter)
        {
            if (!(parameter is int))
                return false;
            if ((int)parameter < 1)
                return false;
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            Model.SetSize((int)parameter);
            Model.UpdateItems();
        }
    }

}
