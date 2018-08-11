using CompleetKassa.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace CompleetKassa.Module.Customer.ViewModels
{
	public class CustomersViewModel : BindableBase
	{

		IEventAggregator _eventAggregator;

		public DelegateCommand OnCloseCommand { get; private set; }

		private void Close()
		{
			//TODO: How to make it dynamically get the view name?
			_eventAggregator.GetEvent<CloseEvent>().Publish("Customers");
		}

		public CustomersViewModel(IEventAggregator eventAggregator)
		{
			_eventAggregator = eventAggregator;

			OnCloseCommand = new DelegateCommand(Close);
		}
	}
}
