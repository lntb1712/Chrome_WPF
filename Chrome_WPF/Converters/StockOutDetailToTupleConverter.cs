using Chrome_WPF.Models.StockOutDetailDTO;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Chrome_WPF.Converters
{
    public class StockOutDetailToTupleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Button button && button.DataContext is StockOutDetailResponseDTO stockOutDetail)
            {
                return new Tuple<StockOutDetailResponseDTO, Button>(stockOutDetail, button);
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported.");
        }
    }
}