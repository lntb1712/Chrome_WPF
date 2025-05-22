using Chrome_WPF.Helpers;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Chrome_WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AuthViewModel _authViewModel;
        private readonly INotificationService _notificationService;
        private bool isSidebarCollapsed = false;
        private readonly double sidebarExpandedWidth = 270;
        private readonly double sidebarCollapsedWidth = 60;
        public MainWindow(AuthViewModel authViewModel,INotificationService notificationService)
        {
            InitializeComponent();
            _authViewModel = authViewModel;
            _notificationService = notificationService;
            DataContext = _authViewModel;
            _notificationService.RegisterSnackbar(MainSnackbar);
        }
        private void ToggleSideBar_Click(object sender, RoutedEventArgs e)
        {
            double targetWidth = isSidebarCollapsed ? sidebarExpandedWidth : sidebarCollapsedWidth;

            // Tạo animation cho attached property AnimatedWidth
            var animation = new DoubleAnimation
            {
                To = targetWidth,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            SidebarColumn.BeginAnimation(GridLengthAnimationHelper.AnimatedWidthProperty, animation);

            // Ẩn/hiện text trong MainMenuListBox
            ToggleTextVisibility(MainMenuListBox);

            // Ẩn/hiện text trong SettingsListBox
            ToggleTextVisibility(SettingsListBox);

            // Ẩn/hiện text của logo
            LogoText.Visibility = isSidebarCollapsed ? Visibility.Visible : Visibility.Collapsed;

            // Cập nhật trạng thái sidebar và icon nút
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
                    // Xử lý các ListBoxItem sử dụng TitleItemStyle
                    if (listBoxItem.Style == (Style)FindResource("TitleItemStyle"))
                    {
                        listBoxItem.Visibility = isSidebarCollapsed ? Visibility.Visible : Visibility.Collapsed;
                    }
                    // Xử lý các ListBoxItem chứa StackPanel (menu items)
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

    }
}
