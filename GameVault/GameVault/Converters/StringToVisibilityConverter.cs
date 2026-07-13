using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GameVault.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            var hasValue = value is string text && !string.IsNullOrWhiteSpace(text);

            var invert = string.Equals(parameter as string, "Invert", StringComparison.OrdinalIgnoreCase);

            if (invert)
            {
                hasValue = !hasValue;
            }

            return hasValue ? Visibility.Visible : Visibility.Collapsed;
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
