using System.Windows;
using System.Windows.Data;

namespace Chrome_WPF.Converters
{
    public class LengthToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string text)
            {
                return string.IsNullOrEmpty(text) ? Visibility.Visible : Visibility.Hidden;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
