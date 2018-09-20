using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompleetKassa.DataValidation;
using Microsoft.Practices.Unity;
using Prism;
using Prism.Events;
using Prism.Regions;
using Prism.Commands;
using CompleetKassa.Models;
using CompleetKassa.Database.Services;
using System.Windows.Data;
using System.ComponentModel;
using static CompleetKassa.Module.ProductManagement.Definitions.Enumeration;
using System.Windows.Media;
using CompleetKassa.Events;

namespace CompleetKassa.Module.ProductManagement.ViewModels
{
    class CategoryRegistrationViewModel : ViewModelValidationBase, IActiveAware
    {
        #region Fields
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
        public DelegateCommand OnSaveCommand { get; private set; }
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
            set
            {
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

        private string _holdCategoryColor;
        public string HoldCategoryColor
        {
            get { return _holdCategoryColor; }
            set { SetProperty(ref _holdCategoryColor, value); }
        }

        private int _holdCategoryStatus;
        public int HoldCategoryStatus
        {
            get { return _holdCategoryStatus; }
            set { SetProperty(ref _holdCategoryStatus, value); }
        }

        private int _holdCategoryParent;

        public event EventHandler IsActiveChanged;

        public int HoldCategoryParent
        {
            get { return _holdCategoryParent; }
            set { SetProperty(ref _holdCategoryParent, value); }
        }

        public bool IsActive { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion Category Bindable Property


        #endregion "Bindable Property"

        
        #region Constructor
        public CategoryRegistrationViewModel(IUnityContainer container)
        {

            // Resolve services
            _regionManager = container.Resolve<IRegionManager>();
            _categoryService = container.Resolve<ICategoryService>();
            _eventAggregator = container.Resolve<IEventAggregator>();

            // Get data from Database
            Initialization = InitializeCategoryListAsync();

        
            CategoryListView.MoveCurrentToFirst();

            _title = "-Product Management Category Title-";
            _currentCommand = CompleetKassa.Module.ProductManagement.Definitions.Enumeration.Commands.None;
           
            NewCategoryFormVisibility = false;
          
            EnableCategoryList = true;
            ReadOnlyNewCategoryForm = true;


            SetCategoryValidationRules();
       

            // Event Handler
            OnFirstCommand = new DelegateCommand(FirstCommandHandler);
            OnPreviousCommand = new DelegateCommand(PreviousCommandHandler);
            OnNextCommand = new DelegateCommand(NextCommandHandler);
            OnLastCommand = new DelegateCommand(LastCommandHandler);
            OnAddCommand = new DelegateCommand(AddCommandHandler);
            OnEditCategoryCommand = new DelegateCommand<CategoryModel>(EditCategoryCommandHandler);
            OnSaveCommand = new DelegateCommand(SaveCommandHandler);
            OnDeleteCategoryCommand = new DelegateCommand<CategoryModel>(DeleteCategoryCommandHandler);
            OnCancelCommand = new DelegateCommand(CancelCommandHandler);
            OnCloseCommand = new DelegateCommand(CloseCommandHandler);
        }
        #endregion Constructor

        #region CommandHandler
        private void SetCategoryValidationRules()
        {
            AddRule(() => HoldCategoryName, () => string.IsNullOrEmpty(HoldCategoryName) == true || HoldCategoryName.Length == 5, "Must be 5 characters.");
        }

      

        private void HoldSelectedCategory(CategoryModel category)
        {
            if (category != null)
            {
                HoldCategoryID = category.ID;
                HoldCategoryName = category.Name;
                HoldCategoryDetail = category.Detail;
                HoldCategoryColor = category.Color;
                HoldCategoryStatus = category.Status;
                HoldCategoryParent = category.Parent;
            }
        }

        private void InitializeNewCategory()
        {
            HoldCategoryName = string.Empty;
            HoldCategoryDetail = string.Empty;
            HoldCategoryColor = string.Empty;
            HoldCategoryStatus = 1;
            HoldCategoryParent = 0;
        }

        private CategoryModel CreateNewCategory()
        {
            return new CategoryModel
            {
                Name = HoldCategoryName,
                Detail = HoldCategoryDetail,
                Color = HoldCategoryColor,
                Status = HoldCategoryStatus,
                Parent = HoldCategoryParent
            };
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

      

        private void LastCommandHandler()
        {
            CategoryListView.MoveCurrentToLast();
        }

        private void PreviousCommandHandler()
        {
            // Category
            CategoryListView.MoveCurrentToPrevious();

            if (CategoryListView.IsCurrentBeforeFirst == true)
            {
                CategoryListView.MoveCurrentToFirst();
            }
        }
        private void FirstCommandHandler()
        {
            // Category
            CategoryListView.MoveCurrentToFirst();

        }
        private void NextCommandHandler()
        {
            // Category
            CategoryListView.MoveCurrentToNext();

            if (CategoryListView.IsCurrentAfterLast == true)
            {
                CategoryListView.MoveCurrentToLast();
            }
        }
        
        private void AddCommandHandler()
        {
            _currentCommand = Definitions.Enumeration.Commands.AddCategory;
            InitializeNewCategory();
            NewCategoryFormVisibility = true;
            EnableCategoryList = false;
        }

        private void EditCategoryCommandHandler(CategoryModel category)
        {
            _currentCommand = Definitions.Enumeration.Commands.EditCategory;
            EnableCategoryList = false;
            ReadOnlyNewCategoryForm = false;
        }

      

        private async void SaveCommandHandler()
        {
            NewCategoryFormVisibility = false;
            EnableCategoryList = true;
            ReadOnlyNewCategoryForm = true;

            if (_currentCommand == Definitions.Enumeration.Commands.AddCategory)
            {
                await SaveNewCategory();
            }
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

       
        private void CancelCommandHandler()
        {
            NewCategoryFormVisibility = false;
            EnableCategoryList = true;
            ReadOnlyNewCategoryForm = true;

            HoldSelectedCategory(SelectedCategory);
        }

        private void CloseCommandHandler()
        {
            _eventAggregator.GetEvent<CloseEvent>().Publish("Products");
        }
        #endregion

    }
}
