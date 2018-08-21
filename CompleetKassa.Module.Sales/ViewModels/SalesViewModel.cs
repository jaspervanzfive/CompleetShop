using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using CompleetKassa.Database.Services;
using CompleetKassa.DataTypes.Enumerations;
using CompleetKassa.Models;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Mvvm;

namespace CompleetKassa.Modules.Sales.ViewModels
{
	public class SalesViewModel : BindableBase
	{
		#region Fields
		private IProductService _productService;

		#endregion Fields

		#region Property
		public Task Initialization { get; private set; }
		#endregion Property

		private IList<ProductModel> _dbProductList;

		private ObservableCollection<ProductCategoryModel> _categories;
		private ObservableCollection<ProductSubCategoryModel> _subCategories;

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

		private ProductSubCategoryModel _selectedSubCategory;
		public ProductSubCategoryModel SelectedSubCategory
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

		private ProductCategoryModel _selectedCategory;
		public ProductCategoryModel SelectedCategory
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

		public ObservableCollection<ProductCategoryModel> Categories
		{
			get { return _categories; }
			set
			{
				SetProperty(ref _categories, value);
			}
		}

		public ObservableCollection<ProductSubCategoryModel> SubCategories
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

		public SalesViewModel(IUnityContainer container)
		{
			_productService = container.Resolve<IProductService>();

			_categoryFilter = string.Empty;
			_subCategoryFilter = string.Empty;

			_categories = new ObservableCollection<ProductCategoryModel>();
			_purchasedProducts = new ObservableCollection<SelectedProductModel>();
			_receiptList = new ObservableCollection<PurchasedProductModel>();
			SubCategories = new ObservableCollection<ProductSubCategoryModel>();
			Categories = new ObservableCollection<ProductCategoryModel>();

			//Get Products from DB
			Initialization = InitializeProductListAsync();
			if (Initialization.IsCompleted)
			{
				ProductList = CollectionViewSource.GetDefaultView(_dbProductList);
				ProductList.Filter += ProductCategoryFilter;
				ProductList.Filter += ProductSubCategoryFilter;

				// Set the first product as active category
				_categoryFilter = _categories.FirstOrDefault() == null ? string.Empty : _categories.FirstOrDefault().Name;
				SetSubCategories(_categoryFilter);
				SelectFirstCategory();
			}

			CreateNewReceipt();

			// Event Handlers
			OnPurchased = new DelegateCommand<SelectedProductModel>(Puchase);
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
			var product = item as ProductModel;
			return product == null ? true : product.Category.Contains(_categoryFilter);
		}

		private bool ProductSubCategoryFilter(Object item)
		{
			var product = item as ProductModel;

			if(string.IsNullOrEmpty(_subCategoryFilter) == true)
			{
				return true;
			}

			if(product.SubCategory == null)
			{
				return product.Category.Contains(_categoryFilter);
			}
			else
			{
				return (product.Category.Contains(_categoryFilter) &&
				product.SubCategory.Contains(_subCategoryFilter));
			}
		}

		private void SetSubCategories(string category)
		{
			if(0 < _categories.Count)
			{
				var result = _categories.Where(x => x.Name == category).FirstOrDefault();
				if(result != null)
				{
					SubCategories = new ObservableCollection<ProductSubCategoryModel>(result.SubCategories);
				}
			}
			
			SubCategoryFilter = string.Empty;
		}

		private void GetCategories(IList<ProductModel> products)
		{
			// TODO: Categories can be obtained from DB especially the color
			var categories = products.Select(x => x.Category).Distinct();

			foreach (var category in categories)
			{
				var subCategories = products.Where(x => x.Category == category)
									.Select(x => x.SubCategory).Distinct();

				var productSubCategories = new List<ProductSubCategoryModel>();
				foreach (var subCategory in subCategories)
				{
					productSubCategories.Add(new ProductSubCategoryModel
					{
						Name = subCategory,
						Color = "Red"
					});
				}

				_categories.Add(new ProductCategoryModel
				{
					Name = category,
					Color = "#B422B9",
					SubCategories = productSubCategories
				});
			}
		}

		private async Task InitializeProductListAsync()
		{
			var result = await _productService.GetProductsWithCategoryAsync();
			if (result.DidError == false)
			{
				_dbProductList = result.Model.ToList();
				GetCategories(_dbProductList);
			}
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
