using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Wodsoft.ComBoost.Business.Controls.EditorItems
{
    public class ImageUrlEditor : EditorItem
    {
        private Image Image;

        public ImageUrlEditor(WorkFrame frame)
            : base(frame)
        {
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;

            TextBox url = new TextBox();
            url.Width = 320;
            url.DataContext = this;
            var binding = new Binding("Value");
            binding.Mode = BindingMode.TwoWay;
            url.SetBinding(TextBox.TextProperty, binding);

            Image Image = new Image();
            Image.Width = 160;
            Image.Height = 160;
            Image.Stretch = System.Windows.Media.Stretch.UniformToFill;
            panel.Children.Add(Image);

            panel.Children.Add(url);
            panel.Children.Add(Image);
        }

        protected override void OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "Value")
            {
                if (string.IsNullOrEmpty(e.NewValue as string))
                {
                    Image.Source = null;
                }
                else
                {
                    Image.Source = new BitmapImage(new Uri((string)e.NewValue));
                }
            }
            base.OnPropertyChanged(e);
        }
    }
}
