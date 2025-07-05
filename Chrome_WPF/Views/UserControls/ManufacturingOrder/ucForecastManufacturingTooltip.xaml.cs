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

namespace Chrome_WPF.ViewModels.ManufacturingOrderViewModel
{
    /// <summary>
    /// Interaction logic for ucForecastManufacturingTooltip.xaml
    /// </summary>
    public partial class ucForecastManufacturingTooltip : UserControl
    {
        private readonly ForecastManufacturingTooltipViewModel _viewModel;
        public ucForecastManufacturingTooltip(ForecastManufacturingTooltipViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
