using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Chrome_WPF.Helpers
{
    public class DataGridBehaviors
    {
        public static readonly DependencyProperty PreviewMouseMoveCommandProperty =
            DependencyProperty.RegisterAttached(
                "PreviewMouseMoveCommand",
                typeof(ICommand),
                typeof(DataGridBehaviors),
                new PropertyMetadata(null, OnPreviewMouseMoveCommandChanged));

        public static ICommand GetPreviewMouseMoveCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(PreviewMouseMoveCommandProperty);
        }

        public static void SetPreviewMouseMoveCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(PreviewMouseMoveCommandProperty, value);
        }

        private static void OnPreviewMouseMoveCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid)
            {
                if (e.OldValue != null)
                {
                    dataGrid.PreviewMouseMove -= DataGrid_PreviewMouseMove;
                }
                if (e.NewValue != null)
                {
                    dataGrid.PreviewMouseMove += DataGrid_PreviewMouseMove;
                }
            }
        }

        private static void DataGrid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                var row = DataGridRow.GetRowContainingElement((FrameworkElement)(e.OriginalSource as DependencyObject)!);
                if (row != null)
                {
                    var command = GetPreviewMouseMoveCommand(dataGrid);
                    var parameter = row.DataContext;
                    if (command?.CanExecute(parameter) == true)
                    {
                        command.Execute(parameter);
                    }
                }
            }
        }
    }
}