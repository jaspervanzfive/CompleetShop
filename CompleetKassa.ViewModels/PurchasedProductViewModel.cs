using System.Collections.ObjectModel;
using System.Linq;

namespace CompleetKassa.ViewModels
{
    public class PurchasedProductViewModel : BaseViewModel
    {
        private decimal _discount;
        private decimal _tax;
        private decimal _subTotal;
        private decimal _due;
        private ObservableCollection<SelectedProductViewModel> _products;

        public PurchasedProductViewModel() : base ("PurchasedProduct", "#FDAC94","dsdsds")
		{
            ID = 0;
            _tax = 0.0m;
            _discount = 0.0m;
            _subTotal = 0.0m;
            _due = 0.0m;
            _products = new ObservableCollection<SelectedProductViewModel>();

        }
        public int ID { get; set; }
        public string Label { get; set; }

        public ObservableCollection<SelectedProductViewModel> Products
        {
            get { return _products; }

            set { SetProperty(ref _products, value); }
        }

        public decimal Discount
        {
            get { return _discount; }

            set { SetProperty(ref _discount, value); }
        }

        public decimal SubTotal
        {
            get { return _subTotal; }

            set { SetProperty(ref _subTotal, value); }
        }

        public decimal Due
        {
            get { return _due; }

            set { SetProperty(ref _due, value); }
        }

        public decimal Tax
        {
            get { return _tax; }

            set { SetProperty(ref _tax, value); }
        }

        public void ComputeTotal()
        {
            SubTotal = Products.Sum(x => x.SubTotal);
            Due = (SubTotal + Tax) - Discount;
        }
    }
}
