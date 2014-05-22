using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Wodsoft.ComBoost.Wpf.Editor
{
    public class EmailAddressEditor : DefaultEditor
    {
        public EmailAddressEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EmailAddressEditor), new FrameworkPropertyMetadata(typeof(EmailAddressEditor)));
        }

        protected override bool ValidateCore()
        {
            if (!base.ValidateCore())
                return false;
            if (CurrentValue == null)
                return true;
            return System.Text.RegularExpressions.Regex.IsMatch((string)CurrentValue, @"^[a-zA-Z0-9_+.-]+\@([a-zA-Z0-9-]+\.)+[a-zA-Z0-9]{2,4}$");
        }
    }
}
