using CompleetKassa.Definitions;
using CompleetKassa.Events;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace CompleetKassa.ViewModels
{
	public class MainWindowViewModel : BindableBase
	{
		IEventAggregator _eventAggregator;
		IRegionManager _regionManager;
		private string _title = "Put title here...";
		public string Title
		{
			get { return _title; }
			set { SetProperty(ref _title, value); }
		}

		public MainWindowViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
		{
			_regionManager = regionManager;
			_eventAggregator = eventAggregator;
			_eventAggregator.GetEvent<CloseEvent>().Subscribe(CloseViewEventReceived);
		}

		private void CloseViewEventReceived(string viewName)
		{
			// TODO: Define default view
			_regionManager.RequestNavigate(RegionNames.ContentRegion, "Sales");
		}
	}
}
