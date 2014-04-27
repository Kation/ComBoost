using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Wodsoft.ComBoost.Business.Input;

namespace Wodsoft.ComBoost.Business.Controls.WorkPages
{
    public class Editor<TEntity> : WorkPage where TEntity : EntityBase, new()
    {
        public Editor()
        {
            EntityEditorViewModel<TEntity> viewModel = new EntityEditorViewModel<TEntity>(BussinessApplication.Current.ContextBuilder.GetContext<TEntity>());
            viewModel.Item = viewModel.EntityContext.Create();
            viewModel.Item.OnCreateCompleted();
            Initialize(viewModel, new DefaultEditorItemFactory());
        }

        public Editor(EditorItemFactory factory)
        {
            EntityEditorViewModel<TEntity> viewModel = new EntityEditorViewModel<TEntity>(BussinessApplication.Current.ContextBuilder.GetContext<TEntity>());
            viewModel.Item = Activator.CreateInstance<TEntity>();
            viewModel.Item.OnCreateCompleted();
            Initialize(viewModel, factory);
        }

        public Editor(EntityEditorViewModel<TEntity> viewModel, EditorItemFactory factory)
        {
            Initialize(viewModel, factory);
        }

        public EntityEditorViewModel<TEntity> ViewModel { get; private set; }

        private void Initialize(EntityEditorViewModel<TEntity> viewModel, EditorItemFactory factory)
        {
            if (viewModel == null)
                throw new ArgumentNullException("viewModel");

            ViewModel = viewModel;
            Type type = typeof(TEntity);

            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(100, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(100, GridUnitType.Pixel) });
            ParentAttribute parent = type.GetCustomAttributes(typeof(ParentAttribute), true).FirstOrDefault() as ParentAttribute;

            Grid dataGrid = new Grid();
            dataGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200, GridUnitType.Pixel) });
            dataGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            Items = new List<EditorItem>();
            foreach (var property in viewModel.ViewBuilder.EditableProperties)
            {
                dataGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto), MinHeight = 32 });
                var name = new Label();
                name.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                name.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                name.Margin = new Thickness(4);
                var display = type.GetProperty(property).GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
                if (display == null)
                    name.Content = property;
                else
                {
                    name.Content = display.Name;
                    name.ToolTip = display.Description;
                }
                Grid.SetRow(name, dataGrid.RowDefinitions.Count - 1);
                dataGrid.Children.Add(name);

                EditorItem editor = factory.GetEditorItem(type.GetProperty(property));
                editor.Title = (string)name.Content;
                editor.Tag = type.GetProperty(property);
                editor.Value = type.GetProperty(property).GetValue(viewModel.Item, null);
                Items.Add(editor);
                editor.Margin = new Thickness(4);
                editor.Initialized = true;
                Grid.SetRow(editor, dataGrid.RowDefinitions.Count - 1);
                Grid.SetColumn(editor, 1);
                dataGrid.Children.Add(editor);
            }

            ScrollViewer scroll = new ScrollViewer();
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            scroll.PanningMode = PanningMode.VerticalOnly;
            scroll.Content = dataGrid;
            grid.Children.Add(scroll);

            AppButtonPanel appButtonPanel = new AppButtonPanel();
            Grid.SetRow(appButtonPanel, 1);
            grid.Children.Add(appButtonPanel);

            AppButton ok = new AppButton(new CustomCommand(null, Ok));
            ok.Text = "确定";
            ok.Image = (Canvas)Resources["appbar_check"];
            appButtonPanel.Items.Add(ok);

            AppButton cancel = new AppButton(new CustomCommand(null, Cancel));
            cancel.Text = "取消";
            cancel.Image = (Canvas)Resources["appbar_close"];
            appButtonPanel.Items.Add(cancel);

            Content = grid;
        }

        protected override void OnNavigatedTo(WorkPage page)
        {
            Title = Frame.MainTitle + " 编辑";
            base.OnNavigatedTo(page);
        }

        protected override void OnNavigatedFrom(WorkPage page)
        {
            if (page == null)
                foreach (var item in Items)
                    item.Dispose();

            base.OnNavigatedFrom(page);
        }

        private List<EditorItem> Items;

        private void Ok(object sender, ExecutedEventArgs e)
        {
            foreach (var item in Items)
            {
                if (!item.ValidateData())
                {
                    MessageBox.Show(item.Title + "验证失败。", "验证失败", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            foreach (var item in Items)
            {
                if (!item.IsChanged)
                    continue;
                item.UpdateValue();
                PropertyInfo property = (PropertyInfo)item.Tag;
                if (item.CustomSetter != null)
                    item.CustomSetter(ViewModel.Item, item, property);
                else
                    property.SetValue(ViewModel.Item, item.Value, null);
                item.Dispose();
            }
            string validation = ViewModel.Item.ValidateData();
            if (validation != null)
            {
                MessageBox.Show(validation, "验证失败", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string message = ViewModel.Item.EditMessage();
            if (message != null)
                if (MessageBox.Show(message, "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.Cancel)
                    return;
            ViewModel.Item.OnEditCompleted();
            DialogResult = true;
            Frame.NavigationService.GoBack();
        }

        private void Cancel(object sender, ExecutedEventArgs e)
        {
            foreach (var item in Items)
                item.Dispose();
            DialogResult = false;
            Frame.NavigationService.GoBack();
        }

        public bool DialogResult { get; private set; }

    }
}
