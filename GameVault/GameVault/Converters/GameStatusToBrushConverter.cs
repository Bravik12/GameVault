using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using GameVault.Models;

namespace GameVault.Converters
{
    public class GameStatusToBrushConverter : IValueConverter
    {
        private static readonly SolidColorBrush NotStartedBrush = new(Color.FromRgb(0x63, 0x5E, 0x85));
        private static readonly SolidColorBrush PlayingBrush = new(Color.FromRgb(0x7C, 0x4D, 0xFF));
        private static readonly SolidColorBrush CompletedBrush = new(Color.FromRgb(0x3A, 0x6F, 0xF7));
        private static readonly SolidColorBrush DroppedBrush = new(Color.FromRgb(0xE5, 0x48, 0x4D));

        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return value is GameStatus status
                ? status switch
                {
                    GameStatus.Playing => PlayingBrush,
                    GameStatus.Completed => CompletedBrush,
                    GameStatus.Dropped => DroppedBrush,
                    _ => NotStartedBrush
                }
                : NotStartedBrush;
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
