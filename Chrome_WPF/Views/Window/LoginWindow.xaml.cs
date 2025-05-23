using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.ViewModels;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Chrome_WPF.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private bool _isPasswordVisible = false;
        private readonly LoginViewModel _viewModel;
        private readonly INotificationService _notificationService;
        public LoginWindow(LoginViewModel viewModel, INotificationService notificationService)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel; // Gán DataContext
            _notificationService = notificationService;
            _notificationService.RegisterSnackbar(LoginSnackbar);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.LoginRequestDTO.Password = ((PasswordBox)sender).Password;
                passwordPlaceholder.Visibility = string.IsNullOrEmpty(vm.LoginRequestDTO.Password) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;

            if (_isPasswordVisible)
            {
                ToggleIcon.Kind = PackIconKind.EyeOff; // Chuyển icon thành "EyeOff"
                passwordPlaceholder.Visibility = Visibility.Collapsed; // Ẩn watermark
                txtPassword.Visibility = Visibility.Collapsed; // Ẩn PasswordBox
                txtVisiblePassword.Visibility = Visibility.Visible; // Hiện TextBox
                txtVisiblePassword.Text = txtPassword.Password; // Hiển thị mật khẩu trong TextBox
            }
            else
            {
                ToggleIcon.Kind = PackIconKind.Eye; // Chuyển icon thành "Eye"
                passwordPlaceholder.Visibility = Visibility.Visible; // Hiện watermark
                txtPassword.Visibility = Visibility.Visible; // Hiện PasswordBox
                txtVisiblePassword.Visibility = Visibility.Collapsed; // Ẩn TextBox
                txtPassword.Password = txtVisiblePassword.Text; // Cập nhật mật khẩu trong PasswordBox
            }
        }

        private void TxtVisiblePassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.LoginRequestDTO.Password = txtVisiblePassword.Text;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnLogin_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DataContext is LoginViewModel vm && vm.LoginCommand.CanExecute(null))
                {
                    vm.LoginCommand.Execute(null);
                }
            }
        }


    }
}
