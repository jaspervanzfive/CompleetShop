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
using Microsoft.Practices.Unity;
using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

namespace CompleetKassa.Module.ProductManagement.ViewModels
{
    internal class ProductRegistrationViewModel : ViewModelValidationBase, IActiveAware
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
        private CompleetKassa.Module.ProductManagement.Definitions.Enumeration.Commands _currentCommand;

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

        private string _newProductBrand;
        public string NewProductBrand
        {
            get { return _newProductBrand; }
            set { SetProperty(ref _newProductBrand, value); }
        }

        private string _newProductImage;
        public string NewProductImage
        {
            get { return _newProductImage; }
            set { SetProperty(ref _newProductImage, value); }
        }

        private decimal _newProductPrice;
        public decimal NewProductPrice
        {
            get { return _newProductPrice; }
            set { SetProperty(ref _newProductPrice, value); }
        }

        private decimal _newProductSalePrice;
        public decimal NewProductSalePrice
        {
            get { return _newProductSalePrice; }
            set { SetProperty(ref _newProductSalePrice, value); }
        }

        private int _newProductQuantity;
        public int NewProductQuantity
        {
            get { return _newProductQuantity; }
            set { SetProperty(ref _newProductQuantity, value); }
        }

        private int _newProductMinimumStock;
        public int NewProductMinimumStock
        {
            get { return _newProductMinimumStock; }
            set { SetProperty(ref _newProductMinimumStock, value); }
        }

        private int _newProductStatus;
        public int NewProductStatus
        {
            get { return _newProductStatus; }
            set { SetProperty(ref _newProductStatus, value); }
        }

        private bool _newProductFavorite;
        public bool NewProductFavorite
        {
            get { return _newProductFavorite; }
            set { SetProperty(ref _newProductFavorite, value); }
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

        #endregion Category Bindable Property


        #endregion "Bindable Property"

        #region Constructor
        public ProductRegistrationViewModel(IUnityContainer container)
        {

            // Resolve services
            _regionManager = container.Resolve<IRegionManager>();
            _productService = container.Resolve<IProductService>();
            _eventAggregator = container.Resolve<IEventAggregator>();

            // Get data from Database
            Initialization = InitializeProductListAsync();

            ProductListView.MoveCurrentToFirst();

            _title = "-Product Management Title-";
            _currentCommand = Definitions.Enumeration.Commands.None;
            NewProductFormVisibility = false;
            EnableProductList = true;
            SetProductValidationRules();

            // Event Handler
            OnFirstCommand = new DelegateCommand(FirstCommandHandler);
            OnPreviousCommand = new DelegateCommand(PreviousCommandHandler);
            OnNextCommand = new DelegateCommand(NextCommandHandler);
            OnLastCommand = new DelegateCommand(LastCommandHandler);
            OnAddCommand = new DelegateCommand(AddCommandHandler);

            OnEditProductCommand = new DelegateCommand<ProductModel>(EditProductCommandHandler);
            OnSaveCommand = new DelegateCommand(SaveCommandHandler);
            OnDeleteProductCommand = new DelegateCommand<ProductModel>(DeleteProductCommandHandler);

            OnCancelCommand = new DelegateCommand(CancelCommandHandler);
            OnCloseCommand = new DelegateCommand(CloseCommandHandler);
            OnSelectImageCommand = new DelegateCommand(SelectImageHandler);
        }
        #endregion Constructor

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnIsActiveChanged();
            }
        }
        private void OnIsActiveChanged()
        {
            OnCancelCommand.IsActive = IsActive;
            OnAddCommand.IsActive = IsActive;
            //OnEditCommand.IsActive = IsActive;
            //OnDeleteCommand.IsActive = IsActive;
            //OnSaveCommand.IsActive = IsActive;

            //OnFirstNavCommand.IsActive = IsActive;
            //OnPreviousNavCommand.IsActive = IsActive;
            //OnNextNavCommand.IsActive = IsActive;
            //OnLastNavCommand.IsActive = IsActive;

            IsActiveChanged?.Invoke(this, new EventArgs());
        }

        public event EventHandler IsActiveChanged;

        #region CommandHandler

        private void SetProductValidationRules()
        {
            AddRule(() => NewProductName, () => string.IsNullOrEmpty(NewProductName) == true || NewProductName.Length == 5, "Must be 5 characters.");
        }

        private void InitializeNewProduct()
        {
            NewProductCode = string.Empty;
            NewProductName = string.Empty;
            NewProductBrand = string.Empty;
            NewProductImage = string.Empty;
            NewProductPrice = 0.0m;
            NewProductSalePrice = 0.0m;
            NewProductQuantity = 0;
            NewProductMinimumStock = 0;
            NewProductStatus = 1;
            NewProductFavorite = false;
            NewProductCategory = 0;
        }


        private ProductModel CreateNewProduct()
        {
            return new ProductModel
            {
                Code = NewProductCode,
                Name = NewProductName,
                Brand = NewProductBrand,
                Image = NewProductImage,
                Price = NewProductPrice,
                SalePrice = NewProductSalePrice,
                Quantity = NewProductQuantity,
                MinimumStock = NewProductMinimumStock,
                Status = NewProductStatus,
                Favorite = NewProductFavorite,
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
            _currentCommand = Definitions.Enumeration.Commands.AddProduct;
            InitializeNewProduct();
            NewProductFormVisibility = true;
            EnableProductList = false;
        }



        private void EditProductCommandHandler(ProductModel product)
        {
            _currentCommand = Definitions.Enumeration.Commands.EditProduct;
            EnableProductList = false;
        }

        private async void SaveCommandHandler()
        {
            NewProductFormVisibility = false;
            EnableProductList = true;

            if (_currentCommand == Definitions.Enumeration.Commands.AddProduct)
            {
                await SaveNewProduct();
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
            NewProductFormVisibility = false;
            EnableProductList = true;
        }

        private void CloseCommandHandler()
        {
            _eventAggregator.GetEvent<CloseEvent>().Publish("Products");
        }

        #endregion
    }
}
