using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Chrome_WPF.Converters
{
    public class CurrentItemConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is int index && values[1] is IEnumerable<object> items)
            {
                if (index == 0) return null!; // Mục "Tổng" không cần dữ liệu từ danh sách
                var list = new List<object>(items);
                return index > 0 && index <= list.Count ? list[index - 1] : null!;
            }
            return null!;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
