using System;
using System.Collections.Generic;
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

namespace Wodsoft.ComBoost.Wpf
{
    [TemplatePart(Name = "PART_ItemsView", Type = typeof(ListView))]
    [TemplatePart(Name = "PART_GridView", Type = typeof(GridView))]
    public class EntityViewer : EntityWindow
    {
        static EntityViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EntityViewer), new FrameworkPropertyMetadata(typeof(EntityViewer)));
        }

        public EntityViewer()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Loaded += EntityViewer_Loaded;

            var service = new ExtendServiceProvider(EntityResolver.Current.InnerResolver);
            service.RegisterInstance<EntityViewer>(this);
            ServiceProvider = service;
        }

        private ListView _ItemsView;

        public IServiceProvider ServiceProvider { get; private set; }

        private void EntityViewer_Loaded(object sender, RoutedEventArgs e)
        {
            var model = Model;
            if (model != null)
            {
                IsLoading = true;
                var task = Task.Run(() =>
                {
                    model.UpdateItems();
                    Dispatcher.Invoke(() => IsLoading = false);
                });
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _ItemsView = (ListView)GetTemplateChild("PART_ItemsView");
            GridView gridView = (GridView)GetTemplateChild("PART_GridView");
            foreach (var header in Model.Headers)
            {
                GridViewColumn column = new GridViewColumn();
                column.Header = header.ShortName ?? header.Name;
                column.DisplayMemberBinding = new Binding { Path = new PropertyPath(header.ClrName) };
                gridView.Columns.Add(column);
            }
        }

        public IEntityViewModel Model { get { return (IEntityViewModel)GetValue(ModelProperty); } set { SetValue(ModelProperty, value); } }
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(IEntityViewModel), typeof(EntityViewer), new PropertyMetadata(ModelChanged));
        private static void ModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EntityViewer viewer = (EntityViewer)d;
            if (e.NewValue == null)
            {
                viewer.NavigatePageCommand = null;
                viewer.ChangePageSizeCommand = null;
                viewer.EntityViewCommand = null;
                viewer.EntityItemCommand = null;
            }
            else
            {
                IEntityViewModel model = (IEntityViewModel)e.NewValue;
                viewer.NavigatePageCommand = new ViewerNavigatePageCommand(viewer.ServiceProvider);
                viewer.ChangePageSizeCommand = new ViewerChangePageSizeCommand(viewer.ServiceProvider);
                viewer.EntityViewCommand = new ViewerEntityViewCommand(viewer.ServiceProvider);
                viewer.EntityItemCommand = new ViewerEntityItemCommand(viewer.ServiceProvider);
                viewer.Title = "列表 " + model.Metadata.Name;
            }
        }

        public ICommand NavigatePageCommand { get; protected set; }

        public ICommand ChangePageSizeCommand { get; protected set; }

        public ICommand EntityViewCommand { get; protected set; }

        public ICommand EntityItemCommand { get; protected set; }

        public abstract class ViewCommand : ICommand
        {
            public ViewCommand(IServiceProvider serviceProvider)
            {
                ServiceProvider = serviceProvider;
            }

            public IServiceProvider ServiceProvider { get; private set; }

            public virtual bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public abstract void Execute(object parameter);
        }

        public class ViewerEntityViewCommand : ViewCommand
        {
            public ViewerEntityViewCommand(IServiceProvider serviceProvider)
                : base(serviceProvider)
            { }

            public override void Execute(object parameter)
            {
                EntityViewButton button = (EntityViewButton)parameter;
                button.SetTarget(ServiceProvider);
                button.InvokeDelegate.Start();
            }
        }

        public class ViewerEntityItemCommand : ViewCommand
        {
            public ViewerEntityItemCommand(IServiceProvider serviceProvider)
                : base(serviceProvider)
            { }

            public override void Execute(object parameter)
            {
                var viewer = (EntityViewer)ServiceProvider.GetService(typeof(EntityViewer));
                var entity = (IEntity)viewer._ItemsView.SelectedItem;
                EntityItemButton button = (EntityItemButton)parameter;
                button.SetTarget(ServiceProvider, entity);
                button.InvokeDelegate.Start();
            }
        }

        public class ViewerNavigatePageCommand : ViewCommand
        {
            public ViewerNavigatePageCommand(IServiceProvider serviceProvider)
                : base(serviceProvider)
            {
                _Viewer = (EntityViewer)serviceProvider.GetService(typeof(EntityViewer));
            }

            private EntityViewer _Viewer;
            
            public override bool CanExecute(object parameter)
            {
                if (!(parameter is int))
                    return false;
                int page = (int)parameter;
                if (page < 1)
                    return false;
                return page <= _Viewer.Model.TotalPage;
            }

            public override void Execute(object parameter)
            {
                int page = (int)parameter;
                _Viewer.IsLoading = true;
                Task.Run(() =>
                {
                    _Viewer.Model.SetPage(page);
                    _Viewer.Model.UpdateItems();
                    _Viewer.Dispatcher.Invoke(() => _Viewer.IsLoading = false);
                });
            }
        }

        public class ViewerChangePageSizeCommand : ViewCommand
        {
            public ViewerChangePageSizeCommand(IServiceProvider serviceProvider)
                : base(serviceProvider)
            {
                _Viewer = (EntityViewer)serviceProvider.GetService(typeof(EntityViewer));
            }

            private EntityViewer _Viewer;

            public override bool CanExecute(object parameter)
            {
                if (!(parameter is int))
                    return false;
                if ((int)parameter < 1)
                    return false;
                return true;
            }

            public override void Execute(object parameter)
            {
                int page = (int)parameter;
                _Viewer.IsLoading = true;
                Task.Run(() =>
                {
                    _Viewer.Model.SetPage(page);
                    _Viewer.Model.UpdateItems();
                    _Viewer.Dispatcher.Invoke(() => _Viewer.IsLoading = false);
                });
            }
        }
    }

}
