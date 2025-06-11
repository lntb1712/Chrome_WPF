using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Chrome_WPF.Converters
{
    public class CategoryNameToIconKindConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string category = value?.ToString()!;

            return category switch
            {
                "SFG" => PackIconKind.Cog,
                "MAT" => PackIconKind.Archive,
                "FG" => PackIconKind.PackageCheck,
                _ => PackIconKind.HelpCircleOutline // default icon
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
