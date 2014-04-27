using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Wodsoft.ComBoost.Business.Input;

namespace Wodsoft.ComBoost.Business.Controls.WorkPages
{
    public class MultipleSelector<TEntity> : SelectorPage where TEntity : EntityBase, new()
    {
        private EntityViewerViewModel<TEntity> ViewModel;
        private AppButtonPanel AppButtonPanel;
        private TreeView ParentTree;

        public MultipleSelector()
        {
            EntityViewerViewModel<TEntity> viewModel = new EntityViewerViewModel<TEntity>(BussinessApplication.Current.ContextBuilder.GetContext<TEntity>());
            Initialize(viewModel);
        }

        public MultipleSelector(EntityViewerViewModel<TEntity> viewModel)
        {
            Initialize(viewModel);
        }

        private TreeViewItem[] GetParentItems(EntityParentSelectorViewModel[] tree)
        {
            List<TreeViewItem> list = new List<TreeViewItem>();
            foreach (var node in tree)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = node.Name;
                item.Tag = node.Parents;
                item.IsExpanded = true;
                if (node.SubItems != null)
                    item.ItemsSource = GetParentItems(node.SubItems);
                list.Add(item);
            }
            return list.ToArray();
        }


        private void Initialize(EntityViewerViewModel<TEntity> viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException("viewModel");
            ViewModel = viewModel;
            viewModel.PropertyChanged += viewModel_PropertyChanged;
            viewModel.PageSize = 30;

            if (ViewModel.Items.Count == 0 && ViewModel.ParentModels == null)
                viewModel.ParentModels = EntityParentSelectorViewModel.CreateModel<TEntity>(BussinessApplication.Current.ContextBuilder);

            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(100, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(100, GridUnitType.Pixel) });
            if (viewModel.ParentModels != null && viewModel.ParentModels.Length != 0)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200, GridUnitType.Pixel) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                ParentTree = new TreeView();
                ParentTree.BorderThickness = new Thickness();
                TreeViewItem item = new TreeViewItem();
                item.Header = "全部";
                item.IsExpanded = true;
                item.ItemsSource = GetParentItems(viewModel.ParentModels);
                ParentTree.Items.Add(item);
                ParentTree.SelectedItemChanged += ParentTree_SelectedItemChanged;
                item.Margin = new Thickness(0, 0, 8, 0);
                grid.Children.Add(ParentTree);

                GridSplitter splitter = new GridSplitter();
                splitter.Width = 8;
                splitter.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                grid.Children.Add(splitter);
            }


            #region 列表显示

            ListView dataGrid = new ListView();
            dataGrid.MouseDoubleClick += dataGrid_MouseDoubleClick;
            dataGrid.BorderThickness = new Thickness();
            if (viewModel.ParentModels != null && viewModel.ParentModels.Length != 0)
                Grid.SetColumn(dataGrid, 1);
            dataGrid.SelectionMode = SelectionMode.Extended;
            GridView view = new GridView();
            view.AllowsColumnReorder = false;
            var entityType = typeof(TEntity);
            var viewBuilder = viewModel.ViewBuilder;
            var visableProperties = viewBuilder.VisableProperties;
            var hideProperties = viewBuilder.HideProperties;

            PropertyInfo[] properties;
            if (hideProperties == null)
                properties = visableProperties.Select(v => entityType.GetProperty(v)).Where(t => t != null).ToArray();
            else
                properties = visableProperties.Except(hideProperties).Select(v => entityType.GetProperty(v)).Where(t => t != null).ToArray();

            foreach (var property in properties)
            {
                GridViewColumn column = new GridViewColumn();
                var display = property.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
                if (display == null)
                    column.Header = property.Name;
                else
                    column.Header = new Label { Content = display.Name, ToolTip = display.Description };

                DataTemplate dt = new DataTemplate();
                FrameworkElementFactory fef = new FrameworkElementFactory(typeof(Label));
                Binding binding = new Binding();
                binding.Path = new PropertyPath(property.Name);
                fef.SetBinding(Label.ContentProperty, binding);
                dt.VisualTree = fef;
                column.CellTemplate = dt;
                view.Columns.Add(column);
            }
            dataGrid.View = view;

            viewModel.UpdateSource();
            dataGrid.DataContext = viewModel;
            dataGrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding("ItemsSource"));

            grid.Children.Add(dataGrid);

            #endregion

            #region 按钮显示

            AppButtonPanel = new AppButtonPanel();
            Grid.SetRow(AppButtonPanel, 1);
            if (viewModel.ParentModels != null && viewModel.ParentModels.Length != 0)
                Grid.SetColumnSpan(AppButtonPanel, 2);
            grid.Children.Add(AppButtonPanel);

            AppButton back = new AppButton(new CustomCommand(null, Back));
            back.Text = "返回";
            back.Image = (Canvas)Resources["appbar_arrow_left"];
            AppButtonPanel.Items.Add(back);

            AppButton previousPage = new AppButton(new CustomCommand(CanPreviousPage, PreviousPage));
            previousPage.Text = "上一页";
            previousPage.Image = (Canvas)Resources["appbar_navigate_previous"];
            AppButtonPanel.Items.Add(previousPage);

            AppButton nextPage = new AppButton(new CustomCommand(CanNextPage, NextPage));
            nextPage.Text = "下一页";
            nextPage.Image = (Canvas)Resources["appbar_navigate_next"];
            AppButtonPanel.Items.Add(nextPage);


            AppButton ok = new AppItemButton(dataGrid, OK);
            ok.Text = "选择";
            ok.Image = (Canvas)Resources["appbar_check"];
            AppButtonPanel.Items.Add(ok);

            #endregion

            Content = grid;

            Loaded += Viewer_Loaded;
        }

        private void dataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListView dataGrid = (ListView)sender;
            if (dataGrid.SelectedItem != null)
            {
                DialogResult = true;
                SelectedItem = new object[] { dataGrid.SelectedItem };
                Frame.NavigationService.GoBack();
            }
        }

        private void ParentTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (ParentTree.SelectedItem != null)
            {
                Frame.ShowLoading();
                TreeViewItem item = (TreeViewItem)ParentTree.SelectedItem;
                if (item.Tag == null)
                {
                    Thread thread = new Thread(() =>
                    {
                        ViewModel.Items.Clear();
                        foreach (var key in ViewModel.EntityContext.GetKeys())
                            ViewModel.Items.Add(key);
                        ViewModel.UpdateSource();
                        Dispatcher.Invoke(new Action(() => Frame.HideLoading()));
                    });
                    thread.Start();
                }
                else
                {
                    Guid[] parents = (Guid[])item.Tag;
                    Thread thread = new Thread(() =>
                    {
                        ViewModel.Items.Clear();
                        foreach (var key in ViewModel.EntityContext.InParent(parents))
                            ViewModel.Items.Add(key);
                        ViewModel.UpdateSource();
                        Dispatcher.Invoke(new Action(() => Frame.HideLoading()));
                    });
                    thread.Start();
                }
            }
        }


        private void Viewer_Loaded(object sender, RoutedEventArgs e)
        {
            if (Frame == null)
                return;
            Frame.ShowLoading();
            Thread thread = new Thread(() =>
            {
                if (ViewModel.Items.Count == 0)
                {
                    foreach (var key in ViewModel.EntityContext.GetKeys())
                        ViewModel.Items.Add(key);
                }
                ViewModel.UpdateSource();
                Dispatcher.Invoke(new Action(() => Frame.HideLoading()));
            });
            thread.Start();
        }

        private void viewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (AppButtonPanel != null)
                Dispatcher.Invoke(new Action(() => AppButtonPanel.UpdateCommand()));
        }

        #region 按钮事件

        private void Back(object sender, ExecutedEventArgs e)
        {
            DialogResult = false;
            Frame.NavigationService.GoBack();
        }

        private void CanFirstPage(object sender, CanExecuteEventArgs e)
        {
            e.Cancel = ViewModel.CurrentPage == 1;
        }

        private void CanPreviousPage(object sender, CanExecuteEventArgs e)
        {
            e.Cancel = ViewModel.CurrentPage <= 1;
        }

        private void CanNextPage(object sender, CanExecuteEventArgs e)
        {
            e.Cancel = ViewModel.CurrentPage >= ViewModel.MaxPage;
        }

        private void CanLastPage(object sender, CanExecuteEventArgs e)
        {
            e.Cancel = ViewModel.CurrentPage == ViewModel.MaxPage;
        }

        private void FirstPage(object sender, ExecutedEventArgs e)
        {
            Frame.ShowLoading();
            ViewModel.CurrentPage = 1;
            ViewModel.UpdateSource();
            Frame.HideLoading();
        }

        private void PreviousPage(object sender, ExecutedEventArgs e)
        {
            Frame.ShowLoading();
            ViewModel.CurrentPage--;
            ViewModel.UpdateSource();
            Frame.HideLoading();
        }

        private void NextPage(object sender, ExecutedEventArgs e)
        {
            Frame.ShowLoading();
            ViewModel.CurrentPage++;
            ViewModel.UpdateSource();
            Frame.HideLoading();
        }

        private void LastPage(object sender, ExecutedEventArgs e)
        {
            Frame.ShowLoading();
            ViewModel.CurrentPage = ViewModel.MaxPage;
            ViewModel.UpdateSource();
            Frame.HideLoading();
        }

        private void OK(object sender, ItemExecutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            DialogResult = true;
            SelectedItem = listView.SelectedItems.Cast<TEntity>().ToArray();
            Frame.NavigationService.GoBack();
        }

        #endregion

        protected override void OnNavigatedTo(WorkPage page)
        {
            Title = Frame.MainTitle + " 选择";

            if (page is Editor<TEntity>)
            {
                Editor<TEntity> editor = (Editor<TEntity>)page;
                if (editor.DialogResult == false)
                    return;
                if (editor.ViewModel.Item.BaseIndex == default(Guid))
                {
                    editor.ViewModel.Item.BaseIndex = Guid.NewGuid();
                    BussinessApplication.Current.ContextBuilder.GetContext<TEntity>().Add(editor.ViewModel.Item);
                    ViewModel.Items.Insert(0, editor.ViewModel.Item.BaseIndex);
                    ViewModel.CurrentPage = 1;
                    ViewModel.UpdateSource();
                }
                else
                {
                    BussinessApplication.Current.ContextBuilder.GetContext<TEntity>().Edit(editor.ViewModel.Item);
                }
            }

            base.OnNavigatedTo(page);
        }

        public EditorItemFactory EditorItemFactory { get; set; }
    }
}
