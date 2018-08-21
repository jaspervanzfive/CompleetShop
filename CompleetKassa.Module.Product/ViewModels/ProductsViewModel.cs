using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using CompleetKassa.Database.Services;
using CompleetKassa.DataValidation;
using CompleetKassa.Definitions;
using CompleetKassa.Events;
using CompleetKassa.Models;
using CompleetKassa.Module.Product.Definitions;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using static CompleetKassa.Module.Product.Definitions.Enumeration;

namespace CompleetKassa.Modules.Products.ViewModels
{
	public class ProductsViewModel : ModelValidationBase
	{
		#region Fields
		private IProductService _productService;
		private ICategoryService _categoryService;
		private IEventAggregator _eventAggregator;
		private IRegionManager _regionManager;
		#endregion Fields

		#region Property
		public Task Initialization { get; private set; }
		#endregion Property

		#region Command Property
		public DelegateCommand OnFirstCommand { get; private set; }
		public DelegateCommand OnPreviousCommand { get; private set; }
		public DelegateCommand OnNextCommand { get; private set; }
		public DelegateCommand OnLastCommand { get; private set; }
		public DelegateCommand OnAddCommand { get; private set; }
		public DelegateCommand<CategoryModel> OnEditCategoryCommand { get; private set; }
		public DelegateCommand<ProductModel> OnEditProductCommand { get; private set; }
		public DelegateCommand OnSaveCommand { get; private set; }
		public DelegateCommand<ProductModel> OnDeleteProductCommand { get; private set; }
		public DelegateCommand<CategoryModel> OnDeleteCategoryCommand { get; private set; }
		public DelegateCommand OnCancelCommand { get; private set; }
		public DelegateCommand OnCloseCommand { get; private set; }
		public DelegateCommand OnSelectImageCommand { get; private set; }
		#endregion Command Property

		#region "Bindable Property"
		private string _title;
		public string Title
		{
			get { return _title; }
			set { SetProperty(ref _title, value); }
		}

		private int _selectedTabIndex;
		public int SelectedTabIndex
		{
			get { return _selectedTabIndex; }
			set { SetProperty(ref _selectedTabIndex, value); }
		}

		private Commands _currentCommand;

		#region Product Bindable Property
		private bool _enableProductList;
		public bool EnableProductList
		{
			get { return _enableProductList; }
			set { SetProperty(ref _enableProductList, value); }
		}

		private bool _newProductFormVisibility;
		public bool NewProductFormVisibility
		{
			get { return _newProductFormVisibility; }
			set { SetProperty(ref _newProductFormVisibility, value); }
		}

		public ICollectionView _productListView;
		public ICollectionView ProductListView
		{
			get { return _productListView; }
			private set { SetProperty(ref _productListView, value); }
		}

		private string _newProductCode;
		public string NewProductCode
		{
			get { return _newProductCode; }
			set { SetProperty(ref _newProductCode, value); }
		}

		private string _newProductName;
		public string NewProductName
		{
			get { return _newProductName; }
			set { SetProperty(ref _newProductName, value); }
		}

		private string _newProductDetail;
		public string NewProductDetail
		{
			get { return _newProductDetail; }
			set { SetProperty(ref _newProductDetail, value); }
		}

		private int _newProductQuantity;
		public int NewProductQuantity
		{
			get { return _newProductQuantity; }
			set { SetProperty(ref _newProductQuantity, value); }
		}

		private string _newProductModel;
		public string NewProductModel
		{
			get { return _newProductModel; }
			set { SetProperty(ref _newProductModel, value); }
		}

		private decimal _newProductPrice;
		public decimal NewProductPrice
		{
			get { return _newProductPrice; }
			set { SetProperty(ref _newProductPrice, value); }
		}

		private string _newProductImage;
		public string NewProductImage
		{
			get { return _newProductImage; }
			set { SetProperty(ref _newProductImage, value); }
		}

		private int _newProductStatus;
		public int NewProductStatus
		{
			get { return _newProductStatus; }
			set { SetProperty(ref _newProductStatus, value); }
		}

		private int _newProductCategory;
		public int NewProductCategory
		{
			get { return _newProductCategory; }
			set { SetProperty(ref _newProductCategory, value); }
		}

		private Uri _selectedImageUri;
		public Uri SelectedImageUri
		{
			get { return _selectedImageUri; }
			set { SetProperty(ref _selectedImageUri, value); }
		}

		#endregion Product Bindable Property

		#region Category Bindable Property
		private bool _readOnlyNewCategoryForm;
		public bool ReadOnlyNewCategoryForm
		{
			get { return _readOnlyNewCategoryForm; }
			set { SetProperty(ref _readOnlyNewCategoryForm, value); }
		}

		private bool _enableCategoryList;
		public bool EnableCategoryList
		{
			get { return _enableCategoryList; }
			set { SetProperty(ref _enableCategoryList, value); }
		}

		private bool _newCategoryFormVisibility;
		public bool NewCategoryFormVisibility
		{
			get { return _newCategoryFormVisibility; }
			set { SetProperty(ref _newCategoryFormVisibility, value); }
		}

		// All categories
		public ICollectionView _categoryListView;
		public ICollectionView CategoryListView
		{
			get { return _categoryListView; }
			private set { SetProperty(ref _categoryListView, value); }
		}

		// Categories without sub categories
		public ICollectionView _mainCategoryListView;
		public ICollectionView MainCategoryListView
		{
			get { return _mainCategoryListView; }
			private set { SetProperty(ref _mainCategoryListView, value); }
		}

		// Active Categories
		public ICollectionView _activeCategoryListView;
		public ICollectionView ActiveCategoryListView
		{
			get { return _activeCategoryListView; }
			private set { SetProperty(ref _activeCategoryListView, value); }
		}

		private CategoryModel _selectedCategory;
		public CategoryModel SelectedCategory
		{
			get { return _selectedCategory; }
			set {
				SetProperty(ref _selectedCategory, value);
				HoldSelectedCategory(_selectedCategory);
			}
		}

		private int _holdCategoryID;
		public int HoldCategoryID
		{
			get { return _holdCategoryID; }
			set { SetProperty(ref _holdCategoryID, value); }
		}

		private string _holdCategoryName;
		public string HoldCategoryName
		{
			get { return _holdCategoryName; }
			set { SetProperty(ref _holdCategoryName, value); }
		}

		private string _holdCategoryDetail;
		public string HoldCategoryDetail
		{
			get { return _holdCategoryDetail; }
			set { SetProperty(ref _holdCategoryDetail, value); }
		}

		private int _holdCategoryStatus;
		public int HoldCategoryStatus
		{
			get { return _holdCategoryStatus; }
			set { SetProperty(ref _holdCategoryStatus, value); }
		}

		private int _holdCategoryParent;
		public int HoldCategoryParent
		{
			get { return _holdCategoryParent; }
			set { SetProperty(ref _holdCategoryParent, value); }
		}

		#endregion Category Bindable Property


		#endregion "Bindable Property"

		public ProductsViewModel(IUnityContainer container)
		{
			// Resolve services
			_regionManager = container.Resolve<IRegionManager>();
			_productService = container.Resolve<IProductService>();
			_categoryService = container.Resolve<ICategoryService>();
			_eventAggregator = container.Resolve<IEventAggregator>();

			// Get data from Database
			Initialization = InitializeCategoryListAsync();
			Initialization = InitializeProductListAsync();
			
			ProductListView.MoveCurrentToFirst();
			CategoryListView.MoveCurrentToFirst();

			_title = "-Product Management Title-";
			_currentCommand = Commands.None;
			NewProductFormVisibility = false;
			NewCategoryFormVisibility = false;
			EnableProductList = true;
			EnableCategoryList = true;
			ReadOnlyNewCategoryForm = true;

			SetCategoryValidationRules();
			SetProductValidationRules();

			// Event Handler
			OnFirstCommand = new DelegateCommand(FirstCommandHandler);
			OnPreviousCommand = new DelegateCommand(PreviousCommandHandler);
			OnNextCommand = new DelegateCommand(NextCommandHandler);
			OnLastCommand = new DelegateCommand(LastCommandHandler);
			OnAddCommand = new DelegateCommand(AddCommandHandler);
			OnEditCategoryCommand = new DelegateCommand<CategoryModel>(EditCategoryCommandHandler);
			OnEditProductCommand = new DelegateCommand<ProductModel>(EditProductCommandHandler);
			OnSaveCommand = new DelegateCommand(SaveCommandHandler);
			OnDeleteProductCommand = new DelegateCommand<ProductModel>(DeleteProductCommandHandler);
			OnDeleteCategoryCommand = new DelegateCommand<CategoryModel>(DeleteCategoryCommandHandler);
			OnCancelCommand = new DelegateCommand(CancelCommandHandler);
			OnCloseCommand = new DelegateCommand(CloseCommandHandler);
			OnSelectImageCommand = new DelegateCommand(SelectImageHandler);
		}

		private void SetCategoryValidationRules()
		{
			AddRule(() => HoldCategoryName, () => string.IsNullOrEmpty(HoldCategoryName) == true || HoldCategoryName.Length == 5, "Must be 5 characters.");
		}

		private void SetProductValidationRules()
		{
			AddRule(() => NewProductName, () => string.IsNullOrEmpty(NewProductName) == true || NewProductName.Length == 5, "Must be 5 characters.");
		}

		private void InitializeNewProduct()
		{
			NewProductCode = string.Empty;
			NewProductName = string.Empty;
			NewProductDetail = string.Empty;
			NewProductQuantity = 0;
			NewProductModel = string.Empty;
			NewProductPrice = 0.0m;
			NewProductImage = string.Empty;
			NewProductStatus = 1;
			NewProductCategory = 0;
		}

		private void HoldSelectedCategory (CategoryModel category)
		{
			if (category != null)
			{
				HoldCategoryID = category.ID;
				HoldCategoryName = category.Name;
				HoldCategoryDetail = category.Detail;
				HoldCategoryStatus = category.Status;
				HoldCategoryParent = category.Parent;
			}
		}

		private void InitializeNewCategory()
		{
			HoldCategoryName = string.Empty;
			HoldCategoryDetail = string.Empty;
			HoldCategoryStatus = 1;
			HoldCategoryParent = 0;
		}

		private CategoryModel CreateNewCategory()
		{
			return new CategoryModel
			{
				Name = HoldCategoryName,
				Detail = HoldCategoryDetail,
				Status = HoldCategoryStatus,
				Parent = HoldCategoryParent
			};
		}

		private ProductModel CreateNewProduct()
		{
			return new ProductModel
			{
				Code = NewProductCode,
				Name = NewProductName,
				Detail = NewProductDetail,
				Quantity = NewProductQuantity,
				Model = NewProductModel,
				Price = NewProductPrice,
				Image = NewProductImage,
				Status = NewProductStatus,
				CategoryID = NewProductCategory
			};
		}

		private void SelectImageHandler()
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				// Search from Desktop by default
				openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
				openFileDialog.Filter = "Image Files(*.PNG;*.JPG)|*.PNG;*.JPG|All files (*.*)|*.*";
				openFileDialog.FilterIndex = 1;
				openFileDialog.RestoreDirectory = true;

				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					var targetPath = Path.Combine(Path.GetTempPath(), ApplicationFolders.ProductsTmpFolder);

					// Create a new target folder, if necessary.
					if (!Directory.Exists(targetPath))
					{
						Directory.CreateDirectory(targetPath);
					}

					NewProductImage = Path.Combine(ApplicationFolders.Products, Path.GetFileName(openFileDialog.FileName));
					var destFile = Path.Combine(targetPath, Path.GetFileName(openFileDialog.FileName));
					// To copy a file to another location and overwrite the destination file if it already exists.
					File.Copy(openFileDialog.FileName, destFile, true);

					SelectedImageUri = new Uri(destFile);
				}
			}
		}

		private async Task InitializeCategoryListAsync()
		{
			var result = await _categoryService.GetCategoriesWithParentCategoriesAsync();
			if (result.DidError == false)
			{
				// All categories
				CategoryListView = CollectionViewSource.GetDefaultView(result.Model.ToList());

				// Categories without sub category
				MainCategoryListView = CollectionViewSource.GetDefaultView(result.Model.ToList());
				MainCategoryListView.Filter = c =>
				{
					CategoryModel category = c as CategoryModel;
					return category != null && category.Parent == 0;
				};

				// Active categories
				ActiveCategoryListView = CollectionViewSource.GetDefaultView(result.Model.ToList());
				MainCategoryListView.Filter = c =>
				{
					CategoryModel category = c as CategoryModel;
					return category != null && category.Status == 1;
				};
			}
		}

		private async Task InitializeProductListAsync()
		{
			var result = await _productService.GetProductsWithCategoryAsync();
			if (result.DidError == false)
			{
				ProductListView = CollectionViewSource.GetDefaultView(result.Model.ToList());
			}
		}

		private void LastCommandHandler()
		{
			// Product
			if (SelectedTabIndex == (int)TabIndexes.Product)
			{
				ProductListView.MoveCurrentToLast();
			}
			// Category
			else if (SelectedTabIndex == (int)TabIndexes.Category)
			{
				CategoryListView.MoveCurrentToLast();
			}
		}

		private void PreviousCommandHandler()
		{
			// Product
			if (SelectedTabIndex == (int)TabIndexes.Product)
			{
				ProductListView.MoveCurrentToPrevious();

				if (ProductListView.IsCurrentBeforeFirst == true)
				{
					ProductListView.MoveCurrentToFirst();
				}
			}
			// Category
			else if (SelectedTabIndex == (int)TabIndexes.Category)
			{
				CategoryListView.MoveCurrentToPrevious();

				if (CategoryListView.IsCurrentBeforeFirst == true)
				{
					CategoryListView.MoveCurrentToFirst();
				}
			}
		}

		private void NextCommandHandler()
		{
			// Product
			if (SelectedTabIndex == (int)TabIndexes.Product)
			{
				ProductListView.MoveCurrentToNext();

				if (ProductListView.IsCurrentAfterLast == true)
				{
					ProductListView.MoveCurrentToLast();
				}
			}
			// Category
			else if (SelectedTabIndex == (int)TabIndexes.Category)
			{
				CategoryListView.MoveCurrentToNext();

				if (CategoryListView.IsCurrentAfterLast == true)
				{
					CategoryListView.MoveCurrentToLast();
				}
			}
		}

		private void FirstCommandHandler()
		{
			// Product
			if (SelectedTabIndex == (int)TabIndexes.Product)
			{
				ProductListView.MoveCurrentToFirst();
			}
			// Category
			else if (SelectedTabIndex == (int)TabIndexes.Category)
			{
				CategoryListView.MoveCurrentToFirst();
			}
		}

		private void AddCommandHandler()
		{
			// Products
			if (SelectedTabIndex == (int)TabIndexes.Product)
			{
				_currentCommand = Commands.AddProduct;
				InitializeNewProduct();
				NewProductFormVisibility = true;
				EnableProductList = false;
			}
			// Category
			else if (SelectedTabIndex == (int)TabIndexes.Category)
			{
				_currentCommand = Commands.AddCategory;
				InitializeNewCategory();
				NewCategoryFormVisibility = true;
				EnableCategoryList = false;
			}
		}

		private void EditCategoryCommandHandler(CategoryModel category)
		{
			_currentCommand = Commands.EditCategory;
			EnableCategoryList = false;
			ReadOnlyNewCategoryForm = false;
		}

		private void EditProductCommandHandler(ProductModel product)
		{
			_currentCommand = Commands.EditProduct;
			EnableProductList = false;
		}

		private async void SaveCommandHandler()
		{
			if (SelectedTabIndex == (int)TabIndexes.Product && NewProductFormVisibility == true)
			{
				NewProductFormVisibility = false;
				EnableProductList = true;

				if(_currentCommand == Commands.AddProduct)
				{
					await SaveNewProduct();
				}
				
			}
			else if (SelectedTabIndex == (int)TabIndexes.Category && NewCategoryFormVisibility == true)
			{
				NewCategoryFormVisibility = false;
				EnableCategoryList = true;
				ReadOnlyNewCategoryForm = true;

				if (_currentCommand == Commands.AddCategory)
				{
					await SaveNewCategory();
				}
			}
		}

		private async Task SaveNewProduct()
		{
			ProductModel newProduct = CreateNewProduct();

			var targetPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ApplicationFolders.Products);
			// Create a new target folder, if necessary.
			if (!Directory.Exists(targetPath))
			{
				Directory.CreateDirectory(targetPath);
			}

			var destFile = Path.Combine(targetPath, Path.GetFileName(SelectedImageUri.ToString()));
			// To copy a file to another location and 
			// overwrite the destination file if it already exists.
			File.Copy(SelectedImageUri.LocalPath, destFile, true);

			var result = await _productService.AddProductAsync(newProduct);
			await InitializeProductListAsync();
		}

		private async Task SaveNewCategory()
		{
			CategoryModel newCategory = CreateNewCategory();
			await _categoryService.AddCategoryAsync(newCategory);

			await InitializeCategoryListAsync();
		}

		private async void DeleteCategoryCommandHandler(CategoryModel category)
		{
			if (category != null)
			{
				await _categoryService.RemoveCategoryAsync(category.ID);
				await InitializeCategoryListAsync();
			}
		}

		private async void DeleteProductCommandHandler(ProductModel product)
		{
			if (product != null)
			{
				await _productService.RemoveProductAsync(product.ID);
				await InitializeProductListAsync();
			}
		}

		private void CancelCommandHandler()
		{
			// Product
			if (SelectedTabIndex == (int)TabIndexes.Product)
			{
				NewProductFormVisibility = false;
				EnableProductList = true;
			}
			// Category
			else if (SelectedTabIndex == (int)TabIndexes.Category)
			{
				NewCategoryFormVisibility = false;
				EnableCategoryList = true;
				ReadOnlyNewCategoryForm = true;

				HoldSelectedCategory(SelectedCategory);
			}
		}

		private void CloseCommandHandler()
		{
			_eventAggregator.GetEvent<CloseEvent>().Publish("Products");
		}
	}
}
