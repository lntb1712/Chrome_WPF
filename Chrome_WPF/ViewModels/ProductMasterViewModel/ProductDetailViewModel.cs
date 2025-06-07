using Chrome_WPF;
using Chrome_WPF.Helpers;
using Chrome_WPF.Models.CategoryDTO;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Models.ProductSupplierDTO;
using Chrome_WPF.Models.SupplierMasterDTO;
using Chrome_WPF.Properties;
using Chrome_WPF.Services.CategoryService;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.ProductMasterService;
using Chrome_WPF.Services.ProductSupplierService;
using Chrome_WPF.Services.SupplierMasterService;
using Chrome_WPF.ViewModels;
using Chrome_WPF.Views.UserControls.ProductMaster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

public class ProductDetailViewModel : BaseViewModel
{
    private readonly IProductMasterService _productMasterService;
    private readonly IProductSupplierService _productSupplierService;
    private readonly ICategoryService _categoryService;
    private readonly INotificationService _notificationService;
    private readonly INavigationService _navigationService;
    private readonly IMessengerService _messengerService;
    private readonly ISupplierMasterService _supplierService;

    private readonly RelayCommand _saveCommand;
    private readonly RelayCommand _selectProductImageCommand;
    private readonly RelayCommand _backCommand;
    private readonly RelayCommand _nextPageCommand;
    private readonly RelayCommand _previousPageCommand;
    private readonly RelayCommand _selectPageCommand;
    private readonly RelayCommand _addSupplierLineCommand;
    private readonly RelayCommand _deleteSupplierLineCommand;

    private ProductMasterRequestDTO? _productMasterRequestDTO;
    private ObservableCollection<CategoryResponseDTO> _categories;
    private ObservableCollection<ProductSupplierResponseDTO> _lstProductSupplier;
    private ObservableCollection<SupplierMasterResponseDTO> _lstSuppliers;
    private CategoryResponseDTO _selectedCategory;
    private bool _isAddingNew;
    private string _productImage;
    private double? _totalOnHand;
    private ObservableCollection<object> _displayPages;
    private int _currentPage;
    private int _pageSize = 10;
    private int _totalPages;

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
            ProductImage = _productMasterRequestDTO.ProductImg;
            _ = LoadProductSupplierAsync();
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

    public ObservableCollection<object> DisplayPages
    {
        get => _displayPages;
        set
        {
            _displayPages = value;
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
                _ = LoadProductSupplierAsync();
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
            _ = LoadProductSupplierAsync();
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
                _productMasterRequestDTO.ProductImg = value;
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

    public RelayCommand SelectProductImageCommand => _selectProductImageCommand;
    public RelayCommand SaveCommand => _saveCommand;
    public RelayCommand BackCommand => _backCommand;
    public RelayCommand NextPageCommand => _nextPageCommand;
    public RelayCommand PreviousPageCommand => _previousPageCommand;
    public RelayCommand SelectPageCommand => _selectPageCommand;
    public RelayCommand AddSupplierLineCommand => _addSupplierLineCommand;
    public RelayCommand DeleteSupplierLineCommand => _deleteSupplierLineCommand;

    public ProductDetailViewModel(
    IProductMasterService productMasterService,
    ICategoryService categoryService,
    INotificationService notificationService,
    INavigationService navigationService,
    IMessengerService messengerService,
    IProductSupplierService productSupplierService,
    ISupplierMasterService supplierMasterService,
    bool isAddingNew = true,
    double TotalOnHand = 0.00)
    {
        // Khởi tạo hiện tại...
        _productMasterService = productMasterService ?? throw new ArgumentNullException(nameof(productMasterService));
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));
        _productSupplierService = productSupplierService ?? throw new ArgumentNullException(nameof(productSupplierService));
        _supplierService = supplierMasterService ?? throw new ArgumentException(nameof(supplierMasterService));

        _productMasterRequestDTO = new ProductMasterRequestDTO();
        IsAddingNew = isAddingNew;
        _currentPage = 1;
        _selectedCategory = null!;
        _productImage = string.Empty;
        _totalOnHand = TotalOnHand;

        _displayPages = new ObservableCollection<object>();
        _categories = new ObservableCollection<CategoryResponseDTO>();
        _lstProductSupplier = new ObservableCollection<ProductSupplierResponseDTO>();
        _lstSuppliers = new ObservableCollection<SupplierMasterResponseDTO>();

        _selectProductImageCommand = new RelayCommand(SelectImage);
        _saveCommand = new RelayCommand(async parameter => await SaveAsync(parameter), CanSave);
        _backCommand = new RelayCommand(NavigateBack);
        _nextPageCommand = new RelayCommand(_ => NextPage());
        _previousPageCommand = new RelayCommand(_ => PreviousPage());
        _selectPageCommand = new RelayCommand(page => SelectPage((int)page));
        _addSupplierLineCommand = new RelayCommand(AddSupplierLine);
        _deleteSupplierLineCommand = new RelayCommand(async productSupplier =>  await DeleteSupplierLineAsync((ProductSupplierResponseDTO) productSupplier));

        _productMasterRequestDTO.PropertyChanged += OnPropertyChangedHandler!;

        // Nạp cả danh mục và nhà cung cấp
        _ = LoadCategoriesAsync(); // Thêm dòng này
        _ = LoadSupplierAsync();
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
                    _ = LoadProductSupplierAsync();
                    _notificationService.ShowMessage(deleteResult.Message, "OK", isError: false);
                }
                else
                {
                    _notificationService.ShowMessage(deleteResult.Message, "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi :{ex.Message}", "OK", isError: true);
            }
        }
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

    private async Task LoadSupplierAsync()
    {
        try
        {
            var response = await _supplierService.GetAllSupplierMaster(1, int.MaxValue);
            if (response.Success && response.Data?.Data != null)
            {
                // Lấy danh sách SupplierCode đã có trong LstProductSupplier
                var existingSupplierCodes = LstProductSupplier
                    .Where(ps => !string.IsNullOrEmpty(ps.SupplierCode))
                    .Select(ps => ps.SupplierCode)
                    .ToHashSet();

                LstSupplier.Clear();
                foreach (var item in response.Data.Data)
                {
                    // Chỉ thêm nhà cung cấp chưa có trong LstProductSupplier
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
            var result = await _productSupplierService.GetAllProductSupplier(ProductMasterRequestDTO.ProductCode, CurrentPage, PageSize);
            if (result.Success && result.Data != null)
            {
                LstProductSupplier.Clear();
                foreach (var item in result.Data.Data ?? Enumerable.Empty<ProductSupplierResponseDTO>())
                {

                    var supplier = LstSupplier.FirstOrDefault(s => s.SupplierCode == item.SupplierCode);
                    item.SelectedSupplier = supplier!; // Gán đối tượng SupplierMasterResponseDTO
                    item.SupplierName = supplier?.SupplierName ?? "Không xác định";
                    LstProductSupplier.Add(item);
                    item.IsNewRow = false;
                }
                TotalPages = result.Data.TotalPages;
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
            Quantity = 0.00,
            LeadTime = 0,
            IsNewRow = true,
            UpdateTime = DateTime.Now.ToString("dd/MM/yyyy"),
            SelectedSupplier = null // Khởi tạo là null
        };
        LstProductSupplier.Add(newSupplier);
        _ = LoadSupplierAsync();

        // Tự động focus vào dòng mới
        _messengerService.SendMessageAsync("FocusNewSupplierRow");
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
                    Quantity = supplier.Quantity,
                    LeadTime = supplier.LeadTime,
                };

                // Kiểm tra xem nhà cung cấp đã tồn tại chưa
                var existingSupplier = await _productSupplierService.GetAllProductSupplier(ProductMasterRequestDTO.ProductCode, 1, int.MaxValue);
                var exists = existingSupplier.Success && (existingSupplier.Data?.Data.Any(s => s.SupplierCode == supplier.SupplierCode) ?? false);

                if (exists == true)
                {
                    // Cập nhật nhà cung cấp
                    var updateResult = await _productSupplierService.UpdateProductSupplier(supplierRequest);
                    if (!updateResult.Success)
                    {
                        _notificationService.ShowMessage($"Lỗi :{supplier.SupplierName}: {updateResult.Message}", "OK", isError: true);
                        return;
                    }
                }
                else
                {
                    // Thêm nhà cung cấp mới
                    var addResult = await _productSupplierService.AddProductSupplier(supplierRequest);
                    if (!addResult.Success)
                    {
                        _notificationService.ShowMessage($"Lỗi: {supplier.SupplierName}: {addResult.Message}", "OK", isError: true);
                        return;
                    }
                }
            }

            _notificationService.ShowMessage(result.Message ?? (IsAddingNew ? "Thêm sản phẩm và nhà cung cấp thành công!" : "Cập nhật sản phẩm và nhà cung cấp thành công!"), "OK", isError: false);
            if (IsAddingNew)
            {
                ProductMasterRequestDTO.ClearValidation();
                ProductMasterRequestDTO = new ProductMasterRequestDTO();
                LstProductSupplier.Clear();
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
    }
}