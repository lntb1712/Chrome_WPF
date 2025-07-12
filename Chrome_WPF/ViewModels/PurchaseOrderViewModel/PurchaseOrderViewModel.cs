using Chrome_WPF.Helpers;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.PurchaseOrderDTO;
using Chrome_WPF.Models.StatusMasterDTO;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.PurchaseOrderDetailService;
using Chrome_WPF.Services.PurchaseOrderService;
using Chrome_WPF.Views.UserControls.PurchaseOrder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.PurchaseOrderViewModel
{
    public class PurchaseOrderViewModel : BaseViewModel
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IMessengerService _messengerService;

        private ObservableCollection<PurchaseOrderResponseDTO> _purchaseOrderList;
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

        public ObservableCollection<PurchaseOrderResponseDTO> PurchaseOrderList
        {
            get => _purchaseOrderList;
            set
            {
                _purchaseOrderList = value;
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
                    _ = LoadPurchaseOrdersAsync();
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
                    _ = LoadPurchaseOrdersAsync();
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
                    _ = LoadPurchaseOrdersAsync();
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
                    _ = LoadPurchaseOrdersAsync();
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

        public PurchaseOrderViewModel(
            IPurchaseOrderService purchaseOrderService,
            INotificationService notificationService,
            INavigationService navigationService,
            IMessengerService messengerService)
        {
            _purchaseOrderService = purchaseOrderService ?? throw new ArgumentNullException(nameof(purchaseOrderService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));

            _purchaseOrderList = new ObservableCollection<PurchaseOrderResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            _statuses = new ObservableCollection<StatusMasterResponseDTO>();
            _searchText = string.Empty;
            _currentPage = 1;
            _selectedStatusIndex = 0;
            _lastLoadedPage = 0;
            _lastSearchText = string.Empty;
            _lastStatusIndex = 0;

            SearchCommand = new RelayCommand(async _ => await SearchPurchaseOrdersAsync());
            RefreshCommand = new RelayCommand(async _ => await LoadPurchaseOrdersAsync(true));
            AddCommand = new RelayCommand(_ => OpenEditor(null!));
            DeleteCommand = new RelayCommand(async purchaseOrder => await DeletePurchaseOrderAsync((PurchaseOrderResponseDTO)purchaseOrder));
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            ViewDetailCommand = new RelayCommand(purchaseOrder => OpenEditor((PurchaseOrderResponseDTO)purchaseOrder));

            _ = messengerService.RegisterMessageAsync("ReloadPurchaseOrderList", async _ => await LoadPurchaseOrdersAsync(true));
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
                await LoadPurchaseOrdersAsync();
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
                var result = await _purchaseOrderService.GetListStatusMaster();
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

        private async Task LoadPurchaseOrdersAsync(bool forceRefresh = false)
        {
            try
            {
                if (!forceRefresh && _lastLoadedPage == CurrentPage && _lastSearchText == SearchText && _lastStatusIndex == SelectedStatusIndex)
                {
                    return;
                }

                ApiResult<PagedResponse<PurchaseOrderResponseDTO>> result;
                if (SelectedStatusIndex > 0 && Statuses[SelectedStatusIndex].StatusId != 0)
                {
                    result = await _purchaseOrderService.GetAllPurchaseOrdersWithStatus(Statuses[SelectedStatusIndex].StatusId, CurrentPage, PageSize);
                }
                else
                {
                    result = await _purchaseOrderService.GetAllPurchaseOrders(CurrentPage, PageSize);
                }

                if (result.Success && result.Data != null)
                {
                    PurchaseOrderList.Clear();
                    foreach (var purchaseOrder in result.Data.Data ?? Enumerable.Empty<PurchaseOrderResponseDTO>())
                    {
                        PurchaseOrderList.Add(purchaseOrder);
                    }
                    TotalPages = result.Data.TotalPages;
                    _lastLoadedPage = CurrentPage;
                    _lastSearchText = SearchText;
                    _lastStatusIndex = SelectedStatusIndex;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải danh sách phiếu đặt hàng.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải phiếu đặt hàng: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task SearchPurchaseOrdersAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    await LoadPurchaseOrdersAsync();
                    return;
                }

                var result = await _purchaseOrderService.SearchPurchaseOrderAsync(SearchText, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    PurchaseOrderList.Clear();
                    foreach (var purchaseOrder in result.Data.Data ?? Enumerable.Empty<PurchaseOrderResponseDTO>())
                    {
                        PurchaseOrderList.Add(purchaseOrder);
                    }
                    TotalPages = result.Data.TotalPages;
                    _lastLoadedPage = CurrentPage;
                    _lastSearchText = SearchText;
                    _lastStatusIndex = SelectedStatusIndex;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tìm kiếm phiếu đặt hàng.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tìm kiếm phiếu đặt hàng: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task DeletePurchaseOrderAsync(PurchaseOrderResponseDTO purchaseOrder)
        {
            if (purchaseOrder == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa phiếu đặt hàng {purchaseOrder.PurchaseOrderCode}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deleteResult = await _purchaseOrderService.DeletePurchaseOrderAsync(purchaseOrder.PurchaseOrderCode);
                    if (deleteResult.Success)
                    {
                        await LoadPurchaseOrdersAsync(true);
                        _notificationService.ShowMessage("Xóa phiếu đặt hàng thành công.", "OK", isError: false);
                        await _messengerService.SendMessageAsync("ReloadPurchaseOrderList");
                    }
                    else
                    {
                        _notificationService.ShowMessage(deleteResult.Message ?? "Không thể xóa phiếu đặt hàng.", "OK", isError: true);
                    }
                }
                catch (Exception ex)
                {
                    _notificationService.ShowMessage($"Lỗi khi xóa phiếu đặt hàng: {ex.Message}", "OK", isError: true);
                }
            }
        }

        private void OpenEditor(PurchaseOrderResponseDTO purchaseOrder)
        {
            var purchaseOrderDetail = App.ServiceProvider!.GetRequiredService<ucPurchaseOrderDetail>();
            var viewModel = new PurchaseOrderDetailViewModel(
                App.ServiceProvider!.GetRequiredService<IPurchaseOrderDetailService>(),
                App.ServiceProvider!.GetRequiredService<IPurchaseOrderService>(),
                App.ServiceProvider!.GetRequiredService<INotificationService>(),
                App.ServiceProvider!.GetRequiredService<INavigationService>(),
                App.ServiceProvider!.GetRequiredService<IMessengerService>(),
                purchaseOrder);

            purchaseOrderDetail.DataContext = viewModel;
            _navigationService.NavigateTo(purchaseOrderDetail);
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