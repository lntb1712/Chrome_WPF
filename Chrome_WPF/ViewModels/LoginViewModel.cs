using Chrome_WPF.Helpers;
using Chrome_WPF.Models.LoginDTO;
using Chrome_WPF.Services.AuthServices;
using Chrome_WPF.Services.LoginServices;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly ILoginService _loginService;
        private LoginRequestDTO _loginRequest;
        private readonly IAuthService _authService;
        private readonly INotificationService _notificationService;

        public LoginRequestDTO LoginRequestDTO
        {
            get => _loginRequest;
            set
            {
                _loginRequest = value;
                OnPropertyChanged(nameof(LoginRequestDTO));
                ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand LoginCommand { get; set; }
        public LoginViewModel(ILoginService loginService, IAuthService authService, INotificationService notificationService)
        {
            _loginService = loginService;
            _notificationService = notificationService;
            _authService = authService;
            _loginRequest = new LoginRequestDTO();
            LoginCommand = new RelayCommand(async _ => await ExecuteLogin(null!));
        }

        private async Task ExecuteLogin(object parameter)
        {
            try
            {
                var response = await _loginService.AuthenticateAsync(LoginRequestDTO);
                if (response.Success)
                {
                    Properties.Settings.Default.AccessToken = response.Data!.Token;
                    Properties.Settings.Default.UserName = response.Data!.Username;
                    Properties.Settings.Default.FullName = await _authService.GetName(response.Data!.Token!);
                    Properties.Settings.Default.Role = await _authService.GetRole(response.Data!.Token!);
                    Properties.Settings.Default.Save();

                    _notificationService.QueueMessageForNextSnackbar("Đăng nhập thành công!", "OK", isError: false);
                    // Lấy MainWindow từ DI container
                    var existingMainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                    if (existingMainWindow != null)
                    {
                        existingMainWindow.Activate(); // Bring existing window to front
                        return;
                    }

                    var app = (App)Application.Current;
                    var mainWindow = App.ServiceProvider!.GetRequiredService<MainWindow>();
                    mainWindow.Show();

                    Application.Current.Windows
                        .OfType<LoginWindow>()
                        .FirstOrDefault()?.Close();
                }
                else
                {
                    // Thất bại: Màu đỏ (isError = true)
                    _notificationService.ShowMessage(response.Message!, "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                // Lỗi: Màu đỏ (isError = true)
                _notificationService.ShowMessage($"Đăng nhập thất bại! {ex.Message}", "OK", null!, TimeSpan.FromSeconds(5), isError: true);
            }
        }
    }
}
