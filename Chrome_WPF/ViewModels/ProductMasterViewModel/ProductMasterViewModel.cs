using Chrome_WPF.Helpers;
using Chrome_WPF.Models.CategoryDTO;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Services.CategoryService;
using Chrome_WPF.Services.CustomerMasterService;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.ProductCustomerService;
using Chrome_WPF.Services.ProductMasterService;
using Chrome_WPF.Services.ProductSupplierService;
using Chrome_WPF.Services.SupplierMasterService;
using Chrome_WPF.Views.UserControls.ProductMaster;
using CommunityToolkit.Mvvm.Messaging;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.ProductMasterViewModel
{
    public class ProductMasterViewModel: BaseViewModel
    {
        private readonly IProductMasterService _productMasterService;
        private readonly ICategoryService _categoryService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IMessengerService _messengerService;
        private ObservableCollection<ProductMasterResponseDTO> _lstProduct;
        private ObservableCollection<CategorySummaryDTO>_lstCategorySummary;
        private ObservableCollection<object> _displayPages;
        private string _searchText;
        private int _currentPage;
        private int _pageSize=10;
        private int _totalPages;
        private string _selectedCategoryId;
        private ProductMasterResponseDTO _selectedProductMaster;
        private ProductMasterRequestDTO? _productMasterRequestDTO;
        private bool _isEditorOpen;
        private int _totalProductCount;

        public ObservableCollection<ProductMasterResponseDTO> ProductMasterList
        {
            get => _lstProduct;
            set
            {
                _lstProduct = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CategorySummaryDTO> CategorySummaryList
        {
            get => _lstCategorySummary;
            set
            {
                _lstCategorySummary = value; OnPropertyChanged();
            }
        }
        
        public ProductMasterRequestDTO ProductMasterRequestDTO
        {
            get => _productMasterRequestDTO!;
            set
            {
                _productMasterRequestDTO = value;
                OnPropertyChanged();
            }
        }

        public ProductMasterResponseDTO SelectedProduct
        {
            get => _selectedProductMaster;
            set
            {
                _selectedProductMaster = value; 
                OnPropertyChanged();
                if(SelectedProduct != null)
                {
                    ProductMasterRequestDTO = new ProductMasterRequestDTO
                    {
                        ProductCode = SelectedProduct.ProductCode,
                        ProductName = SelectedProduct.ProductName!,
                        ProductDescription = SelectedProduct.ProductDescription!,
                        BaseQuantity = (double)SelectedProduct.BaseQuantity!,
                        BaseUOM = SelectedProduct.BaseUom!,
                        CategoryId = SelectedProduct.CategoryId!,
                        ProductImage = SelectedProduct.ProductImage!,
                        UOM = SelectedProduct.Uom!,
                    };
                }
                else
                {
                    ProductMasterRequestDTO = new ProductMasterRequestDTO();
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
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged();
                    UpdateDisplayPages();
                    _ = LoadProductAsync();
                }
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
                _ = LoadProductAsync();
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

        public string SelectedCategoryId
        {
            get => _selectedCategoryId;
            set
            {
                _selectedCategoryId = value;
                OnPropertyChanged();
                CurrentPage = 1;
                if (!string.IsNullOrEmpty(value))
                {
                    _ = LoadProductByCategoryAsync(value);
                }
                else
                {
                    _ = LoadProductAsync();
                }

            }
        }

        public bool IsEditorOpen
        {
            get => _isEditorOpen;
            set
            {
                _isEditorOpen = value;
                OnPropertyChanged();
            }
        }

        public int TotalProductCount
        {
            get => _totalProductCount;
            set
            {
                _totalProductCount = value;
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
        public ICommand FilterByCategoryCommand {get; }
        public ICommand ExportExcelCommand { get; } 
        public ICommand ImportExcelCommand { get; }
        public ICommand FilterAllCommand {  get; }

        public ProductMasterViewModel(
            IProductMasterService productMasterService,
            ICategoryService categoryService, 
            INotificationService notificationService,
            INavigationService navigationService,
            IMessengerService messengerService)
        {
            _productMasterService = productMasterService ?? throw new ArgumentException(nameof(productMasterService));
            _categoryService = categoryService ?? throw new ArgumentException(nameof(categoryService));
            _notificationService = notificationService ?? throw new ArgumentException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentException(nameof(messengerService));
            _lstProduct = new ObservableCollection<ProductMasterResponseDTO>();
            _lstCategorySummary = new ObservableCollection<CategorySummaryDTO>();
            ProductMasterRequestDTO = new ProductMasterRequestDTO();
            _currentPage = 1;
            _searchText = string.Empty;
            _selectedCategoryId =string.Empty;
            _selectedProductMaster = null!;
            _displayPages = new ObservableCollection<object>();
            
            SearchCommand = new RelayCommand(async _ => await SearchProductAsync());
            RefreshCommand = new RelayCommand(async _ => await LoadProductAsync());
            AddCommand = new RelayCommand(_ => OpenEditor(null!));
            DeleteCommand = new RelayCommand(async product => await DeleteProductAsync((ProductMasterResponseDTO)product));
            UpdateCommand = new RelayCommand(product => OpenEditor((ProductMasterResponseDTO)product));
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            FilterByCategoryCommand = new RelayCommand(categoryId => SelectedCategoryId = (string)categoryId);
            ExportExcelCommand = new RelayCommand(async p => await ExportExcelAsync(p));
            FilterAllCommand = new RelayCommand(async _ => await LoadProductAsync());
            ImportExcelCommand = new RelayCommand(async p => await ImportExcelAsync(p));

            _= _messengerService.RegisterMessageAsync("ReloadProductList", async (obj) =>
            {
                await LoadCategoryAsync(); // Giả sử là 1 hàm bất đồng bộ
            });
            _ =LoadProductAsync();
            _ = LoadCategoryAsync();
            _ = GetTotalsProduct();


        }

        private async Task ImportExcelAsync(object p)
        {
            throw new NotImplementedException();
        }

        private void UpdateDisplayPages()
        {
            DisplayPages = new ObservableCollection<object>();
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

        private async Task LoadCategoryAsync()
        {
            try
            {
                var result = await _categoryService.GetCategorySummary();
                if (result.Success && result.Data != null)
                {
                    CategorySummaryList.Clear();
                    foreach (var item in result.Data ?? Enumerable.Empty<CategorySummaryDTO>())
                    {
                        CategorySummaryList.Add(item);
                    }
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tính tổng sản phẩm theo danh mục", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}","OK",isError: true);
            }
        }

        private async Task ExportExcelAsync(object p)
        {
            throw new NotImplementedException();
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


        private async Task DeleteProductAsync(ProductMasterResponseDTO product)
        {
            if (product == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa sản phẩm {product.ProductName}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deleteResult = await _productMasterService.DeleteProductMaster(product.ProductCode);
                    if (deleteResult.Success)
                    {
                        _ = LoadProductAsync();
                        _notificationService.ShowMessage(deleteResult.Message, "OK", isError: false);
                    }
                    else
                    {
                        _notificationService.ShowMessage(deleteResult.Message ?? "Lỗi khi xóa sản phẩm", "OK", isError: true);
                    }
                }
                catch (Exception ex)
                {
                    _notificationService.ShowMessage($"Lỗi :{ex.Message}", "OK", isError: true);
                }
            }
        }

        private void OpenEditor(ProductMasterResponseDTO product)
        {
            var productDetail = App.ServiceProvider!.GetRequiredService<ucProductDetail>();
            productDetail.DataContext = new ProductDetailViewModel(
                _productMasterService,
                _categoryService,
                _notificationService,
                _navigationService,
                _messengerService,
                App.ServiceProvider!.GetRequiredService<IProductSupplierService>(),
                App.ServiceProvider!.GetRequiredService<ISupplierMasterService>(),
                App.ServiceProvider!.GetRequiredService<IProductCustomerService>(),
                App.ServiceProvider!.GetRequiredService<ICustomerMasterService>(),
                isAddingNew: product == null,
                totalOnHand: product !=null? (double)product!.TotalOnHand! : 0.00)
            {
                ProductMasterRequestDTO = product == null ? new ProductMasterRequestDTO() : new ProductMasterRequestDTO
                {
                    ProductCode = product.ProductCode,
                    ProductName = product.ProductName!,
                    ProductDescription = product.ProductDescription!,
                    ProductImage = product.ProductImage!,   
                    CategoryId = product.CategoryId!,
                    BaseQuantity = (double)product.BaseQuantity!,
                    BaseUOM = product.BaseUom!,
                    UOM = product.Uom!,
                    Valuation = (double)product.Valuation!
                }
            };
            _navigationService.NavigateTo(productDetail);
        }

        private async Task SearchProductAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    await LoadProductAsync();
                    return;
                }
                var result = await _productMasterService.SearchProductInList(SearchText, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    ProductMasterList.Clear();
                    foreach (var item in result.Data.Data ?? Enumerable.Empty<ProductMasterResponseDTO>())
                    {
                        ProductMasterList.Add(item);
                    }
                    TotalPages = result.Data.TotalPages;
                    OnPropertyChanged(nameof(ProductMasterList));
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tìm kiếm sản phẩm", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadProductAsync()
        {
            try
            {
                var result = await _productMasterService.GetAllProductMaster(CurrentPage, PageSize);
                if (result.Success && result.Data!=null)
                {
                    ProductMasterList.Clear();
                    foreach(var item in result.Data.Data?? Enumerable.Empty<ProductMasterResponseDTO>())
                    {
                        ProductMasterList.Add(item);
                    }
                    TotalPages = result.Data.TotalPages;
                    _ = GetTotalsProduct();
                }
                else
                {
                    ProductMasterList.Clear();
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách sản phẩm", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadProductByCategoryAsync(string category)
        {
            try
            {
                var result = await _productMasterService.GetAllProductWithCategoryId(category, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    ProductMasterList.Clear();
                    foreach (var item in result.Data.Data ?? Enumerable.Empty<ProductMasterResponseDTO>())
                    {
                        ProductMasterList.Add(item);
                    }
                    TotalPages = result.Data.TotalPages;
                    _ = GetTotalsProduct();
                }
                else
                {
                    ProductMasterList.Clear();
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách sản phẩm", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }
        private async Task GetTotalsProduct()
        {
            try
            {
                var result = await _productMasterService.GetTotalProductCount();
                if(result.Success)
                {
                    TotalProductCount = result.Data;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi lấy tổng số lượng sản phẩm","OK",isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }
    }
}
