using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.DashboardDTO;
using Chrome_WPF.Models.InventoryDTO;
using Chrome_WPF.Models.ManufacturingOrderDTO;
using Chrome_WPF.Services.DashboardService;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
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
        [ObservableProperty] private int quantityToCompletePurchaseOrder;
        [ObservableProperty] private int quantityToCompleteStocktake;
        [ObservableProperty] private string? upcomingDeadlinesWarning;
        [ObservableProperty] private SeriesCollection statusBarSeries;
        [ObservableProperty] private string[] statusBarLabels;
        [ObservableProperty] private SeriesCollection monthlyStockSeries;
        [ObservableProperty] private string[] monthlyStockLabels;
        [ObservableProperty] private int? selectedMonth;
        [ObservableProperty] private int? selectedYear;
        [ObservableProperty] private int? selectedQuarter;
        [ObservableProperty] private string selectedWarehouseCode;

        public Func<double, string> YLabelFormatter => value => value.ToString("0.##");

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
        private string[] _warehouseCodes;
        public string ApplicableWarehouseCodes
        {
            get => _applicableWarehouseCodes;
            set
            {
                _applicableWarehouseCodes = value;
                OnPropertyChanged();
            }
        }

        public ICommand FilterCommand { get; }
        public ICommand ClearFilterCommand { get; } // Thêm lệnh Clear Filter
        public List<string> AvailableWarehouseCodes { get; } = new List<string>();
        public List<int> AvailableYears { get; } = Enumerable.Range(2000, DateTime.Today.Year - 2000 + 1).ToList();
        public List<int> AvailableQuarters { get; } = new List<int> { 1, 2, 3, 4 };
        private List<int> _availableMonths = Enumerable.Range(1, 12).ToList();
        public List<int> AvailableMonths
        {
            get => _availableMonths;
            set
            {
                _availableMonths = value;
                OnPropertyChanged(nameof(AvailableMonths));
            }
        }

        public DashboardViewModel(IDashboardService dashboardService, API_Constant apiConstant)
        {
            _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));

            InventoryPieSeries = new SeriesCollection();
            ProgressBarSeries = new SeriesCollection();
            StatusBarSeries = new SeriesCollection();
            MonthlyStockSeries = new SeriesCollection();
            _progressBarLabels = new List<string>();
            _warehouseCodes = new string[] { };
            monthlyStockLabels = Array.Empty<string>();
            statusBarLabels = Array.Empty<string>();
            SelectedWarehouseCode = string.Empty;
            SelectedYear = DateTime.Today.Year; // Default to current year
            SelectedQuarter = null;
            SelectedMonth = null;

            // Load warehouse permissions
            var savedPermissions = Properties.Settings.Default.WarehousePermission;
            if (savedPermissions != null)
            {
                _warehouseCodes = savedPermissions.Cast<string>().ToArray();
                AvailableWarehouseCodes.AddRange(_warehouseCodes);
                ApplicableWarehouseCodes = string.Join(", ", _warehouseCodes);
                if (_warehouseCodes.Any())
                    SelectedWarehouseCode = _warehouseCodes[0];
            }

            // Define commands
            FilterCommand = new AsyncRelayCommand(LoadDashboardDataAsyncCommand);
            ClearFilterCommand = new RelayCommand(ClearFilter); // Thêm Clear Filter command
            _ = LoadDashboardDataAsyncCommand(); // Initial load

            // Theo dõi thay đổi SelectedQuarter để cập nhật AvailableMonths
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SelectedQuarter))
                {
                    UpdateAvailableMonths();
                }
            };
        }

        [RelayCommand]
        public async Task LoadDashboardDataAsyncCommand()
        {
            IsLoading = true;

            try
            {
                var request = new DashboardRequestDTO
                {
                    warehouseCodes = string.IsNullOrEmpty(SelectedWarehouseCode) ? _warehouseCodes : new string[] { SelectedWarehouseCode },
                    Month = SelectedMonth,
                    Year = SelectedYear,
                    Quarter = SelectedQuarter
                };

                var dashboardTask = _dashboardService.GetDashboardInformation(request);
                var stockInOutTask = _dashboardService.GetStockInOutSummaryAsync(request);

                await Task.WhenAll(dashboardTask, stockInOutTask);

                var dashboardResult = await dashboardTask;
                var stockInOutResult = await stockInOutTask;

                if (!dashboardResult.Success || dashboardResult.Data == null)
                {
                    ProgressBarMessage = "Không thể tải dữ liệu dashboard: " + (dashboardResult.Message ?? "Lỗi không xác định");
                    return;
                }

                if (!stockInOutResult.Success || stockInOutResult.Data == null)
                {
                    ProgressBarMessage = "Không thể tải dữ liệu nhập/xuất kho: " + (stockInOutResult.Message ?? "Lỗi không xác định");
                    return;
                }

                var data = dashboardResult.Data;
                var stockInOut = stockInOutResult.Data;

                // Pie Chart - Tồn kho theo danh mục sản phẩm
                InventoryPieSeries.Clear();
                var inventoryGroups = (data.InventorySummaryDTOs ?? new List<InventorySummaryDTO>())
                    .GroupBy(p => GetCategoryFromProductCode(p.ProductCode))
                    .Select(g => new
                    {
                        Category = g.Key,
                        TotalQuantity = g.Sum(p => p.BaseQuantity ?? 0),
                        ProductCodes = g.Select(p => p.ProductCode ?? "Unknown").ToList()
                    });

                foreach (var group in inventoryGroups)
                {
                    var category = group.Category;
                    var quantity = group.TotalQuantity;
                    var displayValue = quantity > 0 ? Math.Log10(quantity + 1) : 0;

                    InventoryPieSeries.Add(new PieSeries
                    {
                        Title = category,
                        Values = new ChartValues<double> { displayValue },
                        DataLabels = true,
                        LabelPoint = point =>
                        {
                            var actualQuantity = Math.Round(Math.Pow(10, point.Y) - 1, 2);
                            return $"{category}: {actualQuantity}";
                        },
                        ToolTip = group.ProductCodes,
                        Fill = GetColorForCategory(category),
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(Colors.White)
                    });
                }

                // Horizontal Bar Chart - Tiến độ sản xuất
                ProgressBarSeries.Clear();
                var progresses = (data.ProgressManufacturingOrderDTOs ?? new List<ProgressManufacturingOrderDTO>())
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
                    ProgressBarLabels = progresses.Select(p => p.ManufacturingOrderCode ?? "Unknown").ToList();
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
                QuantityToCompletePurchaseOrder = data.QuantityToCompletePurchaseOrder;
                QuantityToCompleteStocktake = data.QuantityToCompleteStocktake;

                // Cảnh báo hạn chót hôm nay
                var today = DateTime.Today;
                var nearest = (data.UpcomingDeadlines ?? new List<UpcomingDeadlineDTO>())
                    .FirstOrDefault(d => DateTime.TryParse(d.Deadline, out var dt) && dt.Date == today);

                UpcomingDeadlinesWarning = nearest != null
                    ? $"⚠️ Hạn chót hôm nay: {nearest.OrderCode} - {nearest.Deadline}"
                    : null;

                // Bar Chart - Trạng thái đơn hàng theo loại
                StatusBarSeries.Clear();
                var statusOrders = data.StatusOrderCodeDTOs ?? new List<StatusOrderCodeDTO>();
                StatusBarSeries.Add(new ColumnSeries
                {
                    Title = "Bắt đầu",
                    Values = new ChartValues<int>(statusOrders.Select(x => x.CountStatusStart)),
                    Fill = new SolidColorBrush(Color.FromRgb(135, 206, 235)), // SkyBlue
                    Stroke = new SolidColorBrush(Colors.White),
                    StrokeThickness = 1,
                    ColumnPadding = 0.1 // Thu hẹp cột
                });
                StatusBarSeries.Add(new ColumnSeries
                {
                    Title = "Đang xử lý",
                    Values = new ChartValues<int>(statusOrders.Select(x => x.CountStatusInProgress)),
                    Fill = new SolidColorBrush(Color.FromRgb(255, 165, 0)), // Orange
                    Stroke = new SolidColorBrush(Colors.White),
                    StrokeThickness = 1,
                    ColumnPadding = 0.1 // Thu hẹp cột
                });
                StatusBarSeries.Add(new ColumnSeries
                {
                    Title = "Hoàn thành",
                    Values = new ChartValues<int>(statusOrders.Select(x => x.CountStatusCompleted)),
                    Fill = new SolidColorBrush(Color.FromRgb(0, 128, 0)), // Green
                    Stroke = new SolidColorBrush(Colors.White),
                    StrokeThickness = 1,
                    ColumnPadding = 0.1 // Thu hẹp cột
                });

                StatusBarLabels = statusOrders.Select(x => x.OrderTypeCode ?? "Unknown").ToArray();

                // Line Chart - Monthly Stock In/Out
                MonthlyStockSeries.Clear();
                var monthlyData = stockInOut.MonthlyStockInOuts ?? new List<MonthlyStockInOutDTO>();
                if (monthlyData.Any())
                {
                    MonthlyStockSeries.Add(new LineSeries
                    {
                        Title = "Phiếu nhập",
                        Values = new ChartValues<int>(monthlyData.Select(x => x.StockInCount)),
                        Stroke = new SolidColorBrush(Color.FromRgb(33, 150, 243)), // Blue
                        Fill = new SolidColorBrush(Colors.Transparent),
                        PointGeometry = DefaultGeometries.Circle,
                        StrokeThickness = 2
                    });
                    MonthlyStockSeries.Add(new LineSeries
                    {
                        Title = "Phiếu xuất",
                        Values = new ChartValues<int>(monthlyData.Select(x => x.StockOutCount)),
                        Stroke = new SolidColorBrush(Color.FromRgb(255, 99, 71)), // Tomato
                        Fill = new SolidColorBrush(Colors.Transparent),
                        PointGeometry = DefaultGeometries.Square,
                        StrokeThickness = 2
                    });
                    MonthlyStockSeries.Add(new LineSeries
                    {
                        Title = "Đơn đặt hàng",
                        Values = new ChartValues<int>(monthlyData.Select(x => x.PurchaseOrderCount)),
                        Stroke = new SolidColorBrush(Color.FromRgb(46, 139, 87)), // SeaGreen
                        Fill = new SolidColorBrush(Colors.Transparent),
                        PointGeometry = DefaultGeometries.Triangle,
                        StrokeThickness = 2
                    });
                    MonthlyStockSeries.Add(new LineSeries
                    {
                        Title = "Kiểm kho",
                        Values = new ChartValues<int>(monthlyData.Select(x => x.StocktakeCount)),
                        Stroke = new SolidColorBrush(Color.FromRgb(255, 215, 0)), // Gold
                        Fill = new SolidColorBrush(Colors.Transparent),
                        PointGeometry = DefaultGeometries.Diamond,
                        StrokeThickness = 2
                    });
                    MonthlyStockLabels = monthlyData.Select(x => x.Month).ToArray();
                }
                else
                {
                    MonthlyStockSeries.Add(new LineSeries
                    {
                        Title = "Không có dữ liệu",
                        Values = new ChartValues<int> { 0 },
                        Stroke = new SolidColorBrush(Colors.Gray),
                        Fill = new SolidColorBrush(Colors.Transparent),
                        StrokeThickness = 2
                    });
                    MonthlyStockLabels = new string[] { "Không có dữ liệu" };
                }
            }
            catch (Exception ex)
            {
                ProgressBarMessage = $"Lỗi khi tải dữ liệu dashboard: {ex.Message}";
                Console.WriteLine($"Lỗi Dashboard: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateAvailableMonths()
        {
            if (SelectedQuarter.HasValue)
            {
                int quarter = SelectedQuarter.Value;
                int startMonth = (quarter - 1) * 3 + 1;
                AvailableMonths = Enumerable.Range(startMonth, 3).ToList();
                SelectedMonth = null; // Reset month khi chọn quý mới
            }
            else
            {
                AvailableMonths = Enumerable.Range(1, 12).ToList(); // Hiển thị tất cả tháng nếu không chọn quý
            }
        }

        private void ClearFilter()
        {
            SelectedWarehouseCode = _warehouseCodes.Any() ? _warehouseCodes[0] : string.Empty;
            SelectedYear = DateTime.Today.Year;
            SelectedQuarter = null;
            SelectedMonth = null;
            UpdateAvailableMonths(); // Cập nhật lại danh sách tháng
            _ = LoadDashboardDataAsyncCommand(); // Tải lại dữ liệu sau khi xóa bộ lọc
        }

        private string GetCategoryFromProductCode(string? productCode)
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