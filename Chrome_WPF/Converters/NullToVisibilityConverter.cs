using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Chrome_WPF.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isInverse = parameter as string == "Inverse";
            bool isVisible = !string.IsNullOrWhiteSpace(value as string);
            return (isVisible && !isInverse) || (!isVisible && isInverse)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}