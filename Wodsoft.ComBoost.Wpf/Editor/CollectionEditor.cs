using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wodsoft.ComBoost.Wpf.Editor
{
    public class CollectionEditor : EditorBase
    {
        static CollectionEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CollectionEditor), new FrameworkPropertyMetadata(typeof(CollectionEditor)));
        }
    }
}
