using Chrome_WPF.Helpers;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.ViewModels;
using Chrome_WPF.Views.UserControls;
using Chrome_WPF.Views.UserControls.BOMMaster;
using Chrome_WPF.Views.UserControls.CustomerMaster;
using Chrome_WPF.Views.UserControls.GroupManagement;
using Chrome_WPF.Views.UserControls.Inventory;
using Chrome_WPF.Views.UserControls.ProductMaster;
using Chrome_WPF.Views.UserControls.SupplierMaster;
using Chrome_WPF.Views.UserControls.WarehouseMaster;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Chrome_WPF.Views
{
    public partial class MainWindow : Window
    {
        private readonly AuthViewModel _authViewModel;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private bool isSidebarCollapsed = false;
        private readonly double sidebarExpandedWidth = 270;
        private readonly double sidebarCollapsedWidth = 60;

        public MainWindow(AuthViewModel authViewModel, INotificationService notificationService, INavigationService navigationService)
        {
            InitializeComponent();
            _authViewModel = authViewModel;
            _notificationService = notificationService;
            _navigationService = navigationService; // Sử dụng thể hiện từ DI
            DataContext = _authViewModel;
            _notificationService.RegisterSnackbar(MainSnackbar);
            // Gán MainContent cho NavigationService
            if (_navigationService is NavigationService navService)
            {
                navService.SetContentControl(MainContent);
            }
        }

        private void ToggleSideBar_Click(object sender, RoutedEventArgs e)
        {
            double targetWidth = isSidebarCollapsed ? sidebarExpandedWidth : sidebarCollapsedWidth;

            var animation = new DoubleAnimation
            {
                To = targetWidth,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            SidebarColumn.BeginAnimation(GridLengthAnimationHelper.AnimatedWidthProperty, animation);

            ToggleTextVisibility(MainMenuListBox);
            ToggleTextVisibility(LogOut);
            LogoText.Visibility = isSidebarCollapsed ? Visibility.Visible : Visibility.Collapsed;

            isSidebarCollapsed = !isSidebarCollapsed;
            ((PackIcon)((Button)sender).Content).Kind = isSidebarCollapsed ? PackIconKind.Menu : PackIconKind.MenuOpen;
        }

        private void ToggleTextVisibility(ListBox listBox)
        {
            if (listBox == null) return;

            // Lấy các mục con cho từng nhóm
            var overviewItems = new List<ListBoxItem>
            {
                listBox.Items.OfType<ListBoxItem>().FirstOrDefault(item => item.Name == "ucDashboard")!
            }.Where(item => item != null).ToList();

            var accountItems = new List<ListBoxItem>
            {
                listBox.Items.OfType<ListBoxItem>().FirstOrDefault(item => item.Name == "ucAccountManagement")!,
                listBox.Items.OfType < ListBoxItem >().FirstOrDefault(item => item.Name == "ucGroupManagement") !
            }.Where(item => item != null).ToList();

            var dataItems = new List<ListBoxItem>
            {
                listBox.Items.OfType<ListBoxItem>().FirstOrDefault(item => item.Name == "ucProductMaster")!,
                listBox.Items.OfType < ListBoxItem >().FirstOrDefault(item => item.Name == "ucSupplierMaster") !,
                listBox.Items.OfType < ListBoxItem >().FirstOrDefault(item => item.Name == "ucCustomerMaster") !,
                listBox.Items.OfType < ListBoxItem >().FirstOrDefault(item => item.Name == "ucWarehouseMaster") !,
                listBox.Items.OfType < ListBoxItem >().FirstOrDefault(item => item.Name == "ucBOMMaster") !
            }.Where(item => item != null).ToList();

            var commandItems = new List<ListBoxItem>
            {
                listBox.Items.OfType < ListBoxItem >().FirstOrDefault(item => item.Name == "ucInventory") ! ,
                listBox.Items.OfType < ListBoxItem >().FirstOrDefault(item => item.Name == "ucStockIn") !,
                listBox.Items.OfType < ListBoxItem >().FirstOrDefault(item => item.Name == "ucStockOut") !,
                listBox.Items.OfType < ListBoxItem >().FirstOrDefault(item => item.Name == "ucTransfer") !,
                listBox.Items.OfType < ListBoxItem >().FirstOrDefault(item => item.Name == "ucMovement") !,
                listBox.Items.OfType<ListBoxItem>().FirstOrDefault(item => item.Name == "ucPickList")!,
                listBox.Items.OfType < ListBoxItem >().FirstOrDefault(item => item.Name == "ucPutAway") !,
                listBox.Items.OfType < ListBoxItem >().FirstOrDefault(item => item.Name == "ucStockTake") !
            }.Where(item => item != null).ToList();

            var productionItems = new List<ListBoxItem>
            {
                listBox.Items.OfType < ListBoxItem >().FirstOrDefault(item => item.Name == "ucProductionOrder") !
            }.Where(item => item != null).ToList();

            // Kiểm tra trạng thái hiển thị của các mục con
            bool hasVisibleOverviewItem = overviewItems.Any(item => item.Visibility == Visibility.Visible);
            bool hasVisibleAccountItem = accountItems.Any(item => item.Visibility == Visibility.Visible);
            bool hasVisibleDataItem = dataItems.Any(item => item.Visibility == Visibility.Visible);
            bool hasVisibleCommandItem = commandItems.Any(item => item.Visibility == Visibility.Visible);
            bool hasVisibleProductionItem = productionItems.Any(item => item.Visibility == Visibility.Visible);

            foreach (var item in listBox.Items)
            {
                if (item is ListBoxItem listBoxItem)
                {
                    if (listBoxItem.Style == (Style)FindResource("TitleItemStyle"))
                    {
                        // Xử lý từng tiêu đề
                        if (listBoxItem.Name == "OverviewTitle")
                        {
                            listBoxItem.Visibility = hasVisibleOverviewItem
                                ? (isSidebarCollapsed ? Visibility.Visible : Visibility.Collapsed)
                                : Visibility.Collapsed;
                        }
                        else if (listBoxItem.Name == "AccountTitle")
                        {
                            listBoxItem.Visibility = hasVisibleAccountItem
                                ? (isSidebarCollapsed ? Visibility.Visible : Visibility.Collapsed)
                                : Visibility.Collapsed;
                        }
                        else if (listBoxItem.Name == "MasterDataTitle")
                        {
                            listBoxItem.Visibility = hasVisibleDataItem
                                ? (isSidebarCollapsed ? Visibility.Visible : Visibility.Collapsed)
                                : Visibility.Collapsed;
                        }
                        else if (listBoxItem.Name == "CommandTitle")
                        {
                            listBoxItem.Visibility = hasVisibleCommandItem
                                ? (isSidebarCollapsed ? Visibility.Visible : Visibility.Collapsed)
                                : Visibility.Collapsed;
                        }
                        else if (listBoxItem.Name == "ProductionTitle")
                        {
                            listBoxItem.Visibility = hasVisibleProductionItem
                                ? (isSidebarCollapsed ? Visibility.Visible : Visibility.Collapsed)
                                : Visibility.Collapsed;
                        }
                        else
                        {
                            // Các tiêu đề không xác định (nếu có)
                            listBoxItem.Visibility = isSidebarCollapsed ? Visibility.Visible : Visibility.Collapsed;
                        }
                    }
                    else if (listBoxItem.Style == (Style)FindResource("SeparatorItemStyle"))
                    {
                        // Xử lý thanh phân cách
                        bool hasVisibleAdjacentItems = false;
                        int itemIndex = listBox.Items.IndexOf(item);
                        if (itemIndex > 0 && itemIndex < listBox.Items.Count - 1)
                        {
                            var prevItem = listBox.Items[itemIndex - 1] as ListBoxItem;
                            var nextItem = listBox.Items[itemIndex + 1] as ListBoxItem;
                            hasVisibleAdjacentItems = (prevItem?.Visibility == Visibility.Visible || nextItem?.Visibility == Visibility.Visible);
                        }
                        listBoxItem.Visibility = hasVisibleAdjacentItems
                            ? (isSidebarCollapsed ? Visibility.Visible : Visibility.Collapsed)
                            : Visibility.Collapsed;
                    }
                    else if (listBoxItem.Content is StackPanel stackPanel)
                    {
                        foreach (var child in stackPanel.Children)
                        {
                            if (child is TextBlock textBlock)
                            {
                                textBlock.Visibility = isSidebarCollapsed ? Visibility.Visible : Visibility.Collapsed;
                            }
                        }
                    }
                }
            }
        }

        private void MainMenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainMenuListBox.SelectedItem is ListBoxItem selectedItem)
            {
                switch (selectedItem.Name)
                {
                    case "ucAccountManagement":
                        _navigationService.NavigateTo<ucAccountManagement>();
                        break;
                    case "ucGroupManagement":
                        _navigationService.NavigateTo<ucGroupManagement>();
                        break;
                    case "ucProductMaster":
                        _navigationService.NavigateTo<ucProductMaster>();
                        break;
                    case "ucSupplierMaster":
                        _navigationService.NavigateTo<ucSupplierMaster>();
                        break;
                    case "ucCustomerMaster":
                        _navigationService.NavigateTo<ucCustomerMaster>();
                        break;
                    case "ucWarehouseMaster":
                        _navigationService.NavigateTo<ucWarehouseMaster>();
                        break;
                    case "ucBOMMaster":
                        _navigationService.NavigateTo<ucBOMMaster>();
                        break;
                    case "ucInventory":
                        _navigationService.NavigateTo<ucInventory>();
                        break;
                    default:
                        break;
                }
            }
        }

        private void LogOut_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0 || !(e.AddedItems[0] is ListBoxItem selectedItem))
                return;

            if (selectedItem.Name != "LogOutItem")
                return;


            try
            {
                // Xóa thông tin đăng nhập
                Properties.Settings.Default.AccessToken = string.Empty;
                Properties.Settings.Default.UserName = string.Empty;
                Properties.Settings.Default.FullName = string.Empty;
                Properties.Settings.Default.Role = string.Empty;
                Properties.Settings.Default.WarehousePermission = new StringCollection();
                Properties.Settings.Default.Save();

                // Đóng các cửa sổ khác
                foreach (Window window in Application.Current.Windows)
                {
                    if (window != this && window.IsVisible)
                    {
                        try
                        {
                            window.Close();
                        }
                        catch (InvalidOperationException ex)
                        {
                            _notificationService.ShowMessage(
                                $"Không thể đóng cửa sổ: {ex.Message}",
                                "OK",
                                isError: true);
                        }
                    }
                }

                // Reset ServiceProvider để tạo mới các dịch vụ
                App.ResetServiceProvider();

                // Hiển thị LoginWindow với ServiceProvider mới
                var loginWindow = App.ServiceProvider!.GetRequiredService<LoginWindow>();
                loginWindow.Show();

                // Đóng cửa sổ hiện tại
                Close();
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage(
                    $"Lỗi khi đăng xuất: {ex.Message}",
                    "OK",
                    isError: true);
                LogOut.SelectedItem = null; // Bỏ chọn mục
            }
        }

    }

}