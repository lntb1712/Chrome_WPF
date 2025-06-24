using Chrome_WPF.Helpers;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.MovementDTO;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.StatusMasterDTO;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.MovementDetailService;
using Chrome_WPF.Services.MovementService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Views.UserControls.Movement;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.MovementViewModel
{
    public class MovementViewModel : BaseViewModel
    {
        private readonly IMovementService _movementService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IMessengerService _messengerService;

        private ObservableCollection<MovementResponseDTO> _movementList;
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

        public ObservableCollection<MovementResponseDTO> MovementList
        {
            get => _movementList;
            set
            {
                _movementList = value;
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
                    _ = LoadMovementsAsync();
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
                    _ = LoadMovementsAsync();
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
                    _ = LoadMovementsAsync();
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
                    _ = LoadMovementsAsync();
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

        public MovementViewModel(
            IMovementService movementService,
            INotificationService notificationService,
            INavigationService navigationService,
            IMessengerService messengerService)
        {
            _movementService = movementService ?? throw new ArgumentNullException(nameof(movementService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));

            _movementList = new ObservableCollection<MovementResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            _statuses = new ObservableCollection<StatusMasterResponseDTO>();
            _searchText = string.Empty;
            _currentPage = 1;
            _selectedStatusIndex = 0;
            _lastLoadedPage = 0;
            _lastSearchText = string.Empty;
            _lastStatusIndex = 0;

            SearchCommand = new RelayCommand(async _ => await SearchMovementsAsync());
            RefreshCommand = new RelayCommand(async _ => await LoadMovementsAsync(true));
            AddCommand = new RelayCommand(_ => OpenEditor(null));
            DeleteCommand = new RelayCommand(async movement => await DeleteMovementAsync((MovementResponseDTO)movement));
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            ViewDetailCommand = new RelayCommand(movement => OpenEditor((MovementResponseDTO)movement));

            _ = messengerService.RegisterMessageAsync("ReloadMovementList", async _ => await LoadMovementsAsync(true));
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
                await LoadMovementsAsync();
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
                var result = await _movementService.GetListStatusMaster();
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

        private async Task LoadMovementsAsync(bool forceRefresh = false)
        {
            try
            {
                if (!forceRefresh && _lastLoadedPage == CurrentPage && _lastSearchText == SearchText && _lastStatusIndex == SelectedStatusIndex)
                {
                    return;
                }

                var warehouseCodes = ApplicableLocation.Split(',').Select(Uri.UnescapeDataString).ToArray();
                ApiResult<PagedResponse<MovementResponseDTO>> result;
                if (SelectedStatusIndex > 0 && Statuses[SelectedStatusIndex].StatusId != 0)
                {
                    result = await _movementService.GetAllMovementsWithStatus(warehouseCodes, Statuses[SelectedStatusIndex].StatusId, CurrentPage, PageSize);
                }
                else
                {
                    result = await _movementService.GetAllMovements(warehouseCodes, CurrentPage, PageSize);
                }

                if (result.Success && result.Data != null)
                {
                    MovementList.Clear();
                    foreach (var movement in result.Data.Data ?? Enumerable.Empty<MovementResponseDTO>())
                    {
                        MovementList.Add(movement);
                    }
                    TotalPages = result.Data.TotalPages;
                    _lastLoadedPage = CurrentPage;
                    _lastSearchText = SearchText;
                    _lastStatusIndex = SelectedStatusIndex;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải danh sách phiếu di chuyển.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải phiếu di chuyển: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task SearchMovementsAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    await LoadMovementsAsync();
                    return;
                }

                var warehouseCodes = ApplicableLocation.Split(',').Select(Uri.UnescapeDataString).ToArray();
                var result = await _movementService.SearchMovementAsync(warehouseCodes, SearchText, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    MovementList.Clear();
                    foreach (var movement in result.Data.Data ?? Enumerable.Empty<MovementResponseDTO>())
                    {
                        MovementList.Add(movement);
                    }
                    TotalPages = result.Data.TotalPages;
                    _lastLoadedPage = CurrentPage;
                    _lastSearchText = SearchText;
                    _lastStatusIndex = SelectedStatusIndex;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tìm kiếm phiếu di chuyển.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tìm kiếm phiếu di chuyển: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task DeleteMovementAsync(MovementResponseDTO movement)
        {
            if (movement == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa phiếu di chuyển {movement.MovementCode}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deleteResult = await _movementService.DeleteMovementAsync(movement.MovementCode);
                    if (deleteResult.Success)
                    {
                        await LoadMovementsAsync(true);
                        _notificationService.ShowMessage("Xóa phiếu di chuyển thành công.", "OK", isError: false);
                        await _messengerService.SendMessageAsync("ReloadMovementList");
                    }
                    else
                    {
                        _notificationService.ShowMessage(deleteResult.Message ?? "Không thể xóa phiếu di chuyển.", "OK", isError: true);
                    }
                }
                catch (Exception ex)
                {
                    _notificationService.ShowMessage($"Lỗi khi xóa phiếu di chuyển: {ex.Message}", "OK", isError: true);
                }
            }
        }

        private void OpenEditor(MovementResponseDTO movement)
        {
            var movementDetail = App.ServiceProvider!.GetRequiredService<ucMovementDetail>();
            var viewModel = new MovementDetailViewModel(
                App.ServiceProvider!.GetRequiredService<IMovementService>(),
                App.ServiceProvider!.GetRequiredService<IMovementDetailService>(),
                App.ServiceProvider!.GetRequiredService<INotificationService>(),
                App.ServiceProvider!.GetRequiredService<INavigationService>(),
                App.ServiceProvider!.GetRequiredService<IMessengerService>(),
                movement);

            movementDetail.DataContext = viewModel;
            _navigationService.NavigateTo(movementDetail);
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