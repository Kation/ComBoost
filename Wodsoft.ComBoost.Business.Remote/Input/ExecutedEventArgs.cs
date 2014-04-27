using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wodsoft.ComBoost.Business.Input
{
    public class ExecutedEventArgs : EventArgs
    {
        public ExecutedEventArgs()
        {
        }

        public ExecutedEventArgs(object parameter)
        {
            Parameter = parameter;
        }

        public object Parameter { get; private set; }

        public object Owner { get; set; }
    }
}
