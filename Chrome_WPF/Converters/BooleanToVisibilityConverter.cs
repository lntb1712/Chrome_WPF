
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Chrome_WPF.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = value is bool boolValue && boolValue;
            bool invert = parameter is string param && param.Equals("Collapsed", StringComparison.OrdinalIgnoreCase);

            if (invert)
            {
                return isVisible ? Visibility.Collapsed : Visibility.Visible;
            }
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert = parameter is string param && param.Equals("Collapsed", StringComparison.OrdinalIgnoreCase);
            bool isVisible = value is Visibility visibility && visibility == Visibility.Visible;

            return invert ? !isVisible : isVisible;
        }
    }
}
