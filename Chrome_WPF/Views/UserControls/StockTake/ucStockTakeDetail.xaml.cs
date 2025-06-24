using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.ViewModels.StockTakeViewModel;
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

namespace Chrome_WPF.Views.UserControls.StockTake
{
    /// <summary>
    /// Interaction logic for ucStockTakeDetail.xaml
    /// </summary>
    public partial class ucStockTakeDetail : UserControl
    {
        private readonly StockTakeDetailViewModel _viewModel;
        private readonly INotificationService _notificationService;
        public ucStockTakeDetail(StockTakeDetailViewModel viewModel, INotificationService notificationService)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _notificationService = notificationService;
            DataContext = _viewModel;
            _notificationService.RegisterSnackbar(StockTakeDetailSnackbar);
        }
    }
}
