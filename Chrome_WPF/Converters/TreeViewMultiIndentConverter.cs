using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Chrome_WPF.Converters
{
    public class TreeViewIndentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TreeViewItem treeViewItem)
            {
                int level = GetTreeViewItemLevel(treeViewItem);
                // Use parameter for indentation size, default to 20 if not provided
                double indentSize = parameter != null && double.TryParse(parameter.ToString(), out double size) ? size : 20;
                double indent = level * indentSize;
                return new Thickness(indent, 0, 0, 0); // Only indent left
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