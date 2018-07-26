using CompleetKassa.Models;
using CompleetKassa.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CompleetKassa.ViewModels
{
    public class ShoesViewModel : BaseViewModel
    {
        private ObservableCollection<Product> _shoesList;
        public ObservableCollection<PurchasedProductViewModel> PurchasedItems { get; private set; }
        public ICommand OnPurchased { get; private set; }

        public ShoesViewModel() : base ("Shoes", "#FDAC94","Icons/product.png")
		{
            PurchasedItems = new ObservableCollection<PurchasedProductViewModel>();
            // TODO: This is where to get data from DB
            _shoesList = new ObservableCollection<Product> {
                 new Product
                {
                    ID = 1,
                    Label = "Product 1",
                    ImagePath ="/CompleetKassa.ViewModels;component/Images/sample.png",
                    Price = 100.0m,
                    Description = "This is sample 1"
                },
                new Product
                {
                    ID = 2,
                    Label = "Product 2",
                    ImagePath ="/CompleetKassa.ViewModels;component/Images/sample.png",
                    Price = 20.0m,
                    Description = "This is sample 2"
                },
            };

            OnPurchased = new BaseCommand(Puchase);
        }

        public ObservableCollection<Product> ShoesList
        {
            get { return _shoesList; }
            set { SetProperty(ref _shoesList, value); }
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
