using CompleetKassa.Models;
using System.Collections.ObjectModel;

namespace CompleetKassa.ViewModels
{
    public class ShoesViewModel : PropertyChangedViewModel
    {
        private ObservableCollection<Product> _shoesList;

        public ShoesViewModel()
        {
			// TODO: This is where to get data from DB
            _shoesList = new ObservableCollection<Product> {
                 new Product
                {
                    Label = "Sample1",
                    ImagePath ="/CompleetKassa.ViewModels;component/Images/sample.png",
                    Description = "This is sample 1"
                },
                new Product
                {
                    Label = "Sample2",
                    ImagePath ="/CompleetKassa.ViewModels;component/Images/sample.png",
                    Description = "This is sample 2"
                },
            };
        }

        public ObservableCollection<Product> ShoesList
        {
            get { return _shoesList; }
            set { SetProperty(ref _shoesList, value); }
        }
    }
}
