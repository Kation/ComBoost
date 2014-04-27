using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Wodsoft.ComBoost.Business.Input;

namespace Wodsoft.ComBoost.Business.Controls
{
    public class AppButton : DependencyObject
    {
        public AppButton(ICommand command)
        {
            Command = command;
        }

        public static DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(Visual), typeof(AppButton));
        public Visual Image { get { return GetValue(ImageProperty) as Visual; } set { SetValue(ImageProperty, value); } }

        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(AppButton));
        public string Text { get { return GetValue(TextProperty) as string; } set { SetValue(TextProperty, value); } }

        public static DependencyProperty ToolTipProperty = DependencyProperty.Register("ToolTip", typeof(string), typeof(AppButton));
        public string ToolTip { get { return GetValue(ToolTipProperty) as string; } set { SetValue(ToolTipProperty, value); } }

        public object Owner
        {
            get
            {
                if (Command is CustomCommand)
                    return ((CustomCommand)Command).Owner;
                else
                    throw new NotSupportedException();
            }
            set
            {
                if (Command is CustomCommand)
                    ((CustomCommand)Command).Owner = value;
                else
                    throw new NotSupportedException();
            }
        }

        public ICommand Command { get; private set; }
    }
}
