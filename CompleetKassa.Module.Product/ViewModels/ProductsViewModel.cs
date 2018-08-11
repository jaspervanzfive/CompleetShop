using System.Collections.ObjectModel;
using System.Windows.Input;
using CompleetKassa.Events;
using CompleetKassa.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace CompleetKassa.Modules.Products.ViewModels
{
	public class ProductsViewModel : BindableBase
	{

		IEventAggregator _eventAggregator;

		public DelegateCommand OnCloseCommand { get; private set; }

		private void Close()
		{
			//TODO: How to make it dynamically get the view name?
			_eventAggregator.GetEvent<CloseEvent>().Publish("Products");
		}

		public ProductsViewModel(IEventAggregator eventAggregator)
		{
			_eventAggregator = eventAggregator;

			OnCloseCommand = new DelegateCommand(Close);

			_productList = new ObservableCollection<Product> {
				 new Product
				{
					ID = 1,
					Label = "Cheyene Hawk",
					ImagePath ="/CompleetKassa.Modules.Products;component/Images/SampleProduct.png",
					Price = 100.0m,
					Description = "This is sample 1",
					Category = "Shoes",
					SubCategory = "Running"
				},
				new Product
				{
					ID = 2,
					Label = "Shoes 2",
					ImagePath ="/CompleetKassa.Modules.Products;component/Images/SampleProduct.png",
					Price = 20.0m,
					Description = "This is sample 2",
					Category = "Shoes",
					SubCategory = "Walking"
				},
			};
		}

		private ObservableCollection<Product> _productList;
		public ObservableCollection<Product> ProductList
		{
			get { return _productList; }
			set { SetProperty(ref _productList, value); }
		}


		private Product m_selectedroduct;
		public Product SelectedProduct
		{
			get { return m_selectedroduct; }
			set { SetProperty(ref m_selectedroduct, value); }
		}

		private bool CanDelete
		{
			get { return SelectedProduct != null; }
		}

		private ICommand m_deleteCommand;
		public ICommand DeleteCommand
		{
			get
			{
				if (m_deleteCommand == null)
				{
					m_deleteCommand = new DelegateCommand(Delete).ObservesCanExecute(() => CanDelete);
				}
				return m_deleteCommand;
			}
		}

		private void Delete()
		{
			ProductList.Remove(SelectedProduct);
		}
	}
}
