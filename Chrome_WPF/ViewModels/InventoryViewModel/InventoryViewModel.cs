using Chrome_WPF.Helpers;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.CategoryDTO;
using Chrome_WPF.Models.InventoryDTO;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Services.InventoryService;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Views.UserControls.Inventory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.InventoryViewModel
{
    public class InventoryViewModel : BaseViewModel
    {
        private readonly IInventoryService _inventoryService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IMessengerService _messengerService;

        private ObservableCollection<InventorySummaryDTO> _inventoryList;
        private ObservableCollection<LocationUsageDTO> _inventoryVisualList; // Giữ nguyên cho tương thích
        private ObservableCollection<WarehouseUsageDTO> _warehouseRegions; // Thêm để quản lý vùng trực quan
        private ObservableCollection<object> _displayPages;
        private ObservableCollection<CategoryResponseDTO> _categories;
        private string _searchText;
        private List<string> _selectedCategoryIds;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;
        private int _selectedCategoryIndex = 0; // 0 đại diện cho tab "Tất cả"
        private string _applicableLocation;
        private int _viewMode; // 0 cho "Vật lý", 1 cho "Trực quan"

        public ObservableCollection<InventorySummaryDTO> InventoryList
        {
            get => _inventoryList;
            set
            {
                _inventoryList = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LocationUsageDTO> InventoryVisualList
        {
            get => _inventoryVisualList;
            set
            {
                _inventoryVisualList = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<WarehouseUsageDTO> WarehouseRegions
        {
            get => _warehouseRegions;
            set
            {
                _warehouseRegions = value;
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

        public ObservableCollection<CategoryResponseDTO> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
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
                _ = LoadInventoryAsync();
            }
        }

        public List<string> SelectedCategoryIds
        {
            get => _selectedCategoryIds;
            set
            {
                _selectedCategoryIds = value;
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
                _ = LoadInventoryAsync();
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
                _ = LoadInventoryAsync();
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

        public int SelectedCategoryIndex
        {
            get => _selectedCategoryIndex;
            set
            {
                _selectedCategoryIndex = value;
                OnPropertyChanged();
                UpdateSelectedCategory();
                _ = LoadInventoryAsync();
            }
        }

        public string ApplicableLocation
        {
            get => _applicableLocation;
            set
            {
                _applicableLocation = value;
                OnPropertyChanged(nameof(ApplicableLocation));
            }
        }

        public int ViewMode
        {
            get => _viewMode;
            set
            {
                _viewMode = value;
                OnPropertyChanged();
                _ = LoadInventoryAsync(); // Tải dữ liệu phù hợp với chế độ
            }
        }

        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand SelectPageCommand { get; }
        public ICommand ViewDetailCommand { get; }

        public InventoryViewModel(
            IInventoryService inventoryService,
            INotificationService notificationService,
            INavigationService navigationService,
            IMessengerService messengerService)
        {
            _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));

            _inventoryList = new ObservableCollection<InventorySummaryDTO>();
            _inventoryVisualList = new ObservableCollection<LocationUsageDTO>();
            _warehouseRegions = new ObservableCollection<WarehouseUsageDTO>();
            _displayPages = new ObservableCollection<object>();
            _categories = new ObservableCollection<CategoryResponseDTO>();
            _selectedCategoryIds = new List<string>();
            _searchText = string.Empty;
            _currentPage = 1;
            _viewMode = 0; // Mặc định là "Vật lý"

            SearchCommand = new RelayCommand(async _ => await SearchInventoryAsync());
            RefreshCommand = new RelayCommand(async _ => await LoadInventoryAsync());
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            ViewDetailCommand = new RelayCommand(product => OpenDetail((InventorySummaryDTO)product));

            List<string> warehousePermissions = new List<string>();
            var savedPermissions = Properties.Settings.Default.WarehousePermission;
            if (savedPermissions != null)
            {
                warehousePermissions = savedPermissions.Cast<string>().ToList();
            }
            _applicableLocation = string.Join(",", warehousePermissions.Select(id => $"{Uri.EscapeDataString(id)}"));

            _ = LoadCategoriesAsync();
            _ = LoadInventoryAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var result = await _inventoryService.GetAllCategories();
                if (result.Success && result.Data != null)
                {
                    Categories.Clear();
                    Categories.Add(new CategoryResponseDTO { CategoryId = "", CategoryName = "Tất cả" }); // Thêm tab "Tất cả"
                    foreach (var category in result.Data)
                    {
                        Categories.Add(category);
                    }
                    SelectedCategoryIndex = 0; // Mặc định chọn "Tất cả"
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách danh mục.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadInventoryAsync()
        {
            try
            {
                if (ViewMode == 0) // Chế độ "Vật lý"
                {
                    ApiResult<PagedResponse<InventorySummaryDTO>> result;
                    if (SelectedCategoryIds != null && SelectedCategoryIds.Any() && !string.IsNullOrEmpty(SelectedCategoryIds[0]))
                    {
                        result = await _inventoryService.GetListProductInventoryByCategoryIds(SelectedCategoryIds.ToArray(), CurrentPage, PageSize);
                    }
                    else
                    {
                        result = await _inventoryService.GetListProductInventory(CurrentPage, PageSize);
                    }

                    if (result.Success && result.Data != null)
                    {
                        InventoryList.Clear();
                        foreach (var product in result.Data.Data ?? Enumerable.Empty<InventorySummaryDTO>())
                        {
                            InventoryList.Add(product);
                        }
                        TotalPages = result.Data.TotalPages;
                        InventoryVisualList.Clear(); // Xóa dữ liệu chế độ "Trực quan" khi không dùng
                        WarehouseRegions.Clear(); // Xóa dữ liệu vùng khi không dùng
                    }
                    else
                    {
                        _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách tồn kho.", "OK", isError: true);
                    }
                }
                else // Chế độ "Trực quan"
                {
                    var result = await _inventoryService.GetInventoryUsedPercent();
                    if (result.Success && result.Data != null)
                    {
                        InventoryVisualList.Clear();
                        WarehouseRegions.Clear();

                        // Nhóm dữ liệu theo WarehouseCode (dựa trên cấu trúc JSON mới)
                        foreach (var warehouse in result.Data)
                        {
                            var items = new List<LocationUsageDTO>();
                            var index = 0;
                            foreach (var location in warehouse.locationUsageDTOs)
                            {
                                items.Add(new LocationUsageDTO
                                {
                                    LocationCode = location.LocationCode,
                                    LocationName = location.LocationName,
                                    ProductCode = location.ProductCode,
                                    ProductName = location.ProductName,
                                    Quantity = location.Quantity,
                                    UsedPercentage = location.UsedPercentage,
                                 
                                });
                                index++;
                            }
                           

                            WarehouseRegions.Add(new WarehouseUsageDTO
                            {
                                WarehouseCode = warehouse.WarehouseCode,
                                WarehouseName = warehouse.WarehouseName,
                                locationUsageDTOs = items.Take(16).ToList()
                            });
                        }

                        TotalPages = 1; // Chế độ "Trực quan" không dùng phân trang
                        InventoryList.Clear(); // Xóa dữ liệu chế độ "Vật lý" khi không dùng
                    }
                    else
                    {
                        _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải dữ liệu trực quan.", "OK", isError: true);
                    }
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task SearchInventoryAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText) || ViewMode != 0) // Chỉ tìm kiếm trong chế độ "Vật lý"
                {
                    await LoadInventoryAsync();
                    return;
                }

                var result = await _inventoryService.SearchProductInventory(SearchText, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    InventoryList.Clear();
                    foreach (var product in result.Data.Data ?? Enumerable.Empty<InventorySummaryDTO>())
                    {
                        InventoryList.Add(product);
                    }
                    TotalPages = result.Data.TotalPages;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tìm kiếm tồn kho.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private void PreviousPage()
        {
            if (CurrentPage > 1 && ViewMode == 0)
            {
                CurrentPage--;
            }
        }

        private void NextPage()
        {
            if (CurrentPage < TotalPages && ViewMode == 0)
            {
                CurrentPage++;
            }
        }

        private void SelectPage(int page)
        {
            if (page >= 1 && page <= TotalPages && ViewMode == 0)
            {
                CurrentPage = page;
            }
        }

        private void UpdateDisplayPages()
        {
            DisplayPages.Clear();
            if (TotalPages <= 0 || ViewMode != 0) return;

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

        private void OpenDetail(InventorySummaryDTO product)
        {
            if (product == null || ViewMode != 0) return;

            var inventoryDetail = App.ServiceProvider!.GetRequiredService<ucInventoryDetail>();
            var viewModel = new InventoryDetailViewModel(
                App.ServiceProvider!.GetRequiredService<IInventoryService>(),
                App.ServiceProvider!.GetRequiredService<INotificationService>(),
                App.ServiceProvider!.GetRequiredService<INavigationService>(),
                product.ProductCode,
                product.ProductName);

            inventoryDetail.DataContext = viewModel;
            _navigationService.NavigateTo(inventoryDetail);
        }

        private void UpdateSelectedCategory()
        {
            SelectedCategoryIds.Clear();
            if (SelectedCategoryIndex > 0 && SelectedCategoryIndex < Categories.Count) // Bỏ qua index 0 (Tất cả)
            {
                SelectedCategoryIds.Add(Categories[SelectedCategoryIndex].CategoryId);
            }
        }
    }

}