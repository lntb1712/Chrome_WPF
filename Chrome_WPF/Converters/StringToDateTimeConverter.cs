using System;
using System.Globalization;
using System.Windows.Data;

namespace Chrome_WPF.Converters
{
    public class StringToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string dateString && !string.IsNullOrWhiteSpace(dateString))
            {
                if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                {
                    return result;
                }
            }
            return null!; // Trả về null nếu chuỗi không hợp lệ, DatePicker sẽ không hiển thị ngày
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                return date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            return string.Empty; // Trả về chuỗi rỗng nếu không có ngày được chọn
        }
    }
}