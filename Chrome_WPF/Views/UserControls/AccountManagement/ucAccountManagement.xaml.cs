using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chrome_WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for ucAccountManagement.xaml
    /// </summary>
    public partial class ucAccountManagement : UserControl
    {
        private readonly AccountManagementViewModel _accountManagementViewModel;
        private readonly INotificationService _notificationService;
        public ucAccountManagement(AccountManagementViewModel accountManagementViewModel, INotificationService notificationService)
        {
            InitializeComponent();
            _accountManagementViewModel = accountManagementViewModel;
            _notificationService = notificationService;
            DataContext = _accountManagementViewModel;
            _notificationService.RegisterSnackbar(AccountManagementSnackbar);
        }

        private void MoreActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.ContextMenu != null)
            {
                button.ContextMenu.IsOpen = true;
            }
        }
    }
}
