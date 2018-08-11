using System.Collections.ObjectModel;
using System.Linq;
using Prism.Mvvm;

namespace CompleetKassa.Models
{
    public class PurchasedProductModel : BindableBase
	{
        private decimal _discount;
        private decimal _tax;
        private decimal _subTotal;
        private decimal _due;
        private ObservableCollection<SelectedProductModel> _products;

        public PurchasedProductModel()
		{
            ID = 0;
            _tax = 0.0m;
            _discount = 0.0m;
            _subTotal = 0.0m;
            _due = 0.0m;
            _products = new ObservableCollection<SelectedProductModel>();

        }
        public int ID { get; set; }
        public string Label { get; set; }

        public ObservableCollection<SelectedProductModel> Products
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
