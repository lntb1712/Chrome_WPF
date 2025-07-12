using Chrome_WPF.ViewModels.PurchaseOrderViewModel;
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
using System.Windows.Shapes;

namespace Chrome_WPF.Views.UserControls.PurchaseOrder
{
    /// <summary>
    /// Interaction logic for BackOrderDialog.xaml
    /// </summary>
    public partial class BackOrderDialog : Window
    {
        private readonly BackOrderDialogViewModel _viewModel;
        public BackOrderDialog(BackOrderDialogViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            DataContext = _viewModel;
        }
    }
}
