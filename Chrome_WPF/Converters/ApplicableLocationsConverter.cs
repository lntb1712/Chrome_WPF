using Chrome_WPF.Models.GroupFunctionDTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Chrome_WPF.Converters
{
    public class ApplicableLocationsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<ApplicableLocationResponseDTO> locations && locations.Any())
            {
                // Assuming ApplicableLocationResponseDTO has a property named LocationCode
                return string.Join(",", locations.Select(loc => loc.ApplicableLocation));
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
