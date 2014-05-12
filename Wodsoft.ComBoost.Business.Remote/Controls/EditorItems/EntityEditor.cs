using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Wodsoft.ComBoost.Business.Controls.WorkPages;

namespace Wodsoft.ComBoost.Business.Controls.EditorItems
{
    public class EntityEditor : EditorItem
    {
        TextBox text;
        Type Type;
        SelectorPage selector;

        public EntityEditor(WorkFrame frame, Type type)
            : base(frame)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            Type = type;

            

            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;

            text = new TextBox();
            text.DataContext = this;
            text.MinWidth = 240;
            text.IsReadOnly = true;
            var timeBinding = new Binding("Value");
            timeBinding.Mode = BindingMode.TwoWay;
            text.SetBinding(TextBox.TextProperty, timeBinding);

            Button select = new Button();
            select.Content = "选择";
            select.Click += select_Click;
            select.FontSize = 12;

            Button remove = new Button();
            remove.Content = "清除";
            remove.Click += remove_Click;
            remove.FontSize = 12;

            panel.Children.Add(text);
            panel.Children.Add(select);
            panel.Children.Add(remove);

            Content = panel;

            frame.NavigationService.Navigated += NavigationService_Navigated;
        }

        private void NavigationService_Navigated(WorkPage oldPage, WorkPage newPage)
        {
            if (oldPage == selector)
            {
                if (selector.DialogResult)
                    Value = selector.SelectedItem;
            }
        }

        private void remove_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Value = null;
        }

        private void select_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Type type = typeof(Selector<>).MakeGenericType(Type);
            selector = (SelectorPage)Activator.CreateInstance(type);
            Frame.NavigationService.NavigateTo(selector);
        }
    }
}
