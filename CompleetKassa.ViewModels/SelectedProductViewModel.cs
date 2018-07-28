using System.Collections.ObjectModel;

namespace CompleetKassa.ViewModels
{
    public class SelectedProductViewModel : BaseViewModel
    {
        private int _quantity;
        public int ID { get; set; }
        public string Label { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal {
            get
            {
                return Price * Quantity;
            }
        }

        public int Quantity
        {
            get { return _quantity; }

            set { SetProperty(ref _quantity, value); }
        }


        public SelectedProductViewModel() : base(string.Empty, string.Empty, string.Empty)
        {
            ID = 0;
            Label = string.Empty;
            Price = 0.0m;
            Quantity = 0;
        }
    }
}
