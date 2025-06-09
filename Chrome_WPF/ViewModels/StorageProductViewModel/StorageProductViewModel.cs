using Chrome_WPF;
using Chrome_WPF.Helpers;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Models.StorageProductDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.ProductMasterService;
using Chrome_WPF.Services.StorageProductService;
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
    public class StorageProductViewModel : BaseViewModel
    {
        private readonly IStorageProductService _storageProductService;
        private readonly IProductMasterService _productMasterService;
        private readonly INotificationService _notificationService;
        private readonly IMessengerService _messengerService;
        private readonly INavigationService _navigationService;

        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _backCommand;
        private readonly RelayCommand _addStorageProductLineCommand;
        private readonly RelayCommand _deleteStorageProductLineCommand;
        private readonly RelayCommand _nextPageCommand;
        private readonly RelayCommand _previousPageCommand;
        private readonly RelayCommand _selectPageCommand;

        private StorageProductRequestDTO _storageProductRequestDTO;
        private ObservableCollection<StorageProductResponseDTO> _lstStorageProducts;
        private ObservableCollection<ProductMasterResponseDTO> _availableProducts; // New property for combobox
        private ObservableCollection<object> _displayPages;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;
        private HashSet<string> _existingProductIds = new HashSet<string>();


        public StorageProductRequestDTO StorageProductRequestDTO
        {
            get => _storageProductRequestDTO;
            set
            {
                if (_storageProductRequestDTO != null)
                {
                    _storageProductRequestDTO.PropertyChanged -= OnPropertyChangedHandler!;
                }
                _storageProductRequestDTO = value;
                if (_storageProductRequestDTO != null)
                {
                    _storageProductRequestDTO.PropertyChanged += OnPropertyChangedHandler!;
                }
                OnPropertyChanged(nameof(StorageProductRequestDTO));
                _ = LoadAvailableProductsAsync();
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
                    _ = LoadStorageProductsAsync();
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
                _ = LoadStorageProductsAsync();
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
        public RelayCommand AddStorageProductLineCommand => _addStorageProductLineCommand;
        public RelayCommand DeleteStorageProductLineCommand => _deleteStorageProductLineCommand;
        public RelayCommand NextPageCommand => _nextPageCommand;
        public RelayCommand PreviousPageCommand => _previousPageCommand;
        public RelayCommand SelectPageCommand => _selectPageCommand;

        public StorageProductViewModel(
            IStorageProductService storageProductService,
            INotificationService notificationService,
            IMessengerService messengerService,
            INavigationService navigationService,
            IProductMasterService productMasterService)
        {
            _storageProductService = storageProductService ?? throw new ArgumentNullException(nameof(storageProductService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _productMasterService = productMasterService ?? throw new ArgumentNullException(nameof(productMasterService));

            _lstStorageProducts = new ObservableCollection<StorageProductResponseDTO>();
            _availableProducts = new ObservableCollection<ProductMasterResponseDTO>(); // Initialize combobox collection
            _displayPages = new ObservableCollection<object>();
            _currentPage = 1;
            _storageProductRequestDTO = new StorageProductRequestDTO();
            _saveCommand = new RelayCommand(async parameter => await SaveAsync(parameter), CanSave);
            _backCommand = new RelayCommand(NavigateBack);
            _addStorageProductLineCommand = new RelayCommand(AddStorageProductLine);
            _deleteStorageProductLineCommand = new RelayCommand(async product => await DeleteStorageProductLineAsync((StorageProductResponseDTO)product));
            _nextPageCommand = new RelayCommand(_ => NextPage());
            _previousPageCommand = new RelayCommand(_ => PreviousPage());
            _selectPageCommand = new RelayCommand(page => SelectPage((int)page));
            _existingProductIds = new HashSet<string>();
            _ = LoadStorageProductsAsync();
            _ = LoadAvailableProductsAsync(); // Load products for combobox
        }

        private async Task LoadAvailableProductsAsync()
        {
            try
            {
                var result = await _productMasterService.GetAllProductMaster(1, int.MaxValue);
                if (result.Success && result.Data != null)
                {
                    AvailableProducts.Clear();
                    foreach (var item in result.Data.Data ?? Enumerable.Empty<ProductMasterResponseDTO>())
                    {
                        AvailableProducts.Add(item);
                    }
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách sản phẩm", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task SaveAsync(object parameter)
        {
            try
            {
                foreach (var product in LstStorageProducts)
                {
                    if (string.IsNullOrEmpty(product.StorageProductId) || product.MaxQuantity <= 0)
                    {
                        _notificationService.ShowMessage("Vui lòng chọn sản phẩm và nhập số lượng tối đa hợp lệ.", "OK", isError: true);
                        return;
                    }

                    var storageProductRequest = new StorageProductRequestDTO
                    {
                        StorageProductId = product.StorageProductId,
                        StorageProductName = product.StorageProductName,
                        ProductCode = product.ProductCode,
                        MaxQuantity = product.MaxQuantity
                    };

                    bool exists = _existingProductIds.Contains(product.StorageProductId);

                    if (exists)
                    {
                        var updateResult = await _storageProductService.UpdateStorageProduct(storageProductRequest);
                        if (!updateResult.Success)
                        {
                            _notificationService.ShowMessage($"Lỗi khi cập nhật sản phẩm {product.StorageProductName}: {updateResult.Message}", "OK", isError: true);
                            return;
                        }
                    }
                    else
                    {
                        var addResult = await _storageProductService.AddStorageProduct(storageProductRequest);
                        if (!addResult.Success)
                        {
                            _notificationService.ShowMessage($"Lỗi khi thêm sản phẩm {product.StorageProductName}: {addResult.Message}", "OK", isError: true);
                            return;
                        }

                        // Thêm vào hashset luôn để các lần sau check nhanh
                        _existingProductIds.Add(product.StorageProductId);
                    }
                }

                _notificationService.ShowMessage("Lưu danh sách sản phẩm thành công!", "OK", isError: false);
                await _messengerService.SendMessageAsync("ReloadStorageProductList");
                await LoadStorageProductsAsync();
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }


        private bool CanSave(object parameter)
        {
            var dto = StorageProductRequestDTO;
            var propertiesToValidate = new[] { nameof(dto.StorageProductId), nameof(dto.ProductCode) };

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

        private async Task LoadStorageProductsAsync()
        {
            try
            {
                var result = await _storageProductService.GetAllStorageProducts(CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    _existingProductIds.Clear();
                    LstStorageProducts.Clear();
                    foreach (var item in result.Data.Data ?? Enumerable.Empty<StorageProductResponseDTO>())
                    {
                        LstStorageProducts.Add(item);
                        _existingProductIds.Add(item.StorageProductId);
                    }
                    TotalPages = result.Data.TotalPages;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách sản phẩm lưu trữ", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private void AddStorageProductLine(object parameter)
        {
            var newProduct = new StorageProductResponseDTO
            {
                StorageProductId = string.Empty,
                StorageProductName = string.Empty,
                ProductCode = string.Empty,
                MaxQuantity = 0
            };
            LstStorageProducts.Add(newProduct);
            _messengerService.SendMessageAsync("FocusNewStorageProductRow");
        }

        private async Task DeleteStorageProductLineAsync(StorageProductResponseDTO product)
        {
            if (product == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa sản phẩm {product.StorageProductName}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deleteResult = await _storageProductService.DeleteStorageProduct(product.StorageProductId);
                    if (deleteResult.Success)
                    {
                        _ = LoadStorageProductsAsync();
                        _notificationService.ShowMessage(deleteResult.Message ?? "Xóa sản phẩm thành công!", "OK", isError: false);
                    }
                    else
                    {
                        _notificationService.ShowMessage(deleteResult.Message ?? "Lỗi khi xóa sản phẩm", "OK", isError: true);
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
            _addStorageProductLineCommand?.RaiseCanExecuteChanged();
            _deleteStorageProductLineCommand?.RaiseCanExecuteChanged();
        }
    }
}