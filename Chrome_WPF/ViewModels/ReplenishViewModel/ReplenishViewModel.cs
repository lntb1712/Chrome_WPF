using Chrome.DTO.ReplenishDTO;
using Chrome_WPF;
using Chrome_WPF.Helpers;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Models.ReplenishDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.ProductMasterService;
using Chrome_WPF.Services.ReplenishService;
using Chrome_WPF.Services.WarehouseMasterService;
using Chrome_WPF.Views.UserControls.WarehouseMaster;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Chrome_WPF.ViewModels.WarehouseMasterViewModel
{
    public class ReplenishViewModel : BaseViewModel
    {
        private readonly IReplenishService _replenishService;
        private readonly INotificationService _notificationService;
        private readonly IMessengerService _messengerService;
        private readonly INavigationService _navigationService;

        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _backCommand;
        private readonly RelayCommand _addReplenishRuleLineCommand;
        private readonly RelayCommand _deleteReplenishRuleLineCommand;
        private readonly RelayCommand _nextPageCommand;
        private readonly RelayCommand _previousPageCommand;
        private readonly RelayCommand _selectPageCommand;
        private readonly RelayCommand _searchCommand;
        private readonly RelayCommand _clearSearchCommand;
        private readonly RelayCommand _refreshCommand;

        private ReplenishRequestDTO _replenishRequestDTO;
        private ObservableCollection<ReplenishResponseDTO> _lstReplenishRules;
        private ObservableCollection<WarehouseMasterResponseDTO> _availableWarehouses;
        private ObservableCollection<ProductMasterResponseDTO> _availableProducts;
        private ObservableCollection<object> _displayPages;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;
        private HashSet<string> _existingReplenishKeys;
        private string _searchQuery = string.Empty;
        private string _applicableWarehouseCodes = string.Empty;
        private string _warehouseCodes;

        public ReplenishRequestDTO ReplenishRequestDTO
        {
            get => _replenishRequestDTO;
            set
            {
                if (_replenishRequestDTO != null)
                {
                    _replenishRequestDTO.PropertyChanged -= OnPropertyChangedHandler!;
                }
                _replenishRequestDTO = value;
                if (_replenishRequestDTO != null)
                {
                    _replenishRequestDTO.PropertyChanged += OnPropertyChangedHandler!;
                }
                OnPropertyChanged(nameof(ReplenishRequestDTO));
                _ = LoadComboBoxDataAsync();
            }
        }

        public ObservableCollection<ReplenishResponseDTO> LstReplenishRules
        {
            get => _lstReplenishRules;
            set
            {
                _lstReplenishRules = value;
                OnPropertyChanged(nameof(LstReplenishRules));
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
                    _ = LoadReplenishRulesAsync();
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
                _ = LoadReplenishRulesAsync();
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

        public string ApplicableWarehouseCodes
        {
            get => _applicableWarehouseCodes;
            set
            {
                _applicableWarehouseCodes = value;
                OnPropertyChanged();
            }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));
            }
        }

        public RelayCommand SaveCommand => _saveCommand;
        public RelayCommand BackCommand => _backCommand;
        public RelayCommand AddReplenishRuleLineCommand => _addReplenishRuleLineCommand;
        public RelayCommand DeleteReplenishRuleLineCommand => _deleteReplenishRuleLineCommand;
        public RelayCommand NextPageCommand => _nextPageCommand;
        public RelayCommand PreviousPageCommand => _previousPageCommand;
        public RelayCommand SelectPageCommand => _selectPageCommand;
        public RelayCommand SearchCommand => _searchCommand;
        public RelayCommand ClearSearchCommand => _clearSearchCommand;
        public RelayCommand RefreshCommand => _refreshCommand;

        public ReplenishViewModel(
            IReplenishService replenishService,
            IWarehouseMasterService warehouseMasterService,
            IProductMasterService productMasterService,
            INotificationService notificationService,
            IMessengerService messengerService,
            INavigationService navigationService)
        {
            _replenishService = replenishService ?? throw new ArgumentNullException(nameof(replenishService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            _lstReplenishRules = new ObservableCollection<ReplenishResponseDTO>();
            _availableWarehouses = new ObservableCollection<WarehouseMasterResponseDTO>();
            _availableProducts = new ObservableCollection<ProductMasterResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            _currentPage = 1;
            _replenishRequestDTO = new ReplenishRequestDTO();
            _existingReplenishKeys = new HashSet<string>();
            _warehouseCodes = string.Empty;

            _saveCommand = new RelayCommand(async parameter => await SaveAsync(parameter), CanSave);
            _backCommand = new RelayCommand(NavigateBack);
            _addReplenishRuleLineCommand = new RelayCommand(AddReplenishRuleLine);
            _deleteReplenishRuleLineCommand = new RelayCommand(async rule => await DeleteReplenishRuleLineAsync((ReplenishResponseDTO)rule));
            _nextPageCommand = new RelayCommand(_ => NextPage());
            _previousPageCommand = new RelayCommand(_ => PreviousPage());
            _selectPageCommand = new RelayCommand(page => SelectPage((int)page));
            _searchCommand = new RelayCommand(async _ => await LoadSearchReplenishRulesAsync());
            _clearSearchCommand = new RelayCommand(_ => ClearSearch());
            _refreshCommand = new RelayCommand(async _ => await  LoadReplenishRulesAsync());

            var savedPermissions = Properties.Settings.Default.WarehousePermission;
            if (savedPermissions != null)
            {
                _warehouseCodes = savedPermissions.Cast<string>().First();
                ApplicableWarehouseCodes = _warehouseCodes;
            }
            _lstReplenishRules.CollectionChanged += LstReplenishRules_CollectionChanged!;
            _ = LoadReplenishRulesAsync();
            _ = LoadComboBoxDataAsync();
        }

        private async Task LoadSearchReplenishRulesAsync()
        {
            try
            {
                var result = await _replenishService.SearchReplenishAsync(_warehouseCodes,SearchQuery, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    _existingReplenishKeys.Clear();
                    LstReplenishRules.Clear();


                    foreach (var item in result.Data.Data ?? Enumerable.Empty<ReplenishResponseDTO>())
                    {
                        LstReplenishRules.Add(item);
                        _existingReplenishKeys.Add($"{item.WarehouseCode}-{item.ProductCode}");
                    }
                    TotalPages = result.Data.TotalPages;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách quy tắc bổ sung", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private void LstReplenishRules_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ReplenishResponseDTO newRule in e.NewItems)
                {
                    newRule.PropertyChanged += (s, args) => { };
                }
            }
        }

        private async Task LoadComboBoxDataAsync()
        {
            try
            {
                var productTask = _replenishService.GetListProductForReplenish();
                await productTask;

                var productResult = await productTask;

                

                if (productResult.Success && productResult.Data != null)
                {
                    AvailableProducts.Clear();
                    foreach (var item in productResult.Data ?? Enumerable.Empty<ProductMasterResponseDTO>())
                    {
                        AvailableProducts.Add(item);
                    }
                }
                else
                {
                    _notificationService.ShowMessage(productResult.Message ?? "Lỗi khi tải danh sách sản phẩm", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private void AddReplenishRuleLine(object parameter)
        {
            var newRule = new ReplenishResponseDTO
            {
                WarehouseCode = string.Empty,
                ProductCode = string.Empty,
                MinQuantity = 0,
                MaxQuantity = 0,
                TotalOnHand = 0,
                IsNewRow = true
            };
            LstReplenishRules.Add(newRule);
            _messengerService.SendMessageAsync("FocusNewReplenishRuleRow");
        }

        private async Task SaveAsync(object parameter)
        {
            try
            {
                foreach (var rule in LstReplenishRules)
                {
                    if (string.IsNullOrEmpty(rule.ProductCode) ||
                        rule.MinQuantity <= 0 || rule.MaxQuantity <= 0)
                    {
                        _notificationService.ShowMessage("Vui lòng nhập đầy đủ thông tin cho quy tắc.", "OK", isError: true);
                        return;
                    }

                    var replenishRequest = new ReplenishRequestDTO
                    {
                        WarehouseCode = _warehouseCodes,
                        ProductCode = rule.ProductCode,
                        MinQuantity = rule.MinQuantity ?? 0,
                        MaxQuantity = rule.MaxQuantity ?? 0
                    };

                    string key = $"{_warehouseCodes}-{rule.ProductCode}";
                    bool exists = _existingReplenishKeys.Contains(key);

                    if (exists)
                    {
                        var updateResult = await _replenishService.UpdateReplenishAsync(replenishRequest);
                        if (!updateResult.Success)
                        {
                            _notificationService.ShowMessage($"Lỗi khi cập nhật quy tắc {key}: {updateResult.Message}", "OK", isError: true);
                            return;
                        }
                    }
                    else
                    {
                        var addResult = await _replenishService.AddReplenishAsync(replenishRequest);
                        if (!addResult.Success)
                        {
                            _notificationService.ShowMessage($"Lỗi khi thêm quy tắc {key}: {addResult.Message}", "OK", isError: true);
                            return;
                        }
                        _existingReplenishKeys.Add(key);
                    }
                }

                _notificationService.ShowMessage("Lưu danh sách quy tắc bổ sung thành công!", "OK", isError: false);
                await _messengerService.SendMessageAsync("ReloadReplenishRulesList");
                await LoadReplenishRulesAsync();
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private bool CanSave(object parameter)
        {
            var dto = ReplenishRequestDTO;
            var propertiesToValidate = new[] { nameof(dto.WarehouseCode), nameof(dto.ProductCode), nameof(dto.MinQuantity), nameof(dto.MaxQuantity) };

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

        private async Task LoadReplenishRulesAsync()
        {
            try
            {
                var result = await _replenishService.GetReplenishListAsync(_warehouseCodes, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    _existingReplenishKeys.Clear();
                    LstReplenishRules.Clear();
                   

                    foreach (var item in result.Data.Data ?? Enumerable.Empty<ReplenishResponseDTO>())
                    {
                        LstReplenishRules.Add(item);
                        _existingReplenishKeys.Add($"{item.WarehouseCode}-{item.ProductCode}");
                    }
                    TotalPages = result.Data.TotalPages;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách quy tắc bổ sung", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private void ClearSearch()
        {
            SearchQuery = string.Empty;
            CurrentPage = 1;
            _ = LoadReplenishRulesAsync();
        }

        private async Task DeleteReplenishRuleLineAsync(ReplenishResponseDTO rule)
        {
            if (rule == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa quy tắc {rule.WarehouseCode}-{rule.ProductCode}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deleteResult = await _replenishService.DeleteReplenishAsync(rule.WarehouseCode, rule.ProductCode);
                    if (deleteResult.Success)
                    {
                        _existingReplenishKeys.Remove($"{rule.WarehouseCode}-{rule.ProductCode}");
                        await LoadReplenishRulesAsync();
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

        private void OnPropertyChangedHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _saveCommand?.RaiseCanExecuteChanged();
            _backCommand?.RaiseCanExecuteChanged();
            _addReplenishRuleLineCommand?.RaiseCanExecuteChanged();
            _deleteReplenishRuleLineCommand?.RaiseCanExecuteChanged();
        }
    }
}