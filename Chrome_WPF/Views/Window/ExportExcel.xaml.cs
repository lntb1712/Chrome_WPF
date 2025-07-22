using Chrome_WPF.ViewModels.ExportExcelViewModels;
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

namespace Chrome_WPF.Views.Windows
{
    /// <summary>
    /// Interaction logic for ExportExcel.xaml
    /// </summary>
    public partial class ExportExcel : Window
    {
        private readonly ExportExcelViewModel _viewModel;
        public ExportExcel(ExportExcelViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
