using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wodsoft.ComBoost.Business.Controls
{
    public class SelectorPage : WorkPage
    {
        public object SelectedItem { get; protected set; }

        public bool DialogResult { get; protected set; }
    }
}
