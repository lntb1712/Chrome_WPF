using Chrome.Services.ManufacturingOrderService;
using Chrome_WPF.Helpers;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.ManufacturingOrderDTO;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.StatusMasterDTO;
using Chrome_WPF.Services.CodeGeneratorService;
using Chrome_WPF.Services.ManufacturingOrderDetailService;
using Chrome_WPF.Services.ManufacturingOrderService;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.PickListService;
using Chrome_WPF.Services.PutAwayService;
using Chrome_WPF.Services.ReservationService;
using Chrome_WPF.Views.UserControls.ManufacturingOrder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.ManufacturingOrderViewModel
{
    public class ManufacturingOrderViewModel : BaseViewModel
    {
        private readonly IManufacturingOrderService _manufacturingOrderService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IMessengerService _messengerService;

        private ObservableCollection<ManufacturingOrderResponseDTO> _manufacturingOrderList;
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

        public ObservableCollection<ManufacturingOrderResponseDTO> ManufacturingOrderList
        {
            get => _manufacturingOrderList;
            set
            {
                _manufacturingOrderList = value;
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
                    _ = LoadManufacturingOrdersAsync();
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
                    _ = LoadManufacturingOrdersAsync();
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
                    _ = LoadManufacturingOrdersAsync();
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
                    _ = LoadManufacturingOrdersAsync();
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

        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand SelectPageCommand { get; }
        public ICommand ViewDetailCommand { get; }

        public ManufacturingOrderViewModel(
            IManufacturingOrderService manufacturingOrderService,
            INotificationService notificationService,
            INavigationService navigationService,
            IMessengerService messengerService)
        {
            _manufacturingOrderService = manufacturingOrderService ?? throw new ArgumentNullException(nameof(manufacturingOrderService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));

            _manufacturingOrderList = new ObservableCollection<ManufacturingOrderResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            _statuses = new ObservableCollection<StatusMasterResponseDTO>();
            _searchText = string.Empty;
            _currentPage = 1;
            _selectedStatusIndex = 0;
            _lastLoadedPage = 0;
            _lastSearchText = string.Empty;
            _lastStatusIndex = 0;

            SearchCommand = new RelayCommand(async _ => await SearchManufacturingOrdersAsync());
            RefreshCommand = new RelayCommand(async _ => await LoadManufacturingOrdersAsync(true));
            AddCommand = new RelayCommand(_ => OpenEditor(null!));
            DeleteCommand = new RelayCommand(async manufacturingOrder => await DeleteManufacturingOrderAsync((ManufacturingOrderResponseDTO)manufacturingOrder));
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            ViewDetailCommand = new RelayCommand(manufacturingOrder => OpenEditor((ManufacturingOrderResponseDTO)manufacturingOrder));

            _ = messengerService.RegisterMessageAsync("ReloadManufacturingOrderList", async _ => await LoadManufacturingOrdersAsync(true));
            List<string> warehousePermissions = new List<string>();
            var savedPermissions = Properties.Settings.Default.WarehousePermission;
            if (savedPermissions != null)
            {
                warehousePermissions = savedPermissions.Cast<string>().ToList();
            }
            _applicableLocation = string.Join(",", warehousePermissions.Select(id => $"{Uri.EscapeDataString(id)}"));
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
                await LoadManufacturingOrdersAsync();
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
                var result = await _manufacturingOrderService.GetListStatusMasterAsync();
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

        private async Task LoadManufacturingOrdersAsync(bool forceRefresh = false)
        {
            try
            {
                if (!forceRefresh && _lastLoadedPage == CurrentPage && _lastSearchText == SearchText && _lastStatusIndex == SelectedStatusIndex)
                {
                    return;
                }

                var warehouseCodes = Properties.Settings.Default.WarehousePermission?.Cast<string>().ToArray() ?? Array.Empty<string>();
                ApiResult<PagedResponse<ManufacturingOrderResponseDTO>> result;
                if (SelectedStatusIndex > 0 && Statuses[SelectedStatusIndex].StatusId != 0)
                {
                    result = await _manufacturingOrderService.GetAllManufacturingOrdersWithStatusAsync(warehouseCodes, Statuses[SelectedStatusIndex].StatusId, CurrentPage, PageSize);
                }
                else
                {
                    result = await _manufacturingOrderService.GetAllManufacturingOrdersAsync(warehouseCodes, CurrentPage, PageSize);
                }

                if (result.Success && result.Data != null)
                {
                    ManufacturingOrderList.Clear();
                    foreach (var manufacturingOrder in result.Data.Data ?? Enumerable.Empty<ManufacturingOrderResponseDTO>())
                    {
                        ManufacturingOrderList.Add(manufacturingOrder);
                    }
                    TotalPages = result.Data.TotalPages;
                    _lastLoadedPage = CurrentPage;
                    _lastSearchText = SearchText;
                    _lastStatusIndex = SelectedStatusIndex;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải danh sách lệnh sản xuất.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải lệnh sản xuất: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task SearchManufacturingOrdersAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    await LoadManufacturingOrdersAsync();
                    return;
                }

                var warehouseCodes = Properties.Settings.Default.WarehousePermission?.Cast<string>().ToArray() ?? Array.Empty<string>();
                var result = await _manufacturingOrderService.SearchManufacturingOrdersAsync(warehouseCodes, SearchText, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    ManufacturingOrderList.Clear();
                    foreach (var manufacturingOrder in result.Data.Data ?? Enumerable.Empty<ManufacturingOrderResponseDTO>())
                    {
                        ManufacturingOrderList.Add(manufacturingOrder);
                    }
                    TotalPages = result.Data.TotalPages;
                    _lastLoadedPage = CurrentPage;
                    _lastSearchText = SearchText;
                    _lastStatusIndex = SelectedStatusIndex;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tìm kiếm lệnh sản xuất.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tìm kiếm lệnh sản xuất: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task DeleteManufacturingOrderAsync(ManufacturingOrderResponseDTO manufacturingOrder)
        {
            if (manufacturingOrder == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa lệnh sản xuất {manufacturingOrder.ManufacturingOrderCode}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deleteResult = await _manufacturingOrderService.DeleteManufacturingOrderAsync(manufacturingOrder.ManufacturingOrderCode);
                    if (deleteResult.Success)
                    {
                        await LoadManufacturingOrdersAsync(true);
                        _notificationService.ShowMessage("Xóa lệnh sản xuất thành công.", "OK", isError: false);
                        await _messengerService.SendMessageAsync("ReloadManufacturingOrderList");
                    }
                    else
                    {
                        _notificationService.ShowMessage(deleteResult.Message ?? "Không thể xóa lệnh sản xuất.", "OK", isError: true);
                    }
                }
                catch (Exception ex)
                {
                    _notificationService.ShowMessage($"Lỗi khi xóa lệnh sản xuất: {ex.Message}", "OK", isError: true);
                }
            }
        }

        private void OpenEditor(ManufacturingOrderResponseDTO manufacturingOrder)
        {
            var manufacturingOrderDetail = App.ServiceProvider!.GetRequiredService<ucManufacturingOrderDetail>();
            var viewModel = new ManufacturingOrderDetailViewModel(
                App.ServiceProvider!.GetRequiredService<IManufacturingOrderDetailService>(),
                App.ServiceProvider!.GetRequiredService<IManufacturingOrderService>(),
                App.ServiceProvider!.GetRequiredService<IReservationService>(),
                App.ServiceProvider!.GetRequiredService<IPickListService>(),
                App.ServiceProvider!.GetRequiredService<IPutAwayService>(),
                App.ServiceProvider!.GetRequiredService<INotificationService>(),
                App.ServiceProvider!.GetRequiredService<INavigationService>(),
                App.ServiceProvider!.GetRequiredService<IMessengerService>(),
                App.ServiceProvider!.GetRequiredService<ICodeGenerateService>(),
                manufacturingOrder);

            manufacturingOrderDetail.DataContext = viewModel;
            _navigationService.NavigateTo(manufacturingOrderDetail);
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