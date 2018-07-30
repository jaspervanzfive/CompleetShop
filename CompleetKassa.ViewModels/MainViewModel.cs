using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CompleetKassa.ViewModels.Commands;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;

namespace CompleetKassa.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		public ObservableCollection<BaseViewModel> PageViewModels
		{
			get;
			private set;
		}

		BaseViewModel _currentPageViewModel;
		public BaseViewModel CurrentPageViewModel
		{
			get { return _currentPageViewModel; }
			private set
			{
				if (Equals (value, _currentPageViewModel)) return;
				_currentPageViewModel = value;
				OnPropertyChanged ();
			}
		}

		public ICommand OnChangePageCommand { get; private set; }

		public MainViewModel () : base ("Main","#fff", "Icons/product.png")
		{
			this.CreateContentViewModels ();

            _currentPageViewModel = PageViewModels[0];

            OnChangePageCommand = new BaseCommand (ChangePageCommand);

		}

		void ChangePageCommand (object obj)
		{
			var page = (BaseViewModel)obj;

			if(page != CurrentPageViewModel) {
				CurrentPageViewModel = page;
			}
		}

		public void CreateContentViewModels ()
		{
			var product = new ProductsViewModel ();
			var accessories = new AccessoriesViewModel ();
			accessories.OnClosePageCommand = new BaseCommand (ClosePage);

			PageViewModels = new ObservableCollection<BaseViewModel> ();
			PageViewModels.Add (product);
			PageViewModels.Add (accessories);


			//PageViewModels = new ObservableCollection<BaseViewModel>
			//{

			//	new CustomersViewModel(),
			//	new AccessoriesViewModel(),
			//             new TotalsViewModel(),
			//             new UsersViewModel(),
			//             new SettingsViewModel(),
			//             new SupportViewModel(),
			//             new LockViewModel(),

			//         };
		}

		private void ClosePage (object obj)
		{
			CurrentPageViewModel = PageViewModels[0];
		}
	}
}
