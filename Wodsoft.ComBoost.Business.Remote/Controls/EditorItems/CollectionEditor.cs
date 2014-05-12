using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Wodsoft.ComBoost.Business.Controls.WorkPages;

namespace Wodsoft.ComBoost.Business.Controls.EditorItems
{
    public class CollectionEditor : EditorItem
    {
        private ListBox List;
        private Button Remove, Add;
        private Type Type;
        private SelectorPage Selector;

        public CollectionEditor(WorkFrame frame, Type type)
            : base(frame)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            Type = type;

            CustomSetter = new EditorPropertySetter(Setter);

            Grid panel = new Grid();
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(320, GridUnitType.Pixel) });
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            panel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            panel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            List = new ListBox();
            List.Height = 120;
            List.SelectionChanged += List_SelectionChanged;
            List.BorderThickness = new Thickness(1);
            Grid.SetRowSpan(List, 2);
            panel.Children.Add(List);

            Add = new Button();
            Add.Height = 32;
            Add.Width = 64;
            Add.FontSize = 12;
            Add.Content = "添加";
            Add.Click += Add_Click;
            Add.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Grid.SetColumn(Add, 1);
            panel.Children.Add(Add);

            Remove = new Button();
            Remove.Height = 32;
            Remove.Width = 64;
            Remove.FontSize = 12;
            Remove.Content = "删除";
            Remove.IsEnabled = false;
            Remove.Click += Remove_Click;
            Remove.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Grid.SetColumn(Remove, 1);
            Grid.SetRow(Remove, 1);
            panel.Children.Add(Remove);

            Content = panel;

            Frame.NavigationService.Navigated += NavigationService_Navigated;
        }

        private void NavigationService_Navigated(WorkPage oldPage, WorkPage newPage)
        {
            if (oldPage == Selector)
            {
                if (Selector.DialogResult)
                {
                    object[] result = (object[])Selector.SelectedItem;
                    foreach (var val in result)
                        if (!List.Items.Contains(val))
                        {
                            List.Items.Add(val);
                            IsChanged = true;
                        }
                }
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Type type = typeof(MultipleSelector<>).MakeGenericType(Type);
            SelectorPage page = (SelectorPage)Activator.CreateInstance(type);
            Frame.NavigationService.NavigateTo(page);
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            List.Items.Remove(List.SelectedItem);
            IsChanged = true;
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (List.SelectedItem != null)
                Remove.IsEnabled = true;
            else
                Remove.IsEnabled = false;
        }

        protected override void OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "Value")
            {
                if (!Saving)
                {
                    List.Items.Clear();
                    if (e.NewValue != null)
                        foreach (var item in (IEnumerable)e.NewValue)
                            List.Items.Add(item);
                }
            }
            base.OnPropertyChanged(e);
        }

        bool Saving;
        private void Setter(object entity, EditorItem editorItem, System.Reflection.PropertyInfo property)
        {
            Saving = true;
            if (Value == null)
                Value = Activator.CreateInstance(typeof(List<>).MakeGenericType(Type));
            var collection = typeof(ICollection<>).MakeGenericType(Type);
            var add = collection.GetMethod("Add");
            var clear = collection.GetMethod("Clear");
            clear.Invoke(Value,null);
            foreach (var item in List.Items)
                add.Invoke(Value, new object[] { item });
            Saving = false;
        }
    }
}
