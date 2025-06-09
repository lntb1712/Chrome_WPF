using Chrome_WPF.Helpers;
using Chrome_WPF.Models.WarehouseMasterDTO;
using Chrome_WPF.Services.LocationMasterService;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.StorageProductService;
using Chrome_WPF.Services.WarehouseMasterService;
using Chrome_WPF.Views.UserControls.WarehouseMaster;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.WarehouseMasterViewModel
{
    public class WarehouseMasterViewModel : BaseViewModel
    {
        private readonly IWarehouseMasterService _warehouseMasterService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IMessengerService _messengerService;

        private ObservableCollection<WarehouseMasterResponseDTO> _warehouseList;
        private ObservableCollection<object> _displayPages;
        private string _searchText;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;
        private WarehouseMasterResponseDTO _selectedWarehouse;
        private WarehouseMasterRequestDTO _warehouseMasterRequestDTO;
        private int _totalWarehousesCount;

        public ObservableCollection<WarehouseMasterResponseDTO> WarehouseList
        {
            get => _warehouseList;
            set
            {
                _warehouseList = value;
                OnPropertyChanged();
            }
        }

        public WarehouseMasterRequestDTO WarehouseMasterRequestDTO
        {
            get => _warehouseMasterRequestDTO;
            set
            {
                _warehouseMasterRequestDTO = value;
                OnPropertyChanged();
            }
        }

        public WarehouseMasterResponseDTO SelectedWarehouse
        {
            get => _selectedWarehouse;
            set
            {
                _selectedWarehouse = value;
                OnPropertyChanged();
                if (SelectedWarehouse != null)
                {
                    WarehouseMasterRequestDTO = new WarehouseMasterRequestDTO
                    {
                        WarehouseCode = SelectedWarehouse.WarehouseCode,
                        WarehouseName = SelectedWarehouse.WarehouseName ?? string.Empty,
                        WarehouseDescription = SelectedWarehouse?.WarehouseDescription ?? string.Empty,
                        WarehouseAddress = SelectedWarehouse!.WarehouseAddress ?? string.Empty
                    };
                }
                else
                {
                    WarehouseMasterRequestDTO = new WarehouseMasterRequestDTO();
                }
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

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
                UpdateDisplayPages();
                _ = LoadWarehousesAsync();
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                _pageSize = value;
                OnPropertyChanged();
                CurrentPage = 1;
                _ = LoadWarehousesAsync();
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

        public int TotalWarehousesCount
        {
            get => _totalWarehousesCount;
            set
            {
                _totalWarehousesCount = value;
                OnPropertyChanged();
            }
        }

        public ICommand SearchCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand SelectPageCommand { get; }

        public WarehouseMasterViewModel(
            IWarehouseMasterService warehouseMasterService,
            INotificationService notificationService,
            INavigationService navigationService,
            IMessengerService messengerService)
        {
            _warehouseMasterService = warehouseMasterService ?? throw new ArgumentNullException(nameof(warehouseMasterService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));
            _warehouseList = new ObservableCollection<WarehouseMasterResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            _warehouseMasterRequestDTO = new WarehouseMasterRequestDTO();
            _selectedWarehouse = null!;
            _currentPage = 1;
            _searchText = string.Empty;

            SearchCommand = new RelayCommand(async _ => await SearchWarehousesAsync());
            AddCommand = new RelayCommand(_ => OpenEditor(null));
            DeleteCommand = new RelayCommand(async warehouse => await DeleteWarehouseAsync((WarehouseMasterResponseDTO)warehouse));
            UpdateCommand = new RelayCommand(warehouse => OpenEditor((WarehouseMasterResponseDTO)warehouse));
            RefreshCommand = new RelayCommand(async _ => await LoadWarehousesAsync());
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));

            _ = messengerService.RegisterMessageAsync("ReloadWarehouseListMessage", async (obj) =>
            {
                await LoadWarehousesAsync();
            });

            _ = LoadWarehousesAsync();
        }

        private async Task SearchWarehousesAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    await LoadWarehousesAsync();
                    return;
                }

                var result = await _warehouseMasterService.SearchWarehouseMaster(SearchText, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    WarehouseList.Clear();
                    foreach (var warehouse in result.Data.Data ?? Enumerable.Empty<WarehouseMasterResponseDTO>())
                    {
                        WarehouseList.Add(warehouse);
                    }
                    TotalPages = result.Data.TotalPages;
                    OnPropertyChanged(nameof(WarehouseList));
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tìm kiếm kho.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Error: {ex.Message}", "OK", isError: true);
            }
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
            DisplayPages = new ObservableCollection<object>();
            if (TotalPages <= 0)
                return;

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

        private async Task DeleteWarehouseAsync(WarehouseMasterResponseDTO warehouse)
        {
            if (warehouse == null)
                return;

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa kho {warehouse.WarehouseName} không?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deleteResult = await _warehouseMasterService.DeleteWarehouseMaster(warehouse.WarehouseCode);
                    if (deleteResult.Success)
                    {
                        _notificationService.ShowMessage($"Đã xóa kho {warehouse.WarehouseName} thành công.", "OK", isError: false);
                        await LoadWarehousesAsync();
                    }
                    else
                    {
                        _notificationService.ShowMessage(deleteResult.Message ?? "Lỗi khi xóa kho.", "OK", isError: true);
                    }
                }
                catch (Exception ex)
                {
                    _notificationService.ShowMessage($"Error: {ex.Message}", "OK", isError: true);
                }
            }
        }

        private void OpenEditor(WarehouseMasterResponseDTO? warehouse)
        {
            var warehouseEditor = App.ServiceProvider!.GetRequiredService<ucWarehouseEditor>();

            // Create DTO based on whether we're adding or editing
            var dto = warehouse == null
                ? new WarehouseMasterRequestDTO()
                : new WarehouseMasterRequestDTO
                {
                    WarehouseCode = warehouse.WarehouseCode,
                    WarehouseName = warehouse.WarehouseName ?? string.Empty,
                    WarehouseDescription = warehouse.WarehouseDescription ?? string.Empty,
                    WarehouseAddress = warehouse.WarehouseAddress ?? string.Empty
                };

            // Instantiate ViewModel with all required services
            var viewModel = new WarehouseEditorViewModel(
                App.ServiceProvider!.GetRequiredService<IWarehouseMasterService>(),
                App.ServiceProvider!.GetRequiredService<ILocationMasterService>(),
                App.ServiceProvider!.GetRequiredService<IStorageProductService>(),
                App.ServiceProvider!.GetRequiredService<INotificationService>(),
                App.ServiceProvider!.GetRequiredService<IMessengerService>(),
                App.ServiceProvider!.GetRequiredService<INavigationService>(),
                isAddingNew: warehouse == null);

            // Set the DTO after ViewModel creation
            viewModel.WarehouseMasterRequestDTO = dto;

            // Set DataContext and navigate
            warehouseEditor.DataContext = viewModel;
            _navigationService.NavigateTo(warehouseEditor);
        }

        private async Task TotalWarehousesAsync()
        {
            try
            {
                var result = await _warehouseMasterService.GetTotalWarehouseCount();
                if (result.Success)
                {
                    TotalWarehousesCount = result.Data;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi lấy tổng số kho.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadWarehousesAsync()
        {
            try
            {
                var result = await _warehouseMasterService.GetAllWarehouseMaster(CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    WarehouseList.Clear();
                    foreach (var warehouse in result.Data.Data ?? Enumerable.Empty<WarehouseMasterResponseDTO>())
                    {
                        WarehouseList.Add(warehouse);
                    }
                    TotalPages = result.Data.TotalPages;
                    _ = TotalWarehousesAsync();
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách kho.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Error: {ex.Message}", "OK", isError: true);
            }
        }
    }
}