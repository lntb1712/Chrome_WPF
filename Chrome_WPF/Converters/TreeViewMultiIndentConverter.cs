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
    // Converter for Indentation by Level
    public class TreeViewIndentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TreeViewItem treeViewItem)
            {
                int level = GetTreeViewItemLevel(treeViewItem);
                double indent = level * 20; // Mỗi cấp thụt 20px
                return new Thickness(indent, 0, 0, 0); // Chỉ thụt bên trái
            }
            return new Thickness(0);
        }

        private int GetTreeViewItemLevel(DependencyObject item)
        {
            int level = 0;
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (parent != null && !(parent is TreeView))
            {
                if (parent is TreeViewItem)
                    level++;
                parent = VisualTreeHelper.GetParent(parent);
            }
            return level;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
