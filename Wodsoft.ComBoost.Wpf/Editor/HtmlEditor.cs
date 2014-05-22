using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Wodsoft.ComBoost.Wpf.Editor
{
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    public class HtmlEditor : DefaultEditor
    {
        static HtmlEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HtmlEditor), new FrameworkPropertyMetadata(typeof(HtmlEditor)));
        }
    }
}
