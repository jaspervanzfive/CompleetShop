using CompleetKassa.Models;
using CompleetKassa.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace CompleetKassa.ViewModels
{
    public class ProductsViewModel : BaseViewModel
    {
        private IList<Product> _dbProductList;

        // TODO: Multi receipt
        private IDictionary<int, PurchasedProductViewModel> _purchaseList;
        private PurchasedProductViewModel _currentPurchase;
        private ObservableCollection<SelectedProductViewModel> _purchasedProducts;
        private ObservableCollection<ProductCategory> _categories;
        private ObservableCollection<ProductSubCategory> _subCategories;

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
            set {
                _selectedSubCategory = value;
                SubCategoryFilter = value.Name;
            }
        }

        private ICollectionView _productList;
        public ICollectionView ProductList
        {
            get { return _productList; }
            set { SetProperty(ref _productList, value); }
        }

        public ICommand OnPurchased { get; private set; }
        public ICommand OnSelectCategory { get; private set; }


        public ProductsViewModel() : base ("Shoes", "#FDAC94","Icons/product.png")
		{
            //PurchasedItems = new ObservableCollection<PurchasedProductViewModel>();
            _categories = new ObservableCollection<ProductCategory>();
            _currentPurchase = new PurchasedProductViewModel ();
            _purchasedProducts = new ObservableCollection<SelectedProductViewModel>();

            // TODO: This is where to get data from DB
            GetProducts();

            // Set the first product as active category
            _categoryFilter = _categories.FirstOrDefault() == null ? string.Empty : _categories.FirstOrDefault().Name;

            // TODO: Get the first subcategory
            _subCategoryFilter = "Running";

            _productList = CollectionViewSource.GetDefaultView(_dbProductList);
            _productList.Filter += ProductCategoryFilter;
            _productList.Filter += ProductSubCategoryFilter;

            // Commands
            OnPurchased = new BaseCommand(Puchase);
            OnSelectCategory = new BaseCommand(SelectCategory);
        }

        private bool ProductCategoryFilter(object item)
        {
            var product = item as Product;
            return item == null ? true : product.Category.Contains(_categoryFilter);
        }

        private bool ProductSubCategoryFilter(object item)
        {
            var product = item as Product;
            return (product.Category.Contains(_categoryFilter) &&
                product.SubCategory.Contains(_subCategoryFilter));
        }

        private void SelectCategory(object obj)
        {
            var item = (ProductCategory)obj;

            CategoryFilter = item.Name;
            SetSubCategories(item.Name);
        }

        private void SetSubCategories (string category)
        {
            SubCategories = new ObservableCollection<ProductSubCategory>(_categories.Where(x => x.Name == category).First().SubCategories);
            SubCategoryFilter = SubCategories.FirstOrDefault().Name;
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

                _categories.Add(new ProductCategory {
                    Name = category,
                    Color = "Color",
                    SubCategories = productSubCategories
                });
            }
        }

        private void GetProducts ()
        {
            _dbProductList = new List<Product> {
                 new Product
                {
                    ID = 1,
                    Label = "Shoes 1",
                    ImagePath ="/CompleetKassa.ViewModels;component/Images/sample.png",
                    Price = 100.0m,
                    Description = "This is sample 1",
                    Category = "Shoes",
                    SubCategory = "Running"
                },
                new Product
                {
                    ID = 2,
                    Label = "Shoes 2",
                    ImagePath ="/CompleetKassa.ViewModels;component/Images/sample.png",
                    Price = 20.0m,
                    Description = "This is sample 2",
                    Category = "Shoes",
                    SubCategory = "Walking"
                },
                new Product
                {
                    ID = 3,
                    Label = "Bag 1",
                    ImagePath ="/CompleetKassa.ViewModels;component/Images/sample.png",
                    Price = 20.0m,
                    Description = "This is sample 2",
                    Category = "Bag",
                    SubCategory = "Shoulder Bag"
                },
                new Product
                {
                    ID = 4,
                    Label = "Bag 2",
                    ImagePath ="/CompleetKassa.ViewModels;component/Images/sample.png",
                    Price = 20.0m,
                    Description = "This is sample 2",
                    Category = "Bag",
                    SubCategory = "Shoulder Bag"
                },
                new Product
                {
                    ID = 5,
                    Label = "Belt 1",
                    ImagePath ="/CompleetKassa.ViewModels;component/Images/sample.png",
                    Price = 10.0m,
                    Description = "This is Belt 1",
                    Category = "Belt",
                    SubCategory = "Men's Belt"
                }
            };

            GetCategories(_dbProductList);
        }

        public ObservableCollection<ProductCategory> Categories
        {
            get { return _categories; }
            set { SetProperty(ref _categories, value); }
        }

        public ObservableCollection<ProductSubCategory> SubCategories
        {
            get { return _subCategories; }
            set { SetProperty(ref _subCategories, value); }
        }

        public PurchasedProductViewModel CurrentPurchase
        {
            get { return _currentPurchase; }
            set { SetProperty(ref _currentPurchase, value); }
        }

        public ObservableCollection<SelectedProductViewModel> PurchasedProducts
        {
            get {
                _purchasedProducts = _currentPurchase.Products;
                return _purchasedProducts;
            }
            set { SetProperty(ref _purchasedProducts, value); }
        }

        private void Puchase(object obj)
        {
            var item = (SelectedProductViewModel)obj;

            var purchase = _purchasedProducts.FirstOrDefault(x => x.ID == item.ID);

            if (purchase != null)
            {
                purchase.Quantity++;
            }
            else
            {
                item.Quantity = 1;
                PurchasedProducts.Add(item);
            }

            CurrentPurchase.ComputeTotal();
        }
    }
}
