using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Services.AccountManagementService;
using Chrome_WPF.Services.AuthServices;
using Chrome_WPF.Services.GroupManagementService;
using Chrome_WPF.Services.LoginServices;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.ViewModels;
using Chrome_WPF.ViewModels.GroupManagementViewModel;
using Chrome_WPF.Views;
using Chrome_WPF.Views.UserControls;
using Chrome_WPF.Views.UserControls.AccountManagement;
using Chrome_WPF.Views.UserControls.GroupManagement;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Chrome_WPF
{
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }
        private readonly ServiceCollection _services = new ServiceCollection();

        public App()
        {
            InitializeComponent();
            ConfigureServices(_services);
            ServiceProvider = _services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // Register API_Constant
            services.AddSingleton<API_Constant>();

            // Register Services
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<ILoginService, LoginService>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IAccountManagementService, AccountManagementService>();
            services.AddSingleton<IGroupManagementService, GroupManagementService>();

            // Register IServiceProvider
            services.AddSingleton(sp => sp); // Đăng ký chính container DI như IServiceProvider

            // Register NavigationService with ContentControl and IServiceProvider
            services.AddSingleton<INavigationService>(provider =>
            {
                var serviceProvider = provider.GetRequiredService<IServiceProvider>();
                // Trì hoãn việc lấy MainContent bằng cách sử dụng IServiceProvider
                return new NavigationService(null!, serviceProvider); // MainContent sẽ được gán sau
            });

            // Register ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<AuthViewModel>();
            services.AddTransient<AccountManagementViewModel>();
            services.AddTransient<AccountEditorViewModel>();
            services.AddTransient<GroupManagementViewModel>();
            services.AddTransient<GroupEditorViewModel>();

            // Register Views
            services.AddTransient<LoginWindow>(provider =>
                new LoginWindow(
                    provider.GetRequiredService<LoginViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<MainWindow>(provider =>
                new MainWindow(
                    provider.GetRequiredService<AuthViewModel>(),
                    provider.GetRequiredService<INotificationService>(),
                    provider.GetRequiredService<INavigationService>()));

            services.AddTransient<ucAccountManagement>(provider =>
                new ucAccountManagement(
                    provider.GetRequiredService<AccountManagementViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucAccountEditor>(provider =>
                new ucAccountEditor(
                    provider.GetRequiredService<AccountEditorViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucGroupManagement>(provider =>
                new ucGroupManagement(
                    provider.GetRequiredService<INotificationService>(),
                    provider.GetRequiredService<GroupManagementViewModel>()));
            services.AddTransient<ucGroupEditor>(provider =>
                new ucGroupEditor(
                    provider.GetRequiredService<GroupEditorViewModel>(),
                    provider.GetRequiredService<INotificationService>()));


        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Đặt culture mặc định là vi-VN để sử dụng định dạng dd/MM/yyyy
            CultureInfo culture = new CultureInfo("vi-VN");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            // Hiển thị LoginWindow
            var loginWindow = ServiceProvider!.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }
    }
}