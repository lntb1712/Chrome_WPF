using Chrome_WPF.Models;
using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;


namespace Chrome_WPF.Views.UserControls.AccountManagement
{
    /// <summary>
    /// Interaction logic for ucAccountEditor.xaml
    /// </summary>
    public partial class ucAccountEditor : UserControl
    {
        private readonly AccountEditorViewModel _viewModel;
        private readonly INotificationService _notificationService;
        public ucAccountEditor(AccountEditorViewModel viewModel,INotificationService notificationService)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _notificationService = notificationService;
            DataContext = _viewModel;
            _notificationService.RegisterSnackbar(AccountEditorSnackbar);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ucAccountManagement accountManagement = App.ServiceProvider!.GetRequiredService<ucAccountManagement>();
            // Gán lại nội dung của MainContent trong MainWindow
            var mainWindow = Window.GetWindow(this);
            var mainContent = mainWindow.FindName("MainContent") as ContentControl;
            if (mainContent != null)
            {
                mainContent.Content = accountManagement;
            }
        }

        private void PasswordField_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox && DataContext is AccountEditorViewModel viewModel)
            {
                viewModel.AccountManagementRequestDTO.Password = passwordBox.Password;
            }
        }
    }
}
