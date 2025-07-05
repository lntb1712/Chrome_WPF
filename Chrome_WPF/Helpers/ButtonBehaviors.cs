using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Chrome_WPF.Helpers
{
    public static class ButtonBehaviors
    {
        public static readonly DependencyProperty MouseEnterCommandProperty =
            DependencyProperty.RegisterAttached(
                "MouseEnterCommand",
                typeof(ICommand),
                typeof(ButtonBehaviors),
                new PropertyMetadata(null, OnMouseEnterCommandChanged));

        public static ICommand GetMouseEnterCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(MouseEnterCommandProperty);
        }

        public static void SetMouseEnterCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(MouseEnterCommandProperty, value);
        }

        private static void OnMouseEnterCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Button button)
            {
                if (e.OldValue != null)
                {
                    button.MouseEnter -= Button_MouseEnter;
                }
                if (e.NewValue != null)
                {
                    button.MouseEnter += Button_MouseEnter;
                }
            }
        }

        private static void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button button)
            {
                ICommand command = GetMouseEnterCommand(button);
                object parameter = button.GetValue(Button.CommandParameterProperty) ?? button.DataContext;
                if (command != null && command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }
        }
    }
}