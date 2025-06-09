using Chrome_WPF;
using Chrome_WPF.Helpers;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.LocationMasterDTO;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Models.PutAwayRulesDTO;
using Chrome_WPF.Models.StorageProductDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using Chrome_WPF.Services.LocationMasterService;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.ProductMasterService;
using Chrome_WPF.Services.PutAwayRulesService;
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
using System.Windows.Controls;

namespace Chrome_WPF.ViewModels.WarehouseMasterViewModel
{
    public class PutAwayRulesViewModel : BaseViewModel
    {
        private readonly IPutAwayRulesService _putAwayRulesService;
        private readonly IWarehouseMasterService _warehouseMasterService;
        private readonly IProductMasterService _productMasterService;
        private readonly ILocationMasterService _locationMasterService;
        private readonly IStorageProductService _storageProductService;
        private readonly INotificationService _notificationService;
        private readonly IMessengerService _messengerService;
        private readonly INavigationService _navigationService;

        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _backCommand;
        private readonly RelayCommand _addPutAwayRuleLineCommand;
        private readonly RelayCommand _deletePutAwayRuleLineCommand;
        private readonly RelayCommand _nextPageCommand;
        private readonly RelayCommand _previousPageCommand;
        private readonly RelayCommand _selectPageCommand;

        private PutAwayRulesRequestDTO _putAwayRuleRequestDTO;
        private ObservableCollection<PutAwayRulesResponseDTO> _lstPutAwayRules;
        private ObservableCollection<WarehouseMasterResponseDTO> _availableWarehouses;
        private ObservableCollection<ProductMasterResponseDTO> _availableProducts;
        private ObservableCollection<LocationMasterResponseDTO> _availableLocations;
        private ObservableCollection<StorageProductResponseDTO> _availableStorageProducts;
        private ObservableCollection<object> _displayPages;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;
        private HashSet<string> _existingPutAwayRuleCodes = new HashSet<string>();

        public PutAwayRulesRequestDTO PutAwayRuleRequestDTO
        {
            get => _putAwayRuleRequestDTO;
            set
            {
                if (_putAwayRuleRequestDTO != null)
                {
                    _putAwayRuleRequestDTO.PropertyChanged -= OnPropertyChangedHandler!;
                }
                _putAwayRuleRequestDTO = value;
                if (_putAwayRuleRequestDTO != null)
                {
                    _putAwayRuleRequestDTO.PropertyChanged += OnPropertyChangedHandler!;
                }
                OnPropertyChanged(nameof(PutAwayRuleRequestDTO));
                _ = LoadComboBoxDataAsync();
            }
        }

        public ObservableCollection<PutAwayRulesResponseDTO> LstPutAwayRules
        {
            get => _lstPutAwayRules;
            set
            {
                _lstPutAwayRules = value;
                OnPropertyChanged(nameof(LstPutAwayRules));
            }
        }

        public ObservableCollection<WarehouseMasterResponseDTO> AvailableWarehouses
        {
            get => _availableWarehouses;
            set
            {
                _availableWarehouses = value;
                OnPropertyChanged(nameof(AvailableWarehouses));
            }
        }

        public ObservableCollection<ProductMasterResponseDTO> AvailableProducts
        {
            get => _availableProducts;
            set
            {
                _availableProducts = value;
                OnPropertyChanged(nameof(AvailableProducts));
            }
        }

        public ObservableCollection<LocationMasterResponseDTO> AvailableLocations
        {
            get => _availableLocations;
            set
            {
                _availableLocations = value;
                OnPropertyChanged(nameof(AvailableLocations));
            }
        }

        public ObservableCollection<StorageProductResponseDTO> AvailableStorageProducts
        {
            get => _availableStorageProducts;
            set
            {
                _availableStorageProducts = value;
                OnPropertyChanged(nameof(AvailableStorageProducts));
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
                    _ = LoadPutAwayRulesAsync();
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
                _ = LoadPutAwayRulesAsync();
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

        public RelayCommand SaveCommand => _saveCommand;
        public RelayCommand BackCommand => _backCommand;
        public RelayCommand AddPutAwayRuleLineCommand => _addPutAwayRuleLineCommand;
        public RelayCommand DeletePutAwayRuleLineCommand => _deletePutAwayRuleLineCommand;
        public RelayCommand NextPageCommand => _nextPageCommand;
        public RelayCommand PreviousPageCommand => _previousPageCommand;
        public RelayCommand SelectPageCommand => _selectPageCommand;

        public PutAwayRulesViewModel(
            IPutAwayRulesService putAwayRulesService,
            IWarehouseMasterService warehouseMasterService,
            IProductMasterService productMasterService,
            ILocationMasterService locationMasterService,
            IStorageProductService storageProductService,
            INotificationService notificationService,
            IMessengerService messengerService,
            INavigationService navigationService)
        {
            _putAwayRulesService = putAwayRulesService ?? throw new ArgumentNullException(nameof(putAwayRulesService));
            _warehouseMasterService = warehouseMasterService ?? throw new ArgumentNullException(nameof(warehouseMasterService));
            _productMasterService = productMasterService ?? throw new ArgumentNullException(nameof(productMasterService));
            _locationMasterService = locationMasterService ?? throw new ArgumentNullException(nameof(locationMasterService));
            _storageProductService = storageProductService ?? throw new ArgumentNullException(nameof(storageProductService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            _lstPutAwayRules = new ObservableCollection<PutAwayRulesResponseDTO>();
            _availableWarehouses = new ObservableCollection<WarehouseMasterResponseDTO>();
            _availableProducts = new ObservableCollection<ProductMasterResponseDTO>();
            _availableLocations = new ObservableCollection<LocationMasterResponseDTO>();
            _availableStorageProducts = new ObservableCollection<StorageProductResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            _currentPage = 1;
            _putAwayRuleRequestDTO = new PutAwayRulesRequestDTO();
            _saveCommand = new RelayCommand(async parameter => await SaveAsync(parameter), CanSave);
            _backCommand = new RelayCommand(NavigateBack);
            _addPutAwayRuleLineCommand = new RelayCommand(AddPutAwayRuleLine);
            _deletePutAwayRuleLineCommand = new RelayCommand(async rule => await DeletePutAwayRuleLineAsync((PutAwayRulesResponseDTO)rule));
            _nextPageCommand = new RelayCommand(_ => NextPage());
            _previousPageCommand = new RelayCommand(_ => PreviousPage());
            _selectPageCommand = new RelayCommand(page => SelectPage((int)page));
            _existingPutAwayRuleCodes = new HashSet<string>();
            _lstPutAwayRules.CollectionChanged += LstPutAwayRules_CollectionChanged!;
            _ = LoadPutAwayRulesAsync();
            _ = LoadComboBoxDataAsync();
        }

        private void LstPutAwayRules_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (PutAwayRulesResponseDTO newRule in e.NewItems)
                {
                    newRule.PropertyChanged += async (s, args) =>
                    {
                        if (args.PropertyName == nameof(PutAwayRulesResponseDTO.SelectedWarehouse))
                        {
                            await LoadLocationsForRuleAsync(newRule);
                        }
                    };
                }
            }
        }

        private async Task LoadLocationsForRuleAsync(PutAwayRulesResponseDTO rule)
        {
            try
            {
                if (rule.SelectedWarehouse?.WarehouseCode == null)
                {
                    // Clear locations if no warehouse is selected
                    rule.SelectedLocation = null;
                    return;
                }

                // Fetch locations filtered by the selected warehouse
                var locationResult = await _locationMasterService.GetAllLocationMaster(rule.SelectedWarehouse.WarehouseCode,1, int.MaxValue);
                if (locationResult.Success && locationResult.Data != null)
                {
                    // Assuming you want to update a specific list for the rule, you may need to adjust your data structure
                    // For simplicity, we'll update AvailableLocations, but you might want a separate collection per rule
                    AvailableLocations.Clear();
                    foreach (var item in locationResult.Data.Data ?? Enumerable.Empty<LocationMasterResponseDTO>())
                    {
                        AvailableLocations.Add(item);
                    }

                    // Optionally clear the selected location if it no longer matches the new warehouse
                    if (rule.SelectedLocation != null && !AvailableLocations.Any(l => l.LocationCode == rule.SelectedLocation.LocationCode))
                    {
                        rule.SelectedLocation = null;
                    }
                }
                else
                {
                    _notificationService.ShowMessage(locationResult.Message ?? "Lỗi khi tải danh sách vị trí", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadComboBoxDataAsync()
        {
            try
            {
                // Tạo các task load dữ liệu song song
                var warehouseTask = _warehouseMasterService.GetAllWarehouseMaster(1, int.MaxValue);
                var productTask = _productMasterService.GetAllProductMaster(1, int.MaxValue);
                var storageProductTask = _storageProductService.GetAllStorageProducts(1, int.MaxValue);

                // Đợi tất cả task hoàn thành
                await Task.WhenAll(warehouseTask, productTask, storageProductTask);

                // Xử lý kết quả từng cái
                var warehouseResult = await warehouseTask;
                var productResult = await productTask;
                var storageProductResult = await storageProductTask;

                // Warehouse
                if (warehouseResult.Success && warehouseResult.Data != null)
                {
                    AvailableWarehouses.Clear();
                    foreach (var item in warehouseResult.Data.Data ?? Enumerable.Empty<WarehouseMasterResponseDTO>())
                    {
                        AvailableWarehouses.Add(item);
                    }
                }
                else
                {
                    _notificationService.ShowMessage(warehouseResult.Message ?? "Lỗi khi tải danh sách kho", "OK", isError: true);
                }

                // Product
                if (productResult.Success && productResult.Data != null)
                {
                    AvailableProducts.Clear();
                    foreach (var item in productResult.Data.Data ?? Enumerable.Empty<ProductMasterResponseDTO>())
                    {
                        AvailableProducts.Add(item);
                    }
                }
                else
                {
                    _notificationService.ShowMessage(productResult.Message ?? "Lỗi khi tải danh sách sản phẩm", "OK", isError: true);
                }

                // Storage Product
                if (storageProductResult.Success && storageProductResult.Data != null)
                {
                    AvailableStorageProducts.Clear();
                    foreach (var item in storageProductResult.Data.Data ?? Enumerable.Empty<StorageProductResponseDTO>())
                    {
                        AvailableStorageProducts.Add(item);
                    }
                }
                else
                {
                    _notificationService.ShowMessage(storageProductResult.Message ?? "Lỗi khi tải danh sách định mức", "OK", isError: true);
                }

            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }


        private void AddPutAwayRuleLine(object parameter)
        {
            var newRule = new PutAwayRulesResponseDTO
            {
                PutAwayRuleCode = string.Empty,
                WarehouseToApply = string.Empty,
                ProductCode = string.Empty,
                LocationCode = string.Empty,
                StorageProductId = string.Empty,
                IsNewRow = true
            };
            LstPutAwayRules.Add(newRule);
            _messengerService.SendMessageAsync("FocusNewPutAwayRuleRow");
        }

        private async Task SaveAsync(object parameter)
        {
            try
            {
                foreach (var rule in LstPutAwayRules)
                {
                    if (string.IsNullOrEmpty(rule.PutAwayRuleCode) ||
                        string.IsNullOrEmpty(rule.WarehouseToApply) ||
                        string.IsNullOrEmpty(rule.ProductCode) ||
                        string.IsNullOrEmpty(rule.LocationCode) ||
                        string.IsNullOrEmpty(rule.StorageProductId))
                    {
                        _notificationService.ShowMessage("Vui lòng nhập đầy đủ thông tin cho quy tắc.", "OK", isError: true);
                        return;
                    }

                    var putAwayRuleRequest = new PutAwayRulesRequestDTO
                    {
                        PutAwayRuleCode = rule.PutAwayRuleCode,
                        WarehouseToApply = rule.WarehouseToApply,
                        ProductCode = rule.ProductCode,
                        LocationCode = rule.LocationCode,
                        StorageProductId = rule.StorageProductId
                    };

                    bool exists = _existingPutAwayRuleCodes.Contains(rule.PutAwayRuleCode);

                    if (exists)
                    {
                        var updateResult = await _putAwayRulesService.UpdatePutAwayRule(putAwayRuleRequest);
                        if (!updateResult.Success)
                        {
                            _notificationService.ShowMessage($"Lỗi khi cập nhật quy tắc {rule.PutAwayRuleCode}: {updateResult.Message}", "OK", isError: true);
                            return;
                        }
                    }
                    else
                    {
                        var addResult = await _putAwayRulesService.AddPutAwayRule(putAwayRuleRequest);
                        if (!addResult.Success)
                        {
                            _notificationService.ShowMessage($"Lỗi khi thêm quy tắc {rule.PutAwayRuleCode}: {addResult.Message}", "OK", isError: true);
                            return;
                        }
                        _existingPutAwayRuleCodes.Add(rule.PutAwayRuleCode);
                    }
                }

                _notificationService.ShowMessage("Lưu danh sách quy tắc để hàng thành công!", "OK", isError: false);
                await _messengerService.SendMessageAsync("ReloadPutAwayRulesList");
                await LoadPutAwayRulesAsync();
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private bool CanSave(object parameter)
        {
            var dto = PutAwayRuleRequestDTO;
            var propertiesToValidate = new[] { nameof(dto.PutAwayRuleCode), nameof(dto.WarehouseToApply), nameof(dto.ProductCode), nameof(dto.LocationCode), nameof(dto.StorageProductId) };

            foreach (var prop in propertiesToValidate)
            {
                if (!string.IsNullOrEmpty(dto[prop]))
                {
                    return false;
                }
            }

            return true;
        }

        private void NavigateBack(object parameter)
        {
            var ucWarehouseMaster = App.ServiceProvider!.GetRequiredService<ucWarehouseMaster>();
            _navigationService.NavigateTo<ucWarehouseMaster>();
        }

        private async Task LoadPutAwayRulesAsync()
        {
            try
            {
                var result = await _putAwayRulesService.GetAllPutAwayRules(CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    _existingPutAwayRuleCodes.Clear();
                    LstPutAwayRules.Clear();
                    foreach (var item in result.Data.Data ?? Enumerable.Empty<PutAwayRulesResponseDTO>())
                    {
                        LstPutAwayRules.Add(item);
                        _existingPutAwayRuleCodes.Add(item.PutAwayRuleCode);
                    }
                    TotalPages = result.Data.TotalPages;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách quy tắc để hàng", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

       

        private async Task DeletePutAwayRuleLineAsync(PutAwayRulesResponseDTO rule)
        {
            if (rule == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa quy tắc {rule.PutAwayRuleCode}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deleteResult = await _putAwayRulesService.DeletePutAwayRule(rule.PutAwayRuleCode);
                    if (deleteResult.Success)
                    {
                        _ = LoadPutAwayRulesAsync();
                        _notificationService.ShowMessage(deleteResult.Message ?? "Xóa quy tắc thành công!", "OK", isError: false);
                    }
                    else
                    {
                        _notificationService.ShowMessage(deleteResult.Message ?? "Lỗi khi xóa quy tắc", "OK", isError: true);
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
            _addPutAwayRuleLineCommand?.RaiseCanExecuteChanged();
            _deletePutAwayRuleLineCommand?.RaiseCanExecuteChanged();
        }
    }
}