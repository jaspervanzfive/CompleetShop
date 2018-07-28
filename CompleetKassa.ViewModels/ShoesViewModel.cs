using CompleetKassa.Models;
using CompleetKassa.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace CompleetKassa.ViewModels
{
    public class ShoesViewModel : BaseViewModel
    {
        
        public ObservableCollection<PurchasedProductViewModel> PurchasedItems { get; private set; }
        public ICommand OnPurchased { get; private set; }
        public ICommand OnSelectCategory { get; private set; }

        private ObservableCollection<ProductCategory> _categories;

        IList<Product> _dbShoesList;

        public ShoesViewModel() : base ("Shoes", "#FDAC94","Icons/product.png")
		{
            PurchasedItems = new ObservableCollection<PurchasedProductViewModel>();
            // TODO: This is where to get data from DB
            //ShoesList = new ObservableCollection<Product>();
            Categories = new ObservableCollection<ProductCategory>();

            GetProducts();
            GetCategories();

            _filterString = "Shoes";
            _shoesList = CollectionViewSource.GetDefaultView(_dbShoesList);
            _shoesList.Filter = ProductFilter;

            //_shoesList = new ObservableCollection<Product>(_dbShoesList); ;

            // Set categories based on loaded products
           

            OnPurchased = new BaseCommand(Puchase);

            OnSelectCategory = new BaseCommand(SelectCategory);
        }

        private bool ProductFilter(object item)
        {
            //return true;
            var product = item as Product;
            return product.Category.Contains(_filterString);
        }

        private string _filterString;
        public string FilterString
        {
            get { return _filterString; }
            set {
                SetProperty(ref _filterString, value);
                ShoesList.Refresh();
            }
        }

        private ICollectionView _shoesList;
        public ICollectionView ShoesList
        {
            get { return _shoesList; }
            set { SetProperty(ref _shoesList, value); }
        }

        private void SelectCategory(object obj)
        {
            var item = (ProductCategory)obj;

            FilterString = item.Name;
        }

        private void GetCategories ()
        {
            // TODO: Categories can be obtained from DB especially the color
            _categories = new ObservableCollection<ProductCategory>();
            if (_dbShoesList != null)
            {
                var productCategories = _dbShoesList.Select(x => x.Category).Distinct();

                foreach (var category in productCategories)
                {
                    _categories.Add(new ProductCategory { Name = category, Color = "Red" });
                }
            }
        }

        private void GetProducts ()
        {
            _dbShoesList = new List<Product> {
                 new Product
                {
                    ID = 1,
                    Label = "Shoes 1",
                    ImagePath ="/CompleetKassa.ViewModels;component/Images/sample.png",
                    Price = 100.0m,
                    Description = "This is sample 1",
                    Category = "Shoes",
                    SubCategory = "Running Shoes"
                },
                new Product
                {
                    ID = 2,
                    Label = "Shoes 2",
                    ImagePath ="/CompleetKassa.ViewModels;component/Images/sample.png",
                    Price = 20.0m,
                    Description = "This is sample 2",
                    Category = "Shoes",
                    SubCategory = "Running Shoes"
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
                }
            };
        }

        public ObservableCollection<ProductCategory> Categories
        {
            get { return _categories; }
            set { SetProperty(ref _categories, value); }
        }

        void Puchase(object obj)
        {
            var item = (PurchasedProductViewModel)obj;

            var purchase = PurchasedItems.FirstOrDefault(x => x.ID == item.ID);

            if (purchase != null)
            {
                purchase.Quantity++;
            }
            else
            {
                PurchasedItems.Add(item);
            }
        }
    }
}
