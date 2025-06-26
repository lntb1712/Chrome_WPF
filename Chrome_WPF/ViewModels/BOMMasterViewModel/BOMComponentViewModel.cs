using Chrome_WPF.Helpers;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.BOMComponentDTO;
using Chrome_WPF.Models.BOMMasterDTO;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Services.BOMComponentService;
using Chrome_WPF.Services.BOMMasterService;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.ProductMasterService;
using Chrome_WPF.Views.UserControls.BOMMaster;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Chrome_WPF.ViewModels.BOMMasterViewModel
{
    public class BOMComponentViewModel : BaseViewModel
    {
        private readonly IBOMComponentService _bomComponentService;
        private readonly IBOMMasterService _bomMasterService;
        private readonly IProductMasterService _productMasterService;
        private readonly INotificationService _notificationService;
        private readonly IMessengerService _messengerService;
        private readonly INavigationService _navigationService;
        private readonly IMemoryCache _cache;

        private BOMMasterRequestDTO _bomMasterRequestDTO;
        private ObservableCollection<BOMComponentResponseDTO> _lstComponents;
        private ObservableCollection<object> _displayPages;
        private ObservableCollection<ProductMasterResponseDTO> _availableProducts;
        private ObservableCollection<ProductMasterResponseDTO> _availableComponents;
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalPages;
        private bool _isAddingNew;

        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _backCommand;
        private readonly RelayCommand _addComponentLineCommand;
        private readonly RelayCommand _deleteComponentLineCommand;
        private readonly RelayCommand _nextPageCommand;
        private readonly RelayCommand _previousPageCommand;
        private readonly RelayCommand _selectPageCommand;
        private readonly RelayCommand _bomPreviewCommand;

        private const string ProductsCacheKeyPrefix = "Products_";
        private const string ComponentsCacheKeyPrefix = "Components_";
        private const string BOMComponentsCacheKeyPrefix = "BOMComponents_";
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(5);

        public BOMMasterRequestDTO BOMMasterRequestDTO
        {
            get => _bomMasterRequestDTO;
            set
            {
                if (_bomMasterRequestDTO != value)
                {
                    if (_bomMasterRequestDTO != null)
                    {
                        _bomMasterRequestDTO.PropertyChanged -= OnPropertyChangedHandler!;
                    }
                    _bomMasterRequestDTO = value;
                    if (_bomMasterRequestDTO != null)
                    {
                        _bomMasterRequestDTO.PropertyChanged += OnPropertyChangedHandler!;
                    }
                    OnPropertyChanged(nameof(BOMMasterRequestDTO));
                    _ = LoadComponentsAsync();
                }
            }
        }

        public ObservableCollection<BOMComponentResponseDTO> LstComponents
        {
            get => _lstComponents;
            private set
            {
                _lstComponents = value;
                OnPropertyChanged(nameof(LstComponents));
            }
        }

        public ObservableCollection<object> DisplayPages
        {
            get => _displayPages;
            private set
            {
                _displayPages = value;
                OnPropertyChanged(nameof(DisplayPages));
            }
        }

        public ObservableCollection<ProductMasterResponseDTO> AvailableProducts
        {
            get => _availableProducts;
            private set
            {
                _availableProducts = value;
                OnPropertyChanged(nameof(AvailableProducts));
            }
        }

        public ObservableCollection<ProductMasterResponseDTO> AvailableComponents
        {
            get => _availableComponents;
            private set
            {
                _availableComponents = value;
                OnPropertyChanged(nameof(AvailableComponents));
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
                    _ = LoadComponentsAsync();
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
                    OnPropertyChanged(nameof(PageSize));
                    CurrentPage = 1;
                    _ = LoadComponentsAsync();
                }
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            private set
            {
                _totalPages = value;
                OnPropertyChanged(nameof(TotalPages));
                UpdateDisplayPages();
            }
        }

        public bool IsAddingNew
        {
            get => _isAddingNew;
            private set
            {
                _isAddingNew = value;
                OnPropertyChanged(nameof(IsAddingNew));
            }
        }

        public RelayCommand SaveCommand => _saveCommand;
        public RelayCommand BackCommand => _backCommand;
        public RelayCommand AddComponentLineCommand => _addComponentLineCommand;
        public RelayCommand DeleteComponentLineCommand => _deleteComponentLineCommand;
        public RelayCommand NextPageCommand => _nextPageCommand;
        public RelayCommand PreviousPageCommand => _previousPageCommand;
        public RelayCommand SelectPageCommand => _selectPageCommand;
        public RelayCommand BOMPreviewCommand => _bomPreviewCommand;

        public BOMComponentViewModel(
            IBOMMasterService bomMasterService,
            IBOMComponentService bomComponentService,
            IProductMasterService productMasterService,
            INotificationService notificationService,
            IMessengerService messengerService,
            INavigationService navigationService,
            IMemoryCache cache,
            bool isAddingNew = true)
        {
            _bomMasterService = bomMasterService ?? throw new ArgumentNullException(nameof(bomMasterService));
            _bomComponentService = bomComponentService ?? throw new ArgumentNullException(nameof(bomComponentService));
            _productMasterService = productMasterService ?? throw new ArgumentNullException(nameof(productMasterService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _isAddingNew = isAddingNew;

            _bomMasterRequestDTO = new BOMMasterRequestDTO();
            _lstComponents = new ObservableCollection<BOMComponentResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            _availableProducts = new ObservableCollection<ProductMasterResponseDTO>();
            _availableComponents = new ObservableCollection<ProductMasterResponseDTO>();

            _saveCommand = new RelayCommand(async _ => await SaveAsync(), CanSave);
            _backCommand = new RelayCommand(_ => NavigateBack());
            _addComponentLineCommand = new RelayCommand(_ => AddComponentLine(), CanAddComponentLine);
            _deleteComponentLineCommand = new RelayCommand(async component => await DeleteComponentLineAsync((BOMComponentResponseDTO)component));
            _nextPageCommand = new RelayCommand(_ => CurrentPage++, _ => CurrentPage < TotalPages);
            _previousPageCommand = new RelayCommand(_ => CurrentPage--, _ => CurrentPage > 1);
            _selectPageCommand = new RelayCommand(page => CurrentPage = (int)page, page => (int)page >= 1 && (int)page <= TotalPages);
            _bomPreviewCommand = new RelayCommand(async bom => await OpenPreviewBOMAsync((BOMMasterRequestDTO)bom));

            _bomMasterRequestDTO.PropertyChanged += OnPropertyChangedHandler!;
            // Đăng ký xử lý thông điệp RestoreBOMState
            _messengerService.RegisterMessageAsync("RestoreBOMState", async (message) =>
            {
                var state = message as dynamic;
                string bomCode = state?.BOMCode!;
                string bomVersion = state?.BOMVersion!;
                if (!string.IsNullOrEmpty(bomCode) && !string.IsNullOrEmpty(bomVersion))
                {
                    var stateCacheKey = $"BOMComponentState_{bomCode}_{bomVersion}";
                    if (_cache.TryGetValue(stateCacheKey, out dynamic? cachedState))
                    {
                        BOMMasterRequestDTO = cachedState!.BOMMasterRequestDTO;
                        LstComponents = new ObservableCollection<BOMComponentResponseDTO>(cachedState.LstComponents);
                        await LoadComponentsAsync();
                    }
                }
            });
            _ = InitializeAsync();
        }

        private async Task OpenPreviewBOMAsync(BOMMasterRequestDTO bom)
        {
            try
            {
                if (string.IsNullOrEmpty(bom.BOMCode) || string.IsNullOrEmpty(bom.BOMVersion))
                {
                    _notificationService.ShowMessage("Không thể mở preview: Thiếu thông tin BOM.", "OK", isError: true);
                    return;
                }

                // Lưu trạng thái vào cache
                var stateCacheKey = $"BOMComponentState_{bom.BOMCode}_{bom.BOMVersion}";
                _cache.Set(stateCacheKey, new
                {
                    BOMMasterRequestDTO,
                    LstComponents = LstComponents.ToList()
                }, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

                var bomPreview = App.ServiceProvider?.GetRequiredService<ucBOMPreview>();
                if (bomPreview == null)
                {
                    _notificationService.ShowMessage("Lỗi hệ thống: Không thể khởi tạo giao diện preview.", "OK", isError: true);
                    return;
                }

                var viewModel = new BOMPreviewViewModel(
                    App.ServiceProvider!.GetRequiredService<IBOMComponentService>(),
                    App.ServiceProvider!.GetRequiredService<INotificationService>(),
                    App.ServiceProvider!.GetRequiredService<INavigationService>(),
                    App.ServiceProvider!.GetRequiredService<IMemoryCache>(),
                    App.ServiceProvider!.GetRequiredService<IMessengerService>());

                await viewModel.InitializeAsync(bom);

                bomPreview.DataContext = viewModel;
                _navigationService.NavigateTo(bomPreview);
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi mở preview: {ex.Message}", "OK", isError: true);
            }
        }
        private async Task InitializeAsync()
        {
            await Task.WhenAll(LoadAvailableProductsAsync(), LoadAvailableComponentsAsync(), LoadComponentsAsync());
        }

        private async Task LoadAvailableProductsAsync()
        {
            try
            {
                var cacheKey = $"{ProductsCacheKeyPrefix}FG_SFG";
                if (!_cache.TryGetValue(cacheKey, out List<ProductMasterResponseDTO>? cachedProducts))
                {
                    var result = await _productMasterService.GetProductWithCategoryIDs(new[] { "FG", "SFG" });
                    if (result.Success && result.Data != null)
                    {
                        cachedProducts = result.Data.Take(100).ToList();
                        _cache.Set(cacheKey, cachedProducts, new MemoryCacheEntryOptions
                        {
                            SlidingExpiration = CacheExpiration
                        });
                    }
                    else
                    {
                        _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách sản phẩm.", "OK", isError: true);
                        return;
                    }
                }

                if (cachedProducts != null)
                {
                    AvailableProducts.Clear();
                    foreach (var product in cachedProducts)
                    {
                        AvailableProducts.Add(product);
                    }
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadAvailableComponentsAsync()
        {
            try
            {
                var cacheKey = $"{ComponentsCacheKeyPrefix}SFG_MAT";
                if (!_cache.TryGetValue(cacheKey, out List<ProductMasterResponseDTO>? cachedComponents))
                {
                    var result = await _productMasterService.GetProductWithCategoryIDs(new[] { "SFG", "MAT" });
                    if (result.Success && result.Data != null)
                    {
                        cachedComponents = result.Data.Take(100).ToList();
                        _cache.Set(cacheKey, cachedComponents, new MemoryCacheEntryOptions
                        {
                            SlidingExpiration = CacheExpiration
                        });
                    }
                    else
                    {
                        _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách thành phần.", "OK", isError: true);
                        return;
                    }
                }

                if (cachedComponents != null)
                {
                    AvailableComponents.Clear();
                    foreach (var component in cachedComponents)
                    {
                        AvailableComponents.Add(component);
                    }
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadComponentsAsync()
        {
            if (string.IsNullOrEmpty(BOMMasterRequestDTO.BOMCode) || string.IsNullOrEmpty(BOMMasterRequestDTO.BOMVersion))
            {
                LstComponents.Clear();
                TotalPages = 0;
                return;
            }

            try
            {
                var cacheKey = $"{BOMComponentsCacheKeyPrefix}{BOMMasterRequestDTO.BOMCode}_{BOMMasterRequestDTO.BOMVersion}";
                if (!_cache.TryGetValue(cacheKey, out List<BOMComponentResponseDTO>? cachedComponents))
                {
                    var result = await _bomComponentService.GetAllBOMComponent(BOMMasterRequestDTO.BOMCode, BOMMasterRequestDTO.BOMVersion);
                    if (result.Success && result.Data != null)
                    {
                        cachedComponents = result.Data.ToList();
                        _cache.Set(cacheKey, cachedComponents, new MemoryCacheEntryOptions
                        {
                            SlidingExpiration = CacheExpiration
                        });
                    }
                    else
                    {
                        _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách thành phần.", "OK", isError: true);
                        return;
                    }
                }

                LstComponents.Clear();
                foreach (var component in cachedComponents!)
                {
                    LstComponents.Add(component);
                }
                TotalPages = (int)Math.Ceiling((double)LstComponents.Count / PageSize);
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private bool CanSave(object parameter)
        {
            var dto = BOMMasterRequestDTO;
            var propertiesToValidate = new[] { nameof(dto.ProductCode), nameof(dto.BOMCode),nameof(dto.BOMVersion) };

            foreach (var prop in propertiesToValidate)
            {
                if (!string.IsNullOrEmpty(dto[prop]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CanAddComponentLine(object parameter)
        {
            var dto = BOMMasterRequestDTO;
            var propertiesToValidate = new[] { nameof(dto.ProductCode), nameof(dto.BOMCode), nameof(dto.BOMVersion) };

            foreach (var prop in propertiesToValidate)
            {
                if (!string.IsNullOrEmpty(dto[prop]))
                {
                    return false;
                }
            }

            return true;
        }

        private void AddComponentLine()
        {
            if (!CanAddComponentLine(null!))
            {
                _notificationService.ShowMessage("Vui lòng nhập mã BOM và phiên bản trước khi thêm thành phần.", "OK", isError: true);
                return;
            }

            var newComponent = new BOMComponentResponseDTO
            {
                BOMCode = BOMMasterRequestDTO.BOMCode,
                BOMVersion = BOMMasterRequestDTO.BOMVersion,
                ComponentCode = string.Empty,
                ComponentName = string.Empty,
                ConsumpQuantity = null,
                ScrapRate = null,
                SelectedComponent = null,
                IsNewRow = true
            };
            LstComponents.Add(newComponent);

            // Invalidate cache for BOM components
            var cacheKey = $"{BOMComponentsCacheKeyPrefix}{BOMMasterRequestDTO.BOMCode}_{BOMMasterRequestDTO.BOMVersion}";
            _cache.Remove(cacheKey);

            // Simplified UI interaction
            _messengerService.SendMessageAsync("FocusNewComponentRow", newComponent);
        }

        private async Task DeleteComponentLineAsync(BOMComponentResponseDTO component)
        {
            if (component == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa thành phần {component.ComponentName}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deleteResult = await _bomComponentService.DeleteBomComponent(component.BOMCode, component.BOMVersion, component.ComponentCode);
                    if (deleteResult.Success)
                    {
                        LstComponents.Remove(component);
                        TotalPages = (int)Math.Ceiling((double)LstComponents.Count / PageSize);

                        // Invalidate cache
                        var cacheKey = $"{BOMComponentsCacheKeyPrefix}{component.BOMCode}_{component.BOMVersion}";
                        _cache.Remove(cacheKey);

                        _notificationService.ShowMessage("Xóa thành phần thành công.", "OK", isError: false);
                    }
                    else
                    {
                        _notificationService.ShowMessage(deleteResult.Message ?? "Lỗi khi xóa thành phần.", "OK", isError: true);
                    }
                }
                catch (Exception ex)
                {
                    _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
                }
            }
        }

        private async Task SaveAsync()
        {
            try
            {
                BOMMasterRequestDTO.RequestValidation();
                if (!CanSave(null!))
                {
                    _notificationService.ShowMessage("Vui lòng kiểm tra lại thông tin nhập.", "OK", isError: true);
                    return;
                }

                // Save BOM master
                ApiResult<bool> result;
                if (IsAddingNew)
                {
                    result = await _bomMasterService.AddBOMMaster(BOMMasterRequestDTO);
                }
                else
                {
                    result = await _bomMasterService.UpdateBOMMaster(BOMMasterRequestDTO);
                }

                if (!result.Success)
                {
                    _notificationService.ShowMessage($"Lỗi khi {(IsAddingNew ? "thêm" : "cập nhật")} BOM: {result.Message}", "OK", isError: true);
                    return;
                }

                // Batch process components
                var componentsToAdd = new List<BOMComponentRequestDTO>();
                var componentsToUpdate = new List<BOMComponentRequestDTO>();

                var existingComponentsResult = await _bomComponentService.GetAllBOMComponent(BOMMasterRequestDTO.BOMCode, BOMMasterRequestDTO.BOMVersion);
                var existingComponents = existingComponentsResult.Success ? existingComponentsResult.Data?.ToList() ?? new List<BOMComponentResponseDTO>() : new List<BOMComponentResponseDTO>();

                foreach (var component in LstComponents)
                {
                    if (string.IsNullOrEmpty(component.ComponentCode))
                    {
                        _notificationService.ShowMessage($"Thành phần {component.ComponentName} thiếu mã thành phần.", "OK", isError: true);
                        return;
                    }

                    var componentRequest = new BOMComponentRequestDTO
                    {
                        BOMCode = BOMMasterRequestDTO.BOMCode,
                        BOMVersion = BOMMasterRequestDTO.BOMVersion,
                        ComponentCode = component.ComponentCode,
                        ConsumpQuantity = component.ConsumpQuantity,
                        ScrapRate = component.ScrapRate
                    };

                    var existing = existingComponents.FirstOrDefault(c => c.ComponentCode == component.ComponentCode);
                    if (existing != null)
                    {
                        if (existing.ConsumpQuantity != component.ConsumpQuantity || existing.ScrapRate != component.ScrapRate)
                        {
                            componentsToUpdate.Add(componentRequest);
                        }
                    }
                    else
                    {
                        componentsToAdd.Add(componentRequest);
                    }
                }

                // Batch add components
                foreach (var component in componentsToAdd)
                {
                    var addResult = await _bomComponentService.AddBomComponent(component);
                    if (!addResult.Success)
                    {
                        _notificationService.ShowMessage($"Lỗi khi thêm thành phần {component.ComponentCode}: {addResult.Message}", "OK", isError: true);
                        return;
                    }
                }

                // Batch update components
                foreach (var component in componentsToUpdate)
                {
                    var updateResult = await _bomComponentService.UpdateBomComponent(component);
                    if (!updateResult.Success)
                    {
                        _notificationService.ShowMessage($"Lỗi khi cập nhật thành phần {component.ComponentCode}: {updateResult.Message}", "OK", isError: true);
                        return;
                    }
                }

                // Invalidate cache
                var cacheKey = $"{BOMComponentsCacheKeyPrefix}{BOMMasterRequestDTO.BOMCode}_{BOMMasterRequestDTO.BOMVersion}";
                _cache.Remove(cacheKey);

                _notificationService.ShowMessage(IsAddingNew ? "Thêm BOM và thành phần thành công!" : "Cập nhật BOM và thành phần thành công!", "OK", isError: false);
                if (IsAddingNew)
                {
                    BOMMasterRequestDTO.ClearValidation();
                    BOMMasterRequestDTO = new BOMMasterRequestDTO();
                    LstComponents.Clear();
                }
                await _messengerService.SendMessageAsync("ReloadBOMListMessage");
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private void NavigateBack()
        {
            var ucBOMMaster = App.ServiceProvider!.GetRequiredService<ucBOMMaster>();
            _navigationService.NavigateTo(ucBOMMaster);
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
            _addComponentLineCommand?.RaiseCanExecuteChanged();
        }
    }
}