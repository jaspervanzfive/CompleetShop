using CompleetKassa.ViewModels;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace CompleetKassa.Converters
{
    public class ProductToPurchasedConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return new SelectedProductViewModel
            {
                ID = (int)values[0],
                Label = (string)values[1],
                Price = (decimal)values[2]
               
             
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return targetTypes.Select(t => Binding.DoNothing).ToArray();
        }
    }
}
