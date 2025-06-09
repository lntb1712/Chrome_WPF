using Chrome_WPF.Helpers;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.ViewModels;
using Chrome_WPF.Views.UserControls;
using Chrome_WPF.Views.UserControls.CustomerMaster;
using Chrome_WPF.Views.UserControls.GroupManagement;
using Chrome_WPF.Views.UserControls.ProductMaster;
using Chrome_WPF.Views.UserControls.SupplierMaster;
using Chrome_WPF.Views.UserControls.WarehouseMaster;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

            foreach (var item in listBox.Items)
            {
                if (item is ListBoxItem listBoxItem)
                {
                    if (listBoxItem.Style == (Style)FindResource("TitleItemStyle"))
                    {
                        listBoxItem.Visibility = isSidebarCollapsed ? Visibility.Visible : Visibility.Collapsed;
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
                    default:
                        break;
                }
            }
        }

        private void LogOut_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if( LogOut.SelectedItem is ListBoxItem selectedItem )
            {
                switch (selectedItem.Name)
                {
                    case "LogOutItem":
                        Properties.Settings.Default.AccessToken = string.Empty;
                        Properties.Settings.Default.UserName = string.Empty;
                        Properties.Settings.Default.FullName = string.Empty;
                        Properties.Settings.Default.Role = string.Empty;
                        Properties.Settings.Default.Save();
                        var loginWindow = App.ServiceProvider!.GetRequiredService<LoginWindow>();
                        loginWindow.Show();
                        Close();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}