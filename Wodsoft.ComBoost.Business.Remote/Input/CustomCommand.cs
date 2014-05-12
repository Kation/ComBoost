using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Wodsoft.ComBoost.Business.Input
{
    public class CustomCommand : ICommand
    {
        public CustomCommand(EventHandler<CanExecuteEventArgs> canExecute, EventHandler<ExecutedEventArgs> execute)
        {
            CanExecuteHandler = canExecute;
            ExecuteHandler = execute;
        }

        private EventHandler<CanExecuteEventArgs> CanExecuteHandler;
        private EventHandler<ExecutedEventArgs> ExecuteHandler;

        public object Owner { get; set; }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteHandler == null)
                return true;
            CanExecuteEventArgs e = new CanExecuteEventArgs(parameter);
            e.Owner = Owner;
            CanExecuteHandler(this, e);
            return !e.Cancel;
        }

        public event EventHandler CanExecuteChanged;

        public void Update()
        {
            CanExecuteChanged(this, null);
        }

        public void Execute(object parameter)
        {
            if (ExecuteHandler != null)
            {
                ExecutedEventArgs e = new ExecutedEventArgs(parameter);
                e.Owner = Owner;
                ExecuteHandler(this, e);
                Update();
            }
        }
    }
}
