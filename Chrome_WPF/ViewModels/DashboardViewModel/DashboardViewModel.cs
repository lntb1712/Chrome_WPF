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
            dailyStockLabels = Array.Empty<string>();
            statusBarLabels = Array.Empty<string>();

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
        public async Task LoadDashboardDataAsync()
        {
            IsLoading = true;

            try
            {
                var dashboardTask = _dashboardService.GetDashboardInformation(_warehouseCodes.ToArray());
                var stockInOutTask = _dashboardService.GetStockInOutSummaryAsync(_warehouseCodes.ToArray());

                await Task.WhenAll(dashboardTask, stockInOutTask);

                var data = (await dashboardTask).Data!;
                var stockInOut = (await stockInOutTask).Data!;

                // Pie Chart - Tồn kho theo danh mục sản phẩm
                InventoryPieSeries.Clear();
                var inventoryGroups = data.inventorySummaryDTOs
                    .GroupBy(p => GetCategoryFromProductCode(p.ProductCode))
                    .Select(g => new
                    {
                        Category = g.Key,
                        TotalQuantity = g.Sum(p => p.BaseQuantity ?? 0),
                        ProductCodes = g.Select(p => p.ProductCode).ToList() // Lưu danh sách mã sản phẩm
                    });

                foreach (var group in inventoryGroups)
                {
                    var category = group.Category;
                    var quantity = group.TotalQuantity;
                    var productCodes = group.ProductCodes;
                    var displayValue = quantity > 0 ? Math.Log10(quantity + 1) : 0;
                   
                    InventoryPieSeries.Add(new PieSeries
                    {
                        Title = category,
                        Values = new ChartValues<double> { displayValue },
                        DataLabels = true,
                        LabelPoint = point =>
                        {
                            var actualQuantity = Math.Round(Math.Pow(10, point.Y) - 1, 2);
                            // Danh sách mã sản phẩm
                            return $"{category}: {actualQuantity}";
                        },
                        ToolTip = productCodes,
                        Fill = GetColorForCategory(category),
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(Colors.White)
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
                        Fill = new SolidColorBrush(Color.FromRgb(46, 139, 87)), // SeaGreen
                        Stroke = new SolidColorBrush(Colors.White),
                        StrokeThickness = 1
                    });
                    ProgressBarLabels = progresses.Select(p => p.ManufacturingOrderCode).ToList();
                    ProgressBarMessage = null;
                }
                else
                {
                    ProgressBarSeries.Add(new RowSeries
                    {
                        Title = "Tiến độ",
                        Values = new ChartValues<double> { 0 },
                        DataLabels = false,
                        Fill = new SolidColorBrush(Colors.Transparent)
                    });
                    ProgressBarLabels = new List<string>();
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
                    Fill = new SolidColorBrush(Color.FromRgb(135, 206, 235)), // SkyBlue
                    Stroke = new SolidColorBrush(Colors.White),
                    StrokeThickness = 1
                });
                StatusBarSeries.Add(new ColumnSeries
                {
                    Title = "Đang xử lý",
                    Values = new ChartValues<int>(data.statusOrderCodeDTOs.Select(x => x.CountStatusInProgress)),
                    Fill = new SolidColorBrush(Color.FromRgb(255, 165, 0)), // Orange
                    Stroke = new SolidColorBrush(Colors.White),
                    StrokeThickness = 1
                });
                StatusBarSeries.Add(new ColumnSeries
                {
                    Title = "Hoàn thành",
                    Values = new ChartValues<int>(data.statusOrderCodeDTOs.Select(x => x.CountStatusCompleted)),
                    Fill = new SolidColorBrush(Color.FromRgb(0, 128, 0)), // Green
                    Stroke = new SolidColorBrush(Colors.White),
                    StrokeThickness = 1
                });

                StatusBarLabels = data.statusOrderCodeDTOs.Select(x => x.OrderTypeCode).ToArray();

                // Line Chart - Nhập xuất kho 7 ngày
                DailyStockSeries.Clear();
                DailyStockSeries.Add(new LineSeries
                {
                    Title = "Nhập kho",
                    Values = new ChartValues<int>(stockInOut.DailyStockInOuts.Select(x => x.StockInCount)),
                    Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 255)), // Blue
                    Fill = new SolidColorBrush(Color.FromArgb(40, 0, 0, 255)),
                    PointGeometrySize = 10,
                    StrokeThickness = 2
                });
                DailyStockSeries.Add(new LineSeries
                {
                    Title = "Xuất kho",
                    Values = new ChartValues<int>(stockInOut.DailyStockInOuts.Select(x => x.StockOutCount)),
                    Stroke = new SolidColorBrush(Color.FromRgb(255, 0, 0)), // Red
                    Fill = new SolidColorBrush(Color.FromArgb(40, 255, 0, 0)),
                    PointGeometrySize = 10,
                    StrokeThickness = 2
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

        private string GetCategoryFromProductCode(string productCode)
        {
            if (string.IsNullOrEmpty(productCode))
                return "Unknown";

            if (productCode.StartsWith("FG")) return "FG";
            if (productCode.StartsWith("MAT")) return "MAT";
            if (productCode.StartsWith("SFG")) return "SFG";
            return "Other";
        }

        private SolidColorBrush GetColorForCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
                return new SolidColorBrush(Colors.Gray);

            return category switch
            {
                "FG" => new SolidColorBrush(Color.FromRgb(33, 150, 243)), // Blue
                "MAT" => new SolidColorBrush(Color.FromRgb(76, 175, 80)), // Green
                "SFG" => new SolidColorBrush(Color.FromRgb(255, 152, 0)), // Orange
                _ => new SolidColorBrush(Colors.Gray)
            };
        }
    }
}