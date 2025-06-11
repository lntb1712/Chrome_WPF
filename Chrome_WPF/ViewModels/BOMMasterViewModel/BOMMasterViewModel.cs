using Chrome_WPF.Helpers;
using Chrome_WPF.Models.BOMMasterDTO;
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.BOMMasterViewModel
{
    public class BOMMasterViewModel : BaseViewModel
    {
        private readonly IBOMMasterService _bomMasterService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IMessengerService _messengerService;
        private readonly IMemoryCache _memoryCache;

        private ObservableCollection<BOMMasterResponseDTO> _bomList;
        private ObservableCollection<object> _displayPages;
        private string _searchText;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;
        private BOMMasterResponseDTO _selectedBOM;
        private BOMMasterRequestDTO _bomMasterRequestDTO;
        private int _totalBOMsCount;
        private bool _isSettingPopupOpen;
        private object _selectedBOMVersion;

        private const string BOM_LIST_CACHE_KEY_PREFIX = "BOMList_Page_";
        private const string BOM_TOTAL_PAGES_CACHE_KEY_PREFIX = "BOMList_TotalPages_";
        private const string SEARCH_LIST_CACHE_KEY_PREFIX = "BOMSearch_List_";
        private const string SEARCH_TOTAL_PAGES_CACHE_KEY_PREFIX = "BOMSearch_TotalPages_";
        private const string TOTAL_BOMS_COUNT_KEY = "TotalBOMsCount";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5); // Cache expires after 5 minutes

        public bool IsSettingPopupOpen
        {
            get => _isSettingPopupOpen;
            set
            {
                _isSettingPopupOpen = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<BOMMasterResponseDTO> BOMList
        {
            get => _bomList;
            set
            {
                _bomList = value;
                OnPropertyChanged();
            }
        }

        public BOMMasterRequestDTO BOMMasterRequestDTO
        {
            get => _bomMasterRequestDTO;
            set
            {
                _bomMasterRequestDTO = value;
                OnPropertyChanged();
            }
        }

        public BOMMasterResponseDTO SelectedBOM
        {
            get => _selectedBOM;
            set
            {
                _selectedBOM = value;
                OnPropertyChanged();
                if (_selectedBOM != null)
                {
                    BOMMasterRequestDTO = new BOMMasterRequestDTO
                    {
                        BOMCode = _selectedBOM.BOMCode,
                        ProductCode = _selectedBOM.ProductCode ?? string.Empty
                    };
                    // Không tự động gán SelectedBOMVersion
                    SelectedBOMVersion = null!; // Hoặc giữ giá trị hiện tại nếu cần
                }
                else
                {
                    BOMMasterRequestDTO = new BOMMasterRequestDTO();
                    SelectedBOMVersion = null!;
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
                _ = LoadBOMsAsync();
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
                _ = LoadBOMsAsync();
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

        public int TotalBOMsCount
        {
            get => _totalBOMsCount;
            set
            {
                _totalBOMsCount = value;
                OnPropertyChanged();
            }
        }

        public object SelectedBOMVersion
        {
            get => _selectedBOMVersion;
            set
            {
                _selectedBOMVersion = value;
                OnPropertyChanged(nameof(SelectedBOMVersion));

                if (_selectedBOMVersion is BOMVersionResponseDTO version && SelectedBOM != null)
                {
                    // Kiểm tra xem phiên bản có thuộc về SelectedBOM hay không
                    if (SelectedBOM.BOMVersionResponses?.Any(v => v.BOMVersion == version.BOMVersion) != true)
                    {
                        // Nếu phiên bản không thuộc BOM hiện tại, thông báo lỗi hoặc xử lý
                        _notificationService.ShowMessage("Phiên bản không hợp lệ với BOM đã chọn.", "OK", isError: true);
                        SelectedBOMVersion = null!;
                    }
                }
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
        public ICommand UpdateActiveStatusCommand { get; }
        public ICommand OpenSettingsCommand => new RelayCommand(_ =>
        {
            IsSettingPopupOpen = true;
        });

        public BOMMasterViewModel(
            IBOMMasterService bomMasterService,
            INotificationService notificationService,
            INavigationService navigationService,
            IMessengerService messengerService,
            IMemoryCache memoryCache)
        {
            _bomMasterService = bomMasterService ?? throw new ArgumentNullException(nameof(bomMasterService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _bomList = new ObservableCollection<BOMMasterResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            _bomMasterRequestDTO = new BOMMasterRequestDTO();
            _selectedBOM = null!;
            _currentPage = 1;
            _searchText = string.Empty;
            _selectedBOMVersion = null!;

            SearchCommand = new RelayCommand(async _ => await SearchBOMsAsync());
            AddCommand = new RelayCommand(_ => OpenEditor(null));
            DeleteCommand = new RelayCommand(async version => await DeleteBOMVersionAsync(version));
            UpdateCommand = new RelayCommand(version => OpenEditorForVersion(version));
            RefreshCommand = new RelayCommand(async _ => await LoadBOMsAsync(true));
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            UpdateActiveStatusCommand = new RelayCommand(async version => await UpdateActiveStatusAsync((BOMVersionResponseDTO)version));

            _ = messengerService.RegisterMessageAsync("ReloadBOMListMessage", async (obj) =>
            {
                await LoadBOMsAsync(true); // Force refresh cache on reload
            });

            _ = LoadBOMsAsync();
        }

        private async Task UpdateActiveStatusAsync(BOMVersionResponseDTO version)
        {
            if (version == null || SelectedBOM == null) return;

            try
            {
                if (version.IsActive)
                {
                    foreach (var v in SelectedBOM.BOMVersionResponses!)
                    {
                        if (v != version && v.IsActive)
                        {
                            v.IsActive = false;
                            var updateDTO = new BOMMasterRequestDTO
                            {
                                BOMCode = SelectedBOM.BOMCode,
                                BOMVersion = v.BOMVersion!,
                                IsActive = false,
                                ProductCode = SelectedBOM.ProductCode ?? string.Empty
                            };
                            var result = await _bomMasterService.UpdateBOMMaster(updateDTO);
                            if (!result.Success)
                            {
                                _notificationService.ShowMessage(result.Message ?? $"Lỗi khi cập nhật trạng thái phiên bản {v.BOMVersion}.", "OK", isError: true);
                                return;
                            }
                        }
                    }
                }

                var dto = new BOMMasterRequestDTO
                {
                    BOMCode = SelectedBOM.BOMCode,
                    BOMVersion = version.BOMVersion!,
                    IsActive = version.IsActive,
                    ProductCode = SelectedBOM.ProductCode ?? string.Empty
                };
                var updateResult = await _bomMasterService.UpdateBOMMaster(dto);
                if (updateResult.Success)
                {
                    _notificationService.ShowMessage($"Cập nhật trạng thái phiên bản {version.BOMVersion} thành công.", "OK", isError: false);
                    ClearCache(); // Clear cache after update
                    await LoadBOMsAsync();
                }
                else
                {
                    _notificationService.ShowMessage(updateResult.Message ?? "Lỗi khi cập nhật trạng thái.", "OK", isError: true);
                    version.IsActive = !version.IsActive; // Revert change if update fails
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Error: {ex.Message}", "OK", isError: true);
                version.IsActive = !version.IsActive; // Revert change if error occurs
            }
        }

        private async Task DeleteBOMVersionAsync(object versionObj)
        {
            if (versionObj is not BOMVersionResponseDTO version || SelectedBOM == null)
            {
                _notificationService.ShowMessage("Vui lòng chọn một phiên bản BOM để xóa.", "OK", isError: true);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa phiên bản BOM {version.BOMVersion} của BOM {SelectedBOM.BOMCode} không?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deleteResult = await _bomMasterService.DeleteBOMMaster(SelectedBOM.BOMCode, version.BOMVersion ?? string.Empty);
                    if (deleteResult.Success)
                    {
                        _notificationService.ShowMessage($"Đã xóa phiên bản BOM {version.BOMVersion} thành công.", "OK", isError: false);
                        ClearCache(); // Clear cache after deletion
                        await LoadBOMsAsync();
                    }
                    else
                    {
                        _notificationService.ShowMessage(deleteResult.Message ?? "Lỗi khi xóa phiên bản BOM.", "OK", isError: true);
                    }
                }
                catch (Exception ex)
                {
                    _notificationService.ShowMessage($"Error: {ex.Message}", "OK", isError: true);
                }
            }
        }

        private async Task SearchBOMsAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    await LoadBOMsAsync();
                    return;
                }

                string listCacheKey = $"{SEARCH_LIST_CACHE_KEY_PREFIX}{SearchText}_{CurrentPage}_{PageSize}";
                string totalPagesCacheKey = $"{SEARCH_TOTAL_PAGES_CACHE_KEY_PREFIX}{SearchText}_{CurrentPage}_{PageSize}";

                if (_memoryCache.TryGetValue(listCacheKey, out IEnumerable<BOMMasterResponseDTO>? cachedList) &&
                    _memoryCache.TryGetValue(totalPagesCacheKey, out int cachedTotalPages))
                {
                    BOMList.Clear();
                    foreach (var bom in cachedList ?? Enumerable.Empty<BOMMasterResponseDTO>())
                    {
                        BOMList.Add(bom);
                    }
                    TotalPages = cachedTotalPages;
                    OnPropertyChanged(nameof(BOMList));
                    return;
                }

                var result = await _bomMasterService.SearchBOMMaster(SearchText, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    BOMList.Clear();
                    var boms = result.Data.Data ?? Enumerable.Empty<BOMMasterResponseDTO>();
                    foreach (var bom in boms)
                    {
                        BOMList.Add(bom);
                    }
                    TotalPages = result.Data.TotalPages;

                    _memoryCache.Set(listCacheKey, boms.ToList(), new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = _cacheExpiration
                    });
                    _memoryCache.Set(totalPagesCacheKey, TotalPages, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = _cacheExpiration
                    });

                    OnPropertyChanged(nameof(BOMList));
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tìm kiếm BOM.", "OK", isError: true);
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

        private void OpenEditorForVersion(object version)
        {
            if (version is BOMVersionResponseDTO bomVersion && SelectedBOM != null)
            {
                SelectedBOMVersion = bomVersion;
                OpenEditor(SelectedBOM);
            }
            else
            {
                _notificationService.ShowMessage("Vui lòng chọn một BOM trước khi chỉnh sửa phiên bản.", "OK", isError: true);
            }
        }

        private void OpenEditor(BOMMasterResponseDTO? bom)
        {
            var bomEditor = App.ServiceProvider!.GetRequiredService<ucBOMComponent>();

            var dto = bom == null
                ? new BOMMasterRequestDTO()
                : new BOMMasterRequestDTO
                {
                    BOMCode = bom.BOMCode,
                    ProductCode = bom.ProductCode ?? string.Empty,
                    BOMVersion = (SelectedBOMVersion as BOMVersionResponseDTO)?.BOMVersion ?? string.Empty,
                    IsActive = (SelectedBOMVersion as BOMVersionResponseDTO)?.IsActive ?? false
                };

            var viewModel = new BOMComponentViewModel(
                App.ServiceProvider!.GetRequiredService<IBOMMasterService>(),
                App.ServiceProvider!.GetRequiredService<IBOMComponentService>(),
                App.ServiceProvider!.GetRequiredService<IProductMasterService>(),
                App.ServiceProvider!.GetRequiredService<INotificationService>(),
                App.ServiceProvider!.GetRequiredService<IMessengerService>(),
                App.ServiceProvider!.GetRequiredService<INavigationService>(),
                App.ServiceProvider!.GetRequiredService<IMemoryCache>(),
                isAddingNew: bom == null);

            viewModel.BOMMasterRequestDTO = dto;
            bomEditor.DataContext = viewModel;
            _navigationService.NavigateTo(bomEditor);
        }

        private async Task TotalBOMsAsync()
        {
            if (_memoryCache.TryGetValue(TOTAL_BOMS_COUNT_KEY, out int cachedCount))
            {
                TotalBOMsCount = cachedCount;
                return;
            }

            try
            {
                var result = await _bomMasterService.GetTotalBOMMasterCount();
                if (result.Success)
                {
                    TotalBOMsCount = result.Data;
                    _memoryCache.Set(TOTAL_BOMS_COUNT_KEY, TotalBOMsCount, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = _cacheExpiration
                    });
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi lấy tổng số BOM.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadBOMsAsync(bool forceRefresh = false)
        {
            string listCacheKey = $"{BOM_LIST_CACHE_KEY_PREFIX}{CurrentPage}_{PageSize}";
            string totalPagesCacheKey = $"{BOM_TOTAL_PAGES_CACHE_KEY_PREFIX}{CurrentPage}_{PageSize}";

            if (!forceRefresh &&
                _memoryCache.TryGetValue(listCacheKey, out IEnumerable<BOMMasterResponseDTO>? cachedList) &&
                _memoryCache.TryGetValue(totalPagesCacheKey, out int cachedTotalPages))
            {
                BOMList.Clear();
                foreach (var bom in cachedList ?? Enumerable.Empty<BOMMasterResponseDTO>())
                {
                    BOMList.Add(bom);
                }
                TotalPages = cachedTotalPages;
                await TotalBOMsAsync();
                return;
            }

            try
            {
                var result = await _bomMasterService.GetAllBOMMaster(CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    BOMList.Clear();
                    var boms = result.Data.Data ?? Enumerable.Empty<BOMMasterResponseDTO>();
                    foreach (var bom in boms)
                    {
                        BOMList.Add(bom);
                    }
                    TotalPages = result.Data.TotalPages;

                    _memoryCache.Set(listCacheKey, boms.ToList(), new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = _cacheExpiration
                    });
                    _memoryCache.Set(totalPagesCacheKey, TotalPages, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = _cacheExpiration
                    });

                    await TotalBOMsAsync();
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách BOM.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private void ClearCache()
        {
            _memoryCache.Remove($"{BOM_LIST_CACHE_KEY_PREFIX}{CurrentPage}_{PageSize}");
            _memoryCache.Remove($"{BOM_TOTAL_PAGES_CACHE_KEY_PREFIX}{CurrentPage}_{PageSize}");
            _memoryCache.Remove(TOTAL_BOMS_COUNT_KEY);
        }
    }
}