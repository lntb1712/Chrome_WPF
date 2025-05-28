using Chrome_WPF.Helpers;
using Chrome_WPF.Models.CategoryDTO;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Services.CategoryService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.ProductMasterService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.ProductMasterViewModel
{
    public class ProductMasterViewModel: BaseViewModel
    {
        private readonly IProductMasterService _productMasterService;
        private readonly ICategoryService _categoryService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private ObservableCollection<ProductMasterResponseDTO> _lstProduct;
        private ObservableCollection<CategorySummaryDTO>_lstCategorySummary;
        private ObservableCollection<object> _displayPages;
        private string _searchText;
        private int _currentPage;
        private int _pageSize=10;
        private int _totalPages;
        private string _selectedCategoryId;
        private ProductMasterResponseDTO _selectedProductMaster;
        private ProductMasterRequestDTO _productMasterRequestDTO;
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
            get => _productMasterRequestDTO;
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
                        ProductName = SelectedProduct.ProductName,
                        ProductDescription = SelectedProduct.ProductDescription,
                        BaseQuantity = SelectedProduct.BaseQuantity,
                        BaseUom = SelectedProduct.BaseUom,
                        CategoryId = SelectedProduct.CategoryId,
                        ProductImg = SelectedProduct.ProductImg,
                        Uom = SelectedProduct.Uom,
                        UpdateBy = SelectedProduct.UpdateBy,
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

        public ProductMasterViewModel(IProductMasterService productMasterService,ICategoryService categoryService, INotificationService notificationService, INavigationService navigationService)
        {
            _productMasterService = productMasterService ?? throw new ArgumentException(nameof(productMasterService));
            _categoryService = categoryService ?? throw new ArgumentException(nameof(categoryService));
            _notificationService = notificationService ?? throw new ArgumentException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentException(nameof(navigationService));
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
            UpdateCommand = new RelayCommand(product => OpenEditor((ProductMasterResponseDTO)product);
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            FilterByCategoryCommand = new RelayCommand(categoryId => SelectedCategoryId = (string)categoryId);
            ExportExcelCommand = new RelayCommand(async p => await ExportExcelAsync(p));
            FilterAllCommand = new RelayCommand(async _ => await LoadProductAsync());
            _=LoadProductAsync();
            _ = LoadCategoryAsync();
            _ = GetTotalsProduct();


        }

        private object LoadCategoryAsync()
        {
            throw new NotImplementedException();
        }

        private async Task ExportExcelAsync(object p)
        {
            throw new NotImplementedException();
        }

        private void SelectPage(int page)
        {
            throw new NotImplementedException();
        }

        private void NextPage()
        {
            throw new NotImplementedException();
        }

        private void PreviousPage()
        {
            throw new NotImplementedException();
        }

        private async Task DeleteProductAsync(ProductMasterResponseDTO product)
        {
            throw new NotImplementedException();
        }

        private void OpenEditor(object value)
        {
            throw new NotImplementedException();
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
