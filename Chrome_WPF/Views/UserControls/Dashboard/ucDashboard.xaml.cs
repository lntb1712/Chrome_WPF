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
using System.Windows.Threading;

namespace Chrome_WPF.Views.UserControls.Dashboard
{
    /// <summary>
    /// Interaction logic for ucDashboard.xaml
    /// </summary>
    public partial class ucDashboard : UserControl
    {
        private readonly DispatcherTimer _refreshTimer = new DispatcherTimer();
        private readonly DashboardViewModel _viewModel;
        private readonly INotificationService _notificationService;
        public ucDashboard(DashboardViewModel viewModel, INotificationService notificationService)
        {
            InitializeComponent();
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            DataContext= _viewModel;
            _refreshTimer.Interval = TimeSpan.FromSeconds(30);
            _refreshTimer.Tick += async (s, e) => await _viewModel.LoadDashboardDataAsync();
            _refreshTimer.Start();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 1400)
                VisualStateManager.GoToState(this, "SmallScreen", true);
            else
                VisualStateManager.GoToState(this, "LargeScreen", true);
        }
    }
}
