using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
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

namespace Chrome_WPF.Views.UserControls.ProductMaster
{
    /// <summary>
    /// Interaction logic for ucProductDetail.xaml
    /// </summary>
    public partial class ucProductDetail : UserControl
    {
        private readonly INotificationService _notificationService;
        public ucProductDetail(INotificationService notificationService)
        {
            InitializeComponent();
            DataContext = App.ServiceProvider!.GetRequiredService<ProductDetailViewModel>();
            _notificationService = notificationService;
            _notificationService.RegisterSnackbar(ProductDetailSnackbar);
        }

    }
}
