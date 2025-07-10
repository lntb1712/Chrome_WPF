using Chrome_WPF.Helpers;
using Chrome_WPF.Models.LoginDTO;
using Chrome_WPF.Services.AuthServices;
using Chrome_WPF.Services.LoginServices;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.ReplenishService; // Thêm namespace cho IReplenishService
using Chrome_WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly ILoginService _loginService;
        private readonly IAuthService _authService;
        private readonly INotificationService _notificationService;
        private LoginRequestDTO _loginRequest;

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

        public LoginViewModel(
            ILoginService loginService,
            IAuthService authService,
            INotificationService notificationService)
        {
            _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            
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
                    List<string> warehousePermissions = await _authService.GetWarehousePermissionFromToken(response.Data!.Token!);
                    var stringCollection = new StringCollection();
                    stringCollection.AddRange(warehousePermissions.ToArray());
                    Properties.Settings.Default.WarehousePermission = stringCollection;
                    Properties.Settings.Default.Save();

                  

                    _notificationService.QueueMessageForNextSnackbar("Đăng nhập thành công!", "OK", isError: false);

                    // Lấy MainWindow từ DI container
                    var existingMainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                    if (existingMainWindow != null)
                    {
                        existingMainWindow.Activate(); // Đưa cửa sổ hiện có lên trước
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
                    _notificationService.ShowMessage(response.Message!, "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Đăng nhập thất bại! {ex.Message}", "OK", null!, TimeSpan.FromSeconds(5), isError: true);
            }
        }

        
    }
}