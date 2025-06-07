using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.ViewModels.SupplierMasterViewModel;
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

namespace Chrome_WPF.Views.UserControls.SupplierMaster
{
    /// <summary>
    /// Interaction logic for ucSupplierEditor.xaml
    /// </summary>
    public partial class ucSupplierEditor : UserControl
    {
        private readonly SupplierEditorViewModel _viewModel;
        private readonly INotificationService _notificationService;
        public ucSupplierEditor(SupplierEditorViewModel viewModel, INotificationService notificationService)
        {
            InitializeComponent();
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            DataContext = _viewModel;
            _notificationService.RegisterSnackbar(SupplierEditorSnackbar);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ucSupplierMaster ucSupplierMaster = App.ServiceProvider!.GetRequiredService<ucSupplierMaster>();

            var mainWindow = Window.GetWindow(this);
            var mainContent = mainWindow.FindName("MainContent") as ContentControl;
            if (mainContent != null)
            {
                mainContent.Content = ucSupplierMaster;
            }
        } 
    }
}
