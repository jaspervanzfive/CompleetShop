using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using CompleetKassa.DataTypes.Enumerations;
using CompleetKassa.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace CompleetKassa.Modules.Sales.ViewModels
{
	public class SalesViewModel : BindableBase
	{
		private IList<Product> _dbProductList;

		private ObservableCollection<ProductCategory> _categories;
		private ObservableCollection<ProductSubCategory> _subCategories;

		public string DiscountValue { get; set; }

		// Multi receipt
		private ObservableCollection<PurchasedProductModel> _receiptList;
		public ObservableCollection<PurchasedProductModel> ReceiptList
		{
			get
			{
				return _receiptList;
			}
			set { SetProperty(ref _receiptList, value); }
		}

		private string _categoryFilter;
		public string CategoryFilter
		{
			get { return _categoryFilter; }
			set
			{
				SetProperty(ref _categoryFilter, value);
				ProductList.Refresh();
			}
		}

		private string _subCategoryFilter;
		public string SubCategoryFilter
		{
			get { return _subCategoryFilter; }
			set
			{
				SetProperty(ref _subCategoryFilter, value);
				ProductList.Refresh();
			}
		}

		private ProductSubCategory _selectedSubCategory;
		public ProductSubCategory SelectedSubCategory
		{
			get { return _selectedSubCategory; }
			set
			{
				if (value != null)
				{
					_selectedSubCategory = value;
					SubCategoryFilter = value.Name;
				}
			}
		}

		private ProductCategory _selectedCategory;
		public ProductCategory SelectedCategory
		{
			get { return _selectedCategory; }
			set
			{
				_selectedCategory = value;
				CategoryFilter = value.Name;
				SetSubCategories(value.Name);
			}
		}

		private ICollectionView _productList;
		public ICollectionView ProductList
		{
			get { return _productList; }
			set { SetProperty(ref _productList, value); }
		}

		private ObservableCollection<SelectedProductModel> _purchasedProducts;
		public ObservableCollection<SelectedProductModel> PurchasedProducts
		{
			get
			{
				return _purchasedProducts;
			}
			set { SetProperty(ref _purchasedProducts, value); }
		}

		public ObservableCollection<ProductCategory> Categories
		{
			get { return _categories; }
			set
			{
				SetProperty(ref _categories, value);
			}
		}

		public ObservableCollection<ProductSubCategory> SubCategories
		{
			get { return _subCategories; }
			set
			{
				SetProperty(ref _subCategories, value);
			}
		}

		private PurchasedProductModel _currentPurchase;
		public PurchasedProductModel CurrentPurchase
		{
			get { return _currentPurchase; }
			set
			{
				SetProperty(ref _currentPurchase, value);
				PurchasedProducts = value.Products;
			}
		}

		private int _receiptIndex;
		public int ReceiptIndex
		{
			get { return _receiptIndex; }
			set
			{
				if (value < 0) return;

				SetProperty(ref _receiptIndex, value);
				CurrentPurchase = _receiptList[value];
			}
		}

		public SelectedProductModel SelectedPurchasedProduct
		{
			get; set;
		}

		#region Commands
		public DelegateCommand<SelectedProductModel> OnPurchased { get; private set; }
		public DelegateCommand OnIncrementPurchased { get; private set; }
		public DelegateCommand OnDecrementPurchased { get; private set; }
		public DelegateCommand OnSelectAllPurchased { get; private set; }
		public DelegateCommand OnNewReceipt { get; private set; }
		public DelegateCommand OnPreviousReceipt { get; private set; }
		public DelegateCommand OnNextReceipt { get; private set; }
		public DelegateCommand OnPay { get; private set; }
		public DelegateCommand OnDiscountDollar { get; private set; }
		public DelegateCommand OnDiscountPercent { get; private set; }
		public DelegateCommand OnDeleteProducts { get; private set; }

		#endregion

		public SalesViewModel()
		{
			//PurchasedItems = new ObservableCollection<PurchasedProductViewModel>();
			_categories = new ObservableCollection<ProductCategory>();
			_purchasedProducts = new ObservableCollection<SelectedProductModel>();
			_receiptList = new ObservableCollection<PurchasedProductModel>();

			_categoryFilter = string.Empty;
			_subCategoryFilter = string.Empty;

			// TODO: This is where to get data from DB
			GetProducts();
			ProductList = CollectionViewSource.GetDefaultView(_dbProductList);
			ProductList.Filter += ProductCategoryFilter;
			ProductList.Filter += ProductSubCategoryFilter;

			// Set the first product as active category
			_categoryFilter = _categories.FirstOrDefault() == null ? string.Empty : _categories.FirstOrDefault().Name;
			SetSubCategories(_categoryFilter);
			SelectFirstCategory();

			CreateNewReceipt();

			// Commands
			OnPurchased = new DelegateCommand<SelectedProductModel>(Puchase);

			//TODO: Bind  Commands
			OnIncrementPurchased = new DelegateCommand(IncrementPurchase);
			OnDecrementPurchased = new DelegateCommand(DecrementPurchase);
			OnSelectAllPurchased = new DelegateCommand(SelectAllPurchased);
			OnNewReceipt = new DelegateCommand(CreateNewReceipt);
			OnPreviousReceipt = new DelegateCommand(SelectPreviousReceipt);
			OnNextReceipt = new DelegateCommand(SelectNextReceipt);
			OnPay = new DelegateCommand(Pay);
			OnDiscountDollar = new DelegateCommand(DiscountPurchaseByDollar);
			OnDiscountPercent = new DelegateCommand(DiscountPurchaseByPercent);
			OnDeleteProducts = new DelegateCommand(DeleteProducts);
		}

		private bool ProductCategoryFilter(Object item)
		{
			var product = item as Product;
			return product == null ? true : product.Category.Contains(_categoryFilter);
		}

		private bool ProductSubCategoryFilter(Object item)
		{
			var product = item as Product;
			return (product.Category.Contains(_categoryFilter) &&
				product.SubCategory.Contains(_subCategoryFilter));
		}

		private void SetSubCategories(string category)
		{
			SubCategories = new ObservableCollection<ProductSubCategory>(_categories.Where(x => x.Name == category).First().SubCategories);
			SubCategoryFilter = string.Empty;
		}

		private void GetCategories(IList<Product> products)
		{
			// TODO: Categories can be obtained from DB especially the color
			var categories = products.Select(x => x.Category).Distinct();

			foreach (var category in categories)
			{
				var subCategories = products.Where(x => x.Category == category)
									.Select(x => x.SubCategory).Distinct();

				var productSubCategories = new List<ProductSubCategory>();
				foreach (var subCategory in subCategories)
				{
					productSubCategories.Add(new ProductSubCategory
					{
						Name = subCategory,
						Color = "Red"
					});
				}

				_categories.Add(new ProductCategory
				{
					Name = category,
					Color = "#B422B9",
					SubCategories = productSubCategories
				});
			}
		}

		private void GetProducts()
		{
			_dbProductList = new List<Product> {
				 new Product
				{
					ID = 1,
					Label = "Cheyene Hawk pen Purle with 25mm grip including spacersd dasdas das das",
					ImagePath ="/CompleetKassa.Modules.Sales;component/Images/SampleSaleProduct.jpg",
					Price = 100.0m,
					Description = "This is sample 1",
					Category = "Shoes",
					SubCategory = "Running"
				},
				new Product
				{
					ID = 2,
					Label = "Shoes 2",
					ImagePath ="/CompleetKassa.Modules.Sales;component/Images/SampleSaleProduct.jpg",
					Price = 20.0m,
					Description = "This is sample 2",
					Category = "Shoes",
					SubCategory = "Walking"
				},
				new Product
				{
					ID = 3,
					Label = "Bag 1",
					ImagePath ="/CompleetKassa.Modules.Sales;component/Images/SampleSaleProduct.jpg",
					Price = 20.0m,
					Description = "This is sample 2",
					Category = "Bag",
					SubCategory = "Shoulder Bag"
				},
				new Product
				{
					ID = 4,
					Label = "Bag 2",
					ImagePath ="/CompleetKassa.Modules.Sales;component/Images/SampleSaleProduct.jpg",
					Price = 20.0m,
					Description = "This is sample 2",
					Category = "Bag",
					SubCategory = "Shoulder Bag"
				},
				new Product
				{
					ID = 5,
					Label = "Belt 1",
					ImagePath ="/CompleetKassa.Modules.Sales;component/Images/SampleSaleProduct.jpg",
					Price = 10.0m,
					Description = "This is Belt 1",
					Category = "Belt",
					SubCategory = "Men's Belt"
				}
			};

			GetCategories(_dbProductList);
		}

		private void SelectFirstCategory()
		{
			if (_categories != null && 0 < _categories.Count)
			{
				SelectedCategory = _categories[0];
			}
		}

		private void DiscountPurchaseByPercent()
		{
			var selectedItems = _purchasedProducts.Where(x => x.IsSelected).ToList(); ;
			foreach (var item in selectedItems)
			{
				DiscountedProduct(item, ProductDiscountOptions.Percent);
			}
		}

		private void DiscountPurchaseByDollar()
		{
			var selectedItems = _purchasedProducts.Where(x => x.IsSelected).ToList(); ;
			foreach (var item in selectedItems)
			{
				DiscountedProduct(item, ProductDiscountOptions.Dollar);
			}
		}

		private void IncrementPurchase()
		{
			var selectedItems = _purchasedProducts.Where(x => x.IsSelected).ToList(); ;
			foreach (var item in selectedItems)
			{
				IncrementPurchasedProduct(item);
			}
		}

		private void DeleteProducts()
		{
			var selectedItems = _purchasedProducts.Where(x => x.IsSelected).ToList(); ;
			foreach (var item in selectedItems)
			{
				PurchasedProducts.Remove(item);
			}

			CurrentPurchase.ComputeTotal();
		}

		private void DecrementPurchase()
		{
			var selectedItems = _purchasedProducts.Where(x => x.IsSelected).ToList();
			foreach (var item in selectedItems)
			{
				DecrementPurchasedProduct(item);
			}
		}

		public void SelectAllPurchased()
		{
			foreach (var item in _purchasedProducts)
			{
				item.IsSelected = true;
			}
		}

		private void Puchase(SelectedProductModel item)
		{
			var existItem = _purchasedProducts.FirstOrDefault(x => x.ID == item.ID);
			if (existItem == null)
			{
				AddPurchasedProduct(item);
			}
			else
			{
				IncrementPurchasedProduct(existItem);
			}
		}

		// TODO: Temporary Receipt counter
		private int _receiptCounter;

		private void CreateNewReceipt()
		{
			CurrentPurchase = new PurchasedProductModel();
			CurrentPurchase.Label = $"{++_receiptCounter}";
			ReceiptList.Add(CurrentPurchase);
			ReceiptIndex = ReceiptList.Count() - 1;
		}

		private void SelectPreviousReceipt()
		{
			if (ReceiptIndex == 0) return;
			ReceiptIndex--;
		}

		private void SelectNextReceipt()
		{
			if (ReceiptIndex == _receiptList.Count() - 1) return;
			ReceiptIndex++;
		}

		private void Pay()
		{
			ReceiptList.Remove(CurrentPurchase);

			if (ReceiptList.Count == 0)
			{
				CreateNewReceipt();
			}
			else
			{
				SelectPreviousReceipt();
			}
		}

		private void DiscountedProduct(SelectedProductModel product, ProductDiscountOptions option)
		{
			decimal discount = 0.0m;
			if (Decimal.TryParse(DiscountValue, out discount) == false)
			{
				discount = 0.0m;
			}

			if (option == ProductDiscountOptions.Dollar)
			{
				product.Discount = discount;
			}
			else if (option == ProductDiscountOptions.Percent)
			{
				product.Discount = product.Price * (discount / 100);
			}

			CurrentPurchase.ComputeTotal();
		}

		private void DiscountedProduct(SelectedProductModel product)
		{
			product.Discount = 5.50m;
			CurrentPurchase.ComputeTotal();
		}

		private void AddPurchasedProduct(SelectedProductModel product)
		{
			var item = new SelectedProductModel
			{
				Quantity = 1,
				ID = product.ID,
				Label = product.Label,
				Price = product.Price,
				Discount = product.Discount
			};

			item.ComputeSubTotal();

			PurchasedProducts.Add(item);

			CurrentPurchase.ComputeTotal();
		}

		private void IncrementPurchasedProduct(SelectedProductModel product)
		{
			product.Quantity++;
			CurrentPurchase.ComputeTotal();
		}

		private void DecrementPurchasedProduct(SelectedProductModel product)
		{
			product.Quantity--;
			if (product.Quantity == 0)
			{
				PurchasedProducts.Remove(product);
			}

			CurrentPurchase.ComputeTotal();
		}
	}
}
