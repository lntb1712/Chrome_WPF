using Chrome_WPF;
using Chrome_WPF.Helpers;
using Chrome_WPF.Models.CategoryDTO;
using Chrome_WPF.Models.CustomerMasterDTO;
using Chrome_WPF.Models.ProductCustomerDTO;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Models.ProductSupplierDTO;
using Chrome_WPF.Models.SupplierMasterDTO;
using Chrome_WPF.Properties;
using Chrome_WPF.Services.CategoryService;
using Chrome_WPF.Services.CustomerMasterService;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.ProductCustomerService;
using Chrome_WPF.Services.ProductMasterService;
using Chrome_WPF.Services.ProductSupplierService;
using Chrome_WPF.Services.SupplierMasterService;
using Chrome_WPF.ViewModels;
using Chrome_WPF.Views.UserControls.ProductMaster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Chrome_WPF.ViewModels
{
    public class ProductDetailViewModel : BaseViewModel
    {
        private readonly IProductMasterService _productMasterService;
        private readonly IProductSupplierService _productSupplierService;
        private readonly IProductCustomerService _productCustomerService;
        private readonly ICategoryService _categoryService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IMessengerService _messengerService;
        private readonly ISupplierMasterService _supplierService;
        private readonly ICustomerMasterService _customerService;

        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _selectProductImageCommand;
        private readonly RelayCommand _backCommand;
        private readonly RelayCommand _nextPageCommand;
        private readonly RelayCommand _previousPageCommand;
        private readonly RelayCommand _selectPageCommand;
        private readonly RelayCommand _addSupplierLineCommand;
        private readonly RelayCommand _deleteSupplierLineCommand;
        private readonly RelayCommand _addCustomerLineCommand;
        private readonly RelayCommand _deleteCustomerLineCommand;

        private ProductMasterRequestDTO? _productMasterRequestDTO;
        private ObservableCollection<CategoryResponseDTO> _categories;
        private ObservableCollection<ProductSupplierResponseDTO> _lstProductSupplier;
        private ObservableCollection<SupplierMasterResponseDTO> _lstSuppliers;
        private ObservableCollection<ProductCustomerResponseDTO> _lstProductCustomer;
        private ObservableCollection<CustomerMasterResponseDTO> _lstCustomers;
        private CategoryResponseDTO _selectedCategory;
        private bool _isAddingNew;
        private string _productImage;
        private double? _totalOnHand;
        private ObservableCollection<object> _displayPages;
        private int _currentSupplierPage;
        private int _currentCustomerPage;
        private int _pageSize = 10;
        private int _totalSupplierPages;
        private int _totalCustomerPages;
        private string _activeTab = "Supplier"; // Theo dõi tab đang chọn
        public ProductMasterRequestDTO ProductMasterRequestDTO
        {
            get => _productMasterRequestDTO!;
            set
            {
                if (_productMasterRequestDTO != null)
                {
                    _productMasterRequestDTO.PropertyChanged -= OnPropertyChangedHandler!;
                }
                _productMasterRequestDTO = value;
                _productMasterRequestDTO.PropertyChanged += OnPropertyChangedHandler!;
                ProductImage = _productMasterRequestDTO.ProductImage;
                if (_productMasterRequestDTO.CategoryId.Equals("NVL"))
                {
                    ActiveTab = "Supplier";
                }
                else
                {
                    ActiveTab = "Customer";
                }
                // Load data based on active tab
                if (ActiveTab == "Supplier")
                {
                    _ = LoadProductSupplierAsync();
                }
                else if (ActiveTab == "Customer")
                {
                    _ = LoadProductCustomerAsync();
                }
                OnPropertyChanged(nameof(ProductMasterRequestDTO));
            }
        }

        public ObservableCollection<CategoryResponseDTO> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }

        public ObservableCollection<ProductSupplierResponseDTO> LstProductSupplier
        {
            get => _lstProductSupplier;
            set
            {
                _lstProductSupplier = value;
                OnPropertyChanged(nameof(LstProductSupplier));
            }
        }

        public ObservableCollection<SupplierMasterResponseDTO> LstSupplier
        {
            get => _lstSuppliers;
            set
            {
                _lstSuppliers = value;
                OnPropertyChanged(nameof(LstSupplier));
            }
        }

        public ObservableCollection<ProductCustomerResponseDTO> LstProductCustomer
        {
            get => _lstProductCustomer;
            set
            {
                _lstProductCustomer = value;
                OnPropertyChanged(nameof(LstProductCustomer));
            }
        }

        public ObservableCollection<CustomerMasterResponseDTO> LstCustomer
        {
            get => _lstCustomers;
            set
            {
                _lstCustomers = value;
                OnPropertyChanged(nameof(LstCustomer));
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

        public int CurrentSupplierPage
        {
            get => _currentSupplierPage;
            set
            {
                if (_currentSupplierPage != value)
                {
                    _currentSupplierPage = value;
                    OnPropertyChanged(nameof(CurrentSupplierPage));
                    if (_activeTab == "Supplier")
                    {
                        UpdateDisplayPages();
                        _ = LoadProductSupplierAsync();
                    }
                }
            }
        }

        public int CurrentCustomerPage
        {
            get => _currentCustomerPage;
            set
            {
                if (_currentCustomerPage != value)
                {
                    _currentCustomerPage = value;
                    OnPropertyChanged(nameof(CurrentCustomerPage));
                    if (_activeTab == "Customer")
                    {
                        UpdateDisplayPages();
                        _ = LoadProductCustomerAsync();
                    }
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
                CurrentSupplierPage = 1;
                CurrentCustomerPage = 1;
                if (_activeTab == "Supplier")
                {
                    _ = LoadProductSupplierAsync();
                }
                else if (_activeTab == "Customer")
                {
                    _ = LoadProductCustomerAsync();
                }
            }
        }

        public int TotalSupplierPages
        {
            get => _totalSupplierPages;
            set
            {
                _totalSupplierPages = value;
                OnPropertyChanged(nameof(TotalSupplierPages));
                if (_activeTab == "Supplier") UpdateDisplayPages();
            }
        }

        public int TotalCustomerPages
        {
            get => _totalCustomerPages;
            set
            {
                _totalCustomerPages = value;
                OnPropertyChanged(nameof(TotalCustomerPages));
                if (_activeTab == "Customer") UpdateDisplayPages();
            }
        }

        public CategoryResponseDTO SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                if (_productMasterRequestDTO != null)
                {
                    _productMasterRequestDTO.CategoryId = value?.CategoryId!;
                }
                OnPropertyChanged(nameof(SelectedCategory));
                OnPropertyChanged(nameof(ProductMasterRequestDTO));
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

        public string ProductImage
        {
            get => _productImage;
            set
            {
                _productImage = value;
                if (_productMasterRequestDTO != null)
                {
                    _productMasterRequestDTO.ProductImage = value;
                }
                OnPropertyChanged(nameof(ProductImage));
            }
        }

        public double? TotalOnHand
        {
            get => _totalOnHand;
            set
            {
                _totalOnHand = value;
                OnPropertyChanged(nameof(TotalOnHand));
            }
        }

        public string ActiveTab
        {
            get => _activeTab;
            set
            {
                _activeTab = value;
                UpdateDisplayPages();
                OnPropertyChanged(nameof(ActiveTab));

                // Load data for the newly selected tab
                if (_activeTab == "Supplier")
                {
                    _ = LoadProductSupplierAsync();
                }
                else if (_activeTab == "Customer")
                {
                    _ = LoadProductCustomerAsync();
                }
            }
        }

        public RelayCommand SelectProductImageCommand => _selectProductImageCommand;
        public RelayCommand SaveCommand => _saveCommand;
        public RelayCommand BackCommand => _backCommand;
        public RelayCommand NextPageCommand => _nextPageCommand;
        public RelayCommand PreviousPageCommand => _previousPageCommand;
        public RelayCommand SelectPageCommand => _selectPageCommand;
        public RelayCommand AddSupplierLineCommand => _addSupplierLineCommand;
        public RelayCommand DeleteSupplierLineCommand => _deleteSupplierLineCommand;
        public RelayCommand AddCustomerLineCommand => _addCustomerLineCommand;
        public RelayCommand DeleteCustomerLineCommand => _deleteCustomerLineCommand;

        public ProductDetailViewModel(
            IProductMasterService productMasterService,
            ICategoryService categoryService,
            INotificationService notificationService,
            INavigationService navigationService,
            IMessengerService messengerService,
            IProductSupplierService productSupplierService,
            ISupplierMasterService supplierMasterService,
            IProductCustomerService productCustomerService,
            ICustomerMasterService customerMasterService,
            bool isAddingNew = true,
            double totalOnHand = 0.00)
        {
            _productMasterService = productMasterService ?? throw new ArgumentNullException(nameof(productMasterService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));
            _productSupplierService = productSupplierService ?? throw new ArgumentNullException(nameof(productSupplierService));
            _supplierService = supplierMasterService ?? throw new ArgumentNullException(nameof(supplierMasterService));
            _productCustomerService = productCustomerService ?? throw new ArgumentNullException(nameof(productCustomerService));
            _customerService = customerMasterService ?? throw new ArgumentNullException(nameof(customerMasterService));

            _productMasterRequestDTO = new ProductMasterRequestDTO();
            IsAddingNew = isAddingNew;
            _currentSupplierPage = 1;
            _currentCustomerPage = 1;
            _selectedCategory = null!;
            _productImage = string.Empty;
            _totalOnHand = totalOnHand;

            _displayPages = new ObservableCollection<object>();
            _categories = new ObservableCollection<CategoryResponseDTO>();
            _lstProductSupplier = new ObservableCollection<ProductSupplierResponseDTO>();
            _lstSuppliers = new ObservableCollection<SupplierMasterResponseDTO>();
            _lstProductCustomer = new ObservableCollection<ProductCustomerResponseDTO>();
            _lstCustomers = new ObservableCollection<CustomerMasterResponseDTO>();

            _selectProductImageCommand = new RelayCommand(SelectImage);
            _saveCommand = new RelayCommand(async parameter => await SaveAsync(parameter), CanSave);
            _backCommand = new RelayCommand(NavigateBack);
            _nextPageCommand = new RelayCommand(_ => NextPage());
            _previousPageCommand = new RelayCommand(_ => PreviousPage());
            _selectPageCommand = new RelayCommand(page => SelectPage((int)page));
            _addSupplierLineCommand = new RelayCommand(AddSupplierLine);
            _deleteSupplierLineCommand = new RelayCommand(async productSupplier => await DeleteSupplierLineAsync((ProductSupplierResponseDTO)productSupplier));
            _addCustomerLineCommand = new RelayCommand(AddCustomerLine);
            _deleteCustomerLineCommand = new RelayCommand(async productCustomer => await DeleteCustomerLineAsync((ProductCustomerResponseDTO)productCustomer));

            _productMasterRequestDTO.PropertyChanged += OnPropertyChangedHandler!;
           
                _ = LoadCategoriesAsync();
         
                _ = LoadSupplierAsync();
            
       
                _ = LoadCustomerAsync();
            
      
            
        }

        private async Task DeleteSupplierLineAsync(ProductSupplierResponseDTO productSupplier)
        {
            if (productSupplier == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa thông tin cung cấp {productSupplier.SupplierName}", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deleteResult = await _productSupplierService.DeleteProductSupplier(productSupplier.ProductCode, productSupplier.SupplierCode);
                    if (deleteResult.Success)
                    {
                        if (_activeTab == "Supplier")
                        {
                            _ = LoadProductSupplierAsync();
                        }
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

        private async Task DeleteCustomerLineAsync(ProductCustomerResponseDTO productCustomer)
        {
            if (productCustomer == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa thông tin khách hàng {productCustomer.CustomerName}", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deleteResult = await _productCustomerService.DeleteProductCustomer(productCustomer.ProductCode, productCustomer.CustomerCode);
                    if (deleteResult.Success)
                    {
                        if (_activeTab == "Customer")
                        {
                            _ = LoadProductCustomerAsync();
                        }
                        _notificationService.ShowMessage(deleteResult.Message!, "OK", isError: false);
                    }
                    else
                    {
                        _notificationService.ShowMessage(deleteResult.Message!, "OK", isError: true);
                    }
                }
                catch (Exception ex)
                {
                    _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
                }
            }
        }

        private void UpdateDisplayPages()
        {
            DisplayPages.Clear();
            int totalPages = _activeTab == "Supplier" ? TotalSupplierPages : TotalCustomerPages;
            int currentPage = _activeTab == "Supplier" ? CurrentSupplierPage : CurrentCustomerPage;

            if (totalPages <= 0) return;

            int startPage = Math.Max(1, currentPage - 2);
            int endPage = Math.Min(totalPages, currentPage + 2);

            if (startPage > 1)
                DisplayPages.Add(1);
            if (startPage > 2)
                DisplayPages.Add("...");

            for (int i = startPage; i <= endPage; i++)
                DisplayPages.Add(i);

            if (endPage < totalPages - 1)
                DisplayPages.Add("...");
            if (endPage < totalPages)
                DisplayPages.Add(totalPages);
        }

        private async Task LoadSupplierAsync()
        {
            try
            {
                var response = await _supplierService.GetAllSupplierMaster(1, int.MaxValue);
                if (response.Success && response.Data?.Data != null)
                {
                    var existingSupplierCodes = LstProductSupplier
                        .Where(ps => !string.IsNullOrEmpty(ps.SupplierCode))
                        .Select(ps => ps.SupplierCode)
                        .ToHashSet();

                    LstSupplier.Clear();
                    foreach (var item in response.Data.Data)
                    {
                        if (!existingSupplierCodes.Contains(item.SupplierCode))
                        {
                            LstSupplier.Add(item);
                        }
                    }
                    OnPropertyChanged(nameof(LstSupplier));
                }
                else
                {
                    _notificationService.ShowMessage(response.Message ?? "Lỗi khi tải danh sách nhà cung cấp", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadCustomerAsync()
        {
            try
            {
                var response = await _customerService.GetAllCustomerMaster(1, int.MaxValue);
                if (response.Success && response.Data?.Data != null)
                {
                    var existingCustomerCodes = LstProductCustomer
                        .Where(pc => !string.IsNullOrEmpty(pc.CustomerCode))
                        .Select(pc => pc.CustomerCode)
                        .ToHashSet();

                    LstCustomer.Clear();
                    foreach (var item in response.Data.Data)
                    {
                        if (!existingCustomerCodes.Contains(item.CustomerCode))
                        {
                            LstCustomer.Add(item);
                        }
                    }
                    OnPropertyChanged(nameof(LstCustomer));
                }
                else
                {
                    _notificationService.ShowMessage(response.Message ?? "Lỗi khi tải danh sách khách hàng", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var result = await _categoryService.GetAllCategories();
                if (result.Success && result.Data != null)
                {
                    Categories.Clear();
                    foreach (var category in result.Data)
                    {
                        Categories.Add(category);
                    }
                    UpdateSelectedCategory();
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách danh mục", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadProductSupplierAsync()
        {
            try
            {
                var result = await _productSupplierService.GetAllProductSupplier(ProductMasterRequestDTO.ProductCode, CurrentSupplierPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    LstProductSupplier.Clear();
                    foreach (var item in result.Data.Data ?? Enumerable.Empty<ProductSupplierResponseDTO>())
                    {
                        var supplier = LstSupplier.FirstOrDefault(s => s.SupplierCode == item.SupplierCode);
                        item.SelectedSupplier = supplier!;
                        item.SupplierName = supplier?.SupplierName ?? "Không xác định";
                        item.IsNewRow = false;
                        LstProductSupplier.Add(item);
                    }
                    TotalSupplierPages = result.Data.TotalPages;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách thông tin cung cấp", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadProductCustomerAsync()
        {
            try
            {
                var result = await _productCustomerService.GetAllProductCustomers(ProductMasterRequestDTO.ProductCode, CurrentCustomerPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    LstProductCustomer.Clear();
                    foreach (var item in result.Data.Data ?? Enumerable.Empty<ProductCustomerResponseDTO>())
                    {
                        var customer = LstCustomer.FirstOrDefault(c => c.CustomerCode == item.CustomerCode);
                        item.SelectedCustomer = customer!;
                        item.CustomerName = customer?.CustomerName ?? "Không xác định";
                        item.IsNewRow = false;
                        LstProductCustomer.Add(item);
                    }
                    TotalCustomerPages = result.Data.TotalPages;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách thông tin khách hàng", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private void SelectImage(object parameter)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Chọn hình ảnh sản phẩm",
                    Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|All files (*.*)|*.*",
                    Multiselect = false
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                    if (imageBytes == null || imageBytes.Length == 0)
                    {
                        _notificationService.ShowMessage("Tệp ảnh rỗng hoặc không thể đọc.", "OK", isError: true);
                        return;
                    }

                    string base64String = Convert.ToBase64String(imageBytes);
                    ProductImage = base64String;
                }
            }
            catch (IOException ex)
            {
                _notificationService.ShowMessage($"Lỗi khi đọc tệp ảnh: {ex.Message}", "OK", isError: true);
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi chọn hình ảnh: {ex.Message}", "OK", isError: true);
            }
        }

        private void AddSupplierLine(object parameter)
        {
            var newSupplier = new ProductSupplierResponseDTO
            {
                ProductCode = ProductMasterRequestDTO.ProductCode,
                SupplierCode = string.Empty,
                SupplierName = string.Empty,
                PricePerUnit = 0.00,
                LeadTime = 0,
                IsNewRow = true,
                SelectedSupplier = null
            };
            LstProductSupplier.Add(newSupplier);
            _ = LoadSupplierAsync();

            _messengerService.SendMessageAsync("FocusNewSupplierRow");
        }

        private void AddCustomerLine(object parameter)
        {
            var newCustomer = new ProductCustomerResponseDTO
            {
                ProductCode = ProductMasterRequestDTO.ProductCode,
                CustomerCode = string.Empty,
                CustomerName = string.Empty,
                PricePerUnit = 0.00,
                ExpectedDeliverTime = 0,
                IsNewRow = true,
                SelectedCustomer = null
            };
            LstProductCustomer.Add(newCustomer);
            _ = LoadCustomerAsync();

            _messengerService.SendMessageAsync("FocusNewCustomerRow");
        }

        private async Task SaveAsync(object parameter)
        {
            try
            {
                ProductMasterRequestDTO.RequestValidation();
                if (!CanSave(parameter))
                {
                    _notificationService.ShowMessage("Vui lòng kiểm tra lại thông tin nhập vào.", "OK", isError: true);
                    return;
                }

                // Lưu sản phẩm
                var result = IsAddingNew
                    ? await _productMasterService.AddProductMaster(ProductMasterRequestDTO)
                    : await _productMasterService.UpdateProductMaster(ProductMasterRequestDTO);

                if (!result.Success)
                {
                    _notificationService.ShowMessage(result.Message ?? (IsAddingNew ? "Lỗi khi thêm sản phẩm." : "Lỗi khi cập nhật sản phẩm."), "OK", isError: true);
                    return;
                }

                // Lưu danh sách nhà cung cấp
                foreach (var supplier in LstProductSupplier)
                {
                    var supplierRequest = new ProductSupplierRequestDTO
                    {
                        ProductCode = ProductMasterRequestDTO.ProductCode,
                        SupplierCode = supplier.SelectedSupplier?.SupplierCode ?? supplier.SupplierCode,
                        PricePerUnit = supplier.PricePerUnit,
                        LeadTime = supplier.LeadTime,
                    };

                    var existingSupplier = await _productSupplierService.GetAllProductSupplier(ProductMasterRequestDTO.ProductCode, 1, int.MaxValue);
                    var exists = existingSupplier.Success && (existingSupplier.Data?.Data.Any(s => s.SupplierCode == supplier.SupplierCode) ?? false);

                    if (exists)
                    {
                        var updateResult = await _productSupplierService.UpdateProductSupplier(supplierRequest);
                        if (!updateResult.Success)
                        {
                            _notificationService.ShowMessage($"Lỗi: {supplier.SupplierName}: {updateResult.Message}", "OK", isError: true);
                            return;
                        }
                    }
                    else
                    {
                        var addResult = await _productSupplierService.AddProductSupplier(supplierRequest);
                        if (!addResult.Success)
                        {
                            _notificationService.ShowMessage($"Lỗi: {supplier.SupplierName}: {addResult.Message}", "OK", isError: true);
                            return;
                        }
                    }
                }

                // Lưu danh sách khách hàng
                foreach (var customer in LstProductCustomer)
                {
                    var customerRequest = new ProductCustomerRequestDTO
                    {
                        ProductCode = ProductMasterRequestDTO.ProductCode,
                        CustomerCode = customer.SelectedCustomer?.CustomerCode ?? customer.CustomerCode,
                        PricePerUnit = customer.PricePerUnit,
                        ExpectedDeliverTime = customer.ExpectedDeliverTime,
                    };

                    var existingCustomer = await _productCustomerService.GetAllProductCustomers(ProductMasterRequestDTO.ProductCode, 1, int.MaxValue);
                    var exists = existingCustomer.Success && (existingCustomer.Data?.Data.Any(c => c.CustomerCode == customer.CustomerCode) ?? false);

                    if (exists)
                    {
                        var updateResult = await _productCustomerService.UpdateProductCustomer(customerRequest);
                        if (!updateResult.Success)
                        {
                            _notificationService.ShowMessage($"Lỗi: {customer.CustomerName}: {updateResult.Message}", "OK", isError: true);
                            return;
                        }
                    }
                    else
                    {
                        var addResult = await _productCustomerService.AddProductCustomer(customerRequest);
                        if (!addResult.Success)
                        {
                            _notificationService.ShowMessage($"Lỗi: {customer.CustomerName}: {addResult.Message}", "OK", isError: true);
                            return;
                        }
                    }
                }

                _notificationService.ShowMessage(result.Message ?? (IsAddingNew ? "Thêm sản phẩm, nhà cung cấp và khách hàng thành công!" : "Cập nhật sản phẩm, nhà cung cấp và khách hàng thành công!"), "OK", isError: false);
                if (IsAddingNew)
                {
                    ProductMasterRequestDTO.ClearValidation();
                    ProductMasterRequestDTO = new ProductMasterRequestDTO();
                    LstProductSupplier.Clear();
                    LstProductCustomer.Clear();
                }
                await _messengerService.SendMessageAsync("ReloadProductList");
                NavigateBack(null!);
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private bool CanSave(object parameter)
        {
            var dto = ProductMasterRequestDTO;
            var propertiesToValidate = new[] { nameof(dto.ProductCode), nameof(dto.ProductName) };

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
            var ucProductMaster = App.ServiceProvider!.GetRequiredService<ucProductMaster>();
            _navigationService.NavigateTo<ucProductMaster>();
        }

        private void PreviousPage()
        {
            if (_activeTab == "Supplier" && CurrentSupplierPage > 1)
            {
                CurrentSupplierPage--;
            }
            else if (_activeTab == "Customer" && CurrentCustomerPage > 1)
            {
                CurrentCustomerPage--;
            }
        }

        private void NextPage()
        {
            if (_activeTab == "Supplier" && CurrentSupplierPage < TotalSupplierPages)
            {
                CurrentSupplierPage++;
            }
            else if (_activeTab == "Customer" && CurrentCustomerPage < TotalCustomerPages)
            {
                CurrentCustomerPage++;
            }
        }

        private void SelectPage(int page)
        {
            if (_activeTab == "Supplier" && page >= 1 && page <= TotalSupplierPages)
            {
                CurrentSupplierPage = page;
            }
            else if (_activeTab == "Customer" && page >= 1 && page <= TotalCustomerPages)
            {
                CurrentCustomerPage = page;
            }
        }

        private void UpdateSelectedCategory()
        {
            if (_productMasterRequestDTO != null && Categories != null)
            {
                _selectedCategory = Categories.FirstOrDefault(c => c.CategoryId == _productMasterRequestDTO.CategoryId)!;
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        private void OnPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            _saveCommand?.RaiseCanExecuteChanged();
            _selectProductImageCommand?.RaiseCanExecuteChanged();
            _backCommand?.RaiseCanExecuteChanged();
            _addSupplierLineCommand?.RaiseCanExecuteChanged();
            _addCustomerLineCommand?.RaiseCanExecuteChanged();
        }
    }
}