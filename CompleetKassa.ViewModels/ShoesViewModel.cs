using CompleetKassa.Models;
using CompleetKassa.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CompleetKassa.ViewModels
{
    public class ShoesViewModel : PropertyChangedViewModel
    {
        private ObservableCollection<Product> _shoesList;
        public ObservableCollection<PurchasedProductViewModel> PurchasedItems { get; private set; }
        public ICommand OnPurchased { get; private set; }

        public ShoesViewModel()
        {
            PurchasedItems = new ObservableCollection<PurchasedProductViewModel>();
            // TODO: This is where to get data from DB
            _shoesList = new ObservableCollection<Product> {
                 new Product
                {
                    ID = "Artnr. 12121313131",
                    Label = "Cheyne Hawk",
                    ImagePath ="/CompleetKassa.ViewModels;component/Images/inkblack.jpg",
                    Price = 100.0m,
                    Description = "This is sample 1"
                },
                new Product
                {
                    ID =  "Artnr. 12121312323131",
                    Label = "Product 2",
                    ImagePath ="/CompleetKassa.ViewModels;component/Images/inkblack.jpg",
                    Price = 20.0m,
                    Description = "This is sample 2"
                },
                  new Product
                {
                    ID = "Artnr. 3131",
                    Label = "Product 3",
                    ImagePath ="/CompleetKassa.ViewModels;component/Images/inkblack.jpg",
                    Price = 50.0m,
                    Description = "This is sample 3"
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
