using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Chrome_WPF.Helpers
{
    public static class GridLengthAnimationHelper
    {
        public static readonly DependencyProperty AnimatedWidthProperty =
            DependencyProperty.RegisterAttached(
                "AnimatedWidth",
                typeof(double),
                typeof(GridLengthAnimationHelper),
                new PropertyMetadata(0.0, OnAnimatedWidthChanged));

        public static double GetAnimatedWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(AnimatedWidthProperty);
        }

        public static void SetAnimatedWidth(DependencyObject obj, double value)
        {
            obj.SetValue(AnimatedWidthProperty, value);
        }

        private static void OnAnimatedWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColumnDefinition column)
            {
                column.Width = new GridLength((double)e.NewValue);
            }
        }
    }
}
