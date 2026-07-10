using System;
using System.Globalization;
using System.Windows.Data;

namespace GameVault.Converters
{
    public class BooleanToEnabledConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is bool isNotRated)
            {
                return !isNotRated;
            }

            return true;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}