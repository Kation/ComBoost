using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Wodsoft.ComBoost.Business.Controls.EditorItems
{
    public class ImageEditor : EditorItem
    {
        private Image Image;

        public ImageEditor(WorkFrame frame)
            : base(frame)
        {
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;

            Image = new Image();
            Image.Width = 160;
            Image.Height = 160;
            Image.Stretch = System.Windows.Media.Stretch.Uniform;
            panel.Children.Add(Image);

            Button select = new Button();
            select.Content = "选择";
            select.Click += select_Click;
            select.FontSize = 12;
            select.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            Button remove = new Button();
            remove.Content = "清除";
            remove.Click += remove_Click;
            remove.FontSize = 12;
            remove.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            panel.Children.Add(select);
            panel.Children.Add(remove);

            Content = panel;
        }

        private void remove_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Value = null;
        }

        private void select_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Title = "选择图片";
            dialog.Filter = "图片文件（*.jpg;*.png）|*.jpg;*.png";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = System.IO.File.OpenRead(dialog.FileName);
                    bitmap.EndInit();
                    byte[] data = new byte[bitmap.StreamSource.Length];
                    bitmap.StreamSource.Position = 0;
                    bitmap.StreamSource.Read(data, 0, (int)bitmap.StreamSource.Length);
                    Value = data;
                }
                catch
                {
                    MessageBox.Show("选择的图片格式错误。", "打开失败", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        protected override void OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "Value")
            {
                if (e.NewValue == null)
                {
                    Image.Source = null;
                }
                else
                {
                    try
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = new System.IO.MemoryStream((byte[])e.NewValue);
                        bitmap.EndInit();
                        Image.Source = bitmap;
                    }
                    catch
                    {
                        Image.Source = null;
                        Value = null;
                    }
                }
            }
            base.OnPropertyChanged(e);
        }
    }
}
