using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.ViewModels.WarehouseMasterViewModel;
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

namespace Chrome_WPF.Views.UserControls.Replenish
{
    /// <summary>
    /// Interaction logic for ucReplenish.xaml
    /// </summary>
    public partial class ucReplenish : UserControl
    {
        private readonly ReplenishViewModel _viewModel;
        private readonly INotificationService _notificationService;
        public ucReplenish(ReplenishViewModel viewModel, INotificationService notificationService)
        {
            InitializeComponent();
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            DataContext = _viewModel;
            _notificationService.RegisterSnackbar(ReplenishSnackbar);
        }
    }
}
