using Chrome_WPF.Models.GroupFunctionDTO;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.ViewModels;
using Chrome_WPF.ViewModels.GroupManagementViewModel;
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

namespace Chrome_WPF.Views.UserControls.GroupManagement
{
    /// <summary>
    /// Interaction logic for ucGroupEditor.xaml
    /// </summary>
    public partial class ucGroupEditor : UserControl
    {
        private readonly GroupEditorViewModel _viewModel;
        private readonly INotificationService _notificationService;
        public ucGroupEditor(GroupEditorViewModel viewModel,INotificationService notificationService)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _notificationService = notificationService;
            DataContext = _viewModel;
            _notificationService.RegisterSnackbar(GroupEditorSnackbar);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ucGroupManagement groupManagement = App.ServiceProvider!.GetRequiredService<ucGroupManagement>();
            // Gán lại nội dung của MainContent trong MainWindow
            var mainWindow = Window.GetWindow(this);
            var mainContent = mainWindow.FindName("MainContent") as ContentControl;
            if (mainContent != null)
            {
                mainContent.Content = groupManagement;
            }
        }

        
    }
}
