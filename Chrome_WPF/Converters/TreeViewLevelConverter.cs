using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Chrome_WPF.Converters
{
    public class TreeViewLevelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DependencyObject obj)
            {
                int level = GetTreeViewItemLevel(obj);
                switch (level)
                {
                    case 0: return new SolidColorBrush(Color.FromRgb(189, 189, 189)); // Light gray for top level
                    case 1: return new SolidColorBrush(Color.FromRgb(120, 144, 156)); // Blue-gray for level 1
                    case 2: return new SolidColorBrush(Color.FromRgb(84, 110, 122));  // Darker blue-gray for level 2
                    default: return new SolidColorBrush(Color.FromRgb(69, 90, 100));  // Darkest for deeper levels
                }
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private int GetTreeViewItemLevel(DependencyObject obj)
        {
            int level = 0;
            DependencyObject parent = obj;
            while (parent != null)
            {
                if (parent is TreeView)
                    break;
                if (parent is TreeViewItem)
                    level++;
                parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
            }
            return level;
        }
    }
}
