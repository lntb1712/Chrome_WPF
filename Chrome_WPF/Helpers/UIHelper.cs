using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Chrome_WPF.Helpers
{
    public static class UIHelper
    {
        public static T FindVisualChild<T>(this DependencyObject parent, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild && (string.IsNullOrEmpty(name) || (typedChild as FrameworkElement)?.Name == name))
                {
                    return typedChild;
                }
                var foundChild = FindVisualChild<T>(child, name);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }
            return null!;
        }
        public static T FindAncestor<T>(this DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T ancestor)
                {
                    return ancestor;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            return null!;
        }
    }
}
