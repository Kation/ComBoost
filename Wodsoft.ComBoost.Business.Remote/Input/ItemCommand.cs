using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Wodsoft.ComBoost.Business.Input
{
    public class ItemCommand : ICommand
    {
        private Selector Selector;
        private EventHandler<ItemExecutedEventArgs> Executed;

        public ItemCommand(Selector selector, EventHandler<ItemCanExecuteEventArgs> canExecute, EventHandler<ItemExecutedEventArgs> executed)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");
            Selector = selector;
            Selector.SelectionChanged += Selector_SelectionChanged;
            Executed = executed;
            canExecuteHandler = canExecute;
        }

        private void Selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _CanExecute = true;
            if (canExecuteHandler != null)
            {
                var arg = new ItemCanExecuteEventArgs(Selector.SelectedItem);
                arg.Owner = Owner;
                canExecuteHandler(Selector, arg);
                if (arg.Cancel)
                    _CanExecute = false;
            }
            CanExecuteChanged(this, null);
        }

        public object Owner { get; set; }

        private EventHandler<ItemCanExecuteEventArgs> canExecuteHandler;
        private bool _CanExecute;
        public bool CanExecute(object parameter)
        {
            return _CanExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var arg = new ItemExecutedEventArgs(Selector.SelectedItem);
            arg.Owner = Owner;
            Executed(Selector, arg);
            CanExecuteChanged(this, null);
        }
    }
}
