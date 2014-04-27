using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wodsoft.ComBoost.Business.Input
{
    public class ItemExecutedEventArgs : ExecutedEventArgs
    {
        public ItemExecutedEventArgs(object item)
            : base()
        {
            Item = item;
        }

        public ItemExecutedEventArgs(object item, object parameter)
            : base(parameter)
        {
            Item = item;
        }

        public object Item { get; private set; }
    }

    public class ItemCanExecuteEventArgs : ItemExecutedEventArgs
    {
        public ItemCanExecuteEventArgs(object item)
            : base(item )
        {

        }

        public ItemCanExecuteEventArgs(object item, object parameter)
            : base(item , parameter)
        {

        }

        public bool Cancel { get; set; }
    }
}
