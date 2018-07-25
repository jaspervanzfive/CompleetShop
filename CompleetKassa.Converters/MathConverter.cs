using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CompleetKassa.Converters
{
    public class MathConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double total = System.Convert.ToDouble(values[0]) * System.Convert.ToDouble(values[1]);

            return total;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return targetTypes.Select(t => Binding.DoNothing).ToArray();
        }
    }
}
