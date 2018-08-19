using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media;

namespace CompleetKassa.Converters
{
	public class RelativeToAbsolutePathConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string path = value as string;
			if (path != null)
			{
				string currentAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				string packUri = string.Format(@"{0}\{1}", currentAssemblyPath, path);
				return new Uri (packUri, UriKind.Absolute);
			}

			return string.Empty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
