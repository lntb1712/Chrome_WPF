using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.ViewModels.CustomerMasterViewModel;
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

namespace Chrome_WPF.Views.UserControls.CustomerMaster
{
    /// <summary>
    /// Interaction logic for ucCustomerMaster.xaml
    /// </summary>

    public partial class ucCustomerMaster : UserControl
    {
        private readonly CustomerMasterViewModel _viewModel;
        private readonly INotificationService _notificationService;
        public ucCustomerMaster(CustomerMasterViewModel viewModel, INotificationService notificationService)
        {
            InitializeComponent();
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            DataContext = _viewModel;
            _notificationService.RegisterSnackbar(CustomerManagementSnackbar);
           
        }
    }
}
