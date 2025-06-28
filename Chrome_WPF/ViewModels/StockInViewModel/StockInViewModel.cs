using Chrome_WPF.Helpers;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.StatusMasterDTO;
using Chrome_WPF.Models.StockInDTO;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.PutAwayService;
using Chrome_WPF.Services.StockInDetailService;
using Chrome_WPF.Services.StockInService;
using Chrome_WPF.Views.UserControls.StockIn;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.StockInViewModel
{
    public class StockInViewModel : BaseViewModel
    {
        private readonly IStockInService _stockInService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IMessengerService _messengerService;

        private ObservableCollection<StockInResponseDTO> _stockInList;
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
        private string _applicableLocataion;

        public ObservableCollection<StockInResponseDTO> StockInList
        {
            get => _stockInList;
            set
            {
                _stockInList = value;
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
                    _ = LoadStockInsAsync();
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
                    _ = LoadStockInsAsync();
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
                    _ = LoadStockInsAsync();
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
                    _ = LoadStockInsAsync();
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
            get => _applicableLocataion;
            set
            {
                _applicableLocataion = value;
                OnPropertyChanged(nameof(ApplicableLocation));
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

        public StockInViewModel(
            IStockInService stockInService,
            INotificationService notificationService,
            INavigationService navigationService,
            IMessengerService messengerService)
        {
            _stockInService = stockInService ?? throw new ArgumentNullException(nameof(stockInService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));

            _stockInList = new ObservableCollection<StockInResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            _statuses = new ObservableCollection<StatusMasterResponseDTO>();
            _searchText = string.Empty;
            _currentPage = 1;
            _selectedStatusIndex = 0;
            _lastLoadedPage = 0;
            _lastSearchText = string.Empty;
            _lastStatusIndex = 0;

            SearchCommand = new RelayCommand(async _ => await SearchStockInsAsync());
            RefreshCommand = new RelayCommand(async _ => await LoadStockInsAsync(true));
            AddCommand = new RelayCommand(_ => OpenEditor(null!));
            DeleteCommand = new RelayCommand(async stockIn => await DeleteStockInAsync((StockInResponseDTO)stockIn));
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            ViewDetailCommand = new RelayCommand(stockIn => OpenEditor((StockInResponseDTO)stockIn));

            _ = messengerService.RegisterMessageAsync("ReloadStockInList", async _ => await LoadStockInsAsync(true));
            List<string> warehousePermissions = new List<string>();
            var savedPermissions = Properties.Settings.Default.WarehousePermission;
            if (savedPermissions != null)
            {
                warehousePermissions = savedPermissions.Cast<string>().ToList();
            }
            _applicableLocataion = string.Join(",", warehousePermissions.Select(id => $"{Uri.EscapeDataString(id)}"));
            _ = InitializeAsync();
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
                await LoadStockInsAsync();
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
                var result = await _stockInService.GetListStatusMaster();
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

        private async Task LoadStockInsAsync(bool forceRefresh = false)
        {
            try
            {
                if (!forceRefresh && _lastLoadedPage == CurrentPage && _lastSearchText == SearchText && _lastStatusIndex == SelectedStatusIndex)
                {
                    return;
                }

                ApiResult<PagedResponse<StockInResponseDTO>> result;
                if (SelectedStatusIndex > 0 && Statuses[SelectedStatusIndex].StatusId != 0)
                {
                    result = await _stockInService.GetAllStockInsWithStatus(Statuses[SelectedStatusIndex].StatusId, CurrentPage, PageSize);
                }
                else
                {
                    result = await _stockInService.GetAllStockIns(CurrentPage, PageSize);
                }

                if (result.Success && result.Data != null)
                {
                    StockInList.Clear();
                    foreach (var stockIn in result.Data.Data ?? Enumerable.Empty<StockInResponseDTO>())
                    {
                        StockInList.Add(stockIn);
                    }
                    TotalPages = result.Data.TotalPages;
                    _lastLoadedPage = CurrentPage;
                    _lastSearchText = SearchText;
                    _lastStatusIndex = SelectedStatusIndex;
                 
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải danh sách phiếu nhập kho.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải phiếu nhập kho: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task SearchStockInsAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    await LoadStockInsAsync();
                    return;
                }

                var result = await _stockInService.SearchStockInAsync(SearchText, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    StockInList.Clear();
                    foreach (var stockIn in result.Data.Data ?? Enumerable.Empty<StockInResponseDTO>())
                    {
                        StockInList.Add(stockIn);
                    }
                    TotalPages = result.Data.TotalPages;
                    _lastLoadedPage = CurrentPage;
                    _lastSearchText = SearchText;
                    _lastStatusIndex = SelectedStatusIndex;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tìm kiếm phiếu nhập kho.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tìm kiếm phiếu nhập kho: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task DeleteStockInAsync(StockInResponseDTO stockIn)
        {
            if (stockIn == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa phiếu nhập kho {stockIn.StockInCode}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deleteResult = await _stockInService.DeleteStockInAsync(stockIn.StockInCode);
                    if (deleteResult.Success)
                    {
                        await LoadStockInsAsync(true);
                        _notificationService.ShowMessage("Xóa phiếu nhập kho thành công.", "OK", isError: false);
                        await _messengerService.SendMessageAsync("ReloadStockInList");
                    }
                    else
                    {
                        _notificationService.ShowMessage(deleteResult.Message ?? "Không thể xóa phiếu nhập kho.", "OK", isError: true);
                    }
                }
                catch (Exception ex)
                {
                    _notificationService.ShowMessage($"Lỗi khi xóa phiếu nhập kho: {ex.Message}", "OK", isError: true);
                }
            }
        }

        private void OpenEditor(StockInResponseDTO stockIn)
        {
            var stockInDetail = App.ServiceProvider!.GetRequiredService<ucStockInDetail>();
            var viewModel = new StockInDetailViewModel(
                App.ServiceProvider!.GetRequiredService<IStockInDetailService>(),
                App.ServiceProvider!.GetRequiredService<IStockInService>(),
                App.ServiceProvider!.GetRequiredService<INotificationService>(),
                App.ServiceProvider!.GetRequiredService<INavigationService>(),
                App.ServiceProvider!.GetRequiredService<IMessengerService>(),
                App.ServiceProvider!.GetRequiredService<IPutAwayService>(),
                stockIn);

            stockInDetail.DataContext = viewModel;
            _navigationService.NavigateTo(stockInDetail);
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