using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Wodsoft.ComBoost.Business.Controls.EditorItems
{
    public class HtmlEditor : EditorItem
    {
        private TabControl Panel;
        private WebBrowser Browser;

        public HtmlEditor(WorkFrame frame)
            : base(frame)
        {
            Panel = new TabControl();
            Panel.Height = 640;

            TabItem editTab = new TabItem();
            editTab.Header = "编辑";
            TextBox textBox = new TextBox();
            textBox.DataContext = this;
            var binding = new Binding("Value");
            binding.Mode = BindingMode.TwoWay;
            textBox.SetBinding(TextBox.TextProperty, binding);
            editTab.Content = textBox;

            TabItem viewTab = new TabItem();
            viewTab.Header = "预览";
            Browser = new WebBrowser();
            viewTab.Content = Browser;

            Panel.Items.Add(editTab);
            Panel.Items.Add(viewTab);

            Panel.SelectionChanged += panel_SelectionChanged;

            Content = Panel;
        }

        private void panel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Panel.SelectedIndex == 1)
                Browser.NavigateToString((string)Value);
        }
    }
}
