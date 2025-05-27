using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.ViewModels.GroupManagementViewModel;
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

namespace Chrome_WPF.Views.UserControls.GroupManagement
{
    /// <summary>
    /// Interaction logic for ucGroupManagement.xaml
    /// </summary>
    public partial class ucGroupManagement : UserControl
    {
        private readonly INotificationService _notificationService;
        private readonly GroupManagementViewModel _viewModel;
        public ucGroupManagement(INotificationService notificationService, GroupManagementViewModel viewModel)
        {
            InitializeComponent();
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            DataContext = _viewModel;
            _notificationService.RegisterSnackbar(GroupManagementSnackbar);
        }
    }
}
