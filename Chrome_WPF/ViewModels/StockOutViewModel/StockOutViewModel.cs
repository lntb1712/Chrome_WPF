using Chrome_WPF.Helpers;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.StatusMasterDTO;
using Chrome_WPF.Models.StockOutDTO;
using Chrome_WPF.Services.CodeGeneratorService;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.PickListService;
using Chrome_WPF.Services.ReservationService;
using Chrome_WPF.Services.StockOutDetailService;
using Chrome_WPF.Services.StockOutService;
using Chrome_WPF.ViewModels.ExportExcelViewModels;
using Chrome_WPF.Views.UserControls.StockOut;
using ClosedXML.Excel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.StockOutViewModel
{
    public class StockOutViewModel : BaseViewModel
    {
        private readonly IStockOutService _stockOutService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IMessengerService _messengerService;

        private ObservableCollection<StockOutResponseDTO> _stockOutList;
        private ObservableCollection<object> _displayPages;
        private ObservableCollection<StatusMasterResponseDTO> _statuses;
        private string _searchText;
        private int _selectedStatusIndex;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;
        private int _lastLoadedPage;
        private string _lastSearchText;
        private int _lastStatusIndex;
        private string _applicableLocation;
        private DateTime _selectedMonth;
        private int? _dialogMonth;
        private int? _dialogYear;
        private ObservableCollection<int> _months;
        private ObservableCollection<int> _years;

        public ObservableCollection<StockOutResponseDTO> StockOutList
        {
            get => _stockOutList;
            set
            {
                _stockOutList = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<object> DisplayPages
        {
            get => _displayPages;
            set
            {
                _displayPages = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<StatusMasterResponseDTO> Statuses
        {
            get => _statuses;
            set
            {
                _statuses = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    CurrentPage = 1;
                    _ = LoadStockOutsAsync();
                }
            }
        }

        public int SelectedStatusIndex
        {
            get => _selectedStatusIndex;
            set
            {
                if (_selectedStatusIndex != value)
                {
                    _selectedStatusIndex = value;
                    OnPropertyChanged();
                    CurrentPage = 1;
                    _ = LoadStockOutsAsync();
                }
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged();
                    UpdateDisplayPages();
                    _ = LoadStockOutsAsync();
                }
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (_pageSize != value)
                {
                    _pageSize = value;
                    OnPropertyChanged();
                    CurrentPage = 1;
                    _ = LoadStockOutsAsync();
                }
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                OnPropertyChanged();
                UpdateDisplayPages();
            }
        }

        public string ApplicableLocation
        {
            get => _applicableLocation;
            set
            {
                _applicableLocation = value;
                OnPropertyChanged();
            }
        }

        public DateTime SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                _selectedMonth = value;
                OnPropertyChanged();
            }
        }

        public int? DialogMonth
        {
            get => _dialogMonth;
            set
            {
                _dialogMonth = value;
                OnPropertyChanged();
            }
        }

        public int? DialogYear
        {
            get => _dialogYear;
            set
            {
                _dialogYear = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<int> Months
        {
            get => _months;
            set
            {
                _months = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<int> Years
        {
            get => _years;
            set
            {
                _years = value;
                OnPropertyChanged();
            }
        }

        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand SelectPageCommand { get; }
        public ICommand ViewDetailCommand { get; }
        public ICommand PrintByMonthCommand { get; }

        public StockOutViewModel(
            IStockOutService stockOutService,
            INotificationService notificationService,
            INavigationService navigationService,
            IMessengerService messengerService)
        {
            _stockOutService = stockOutService ?? throw new ArgumentNullException(nameof(stockOutService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));

            _stockOutList = new ObservableCollection<StockOutResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            _statuses = new ObservableCollection<StatusMasterResponseDTO>();
            _searchText = string.Empty;
            _currentPage = 1;
            _selectedStatusIndex = 0;
            _lastLoadedPage = 0;
            _lastSearchText = string.Empty;
            _lastStatusIndex = 0;
            _selectedMonth = DateTime.Now;
            _dialogMonth = DateTime.Now.Month;
            _dialogYear = DateTime.Now.Year;
            _months = new ObservableCollection<int>(Enumerable.Range(1, 12));
            _years = new ObservableCollection<int>(Enumerable.Range(2020, 100));

            SearchCommand = new RelayCommand(async _ => await SearchStockOutsAsync());
            RefreshCommand = new RelayCommand(async _ => await LoadStockOutsAsync(true));
            AddCommand = new RelayCommand(_ => OpenEditor(null!));
            DeleteCommand = new RelayCommand(async stockOut => await DeleteStockOutAsync((StockOutResponseDTO)stockOut));
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            ViewDetailCommand = new RelayCommand(stockOut => OpenEditor((StockOutResponseDTO)stockOut));
            PrintByMonthCommand = new RelayCommand(async _ => await ShowMonthYearDialogAsync());

            _ = messengerService.RegisterMessageAsync("ReloadStockOutList", async _ => await LoadStockOutsAsync(true));
            List<string> warehousePermissions = new List<string>();
            var savedPermissions = Properties.Settings.Default.WarehousePermission;
            if (savedPermissions != null)
            {
                warehousePermissions = savedPermissions.Cast<string>().ToList();
            }
            _applicableLocation = string.Join(",", warehousePermissions.Select(id => $"{Uri.EscapeDataString(id)}"));
            _ = InitializeAsync();
        }
        private async Task ShowMonthYearDialogAsync()
        {
            try
            {
                // Tạo instance của ExportExcelViewModel
                var exportExcelViewModel = new ExportExcelViewModel(_notificationService);

                // Tạo instance của dialog ExportExcel
                var dialog = new Chrome_WPF.Views.Windows.ExportExcel(exportExcelViewModel);

                // Hiển thị dialog
                var result = dialog.ShowDialog();

                // Kiểm tra kết quả dialog
                if (result is bool dialogResult && dialogResult)
                {
                    if (exportExcelViewModel.DialogMonth.HasValue && exportExcelViewModel.DialogYear.HasValue)
                    {
                        // Cập nhật SelectedMonth dựa trên input từ dialog
                        SelectedMonth = new DateTime(exportExcelViewModel.DialogYear.Value, exportExcelViewModel.DialogMonth.Value, 1);
                        await PrintStockOutByMonthAsync();
                    }
                    else
                    {
                        _notificationService.ShowMessage("Vui lòng chọn tháng và năm.", "OK", isError: true);
                    }
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi mở dialog: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task PrintStockOutByMonthAsync()
        {
            try
            {
                string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "BaoCaoXuatKhoTheoThang.xlsx");
                if (!File.Exists(templatePath))
                {
                    _notificationService.ShowMessage("Không tìm thấy file mẫu BaoCaoXuatKhoTheoThang.xlsx.", "OK", isError: true);
                    return;
                }

                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string exportPath = Path.Combine(desktopPath, $"BaoCaoXuatKho_Thang{SelectedMonth:yyyyMMdd_HHmmss}.xlsx");

                // Lấy dữ liệu phiếu xuất kho
                int year = SelectedMonth.Year;
                int month = SelectedMonth.Month;
                var stockOutResult = await _stockOutService.GetListStockOutToReport(month, year);
                if (!stockOutResult.Success || stockOutResult.Data == null || !stockOutResult.Data.Any())
                {
                    _notificationService.ShowMessage(stockOutResult.Message ?? "Không có phiếu xuất kho trong tháng được chọn.", "OK", isError: true);
                    return;
                }

                using (var workbook = new XLWorkbook(templatePath))
                {
                    IXLWorksheet worksheet = workbook.Worksheet(1); // Sheet "Đơn xuất kho"
                    int currentRow = 10; // Bắt đầu từ hàng 10
                    int stt = 1;
                    const int maxRowsPerSheet = 20; // Giới hạn 20 hàng dữ liệu mỗi sheet
                    int sheetNumber = 1;

                    // Điền thông tin header cho sheet đầu tiên
                    worksheet.Cell(3, 5).Value = $"Tháng {month}/{year}";
                    worksheet.Cell(4, 5).Value = Properties.Settings.Default.FullName;
                    worksheet.Cell(5, 5).Value = Properties.Settings.Default.RoleName;
                    worksheet.Cell(6, 5).Value = $"Báo cáo xuất kho tháng {month}/{year}";
                    worksheet.Cell(1, 9).Value += ApplicableLocation;

                    foreach (var stockOut in stockOutResult.Data)
                    {
                        if (stockOut.stockOutDetails == null || !stockOut.stockOutDetails.Any())
                        {
                            continue; // Bỏ qua nếu không có chi tiết
                        }

                        foreach (var detail in stockOut.stockOutDetails)
                        {
                            // Tạo sheet mới nếu vượt quá số hàng tối đa
                            if (currentRow > (9 + maxRowsPerSheet))
                            {
                                sheetNumber++;
                                worksheet = workbook.AddWorksheet($"Báo cáo xuất kho {sheetNumber}");
                                // Sao chép header từ sheet gốc
                                var originalSheet = workbook.Worksheet(1);
                                for (int row = 1; row <= 9; row++)
                                {
                                    for (int col = 1; col <= 8; col++)
                                    {
                                        worksheet.Cell(row, col).Value = originalSheet.Cell(row, col).Value;
                                        worksheet.Cell(row, col).Style = originalSheet.Cell(row, col).Style;
                                    }
                                }
                                // Cập nhật header cho sheet mới
                                worksheet.Cell(3, 5).Value = $"Tháng {month}/{year}";
                                worksheet.Cell(4, 5).Value = Properties.Settings.Default.FullName ?? "N/A";
                                worksheet.Cell(5, 5).Value = Properties.Settings.Default.RoleName ?? "N/A";
                                worksheet.Cell(6, 5).Value = $"Báo cáo xuất kho tháng {month}/{year}";
                                worksheet.Cell(1, 9).Value += ApplicableLocation;
                                currentRow = 10;
                            }

                            // Ghi dữ liệu vào sheet
                            worksheet.Cell(currentRow, 2).Value = stt;
                            worksheet.Cell(currentRow, 3).Value = stockOut.StockOutDate ?? "";
                            worksheet.Cell(currentRow, 4).Value = stockOut.StockOutCode ?? "";
                            worksheet.Cell(currentRow, 5).Value = detail.ProductCode ?? "";
                            worksheet.Cell(currentRow, 6).Value = detail.ProductName ?? "";
                            worksheet.Cell(currentRow, 7).Value = detail.Quantity ?? 0;
                            worksheet.Cell(currentRow, 8).Value = detail.UOM ?? "Cái";
                            worksheet.Cell(currentRow, 9).Value = stockOut.CustomerCode ?? "";
                            currentRow++;
                            stt++;
                        }
                    }

                    workbook.SaveAs(exportPath);
                    _notificationService.ShowMessage($"Xuất báo cáo xuất kho tháng {month}/{year} thành công!\nFile được lưu tại: {exportPath}", "OK", isError: false);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi xuất báo cáo xuất kho: {ex.Message}", "OK", isError: true);
            }
        }
        private async Task InitializeAsync()
        {
            try
            {
                await LoadStatusesAsync();
                if (Statuses.Count == 0)
                {
                    _notificationService.ShowMessage("Không tải được trạng thái. Lọc theo trạng thái sẽ bị vô hiệu hóa.", "OK", isError: true);
                }
                await LoadStockOutsAsync();
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Khởi tạo thất bại: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadStatusesAsync()
        {
            try
            {
                var result = await _stockOutService.GetListStatusMaster();
                if (result.Success && result.Data != null)
                {
                    Statuses.Clear();
                    Statuses.Add(new StatusMasterResponseDTO { StatusId = 0, StatusName = "Tất cả" });
                    foreach (var status in result.Data)
                    {
                        Statuses.Add(status);
                    }
                    SelectedStatusIndex = 0;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải danh sách trạng thái.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải trạng thái: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadStockOutsAsync(bool forceRefresh = false)
        {
            try
            {
                if (!forceRefresh && _lastLoadedPage == CurrentPage && _lastSearchText == SearchText && _lastStatusIndex == SelectedStatusIndex)
                {
                    return;
                }

                ApiResult<PagedResponse<StockOutResponseDTO>> result;
                if (SelectedStatusIndex > 0 && Statuses[SelectedStatusIndex].StatusId != 0)
                {
                    result = await _stockOutService.GetAllStockOutsWithStatus(Statuses[SelectedStatusIndex].StatusId, CurrentPage, PageSize);
                }
                else
                {
                    result = await _stockOutService.GetAllStockOuts(CurrentPage, PageSize);
                }

                if (result.Success && result.Data != null)
                {
                    StockOutList.Clear();
                    foreach (var stockOut in result.Data.Data ?? Enumerable.Empty<StockOutResponseDTO>())
                    {
                        StockOutList.Add(stockOut);
                    }
                    TotalPages = result.Data.TotalPages;
                    _lastLoadedPage = CurrentPage;
                    _lastSearchText = SearchText;
                    _lastStatusIndex = SelectedStatusIndex;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải danh sách phiếu xuất kho.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải phiếu xuất kho: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task SearchStockOutsAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    await LoadStockOutsAsync();
                    return;
                }

                var result = await _stockOutService.SearchStockOutAsync(SearchText, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    StockOutList.Clear();
                    foreach (var stockOut in result.Data.Data ?? Enumerable.Empty<StockOutResponseDTO>())
                    {
                        StockOutList.Add(stockOut);
                    }
                    TotalPages = result.Data.TotalPages;
                    _lastLoadedPage = CurrentPage;
                    _lastSearchText = SearchText;
                    _lastStatusIndex = SelectedStatusIndex;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tìm kiếm phiếu xuất kho.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tìm kiếm phiếu xuất kho: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task DeleteStockOutAsync(StockOutResponseDTO stockOut)
        {
            if (stockOut == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa phiếu xuất kho {stockOut.StockOutCode}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deleteResult = await _stockOutService.DeleteStockOutAsync(stockOut.StockOutCode);
                    if (deleteResult.Success)
                    {
                        await LoadStockOutsAsync(true);
                        _notificationService.ShowMessage("Xóa phiếu xuất kho thành công.", "OK", isError: false);
                        await _messengerService.SendMessageAsync("ReloadStockOutList");
                    }
                    else
                    {
                        _notificationService.ShowMessage(deleteResult.Message ?? "Không thể xóa phiếu xuất kho.", "OK", isError: true);
                    }
                }
                catch (Exception ex)
                {
                    _notificationService.ShowMessage($"Lỗi khi xóa phiếu xuất kho: {ex.Message}", "OK", isError: true);
                }
            }
        }

        private void OpenEditor(StockOutResponseDTO stockOut)
        {
            var stockOutDetail = App.ServiceProvider!.GetRequiredService<ucStockOutDetail>();
            var viewModel = new StockOutDetailViewModel(
                App.ServiceProvider!.GetRequiredService<IStockOutDetailService>(),
                App.ServiceProvider!.GetRequiredService<IStockOutService>(),
                App.ServiceProvider!.GetRequiredService<IReservationService>(),
                App.ServiceProvider!.GetRequiredService<IPickListService>(),
                App.ServiceProvider!.GetRequiredService<INotificationService>(),
                App.ServiceProvider!.GetRequiredService<INavigationService>(),
                App.ServiceProvider!.GetRequiredService<IMessengerService>(),
                App.ServiceProvider!.GetRequiredService<ICodeGenerateService>(),
                stockOut);

            stockOutDetail.DataContext = viewModel;
            _navigationService.NavigateTo(stockOutDetail);
        }

        private void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
            }
        }

        private void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
            }
        }

        private void SelectPage(int page)
        {
            if (page >= 1 && page <= TotalPages)
            {
                CurrentPage = page;
            }
        }

        private void UpdateDisplayPages()
        {
            DisplayPages.Clear();
            if (TotalPages <= 0) return;

            int startPage = Math.Max(1, CurrentPage - 2);
            int endPage = Math.Min(TotalPages, CurrentPage + 2);

            if (startPage > 1)
                DisplayPages.Add(1);
            if (startPage > 2)
                DisplayPages.Add("...");

            for (int i = startPage; i <= endPage; i++)
                DisplayPages.Add(i);

            if (endPage < TotalPages - 1)
                DisplayPages.Add("...");
            if (endPage < TotalPages)
                DisplayPages.Add(TotalPages);
        }
    }
}