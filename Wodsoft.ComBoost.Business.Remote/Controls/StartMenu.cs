using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using MahApps.Metro.Controls;

namespace Wodsoft.ComBoost.Business.Controls
{
    [TemplatePart(Name = "Scroll", Type = typeof(ScrollViewer))]
    public class StartMenu : ItemsControl
    {
        static StartMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StartMenu), new FrameworkPropertyMetadata(typeof(StartMenu)));
        }

        private ScrollViewer Scroll;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Scroll = (ScrollViewer)GetTemplateChild("Scroll");
            
            PreviewMouseWheel += StartMenu_MouseWheel;
        }

        private void StartMenu_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Scroll.ScrollToHorizontalOffset(Scroll.HorizontalOffset - e.Delta);
        }
    }

    public class StartMenuGroup : HeaderedItemsControl
    {
        static StartMenuGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StartMenuGroup), new FrameworkPropertyMetadata(typeof(StartMenuGroup)));
        }
    }

    public class StartMenuGroupPanel : Panel
    {
        public static DependencyProperty BoxWidthProperty = DependencyProperty.Register("BoxWidth", typeof(double), typeof(StartMenuGroupPanel), new PropertyMetadata(256.0));
        public double BoxWidth { get { return (double)GetValue(BoxWidthProperty); } set { SetValue(BoxWidthProperty, value); } }

        public static DependencyProperty BoxHeightProperty = DependencyProperty.Register("BoxHeight", typeof(double), typeof(StartMenuGroupPanel), new PropertyMetadata(128.0));
        public double BoxHeight { get { return (double)GetValue(BoxHeightProperty); } set { SetValue(BoxHeightProperty, value); } }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double column = 0;
            double maxRow = Math.Floor(finalSize.Height / BoxHeight);
            double row = 1;
            double width = 0;
            foreach (UIElement tile in Children)
            {
                Rect rect = new Rect();
                rect.Width = tile.DesiredSize.Width;
                rect.Height = tile.DesiredSize.Height;

                if (width + tile.DesiredSize.Width <= BoxWidth)
                {
                    rect.X = column * BoxWidth + width;
                    rect.Y = (row - 1) * BoxHeight;
                    width += tile.DesiredSize.Width;
                }
                else
                {
                    width = tile.DesiredSize.Width;
                    if (row == maxRow)
                    {
                        column++;
                        row = 1;
                    }
                    else
                        row++;
                    rect.X = column * BoxWidth;
                    rect.Y = (row - 1) * BoxHeight;
                }

                tile.Arrange(rect);
            }
            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size tileSize = new Size(BoxWidth, BoxHeight);
            double maxRow = Math.Floor(availableSize.Height / BoxHeight);
            double row = maxRow;
            double width = BoxWidth;
            bool added = true;
            availableSize.Height = maxRow * BoxHeight;
            availableSize.Width = 0;
            foreach (UIElement tile in Children)
            {
                if (width == BoxWidth)
                {
                    added = true;
                    if (row == maxRow)
                    {
                        row = 1;
                        added = false;
                    }
                    else
                        row++;
                    width = 0;
                }
                tile.Measure(tileSize);
                if (!added)
                    availableSize.Width += tile.DesiredSize.Width;
                if (width + tile.DesiredSize.Width <= BoxWidth)
                {
                    width += tile.DesiredSize.Width;
                }
                else
                {
                    width = tile.DesiredSize.Width;
                }
                if (width == BoxWidth)
                    added = true;
            }
            return availableSize;
            //return base.MeasureOverride(availableSize);
        }
    }
}
