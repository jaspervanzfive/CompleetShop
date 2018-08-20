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
using CompleetKassa.Models;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace CompleetKassa.Modules.Products.ViewModels
{
	public class ProductsViewModel : ModelValidationBase
	{
		#region Fields
		IProductService _productService;
		ICategoryService _categoryService;
		IEventAggregator _eventAggregator;
		IRegionManager _regionManager;
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
		public DelegateCommand OnAddCategoryCommand { get; private set; }
		public DelegateCommand OnSaveCommand { get; private set; }
		public DelegateCommand<ProductModel> OnDeleteCommand { get; private set; }
		public DelegateCommand OnCancelCommand { get; private set; }
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

		private bool _newProductFormVisibility;
		public bool NewProductFormVisibility
		{
			get { return _newProductFormVisibility; }
			set { SetProperty(ref _newProductFormVisibility, value); }
		}

		#region New Product Object
		private ProductModel _newProductModel;
		public ProductModel NewProductModel
		{
			get { return _newProductModel; }
			set { SetProperty(ref _newProductModel, value); }
		}

		private string _selectedImagePath;
		public string SelectedImagePath
		{
			get { return _selectedImagePath; }
			set { SetProperty(ref _selectedImagePath, value); }
		}

		private Uri _selectedImageUri;
		public Uri SelectedImageUri
		{
			get { return _selectedImageUri; }
			set { SetProperty(ref _selectedImageUri, value); }
		}

		#endregion New Product Object

		#region New Category
		private bool _newCategoryFormVisibility;
		public bool NewCategoryFormVisibility
		{
			get { return _newCategoryFormVisibility; }
			set { SetProperty(ref _newCategoryFormVisibility, value); }
		}

		private string _newCategoryName;
		public string NewCategoryName
		{
			get { return _newCategoryName; }
			set { SetProperty(ref _newCategoryName, value); }
		}

		private string _newCategoryDetail;
		public string NewCategoryDetail
		{
			get { return _newCategoryDetail; }
			set { SetProperty(ref _newCategoryDetail, value); }
		}

		private int _newCategoryStatus;
		public int NewCategoryStatus
		{
			get { return _newCategoryStatus; }
			set { SetProperty(ref _newCategoryStatus, value); }
		}

		private int _newCategoryParent;
		public int NewCategoryParent
		{
			get { return _newCategoryParent; }
			set { SetProperty(ref _newCategoryParent, value); }
		}

		public ICollectionView _mainCategoryListView;
		public ICollectionView MainCategoryListView
		{
			get { return _mainCategoryListView; }
			private set { SetProperty(ref _mainCategoryListView, value); }
		}

		public ICollectionView _categoryListView;
		public ICollectionView CategoryListView
		{
			get { return _categoryListView; }
			private set { SetProperty(ref _categoryListView, value); }
		}
		#endregion New Category

		public ICollectionView _productListView;
		public ICollectionView ProductListView
		{
			get { return _productListView; }
			private set { SetProperty(ref _productListView, value); }
		}
		#endregion "Bindable Property"

		public ProductsViewModel(IUnityContainer container)
		{
			_regionManager = container.Resolve<IRegionManager>();
			_productService = container.Resolve<IProductService>();
			_categoryService = container.Resolve<ICategoryService>();

			_eventAggregator = container.Resolve<IEventAggregator>();

			_newProductModel = new ProductModel();
			_title = "-Product Management Title-";
			NewProductFormVisibility = false;

			InitializeNewCategory();
			SetValidationRules();

			OnFirstCommand = new DelegateCommand(FirstCommandHandler);
			OnPreviousCommand = new DelegateCommand(PreviousCommandHandler);
			OnNextCommand = new DelegateCommand(NextCommandHandler);
			OnLastCommand = new DelegateCommand(LastCommandHandler);
			OnAddCommand = new DelegateCommand(AddCommandHandler);
			OnAddCategoryCommand = new DelegateCommand(OnAddCategoryCommandHandler);
			OnSaveCommand = new DelegateCommand(SaveCommandHandler);
			OnDeleteCommand = new DelegateCommand<ProductModel>(DeleteCommandHandler);
			OnCancelCommand = new DelegateCommand(CancelCommandHandler);
			OnSelectImageCommand = new DelegateCommand(SelectImageHandler);

			Initialization = InitializeProductListAsync();
			Initialization = InitializeCategoryListAsync();
			ProductListView.MoveCurrentToFirst();
		}

		private void SetValidationRules()
		{
			AddRule(() => NewCategoryName, () => NewCategoryName.Length == 5, "Must be 5 characters.");
		}

		private void OnAddCategoryCommandHandler()
		{
			NewCategoryFormVisibility = true;
			NewProductFormVisibility = false;
			InitializeNewCategory();
		}

		private void InitializeNewCategory ()
		{
			NewCategoryName = string.Empty;
			NewCategoryDetail = string.Empty;
			NewCategoryStatus = 1;
			NewCategoryParent = 0;
		}

		private CategoryModel CreateNewCategory()
		{
			return new CategoryModel
			{
				Name = NewCategoryName,
				Detail = NewCategoryDetail,
				Status = NewCategoryStatus,
				Parent = NewCategoryParent
			};
		}

		private void SelectImageHandler()
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
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

					SelectedImagePath = Path.Combine(ApplicationFolders.Products, Path.GetFileName(openFileDialog.FileName));
					var destFile = Path.Combine (targetPath, Path.GetFileName(openFileDialog.FileName));
					// To copy a file to another location and 
					// overwrite the destination file if it already exists.
					File.Copy(openFileDialog.FileName, destFile, true);

					SelectedImageUri = new Uri(destFile);
				}
			}
		}

		private async Task InitializeProductListAsync()
		{
			var result = await _productService.GetProductsAsync();
			if (result.DidError == false)
			{
				ProductListView = CollectionViewSource.GetDefaultView(result.Model.ToList());
			}
		}

		private async Task InitializeCategoryListAsync()
		{
			var result = await _categoryService.GetCategoriesAsync();
			if (result.DidError == false)
			{
				CategoryListView = CollectionViewSource.GetDefaultView(result.Model.ToList());

				MainCategoryListView = CollectionViewSource.GetDefaultView(result.Model.ToList());

				MainCategoryListView.Filter = category =>
				{
					CategoryModel entity = category as CategoryModel;
					return entity != null && entity.Parent == 0;
				};
			}
		}

		private void LastCommandHandler()
		{
			ProductListView.MoveCurrentToLast();
		}

		private void PreviousCommandHandler()
		{
			ProductListView.MoveCurrentToPrevious();

			if (ProductListView.IsCurrentBeforeFirst == true)
			{
				ProductListView.MoveCurrentToFirst();
			}
		}

		private void NextCommandHandler()
		{

			ProductListView.MoveCurrentToNext();

			if (ProductListView.IsCurrentAfterLast == true)
			{
				ProductListView.MoveCurrentToLast();
			}
		}

		private void FirstCommandHandler()
		{
			ProductListView.MoveCurrentToFirst();
		}

		private void AddCommandHandler()
		{
			NewProductFormVisibility = true;
			NewCategoryFormVisibility = false;
			NewProductModel = new ProductModel();
		}

		private async void SaveCommandHandler()
		{
			
			if (NewProductFormVisibility == true)
			{
				NewProductFormVisibility = false;
				NewProductModel.Image = SelectedImagePath;

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

				var result = await _productService.AddProductAsync(NewProductModel);
				await InitializeProductListAsync();
			}
			else if(NewCategoryFormVisibility == true)
			{
				NewCategoryFormVisibility = false;

				CategoryModel newCategory = CreateNewCategory();
				await _categoryService.AddCategoryAsync(newCategory);

				await InitializeCategoryListAsync();
			}
		}

		private async void DeleteCommandHandler(ProductModel user)
		{
			if (user != null)
			{
				await _productService.RemoveProductAsync(user.ID);
				await InitializeProductListAsync();
			}
		}

		private void CancelCommandHandler()
		{
			NewProductFormVisibility = false;
			NewCategoryFormVisibility = false;
		}
	}
}
