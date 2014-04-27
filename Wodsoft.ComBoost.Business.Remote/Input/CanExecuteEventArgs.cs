using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wodsoft.ComBoost.Business.Input
{
    public class CanExecuteEventArgs : ExecutedEventArgs
    {
        public CanExecuteEventArgs()
        {
            Cancel = true;
        }

        public CanExecuteEventArgs(object parameter)
            : base(parameter)
        {
            Cancel = true;
        }

        public bool Cancel { get; set; }
    }
}
