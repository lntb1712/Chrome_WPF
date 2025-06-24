using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Chrome_WPF.Converters
{
    public class BoolToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isEmpty)
            {
                return isEmpty ? "Còn trống" : "Hết chỗ";
            }
            return "Không xác định"; // Giá trị mặc định nếu không phải bool
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // Không cần triển khai nếu không hỗ trợ binding hai chiều
        }
    }
}
