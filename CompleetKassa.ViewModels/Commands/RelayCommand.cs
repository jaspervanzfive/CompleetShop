using System;
using System.Windows.Input;

namespace CompleetKassa.ViewModels.Commands
{
	public class RelayCommand<TParameter> : ICommand
	{
		private Predicate<TParameter> canExecute;
		private Action<TParameter> execute;

		public RelayCommand (Predicate<TParameter> canExecute, Action<TParameter> execute)
		{
			this.canExecute = canExecute;
			this.execute = execute;
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute (object parameter)
		{
			if (this.canExecute == null) return true;

			return this.canExecute ((TParameter)parameter);
		}

		public void Execute (object parameter)
		{
			this.execute ((TParameter)parameter);
		}
	}
}
