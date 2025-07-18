using Chrome.Services.ManufacturingOrderService;
using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Services.AccountManagementService;
using Chrome_WPF.Services.AuthServices;
using Chrome_WPF.Services.BOMComponentService;
using Chrome_WPF.Services.BOMMasterService;
using Chrome_WPF.Services.CategoryService;
using Chrome_WPF.Services.CodeGeneratorService;
using Chrome_WPF.Services.CustomerMasterService;
using Chrome_WPF.Services.DashboardService;
using Chrome_WPF.Services.GroupManagementService;
using Chrome_WPF.Services.InventoryService;
using Chrome_WPF.Services.LocationMasterService;
using Chrome_WPF.Services.LoginServices;
using Chrome_WPF.Services.ManufacturingOrderDetailService;
using Chrome_WPF.Services.ManufacturingOrderService;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.MovementDetailService;
using Chrome_WPF.Services.MovementService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.PickListDetailService;
using Chrome_WPF.Services.PickListService;
using Chrome_WPF.Services.ProductCustomerService;
using Chrome_WPF.Services.ProductMasterService;
using Chrome_WPF.Services.ProductSupplierService;
using Chrome_WPF.Services.PurchaseOrderDetailService;
using Chrome_WPF.Services.PurchaseOrderService;
using Chrome_WPF.Services.PutAwayDetailService;
using Chrome_WPF.Services.PutAwayRulesService;
using Chrome_WPF.Services.PutAwayService;
using Chrome_WPF.Services.ReplenishService;
using Chrome_WPF.Services.ReservationDetailService;
using Chrome_WPF.Services.ReservationService;
using Chrome_WPF.Services.StockInDetailService;
using Chrome_WPF.Services.StockInService;
using Chrome_WPF.Services.StockOutDetailService;
using Chrome_WPF.Services.StockOutService;
using Chrome_WPF.Services.StockTakeDetailService;
using Chrome_WPF.Services.StockTakeService;
using Chrome_WPF.Services.StorageProductService;
using Chrome_WPF.Services.SupplierMasterService;
using Chrome_WPF.Services.TransferDetailService;
using Chrome_WPF.Services.TransferService;
using Chrome_WPF.Services.WarehouseMasterService;
using Chrome_WPF.ViewModels;
using Chrome_WPF.ViewModels.BOMMasterViewModel;
using Chrome_WPF.ViewModels.CustomerMasterViewModel;
using Chrome_WPF.ViewModels.GroupManagementViewModel;
using Chrome_WPF.ViewModels.InventoryViewModel;
using Chrome_WPF.ViewModels.MainWindowViewModel;
using Chrome_WPF.ViewModels.ManufacturingOrderViewModel;
using Chrome_WPF.ViewModels.MovementViewModel;
using Chrome_WPF.ViewModels.PickListViewModel;
using Chrome_WPF.ViewModels.ProductMasterViewModel;
using Chrome_WPF.ViewModels.PurchaseOrderViewModel;
using Chrome_WPF.ViewModels.PutAwayViewModel;
using Chrome_WPF.ViewModels.ReservationViewModel;
using Chrome_WPF.ViewModels.StockInViewModel;
using Chrome_WPF.ViewModels.StockOutViewModel;
using Chrome_WPF.ViewModels.StockTakeViewModel;
using Chrome_WPF.ViewModels.SupplierMasterViewModel;
using Chrome_WPF.ViewModels.TransferViewModel;
using Chrome_WPF.ViewModels.WarehouseMasterViewModel;
using Chrome_WPF.Views;
using Chrome_WPF.Views.UserControls;
using Chrome_WPF.Views.UserControls.AccountManagement;
using Chrome_WPF.Views.UserControls.BOMMaster;
using Chrome_WPF.Views.UserControls.CustomerMaster;
using Chrome_WPF.Views.UserControls.Dashboard;
using Chrome_WPF.Views.UserControls.GroupManagement;
using Chrome_WPF.Views.UserControls.Inventory;
using Chrome_WPF.Views.UserControls.ManufacturingOrder;
using Chrome_WPF.Views.UserControls.Movement;
using Chrome_WPF.Views.UserControls.PickList;
using Chrome_WPF.Views.UserControls.ProductMaster;
using Chrome_WPF.Views.UserControls.PurchaseOrder;
using Chrome_WPF.Views.UserControls.PutAway;
using Chrome_WPF.Views.UserControls.Replenish;
using Chrome_WPF.Views.UserControls.Reservation;
using Chrome_WPF.Views.UserControls.StockIn;
using Chrome_WPF.Views.UserControls.StockOut;
using Chrome_WPF.Views.UserControls.StockTake;
using Chrome_WPF.Views.UserControls.SupplierMaster;
using Chrome_WPF.Views.UserControls.Transfer;
using Chrome_WPF.Views.UserControls.WarehouseMaster;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Chrome_WPF
{
    public partial class App : Application, IDisposable
    {
        private static IServiceProvider? _serviceProvider;
        private static ServiceCollection? _services;
        private bool _disposed;

        public static IServiceProvider? ServiceProvider
        {
            get => _serviceProvider;
            private set => _serviceProvider = value;
        }

        public App()
        {
            InitializeComponent();
            InitializeServiceProvider();
        }

        private void InitializeServiceProvider()
        {
            _services = new ServiceCollection();
            ConfigureServices(_services);
            ServiceProvider = _services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // Register API_Constant
            services.AddSingleton<API_Constant>();
            services.AddMemoryCache();

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
            services.AddSingleton<IProductCustomerService, ProductCustomerService>();
            services.AddSingleton<IWarehouseMasterService, WarehouseMasterService>();
            services.AddSingleton<ILocationMasterService, LocationMasterService>();
            services.AddSingleton<IStorageProductService, StorageProductService>();
            services.AddSingleton<IPutAwayRulesService, PutAwayRulesService>();
            services.AddSingleton<IBOMComponentService, BOMComponentService>();
            services.AddSingleton<IBOMMasterService, BOMMasterService>();
            services.AddSingleton<IInventoryService, InventoryService>();
            services.AddSingleton<IStockInService, StockInService>();
            services.AddSingleton<IStockInDetailService, StockInDetailService>();
            services.AddSingleton<IStockOutService, StockOutService>();
            services.AddSingleton<IStockOutDetailService, StockOutDetailService>();
            services.AddSingleton<IReservationService, ReservationService>();
            services.AddSingleton<IReservationDetailService,ReservationDetailService>();
            services.AddSingleton<IPickListService, PickListService>();
            services.AddSingleton<IPickListDetailService, PickListDetailService>();
            services.AddSingleton<IPutAwayService, PutAwayService>();
            services.AddSingleton<IPutAwayDetailService, PutAwayDetailService>();
            services.AddSingleton<IMovementService, MovementService>();
            services.AddSingleton<IMovementDetailService, MovementDetailService>();
            services.AddSingleton<ITransferService, TransferService>();
            services.AddSingleton<ITransferDetailService, TransferDetailService>();
            services.AddSingleton<IStockTakeService, StockTakeService>();
            services.AddSingleton<IStockTakeDetailService, StockTakeDetailService>();
            services.AddSingleton<IManufacturingOrderService,ManufacturingOrderService>();
            services.AddSingleton<IManufacturingOrderDetailService, ManufacturingOrderDetailService>();
            services.AddSingleton<IDashboardService ,DashboardService>();
            services.AddSingleton<IReplenishService, ReplenishService>();
            services.AddSingleton<IPurchaseOrderService, PurchaseOrderService>();
            services.AddSingleton<IPurchaseOrderDetailService, PurchaseOrderDetailService>();
            services.AddSingleton<ICodeGenerateService, CodeGeneratorService>();


            // Register IServiceProvider
            services.AddSingleton(sp => sp);

            // Register NavigationService with ContentControl and IServiceProvider
            services.AddSingleton<INavigationService>(provider =>
            {
                var serviceProvider = provider.GetRequiredService<IServiceProvider>();
                return new NavigationService(null!, serviceProvider);
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
            services.AddTransient<WarehouseMasterViewModel>();
            services.AddTransient<WarehouseEditorViewModel>();
            services.AddTransient<StorageProductViewModel>();
            services.AddTransient<PutAwayRulesViewModel>();
            services.AddTransient<BOMMasterViewModel>();
            services.AddTransient<BOMComponentViewModel>();
            services.AddTransient<BOMPreviewViewModel>();
            services.AddTransient<BOMNodeViewModel>();
            services.AddTransient<InventoryViewModel>();
            services.AddTransient<InventoryDetailViewModel>();
            services.AddTransient<StockInViewModel>();
            services.AddTransient<StockInDetailViewModel>();
            services.AddTransient<ViewModels.StockInViewModel.BackOrderDialogViewModel>();
            services.AddTransient<StockOutViewModel>();
            services.AddTransient<StockOutDetailViewModel>();
            services.AddTransient<ViewModels.StockOutViewModel.BackOrderDialogViewModel>();
            services.AddTransient<ReservationViewModel>();
            services.AddTransient<ReservationDetailViewModel>();
            services.AddTransient<PickListViewModel>();
            services.AddTransient<PickListDetailViewModel>();
            services.AddTransient<PutAwayViewModel>();
            services.AddTransient<PutAwayDetailViewModel>();
            services.AddTransient<MovementViewModel>();
            services.AddTransient<MovementDetailViewModel>();
            services.AddTransient<TransferViewModel>();
            services.AddTransient<TransferDetailViewModel>();
            services.AddTransient<StockTakeViewModel>();
            services.AddTransient<StockTakeDetailViewModel>();
            services.AddTransient<ManufacturingOrderViewModel>();
            services.AddTransient<ManufacturingOrderDetailViewModel>();
            services.AddTransient<ViewModels.ManufacturingOrderViewModel.BackOrderDialogViewModel>();
            services.AddTransient<ForecastTooltipViewModel>();
            services.AddTransient<ForecastManufacturingTooltipViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<ReplenishViewModel>();
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<PurchaseOrderViewModel>();
            services.AddTransient<PurchaseOrderDetailViewModel>();
            services.AddTransient<ViewModels.PurchaseOrderViewModel.BackOrderDialogViewModel>();

            // Register Views
            services.AddTransient<LoginWindow>(provider =>
                new LoginWindow(
                    provider.GetRequiredService<LoginViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<MainWindow>(provider =>
                new MainWindow(
                    provider.GetRequiredService<AuthViewModel>(),
                    provider.GetRequiredService<INotificationService>(),
                    provider.GetRequiredService<INavigationService>(),
                    provider.GetRequiredService<MainWindowViewModel>()));

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

            services.AddTransient<ucWarehouseMaster>(provider =>
                new ucWarehouseMaster(
                    provider.GetRequiredService<WarehouseMasterViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucWarehouseEditor>(provider =>
                new ucWarehouseEditor(
                    provider.GetRequiredService<WarehouseEditorViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucStorageProduct>(provider =>
                new ucStorageProduct(
                    provider.GetRequiredService<StorageProductViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucPutAwayRules>(provider =>
                new ucPutAwayRules(
                    provider.GetRequiredService<PutAwayRulesViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucBOMMaster>(provider =>
                new ucBOMMaster(
                    provider.GetRequiredService<BOMMasterViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucBOMComponent>(provider =>
                new ucBOMComponent(
                    provider.GetRequiredService<BOMComponentViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucBOMPreview>(provider =>
                new ucBOMPreview(
                    provider.GetRequiredService<BOMPreviewViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucInventory>(provider =>
                new ucInventory(
                    provider.GetRequiredService<InventoryViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucInventoryDetail>(provider =>
                new ucInventoryDetail(
                    provider.GetRequiredService<InventoryDetailViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucStockIn>(provider =>
                new ucStockIn(
                    provider.GetRequiredService<StockInViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucStockInDetail>(provider =>
                new ucStockInDetail(
                    provider.GetRequiredService<StockInDetailViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<Views.UserControls.StockIn.BackOrderDialog>(provider =>
                new Views.UserControls.StockIn.BackOrderDialog(
                    provider.GetRequiredService<ViewModels.StockInViewModel.BackOrderDialogViewModel>()));

            services.AddTransient<ucStockOut>(provider =>
                new ucStockOut(
                    provider.GetRequiredService<StockOutViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucStockOutDetail>(provider =>
                new ucStockOutDetail(
                    provider.GetRequiredService<StockOutDetailViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<Views.UserControls.StockOut.BackOrderDialog>(provider =>
                new Views.UserControls.StockOut.BackOrderDialog(
                    provider.GetRequiredService<ViewModels.StockOutViewModel.BackOrderDialogViewModel>()));

            services.AddTransient<ucReservation>(provider =>
                new ucReservation(
                    provider.GetRequiredService<ReservationViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucReservationDetail>(provider =>
                new ucReservationDetail(
                    provider.GetRequiredService<ReservationDetailViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucPickList>(provider =>
                new ucPickList(
                    provider.GetRequiredService<PickListViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucPickListDetail>(provider =>
                new ucPickListDetail(
                    provider.GetRequiredService<PickListDetailViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucPutAway>(provider =>
                new ucPutAway(
                    provider.GetRequiredService<PutAwayViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucPutAwayDetail>(provider =>
                 new ucPutAwayDetail(
                     provider.GetRequiredService<PutAwayDetailViewModel>(),
                     provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucMovement>(provider =>
                 new ucMovement(
                     provider.GetRequiredService<MovementViewModel>(),
                     provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucMovementDetail>(provider =>
                 new ucMovementDetail(
                     provider.GetRequiredService<MovementDetailViewModel>(),
                     provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucTransfer>(provider =>
                  new ucTransfer(
                      provider.GetRequiredService<TransferViewModel>(),
                      provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucTransferDetail>(provider =>
                 new ucTransferDetail(
                     provider.GetRequiredService<TransferDetailViewModel>(),
                     provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucStockTake>(provider =>
                new ucStockTake(
                    provider.GetRequiredService<StockTakeViewModel>(),
                    provider.GetRequiredService<INotificationService>()));
            services.AddTransient<ucStockTakeDetail>(provider =>
               new ucStockTakeDetail(
                   provider.GetRequiredService<StockTakeDetailViewModel>(),
                   provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucManufacturingOrder>(provider =>
                new ucManufacturingOrder(
                    provider.GetRequiredService<ManufacturingOrderViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucManufacturingOrderDetail>(provider =>
                new ucManufacturingOrderDetail(
                    provider.GetRequiredService<ManufacturingOrderDetailViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<Views.UserControls.ManufacturingOrder.BackOrderDialog>(provider =>
                new Views.UserControls.ManufacturingOrder.BackOrderDialog(
                    provider.GetRequiredService<ViewModels.ManufacturingOrderViewModel.BackOrderDialogViewModel>()));

            services.AddTransient<ucForecastTooltip>(provider =>
                new ucForecastTooltip(provider.GetRequiredService<ForecastTooltipViewModel>()));

            services.AddTransient<ucForecastManufacturingTooltip>(provider =>
                new ucForecastManufacturingTooltip(provider.GetRequiredService<ForecastManufacturingTooltipViewModel>()));

            services.AddTransient<ucDashboard>(provider =>
                new ucDashboard(
                    provider.GetRequiredService<DashboardViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucReplenish>(provider =>
                new ucReplenish(
                    provider.GetRequiredService<ReplenishViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucPurchaseOrder>(provider =>
                new ucPurchaseOrder(
                    provider.GetRequiredService<PurchaseOrderViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<ucPurchaseOrderDetail>(provider =>
                new ucPurchaseOrderDetail(
                    provider.GetRequiredService<PurchaseOrderDetailViewModel>(),
                    provider.GetRequiredService<INotificationService>()));

            services.AddTransient<Views.UserControls.PurchaseOrder.BackOrderDialog>(provider =>
                new Views.UserControls.PurchaseOrder.BackOrderDialog(
                    provider.GetRequiredService<ViewModels.PurchaseOrderViewModel.BackOrderDialogViewModel>()));

        }


        public static void ResetServiceProvider()
        {
            // Dispose ServiceProvider hiện tại nếu nó tồn tại
            if (_serviceProvider is IDisposable disposableProvider)
            {
                disposableProvider.Dispose();
            }

            // Xóa ServiceProvider và ServiceCollection cũ
            _serviceProvider = null;
            _services?.Clear();
            _services = null;

            // Tạo mới ServiceCollection và ServiceProvider
            _services = new ServiceCollection();
            var app = Current as App;
            app?.ConfigureServices(_services);
            _serviceProvider = _services.BuildServiceProvider();
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

        protected override void OnExit(ExitEventArgs e)
        {
            Dispose();
            base.OnExit(e);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Dispose ServiceProvider
                if (_serviceProvider is IDisposable disposableProvider)
                {
                    disposableProvider.Dispose();
                }

                // Clear ServiceProvider and ServiceCollection
                _serviceProvider = null;
                _services?.Clear();
                _services = null;
            }

            _disposed = true;
        }

        ~App()
        {
            Dispose(false);
        }
    }
}