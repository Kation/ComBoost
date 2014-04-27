using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using Wodsoft.ComBoost.Business.Input;

namespace Wodsoft.ComBoost.Business.Controls
{
    public class AppItemButton : AppButton
    {
        public AppItemButton(Selector selector, EventHandler<ItemExecutedEventArgs> execute)
            : base(new ItemCommand(selector, null, execute))
        {

        }

        public AppItemButton(Selector selector, EventHandler<ItemCanExecuteEventArgs> canExcute, EventHandler<ItemExecutedEventArgs> execute)
            : base(new ItemCommand(selector, canExcute, execute))
        {

        }

        public object Owner
        {
            get
            {
                return ((ItemCommand)Command).Owner;
            }
            set
            {
                ((ItemCommand)Command).Owner = value;
            }
        }
    }
}
