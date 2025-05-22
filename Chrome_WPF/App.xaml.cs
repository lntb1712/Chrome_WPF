using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Services.AuthServices;
using Chrome_WPF.Services.LoginServices;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.ViewModels;
using Chrome_WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;

namespace Chrome_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider? _serviceProvider { get; private set; }

        public App()
        {
            InitializeComponent();
            var services = new ServiceCollection();
            ConfigureService(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureService(ServiceCollection services)
        {
            // Register API_Constant
            services.AddSingleton<API_Constant>();
            // Register INotificationService
            services.AddSingleton<INotificationService, NotificationService>();

            // Register LoginService and ViewModel
            services.AddSingleton<ILoginService, LoginService>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<LoginWindow>(provider =>
                new LoginWindow(
                    provider.GetRequiredService<LoginViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            //Register MainWindow
            services.AddTransient<AuthViewModel>();
            services.AddTransient<MainWindow>(provider=>
                new MainWindow(
                    provider.GetRequiredService<AuthViewModel>(),
                    provider.GetRequiredService<INotificationService>()));
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var loginWindow = _serviceProvider!.GetRequiredService<LoginWindow>();
            // Đặt culture mặc định là vi-VN để sử dụng định dạng dd/MM/yyyy
            CultureInfo culture = new CultureInfo("vi-VN");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            loginWindow.Show();
        }
    }

}
