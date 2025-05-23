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

namespace Chrome_WPF.Views.UserControls.AccountManagement
{
    /// <summary>
    /// Interaction logic for ucAccountEditor.xaml
    /// </summary>
    public partial class ucAccountEditor : UserControl
    {
        public ucAccountEditor()
        {
            InitializeComponent();
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
    }
}
