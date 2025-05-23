using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Chrome_WPF.Converters
{
    public class PageButtonBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int pageNumber && parameter is int currentPage)
        {
            return pageNumber == currentPage
                ? new SolidColorBrush((Color)Colors.White) // Màu xanh Material Design cho trang hiện tại
                : Brushes.Transparent; // Trong suốt cho các trang khác
        }
        return Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
    }
}
