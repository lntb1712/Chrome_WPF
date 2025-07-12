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
    public class PercentageToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double percentage)
            {
                if (percentage <= 10) return new SolidColorBrush(Color.FromRgb(255, 0, 0)); 
                else if (percentage <= 30) return new SolidColorBrush(Color.FromRgb(255, 165, 0)); // Cam
                else return new SolidColorBrush(Color.FromRgb(0, 128, 0)); 
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
