using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CompleetKassa.ViewModels.Commands
{
    public class BaseCommand : ICommand
    {
        private Action<object> _method;
        public event EventHandler CanExecuteChanged;

        public BaseCommand(Action<object> method)
        {
            _method = method;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _method.Invoke(parameter);
        }
    }
}
