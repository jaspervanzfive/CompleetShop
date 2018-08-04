using CompleetKassa.Models;
using CompleetKassa.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CompleetKassa.ViewModels
{
    public class ProductsViewModel : BaseViewModel
    {
        public ProductsViewModel() : base ("Products", "#FDAC94", "Icons/product.png")
        {
            _productList = new ObservableCollection<Product> {
                 new Product
                {
                    ID = 1,
                    Label = "Cheyene Hawk",
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
                    m_deleteCommand = new BaseCommand(param => Delete(), param => CanDelete);
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
