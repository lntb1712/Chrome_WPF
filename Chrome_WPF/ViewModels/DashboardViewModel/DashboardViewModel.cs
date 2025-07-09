using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.DashboardDTO;
using Chrome_WPF.Models.InventoryDTO;
using Chrome_WPF.Services.DashboardService;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Chrome_WPF.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IDashboardService _dashboardService;

        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private SeriesCollection inventoryPieSeries;
        [ObservableProperty] private SeriesCollection progressBarSeries;
        [ObservableProperty] private string? progressBarMessage;

        [ObservableProperty] private int quantityToCompleteStockIn;
        [ObservableProperty] private int quantityToCompleteStockOut;
        [ObservableProperty] private int quantityToCompleteManufacturingOrder;

        [ObservableProperty] private string? upcomingDeadlinesWarning;

        [ObservableProperty] private SeriesCollection statusBarSeries;
        [ObservableProperty] private string[] statusBarLabels;

        [ObservableProperty] private SeriesCollection dailyStockSeries;
        [ObservableProperty] private string[] dailyStockLabels;
        private List<string> _progressBarLabels;
        public List<string> ProgressBarLabels
        {
            get => _progressBarLabels;
            set
            {
                _progressBarLabels = value;
                OnPropertyChanged(nameof(ProgressBarLabels));
            }
        }
        private string _applicableWarehouseCodes = string.Empty;
        private List<string> _warehouseCodes;
        public string ApplicableWarehouseCodes
        {
            get => _applicableWarehouseCodes;
            set
            {
                _applicableWarehouseCodes = value;
                OnPropertyChanged();
            }
        }
        public DashboardViewModel(IDashboardService dashboardService, API_Constant apiConstant)
        {
            _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));

            InventoryPieSeries = new SeriesCollection();
            ProgressBarSeries = new SeriesCollection();
            StatusBarSeries = new SeriesCollection();
            DailyStockSeries = new SeriesCollection();
            _progressBarLabels = new List<string>();
            _warehouseCodes = new List<string>();

            // Load warehouse permissions
            var savedPermissions = Properties.Settings.Default.WarehousePermission;
            if (savedPermissions != null)
            {
                _warehouseCodes = savedPermissions.Cast<string>().ToList();
                ApplicableWarehouseCodes = string.Join(", ", _warehouseCodes);
            }
            _ = LoadDashboardDataAsync();

        }

        [RelayCommand]
        private async Task LoadDashboardDataAsync()
        {
            IsLoading = true;

            try
            {
                var dashboardTask = _dashboardService.GetDashboardInformation(_warehouseCodes.ToArray());
                var stockInOutTask = _dashboardService.GetStockInOutSummaryAsync(_warehouseCodes.ToArray());

                await Task.WhenAll(dashboardTask, stockInOutTask);

                var data = (await dashboardTask).Data!;
                var stockInOut = (await stockInOutTask).Data!;

                // Pie Chart - Tồn kho theo sản phẩm
                InventoryPieSeries.Clear();
                var inventoryGroups = data.inventorySummaryDTOs.GroupBy(p => p.ProductCode);
                foreach (var group in inventoryGroups)
                {
                    var productCode = group.Key;
                    var quantity = group.Sum(p => p.BaseQuantity);

                    InventoryPieSeries.Add(new PieSeries
                    {
                        Title = productCode,
                        Values = new ChartValues<double> { quantity ?? 0 },
                        DataLabels = true,
                        Fill = GetColorForCategory(productCode)
                    });
                }

                // Horizontal Bar Chart - Tiến độ sản xuất
                ProgressBarSeries.Clear();
                var progresses = data.progressManufacturingOrderDTOs
                    .Where(p => p.Progress.HasValue)
                    .ToList();

                if (progresses.Any())
                {
                    ProgressBarSeries.Add(new RowSeries
                    {
                        Title = "Tiến độ",
                        Values = new ChartValues<double>(progresses.Select(p => p.Progress!.Value)),
                        DataLabels = true,
                        Fill = new SolidColorBrush(Colors.SeaGreen)
                    });

                    // Set the Y-axis labels to MaLenhSanXuat
                    ProgressBarLabels = progresses.Select(p => p.ManufacturingOrderCode).ToList();
                    ProgressBarMessage = null;
                }
                else
                {
                    ProgressBarSeries.Add(new RowSeries
                    {
                        Title = "Tiến độ",
                        Values = new ChartValues<double> { 0 }, // Add a dummy value to avoid empty chart
                        DataLabels = false,
                        Fill = new SolidColorBrush(Colors.Transparent)
                    });
                    ProgressBarLabels = new List<string> (); // Clear labels
                    ProgressBarMessage = "Không có dữ liệu lệnh sản xuất.";
                }

                // Warning Cards
                QuantityToCompleteStockIn = data.QuantityToCompleteStockIn;
                QuantityToCompleteStockOut = data.QuantityToCompleteStockOut;
                QuantityToCompleteManufacturingOrder = data.QuantityToCompleteManufacturingOrder;

                // Cảnh báo hạn chót hôm nay
                var today = DateTime.Today;
                var nearest = data.upcomingDeadlines
                    .FirstOrDefault(d => DateTime.TryParse(d.Deadline, out var dt) && dt.Date == today);

                UpcomingDeadlinesWarning = nearest != null
                    ? $"⚠️ Hạn chót hôm nay: {nearest.OrderCode} - {nearest.Deadline}"
                    : null;

                // Bar Chart - Trạng thái đơn hàng theo loại
                StatusBarSeries.Clear();
                StatusBarSeries.Add(new ColumnSeries
                {
                    Title = "Bắt đầu",
                    Values = new ChartValues<int>(data.statusOrderCodeDTOs.Select(x => x.CountStatusStart)),
                    Fill = new SolidColorBrush(Colors.SkyBlue)
                });
                StatusBarSeries.Add(new ColumnSeries
                {
                    Title = "Đang xử lý",
                    Values = new ChartValues<int>(data.statusOrderCodeDTOs.Select(x => x.CountStatusInProgress)),
                    Fill = new SolidColorBrush(Colors.Orange)
                });
                StatusBarSeries.Add(new ColumnSeries
                {
                    Title = "Hoàn thành",
                    Values = new ChartValues<int>(data.statusOrderCodeDTOs.Select(x => x.CountStatusCompleted)),
                    Fill = new SolidColorBrush(Colors.Green)
                });

                StatusBarLabels = data.statusOrderCodeDTOs.Select(x => x.OrderTypeCode).ToArray();

                // Line Chart - Nhập xuất kho 7 ngày
                DailyStockSeries.Clear();
                DailyStockSeries.Add(new LineSeries
                {
                    Title = "Nhập kho",
                    Values = new ChartValues<int>(stockInOut.DailyStockInOuts.Select(x => x.StockInCount)),
                    Stroke = new SolidColorBrush(Colors.Blue),
                    Fill = new SolidColorBrush(Color.FromArgb(40, 0, 0, 255))
                });
                DailyStockSeries.Add(new LineSeries
                {
                    Title = "Xuất kho",
                    Values = new ChartValues<int>(stockInOut.DailyStockInOuts.Select(x => x.StockOutCount)),
                    Stroke = new SolidColorBrush(Colors.Red),
                    Fill = new SolidColorBrush(Color.FromArgb(40, 255, 0, 0))
                });

                DailyStockLabels = stockInOut.DailyStockInOuts.Select(x => x.Date).ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi Dashboard: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private SolidColorBrush GetColorForCategory(string productCode)
        {
            if (string.IsNullOrEmpty(productCode))
                return new SolidColorBrush(Colors.Gray);

            return productCode switch
            {
                string code when code.StartsWith("FG") => new SolidColorBrush(Colors.Blue),
                string code when code.StartsWith("MAT") => new SolidColorBrush(Colors.Green),
                string code when code.StartsWith("SFG") => new SolidColorBrush(Colors.Orange),
                _ => new SolidColorBrush(Colors.Gray)
            };
        }
    }
}
