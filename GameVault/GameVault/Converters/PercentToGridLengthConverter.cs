using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GameVault.Converters
{
    public class PercentToGridLengthConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            var percent = value is double d ? Math.Clamp(d, 0, 100) : 0;

            var isRemainder = string.Equals(parameter as string, "Remainder", StringComparison.OrdinalIgnoreCase);

            var star = isRemainder ? 100 - percent : percent;

            return new GridLength(star, GridUnitType.Star);
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
