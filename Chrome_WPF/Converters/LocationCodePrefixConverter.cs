using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Chrome_WPF.Converters
{
    public class LocationCodePrefixConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[0] == null || values[1] == null)
                return string.Empty;

            string warehouseCode = values[0].ToString()!;
            string locationCode = values[1].ToString()!;

            return string.IsNullOrEmpty(warehouseCode) || string.IsNullOrEmpty(locationCode)
                ? locationCode
                : $"{warehouseCode}/{locationCode}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return new object[] { Binding.DoNothing, string.Empty };

            string input = value.ToString()!;
            string[] parts = input.Split('/');

            // If input includes prefix (e.g., "WH001/LOC001"), return only LocationCode
            if (parts.Length == 2)
                return new object[] { Binding.DoNothing, parts[1] };

            // If input is just LocationCode (e.g., "LOC001"), return it
            return new object[] { Binding.DoNothing, parts[0] };
        }
    }
}
