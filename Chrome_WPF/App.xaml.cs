using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Services.AccountManagementService;
using Chrome_WPF.Services.AuthServices;
using Chrome_WPF.Services.CategoryService;
using Chrome_WPF.Services.CustomerMasterService;
using Chrome_WPF.Services.GroupManagementService;
using Chrome_WPF.Services.LoginServices;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.ProductMasterService;
using Chrome_WPF.Services.ProductSupplierService;
using Chrome_WPF.Services.SupplierMasterService;
using Chrome_WPF.ViewModels;
using Chrome_WPF.ViewModels.CustomerMasterViewModel;
using Chrome_WPF.ViewModels.GroupManagementViewModel;
using Chrome_WPF.ViewModels.ProductMasterViewModel;
using Chrome_WPF.ViewModels.SupplierMasterViewModel;
using Chrome_WPF.Views;
using Chrome_WPF.Views.UserControls;
using Chrome_WPF.Views.UserControls.AccountManagement;
using Chrome_WPF.Views.UserControls.CustomerMaster;
using Chrome_WPF.Views.UserControls.GroupManagement;
using Chrome_WPF.Views.UserControls.ProductMaster;
using Chrome_WPF.Views.UserControls.SupplierMaster;
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
            services.AddSingleton<IMessengerService, MessengerService>();
            services.AddSingleton<ILoginService, LoginService>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IAccountManagementService, AccountManagementService>();
            services.AddSingleton<IGroupManagementService, GroupManagementService>();
            services.AddSingleton<IProductMasterService, ProductMasterService>();
            services.AddSingleton<ICategoryService, CategoryService>();
            services.AddSingleton<IProductSupplierService, ProductSupplierService>();
            services.AddSingleton<ISupplierMasterService, SupplierMasterService>();
            services.AddSingleton<ICustomerMasterService, CustomerMasterService>();


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
            services.AddTransient<ProductMasterViewModel>();
            services.AddTransient<ProductDetailViewModel>();
            services.AddTransient<SupplierMasterViewModel>();
            services.AddTransient<SupplierEditorViewModel>();
            services.AddTransient<CustomerMasterViewModel>();
            services.AddTransient<CustomerEditorViewModel>();

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

            services.AddTransient<ucProductMaster>(provider =>
                new ucProductMaster(
                    provider.GetRequiredService<ProductMasterViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucProductDetail>(provider =>
                new ucProductDetail(
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucSupplierMaster>(provider =>
                new ucSupplierMaster(
                    provider.GetRequiredService<SupplierMasterViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucSupplierEditor>(provider =>
                new ucSupplierEditor(
                    provider.GetRequiredService<SupplierEditorViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucCustomerMaster>(provider =>
                new ucCustomerMaster(
                    provider.GetRequiredService<CustomerMasterViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucCustomerEditor>(provider =>
                new ucCustomerEditor(
                    provider.GetRequiredService<CustomerEditorViewModel>(),
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