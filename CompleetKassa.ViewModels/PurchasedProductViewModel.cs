using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompleetKassa.ViewModels
{
    public class PurchasedProductViewModel : PropertyChangedViewModel
    {
        private int _quantity;

        public PurchasedProductViewModel()
        {
            ID = "0";
            Label = string.Empty;
            Price = 0.0m;
            _quantity = 1;

        }

        public string ID { get; set; }
        public string Label { get; set; }

        public decimal Price { get; set; }

        public int Quantity
        {
            get { return _quantity; }

            set { SetProperty(ref _quantity, value); }
        }
    }
}
