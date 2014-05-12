using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Wodsoft.ComBoost.Wpf
{
    public class EntityCommand : ICommand
    {
        public EntityCommand(ExecuteDelegate execute) : this(execute, null) { }

        public EntityCommand(ExecuteDelegate execute, CanExecuteDelegate canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _CanExecuteDelegate = canExecute;
            _ExecuteDelegate = execute;
        }

        private CanExecuteDelegate _CanExecuteDelegate;
        private ExecuteDelegate _ExecuteDelegate;

        public bool CanExecute(object parameter)
        {
            if (_CanExecuteDelegate == null)
                return true;
            return _CanExecuteDelegate(parameter as IEntity);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _ExecuteDelegate(parameter as IEntity);
        }

        public delegate bool CanExecuteDelegate(IEntity entity);
        public delegate void ExecuteDelegate(IEntity entity);
    }
}
