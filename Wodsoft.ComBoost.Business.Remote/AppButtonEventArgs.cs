using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wodsoft.ComBoost.Business.Controls;

namespace Wodsoft.ComBoost.Business
{
    public class AppButtonEventArgs : EventArgs
    {
        public AppButton Button { get; internal set; }

        public object SelectedItem { get; internal set; }

        public WorkPage Page { get; internal set; }
    }
}
