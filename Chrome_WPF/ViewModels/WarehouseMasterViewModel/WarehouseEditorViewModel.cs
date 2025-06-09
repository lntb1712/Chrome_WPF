using Chrome_WPF;
using Chrome_WPF.Helpers;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.LocationMasterDTO;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.StorageProductDTO;
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
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Chrome_WPF.ViewModels.WarehouseMasterViewModel
{
    public class WarehouseEditorViewModel : BaseViewModel
    {
        private readonly IWarehouseMasterService _warehouseMasterService;
        private readonly ILocationMasterService _locationMasterService;
        private readonly IStorageProductService _storageProductService;
        private readonly INotificationService _notificationService;
        private readonly IMessengerService _messengerService;
        private readonly INavigationService _navigationService;

        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _backCommand;
        private readonly RelayCommand _addLocationLineCommand;
        private readonly RelayCommand _deleteLocationLineCommand;
        private readonly RelayCommand _nextPageCommand;
        private readonly RelayCommand _previousPageCommand;
        private readonly RelayCommand _selectPageCommand;

        private WarehouseMasterRequestDTO _warehouseMasterRequestDTO;
        private ObservableCollection<LocationMasterResponseDTO> _lstLocations;
        private ObservableCollection<StorageProductResponseDTO> _lstStorageProducts;
        private ObservableCollection<object> _displayPages;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;
        private bool _isAddingNew;

        public WarehouseMasterRequestDTO WarehouseMasterRequestDTO
        {
            get => _warehouseMasterRequestDTO;
            set
            {
                if (_warehouseMasterRequestDTO != null)
                {
                    _warehouseMasterRequestDTO.PropertyChanged -= OnPropertyChangedHandler!;
                }
                _warehouseMasterRequestDTO = value;
                if (_warehouseMasterRequestDTO != null)
                {
                    _warehouseMasterRequestDTO.PropertyChanged += OnPropertyChangedHandler!;
                }
                OnPropertyChanged(nameof(WarehouseMasterRequestDTO));
                _ = LoadLocationsAsync();
            }
        }

        public ObservableCollection<LocationMasterResponseDTO> LstLocations
        {
            get => _lstLocations;
            set
            {
                _lstLocations = value;
                OnPropertyChanged(nameof(LstLocations));
            }
        }

        public ObservableCollection<StorageProductResponseDTO> LstStorageProducts
        {
            get => _lstStorageProducts;
            set
            {
                _lstStorageProducts = value;
                OnPropertyChanged(nameof(LstStorageProducts));
            }
        }

        public ObservableCollection<object> DisplayPages
        {
            get => _displayPages;
            set
            {
                _displayPages = value;
                OnPropertyChanged(nameof(DisplayPages));
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
                    OnPropertyChanged(nameof(CurrentPage));
                    UpdateDisplayPages();
                    _ = LoadLocationsAsync();
                }
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                _pageSize = value;
                OnPropertyChanged(nameof(PageSize));
                CurrentPage = 1;
                _ = LoadLocationsAsync();
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                OnPropertyChanged(nameof(TotalPages));
                UpdateDisplayPages();
            }
        }

        public bool IsAddingNew
        {
            get => _isAddingNew;
            set
            {
                _isAddingNew = value;
                OnPropertyChanged(nameof(IsAddingNew));
            }
        }

        public RelayCommand SaveCommand => _saveCommand;
        public RelayCommand BackCommand => _backCommand;
        public RelayCommand AddLocationLineCommand => _addLocationLineCommand;
        public RelayCommand DeleteLocationLineCommand => _deleteLocationLineCommand;
        public RelayCommand NextPageCommand => _nextPageCommand;
        public RelayCommand PreviousPageCommand => _previousPageCommand;
        public RelayCommand SelectPageCommand => _selectPageCommand;

        public WarehouseEditorViewModel(
            IWarehouseMasterService warehouseMasterService,
            ILocationMasterService locationMasterService,
            IStorageProductService storageProductService,
            INotificationService notificationService,
            IMessengerService messengerService,
            INavigationService navigationService,
            bool isAddingNew = true)
        {
            _warehouseMasterService = warehouseMasterService ?? throw new ArgumentNullException(nameof(warehouseMasterService));
            _locationMasterService = locationMasterService ?? throw new ArgumentNullException(nameof(locationMasterService));
            _storageProductService = storageProductService ?? throw new ArgumentNullException(nameof(storageProductService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            _warehouseMasterRequestDTO = new WarehouseMasterRequestDTO();
            _lstLocations = new ObservableCollection<LocationMasterResponseDTO>();
            _lstStorageProducts = new ObservableCollection<StorageProductResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            _currentPage = 1;
            _isAddingNew = isAddingNew;

            _saveCommand = new RelayCommand(async parameter => await SaveAsync(parameter), CanSave);
            _backCommand = new RelayCommand(NavigateBack);
            _addLocationLineCommand = new RelayCommand(AddLocationLine, CanAddLocationLine);
            _deleteLocationLineCommand = new RelayCommand(async location => await DeleteLocationLineAsync((LocationMasterResponseDTO)location));
            _nextPageCommand = new RelayCommand(_ => NextPage());
            _previousPageCommand = new RelayCommand(_ => PreviousPage());
            _selectPageCommand = new RelayCommand(page => SelectPage((int)page));

            _warehouseMasterRequestDTO.PropertyChanged += OnPropertyChangedHandler!;

            _ = LoadStorageProductsAsync();
        }

        private async Task SaveAsync(object parameter)
        {
            try
            {
                WarehouseMasterRequestDTO.RequestValidation();
                if (!CanSave(parameter))
                {
                    _notificationService.ShowMessage("Vui lòng kiểm tra lại thông tin nhập vào.", "OK", isError: true);
                    return;
                }

                // Save warehouse
                ApiResult<bool> result;
                if (IsAddingNew)
                {
                    result = await _warehouseMasterService.AddWarehouseMaster(WarehouseMasterRequestDTO);
                }
                else
                {
                    result = await _warehouseMasterService.UpdateWarehouseMaster(WarehouseMasterRequestDTO);
                }

                if (!result.Success)
                {
                    _notificationService.ShowMessage(result.Message ?? (IsAddingNew ? "Lỗi khi thêm kho." : "Lỗi khi cập nhật kho."), "OK", isError: true);
                    return;
                }

                // Save locations
                foreach (var location in LstLocations)
                {
                    if (string.IsNullOrEmpty(location.LocationCode))
                    {
                        _notificationService.ShowMessage($"Vị trí {location.LocationName} thiếu mã vị trí.", "OK", isError: true);
                        return;
                    }

                    var locationRequest = new LocationMasterRequestDTO
                    {
                        WarehouseCode = WarehouseMasterRequestDTO.WarehouseCode,
                        LocationCode = location.LocationCode, // Save full WarehouseCode/LocationCode
                        LocationName = location.LocationName,
                        StorageProductId = location.SelectedStorageProduct?.StorageProductId ?? location.StorageProductId
                    };

                    var existingLocations = await _locationMasterService.GetAllLocationMaster(WarehouseMasterRequestDTO.WarehouseCode, 1, int.MaxValue);
                    var exists = existingLocations.Success && existingLocations.Data?.Data.Any(l => l.LocationCode == location.LocationCode) == true;

                    if (exists)
                    {
                        var updateResult = await _locationMasterService.UpdateLocationMaster(locationRequest);
                        if (!updateResult.Success)
                        {
                            _notificationService.ShowMessage($"Lỗi khi cập nhật vị trí {location.LocationName}: {updateResult.Message}", "OK", isError: true);
                            return;
                        }
                    }
                    else
                    {
                        var addResult = await _locationMasterService.AddLocationMaster(locationRequest);
                        if (!addResult.Success)
                        {
                            _notificationService.ShowMessage($"Lỗi khi thêm vị trí {location.LocationName}: {addResult.Message}", "OK", isError: true);
                            return;
                        }
                    }
                }

                _notificationService.ShowMessage(result.Message ?? (IsAddingNew ? "Thêm kho và vị trí thành công!" : "Cập nhật kho và vị trí thành công!"), "OK", isError: false);
                if (IsAddingNew)
                {
                    WarehouseMasterRequestDTO.ClearValidation();
                    WarehouseMasterRequestDTO = new WarehouseMasterRequestDTO();
                    LstLocations.Clear();
                }
                await _messengerService.SendMessageAsync("ReloadWarehouseList");
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private bool CanSave(object parameter)
        {
            var dto = WarehouseMasterRequestDTO;
            var propertiesToValidate = new[] { nameof(dto.WarehouseCode), nameof(dto.WarehouseName) };

            foreach (var prop in propertiesToValidate)
            {
                if (!string.IsNullOrEmpty(dto[prop]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CanAddLocationLine(object parameter)
        {
            return !string.IsNullOrEmpty(WarehouseMasterRequestDTO.WarehouseCode);
        }

        private void NavigateBack(object parameter)
        {
            var ucWarehouseMaster = App.ServiceProvider!.GetRequiredService<ucWarehouseMaster>();
            _navigationService.NavigateTo<ucWarehouseMaster>();
        }

        private async Task LoadStorageProductsAsync()
        {
            try
            {
                var response = await _storageProductService.GetAllStorageProducts(1, int.MaxValue);
                if (response.Success && response.Data?.Data != null)
                {
                    LstStorageProducts.Clear();
                    foreach (var item in response.Data.Data)
                    {
                        LstStorageProducts.Add(item);
                    }
                    OnPropertyChanged(nameof(LstStorageProducts));
                }
                else
                {
                    _notificationService.ShowMessage(response.Message ?? "Lỗi khi tải danh sách sản phẩm lưu trữ", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadLocationsAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(WarehouseMasterRequestDTO.WarehouseCode))
                {
                    LstLocations.Clear();
                    return;
                }

                var result = await _locationMasterService.GetAllLocationMaster(WarehouseMasterRequestDTO.WarehouseCode, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    LstLocations.Clear();
                    foreach (var item in result.Data.Data ?? Enumerable.Empty<LocationMasterResponseDTO>())
                    {
                        var storageProduct = LstStorageProducts.FirstOrDefault(sp => sp.StorageProductId == item.StorageProductId);
                        item.SelectedStorageProduct = storageProduct;
                        item.IsNewRow = false;
                        item.WarehouseCode = WarehouseMasterRequestDTO.WarehouseCode; // Ensure WarehouseCode is set
                        LstLocations.Add(item);
                    }
                    TotalPages = result.Data.TotalPages;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách vị trí", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private void AddLocationLine(object parameter)
        {
            if (string.IsNullOrEmpty(WarehouseMasterRequestDTO.WarehouseCode))
            {
                _notificationService.ShowMessage("Vui lòng nhập mã kho trước khi thêm vị trí.", "OK", isError: true);
                return;
            }

            var newLocation = new LocationMasterResponseDTO
            {
                WarehouseCode = WarehouseMasterRequestDTO.WarehouseCode,
                LocationCode = $"{WarehouseMasterRequestDTO.WarehouseCode}/",
                LocationCodeSuffix = string.Empty,
                LocationName = string.Empty,
                StorageProductId = string.Empty,
                StorageProductName = string.Empty,
                IsNewRow = true,
                SelectedStorageProduct = null
            };
            LstLocations.Add(newLocation);
            _messengerService.SendMessageAsync("FocusNewLocationRow");
        }

        private async Task DeleteLocationLineAsync(LocationMasterResponseDTO location)
        {
            if (location == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa vị trí {location.LocationName}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deleteResult = await _locationMasterService.DeleteLocationMaster(location.WarehouseCode, location.LocationCode);
                    if (deleteResult.Success)
                    {
                        _ = LoadLocationsAsync();
                        _notificationService.ShowMessage(deleteResult.Message, "OK", isError: false);
                    }
                    else
                    {
                        _notificationService.ShowMessage(deleteResult.Message, "OK", isError: true);
                    }
                }
                catch (Exception ex)
                {
                    _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
                }
            }
        }

        private void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
            }
        }

        private void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
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

        private void OnPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            _saveCommand?.RaiseCanExecuteChanged();
            _backCommand?.RaiseCanExecuteChanged();
            _addLocationLineCommand?.RaiseCanExecuteChanged();
            _deleteLocationLineCommand?.RaiseCanExecuteChanged();
        }
    }
}