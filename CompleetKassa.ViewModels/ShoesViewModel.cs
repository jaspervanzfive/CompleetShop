using CompleetKassa.Models;
using System.Collections.ObjectModel;

namespace CompleetKassa.ViewModels
{
    public class ShoesViewModel : PropertyChangedViewModel
    {
        //private readonly PropertyChangedViewModel _mainViewModel;

        //public ShoesViewModel(PropertyChangedViewModel mainViewModel)
        //{
        //    _mainViewModel = mainViewModel;
        //}

        private ObservableCollection<Product> _shoesList;

        public ShoesViewModel()
        {
            _shoesList = new ObservableCollection<Product> {
                 new Product
                {
                    Label = "Sample1",
                    ImagePath ="/MVVMUserControls;component/Images/sample1.png",
                    Description = "This is sample 1"
                },
                new Product
                {
                    Label = "Sample2",
                    ImagePath ="/MVVMUserControls;component/Images/sample1.png",
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
