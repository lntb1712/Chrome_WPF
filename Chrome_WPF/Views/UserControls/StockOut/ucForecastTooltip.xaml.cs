using Chrome_WPF.Models.StockOutDetailDTO;
using Chrome_WPF.ViewModels.StockOutViewModel;
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

namespace Chrome_WPF.Views.UserControls.StockOut
{
    /// <summary>
    /// Interaction logic for ucForecastTooltip.xaml
    /// </summary>
    public partial class ucForecastTooltip : UserControl
    {
        private readonly ForecastTooltipViewModel _viewModel;
        public ucForecastTooltip(ForecastTooltipViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        
    }
}
