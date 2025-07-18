using Chrome_WPF.Helpers;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Models.PurchaseOrderDetailDTO;
using Chrome_WPF.Models.PurchaseOrderDTO;
using Chrome_WPF.Models.SupplierMasterDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using Chrome_WPF.Services.CodeGeneratorService;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.PurchaseOrderDetailService;
using Chrome_WPF.Services.PurchaseOrderService;
using Chrome_WPF.Views.UserControls.PurchaseOrder;
using ClosedXML.Excel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.PurchaseOrderViewModel
{
    public class PurchaseOrderDetailViewModel : BaseViewModel
    {
        private readonly IPurchaseOrderDetailService _purchaseOrderDetailService;
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IMessengerService _messengerService;
        private readonly ICodeGenerateService _codeGenerateService;

        private ObservableCollection<PurchaseOrderDetailResponseDTO> _lstPurchaseOrderDetails;
        private ObservableCollection<ProductMasterResponseDTO> _lstProducts;
        private ObservableCollection<WarehouseMasterResponseDTO> _lstWarehouses;
        private ObservableCollection<SupplierMasterResponseDTO> _lstSuppliers;
        private ObservableCollection<object> _displayPages;
        private PurchaseOrderRequestDTO _purchaseOrderRequestDTO;
        private bool _isAddingNew;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;
        private int _lastLoadedPage;
        private bool _isSaving;
        private string _applicableLocation;
        public ObservableCollection<PurchaseOrderDetailResponseDTO> LstPurchaseOrderDetails
        {
            get => _lstPurchaseOrderDetails;
            set
            {
                if (_lstPurchaseOrderDetails != null)
                {
                    _lstPurchaseOrderDetails.CollectionChanged -= PurchaseOrderDetails_CollectionChanged!;
                }
                _lstPurchaseOrderDetails = value;
                if (_lstPurchaseOrderDetails != null)
                {
                    _lstPurchaseOrderDetails.CollectionChanged += PurchaseOrderDetails_CollectionChanged!;
                }
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProductMasterResponseDTO> LstProducts
        {
            get => _lstProducts;
            set
            {
                _lstProducts = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<WarehouseMasterResponseDTO> LstWarehouses
        {
            get => _lstWarehouses;
            set
            {
                _lstWarehouses = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SupplierMasterResponseDTO> LstSuppliers
        {
            get => _lstSuppliers;
            set
            {
                _lstSuppliers = value;
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

        public PurchaseOrderRequestDTO PurchaseOrderRequestDTO
        {
            get => _purchaseOrderRequestDTO;
            set
            {
                if (_purchaseOrderRequestDTO != null)
                {
                    _purchaseOrderRequestDTO.PropertyChanged -= OnPropertyChangedHandler!;
                }
                _purchaseOrderRequestDTO = value;
                if (_purchaseOrderRequestDTO != null)
                {
                    _purchaseOrderRequestDTO.PropertyChanged += OnPropertyChangedHandler!;
                }
                OnPropertyChanged();
                _ = LoadPurchaseOrderDetailsAsync();
            }
        }

        public bool IsAddingNew
        {
            get => _isAddingNew;
            set
            {
                _isAddingNew = value;
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
                    _ = LoadPurchaseOrderDetailsAsync();
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
                    OnPropertyChanged();
                    CurrentPage = 1;
                    _ = LoadPurchaseOrderDetailsAsync();
                }
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

        public ICommand SaveCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand AddDetailLineCommand { get; }
        public ICommand DeleteDetailLineCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand SelectPageCommand { get; }
        public ICommand ConfirmQuantityCommand { get; }
        public ICommand ExportExcelCommand { get; }

        public PurchaseOrderDetailViewModel(
            IPurchaseOrderDetailService purchaseOrderDetailService,
            IPurchaseOrderService purchaseOrderService,
            INotificationService notificationService,
            INavigationService navigationService,
            IMessengerService messengerService,
            ICodeGenerateService codeGenerateService,
            
            PurchaseOrderResponseDTO? purchaseOrder = null)
        {
            _purchaseOrderDetailService = purchaseOrderDetailService ?? throw new ArgumentNullException(nameof(purchaseOrderDetailService));
            _purchaseOrderService = purchaseOrderService ?? throw new ArgumentNullException(nameof(purchaseOrderService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));
            _codeGenerateService = codeGenerateService ?? throw new ArgumentNullException(nameof(codeGenerateService));

            _lstPurchaseOrderDetails = new ObservableCollection<PurchaseOrderDetailResponseDTO>();
            _lstPurchaseOrderDetails.CollectionChanged += PurchaseOrderDetails_CollectionChanged!;
            _lstProducts = new ObservableCollection<ProductMasterResponseDTO>();
            _lstWarehouses = new ObservableCollection<WarehouseMasterResponseDTO>();
            _lstSuppliers = new ObservableCollection<SupplierMasterResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            _isAddingNew = purchaseOrder == null;
            _currentPage = 1;
            _lastLoadedPage = 0;
            _isSaving = false;
            _purchaseOrderRequestDTO = purchaseOrder == null ? new PurchaseOrderRequestDTO
            {
                OrderDate = DateTime.Now.ToString("dd/MM/yyyy"),
                ExpectedDate = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy")
            } : new PurchaseOrderRequestDTO
            {
                PurchaseOrderCode = purchaseOrder.PurchaseOrderCode ?? string.Empty,
                WarehouseCode = purchaseOrder.WarehouseCode ?? string.Empty,
                SupplierCode = purchaseOrder.SupplierCode ?? string.Empty,
                OrderDate = purchaseOrder.OrderDate ?? string.Empty,
                ExpectedDate = purchaseOrder.ExpectedDate ?? string.Empty,
                PurchaseOrderDescription = purchaseOrder.PurchaseOrderDescription ?? string.Empty
            };

            SaveCommand = new RelayCommand(async parameter => await SavePurchaseOrderAsync(parameter), CanSave);
            BackCommand = new RelayCommand(_ => NavigateBack());
            AddDetailLineCommand = new RelayCommand(_ => AddDetailLine(), CanAddDetailLine);
            DeleteDetailLineCommand = new RelayCommand(async detail => await DeleteDetailLineAsync((PurchaseOrderDetailResponseDTO)detail));
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            ConfirmQuantityCommand = new RelayCommand(async parameter => await CheckQuantityAsync(parameter), CanConfirmQuantity);
            ExportExcelCommand = new RelayCommand(async parameter => await ExportAndPreview(parameter));

            List<string> warehousePermissions = new List<string>();
            var savedPermissions = Properties.Settings.Default.WarehousePermission;
            if (savedPermissions != null)
            {
                warehousePermissions = savedPermissions.Cast<string>().ToList();
            }
            _applicableLocation = warehousePermissions.First().ToString();
            _purchaseOrderRequestDTO.PropertyChanged += OnPropertyChangedHandler!;
            _ = InitializeAsync();
        }

        private void PurchaseOrderDetails_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                foreach (var item in _lstPurchaseOrderDetails)
                {
                    item.PropertyChanged -= PurchaseOrderDetail_PropertyChanged!;
                }
            }
            if (e.OldItems != null)
            {
                foreach (PurchaseOrderDetailResponseDTO item in e.OldItems)
                {
                    item.PropertyChanged -= PurchaseOrderDetail_PropertyChanged!;
                }
            }
            if (e.NewItems != null)
            {
                foreach (PurchaseOrderDetailResponseDTO item in e.NewItems)
                {
                    item.PropertyChanged += PurchaseOrderDetail_PropertyChanged!;
                }
            }
        }

        private void PurchaseOrderDetail_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == nameof(PurchaseOrderDetailResponseDTO.SelectedProduct))
                {
                    var detail = (PurchaseOrderDetailResponseDTO)sender;
                    if (detail?.SelectedProduct != null)
                    {
                        _ = Task.Delay(500); // Consider replacing with proper debouncing if needed
                    }
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi xử lý thay đổi chi tiết: {ex.Message}", "OK", isError: true);
            }
        }
        private async Task GenerateCodeAsync()
        {
            try
            {
                var result = await _codeGenerateService.CodeGeneratorAsync(_applicableLocation,"PO");
                if (result.Success && !string.IsNullOrEmpty(result.Data))
                {
                    PurchaseOrderRequestDTO.PurchaseOrderCode = result.Data;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tạo mã đơn đặt hàng.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tạo mã đơn đặt hàng: {ex.Message}", "OK", isError: true);
            }
        }
        private async Task InitializeAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(PurchaseOrderRequestDTO?.PurchaseOrderCode) && !IsAddingNew)
                {
                    _notificationService.ShowMessage("Mã đơn đặt hàng không hợp lệ. Không thể khởi tạo.", "OK", isError: true);
                    NavigateBack();
                    return;
                }
                if (IsAddingNew)
                {
                    await GenerateCodeAsync();
                    if (string.IsNullOrEmpty(PurchaseOrderRequestDTO!.PurchaseOrderCode))
                    {
                        _notificationService.ShowMessage("Không thể tạo mã đơn đặt hàng mới.", "OK", isError: true);

                        return;
                    }
                }
                if (!LstWarehouses.Any())
                {
                    await LoadWarehousesAsync();
                    if (IsAddingNew && LstWarehouses.Any())
                    {
                        PurchaseOrderRequestDTO!.WarehouseCode = LstWarehouses.First().WarehouseCode;
                    }
                }
                if (!LstSuppliers.Any())
                {
                    await LoadSuppliersAsync();
                    if (IsAddingNew && LstSuppliers.Any())
                    {
                        PurchaseOrderRequestDTO!.SupplierCode = LstSuppliers.First().SupplierCode;
                    }
                }
                if (!IsAddingNew)
                {
                    if (!LstProducts.Any())
                    {
                        await LoadProductsAsync();
                    }
                    await LoadPurchaseOrderDetailsAsync();
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Khởi tạo thất bại: {ex.Message}", "OK", isError: true);
                NavigateBack();
            }
        }

        private async Task CheckQuantityAsync(object parameter)
        {
            try
            {
                if (!LstPurchaseOrderDetails.Any())
                {
                    _notificationService.ShowMessage("Danh sách chi tiết đơn đặt hàng rỗng.", "OK", isError: true);
                    return;
                }

                await SavePurchaseOrderAsync(parameter);
                if (!string.IsNullOrEmpty(PurchaseOrderRequestDTO.Error))
                {
                    return;
                }

                bool hasShortage = LstPurchaseOrderDetails.Any(d => d.QuantityReceived < d.Quantity);
                if (!hasShortage)
                {
                    var confirmResult = await _purchaseOrderDetailService.ConfirmPurchaseOrderDetail(PurchaseOrderRequestDTO.PurchaseOrderCode);
                    if (confirmResult.Success)
                    {
                        _notificationService.ShowMessage("Xác nhận đơn đặt hàng thành công!", "OK", isError: false);
                        await _messengerService.SendMessageAsync("ReloadPurchaseOrderList");
                        NavigateBack();
                    }
                    else
                    {
                        _notificationService.ShowMessage(confirmResult.Message ?? "Không thể xác nhận đơn đặt hàng.", "OK", isError: true);
                    }
                    return;
                }

                var viewModel = new BackOrderDialogViewModel(_notificationService, new ObservableCollection<PurchaseOrderDetailResponseDTO>(LstPurchaseOrderDetails));
                var popup = new Views.UserControls.PurchaseOrder.BackOrderDialog(viewModel)
                {
                    DataContext = viewModel,
                    Owner = Application.Current.MainWindow
                };
                popup.ShowDialog();

                if (viewModel.IsClosed)
                {
                    if (viewModel.CreateBackorder)
                    {
                        var backOrderResult = await _purchaseOrderDetailService.CreateBackOrder(
                            PurchaseOrderRequestDTO.PurchaseOrderCode,
                            $"Tạo back order cho đơn đặt hàng {PurchaseOrderRequestDTO.PurchaseOrderCode}",
                            viewModel.SelectedDate.ToString()!);
                        if (!backOrderResult.Success)
                        {
                            _notificationService.ShowMessage(backOrderResult.Message ?? "Không thể tạo backorder.", "OK", isError: true);
                            return;
                        }

                        var confirmResult = await _purchaseOrderDetailService.ConfirmPurchaseOrderDetail(PurchaseOrderRequestDTO.PurchaseOrderCode);
                        if (confirmResult.Success)
                        {
                            _notificationService.ShowMessage("Xác nhận đơn đặt hàng và tạo backorder thành công!", "OK", isError: false);
                            await _messengerService.SendMessageAsync("ReloadPurchaseOrderList");
                            NavigateBack();
                        }
                        else
                        {
                            _notificationService.ShowMessage(confirmResult.Message ?? "Không thể xác nhận đơn đặt hàng.", "OK", isError: true);
                        }
                    }
                    else if (viewModel.NoBackorder)
                    {
                        var confirmResult = await _purchaseOrderDetailService.ConfirmPurchaseOrderDetail(PurchaseOrderRequestDTO.PurchaseOrderCode);
                        if (confirmResult.Success)
                        {
                            _notificationService.ShowMessage("Xác nhận đơn đặt hàng thành công, không tạo backorder.", "OK", isError: false);
                            await _messengerService.SendMessageAsync("ReloadPurchaseOrderList");
                            NavigateBack();
                        }
                        else
                        {
                            _notificationService.ShowMessage(confirmResult.Message ?? "Không thể xác nhận đơn đặt hàng.", "OK", isError: true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi kiểm tra số lượng: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadPurchaseOrderDetailsAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(PurchaseOrderRequestDTO.PurchaseOrderCode))
                {
                    LstPurchaseOrderDetails.Clear();
                    TotalPages = 0;
                    return;
                }

                var result = await _purchaseOrderDetailService.GetAllPurchaseOrderDetails(PurchaseOrderRequestDTO.PurchaseOrderCode, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    LstPurchaseOrderDetails.Clear();
                    foreach (var detail in result.Data.Data ?? Enumerable.Empty<PurchaseOrderDetailResponseDTO>())
                    {
                        detail.SelectedProduct = LstProducts.FirstOrDefault(p => p.ProductCode == detail.ProductCode);
                        detail.IsNewRow = false;
                        LstPurchaseOrderDetails.Add(detail);
                    }
                    TotalPages = result.Data.TotalPages;
                    _lastLoadedPage = CurrentPage;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải chi tiết đơn đặt hàng.", "OK", isError: true);
                    LstPurchaseOrderDetails.Clear();
                    TotalPages = 0;
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải chi tiết đơn đặt hàng: {ex.Message}", "OK", isError: true);
                LstPurchaseOrderDetails.Clear();
                TotalPages = 0;
            }
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                var result = await _purchaseOrderDetailService.GetListProductToPO(PurchaseOrderRequestDTO.PurchaseOrderCode);
                if (result.Success && result.Data != null)
                {
                    LstProducts.Clear();
                    foreach (var product in result.Data)
                    {
                        LstProducts.Add(product);
                    }
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải danh sách sản phẩm.", "OK", isError: true);
                    LstProducts.Clear();
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải danh sách sản phẩm: {ex.Message}", "OK", isError: true);
                LstProducts.Clear();
            }
        }

        private async Task LoadWarehousesAsync()
        {
            try
            {
                var result = await _purchaseOrderService.GetListWarehousePermission();
                if (result.Success && result.Data != null)
                {
                    LstWarehouses.Clear();
                    foreach (var warehouse in result.Data)
                    {
                        LstWarehouses.Add(warehouse);
                    }
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải danh sách kho.", "OK", isError: true);
                    LstWarehouses.Clear();
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải danh sách kho: {ex.Message}", "OK", isError: true);
                LstWarehouses.Clear();
            }
        }

        private async Task LoadSuppliersAsync()
        {
            try
            {
                var result = await _purchaseOrderService.GetListSupplierMasterAsync();
                if (result.Success && result.Data != null)
                {
                    LstSuppliers.Clear();
                    foreach (var supplier in result.Data)
                    {
                        LstSuppliers.Add(supplier);
                    }
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải danh sách nhà cung cấp.", "OK", isError: true);
                    LstSuppliers.Clear();
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải danh sách nhà cung cấp: {ex.Message}", "OK", isError: true);
                LstSuppliers.Clear();
            }
        }

        private async Task SavePurchaseOrderAsync(object parameter)
        {
            if (_isSaving) return;

            try
            {
                _isSaving = true;

                PurchaseOrderRequestDTO.RequestValidation();
                if (!string.IsNullOrEmpty(PurchaseOrderRequestDTO.Error) || !CanSave(parameter))
                {
                    _notificationService.ShowMessage(PurchaseOrderRequestDTO.Error ?? "Vui lòng kiểm tra lại thông tin nhập vào.", "OK", isError: true);
                    return;
                }

                ApiResult<bool> purchaseOrderResult;
                if (IsAddingNew)
                {
                    purchaseOrderResult = await _purchaseOrderService.AddPurchaseOrder(PurchaseOrderRequestDTO);
                }
                else
                {
                    purchaseOrderResult = await _purchaseOrderService.UpdatePurchaseOrder(PurchaseOrderRequestDTO);
                }

                if (!purchaseOrderResult.Success)
                {
                    _notificationService.ShowMessage(purchaseOrderResult.Message ?? "Không thể lưu thông tin đơn đặt hàng.", "OK", isError: true);
                    return;
                }

                foreach (var detail in LstPurchaseOrderDetails.ToList())
                {
                    if (detail?.SelectedProduct == null)
                    {
                        _notificationService.ShowMessage("Vui lòng chọn sản phẩm cho tất cả các dòng.", "OK", isError: true);
                        return;
                    }

                    detail.ProductCode = detail.SelectedProduct.ProductCode ?? throw new InvalidOperationException("Product code cannot be null.");
                    detail.ProductName = detail.SelectedProduct.ProductName ?? string.Empty;

                    var request = new PurchaseOrderDetailRequestDTO
                    {
                        PurchaseOrderCode = PurchaseOrderRequestDTO.PurchaseOrderCode,
                        ProductCode = detail.ProductCode,
                        Quantity = detail.Quantity ?? 0,
                        QuantityReceived = detail.QuantityReceived ?? 0
                    };

                    request.RequestValidation();
                    if (!string.IsNullOrEmpty(request.Error))
                    {
                        _notificationService.ShowMessage($"Lỗi dữ liệu chi tiết: {request.Error}", "OK", isError: true);
                        return;
                    }

                    ApiResult<bool> detailResult;
                    if (await IsDetailExistsAsync(detail.PurchaseOrderCode, detail.ProductCode))
                    {
                        detailResult = await _purchaseOrderDetailService.UpdatePurchaseOrderDetail(request);
                    }
                    else
                    {
                        detailResult = await _purchaseOrderDetailService.AddPurchaseOrderDetail(request);
                    }

                    if (!detailResult.Success)
                    {
                        _notificationService.ShowMessage(detailResult.Message ?? "Không thể lưu chi tiết đơn đặt hàng.", "OK", isError: true);
                        return;
                    }
                }

                _notificationService.ShowMessage(IsAddingNew ? "Thêm đơn đặt hàng thành công!" : "Cập nhật đơn đặt hàng thành công!", "OK", isError: false);
                if (IsAddingNew)
                {
                    PurchaseOrderRequestDTO.ClearValidation();
                    IsAddingNew = false;
                    if (!LstProducts.Any())
                    {
                        await LoadProductsAsync();
                    }
                }

                await _messengerService.SendMessageAsync("ReloadPurchaseOrderList");
                await LoadPurchaseOrderDetailsAsync();
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi lưu thông tin đơn đặt hàng: {ex.Message}", "OK", isError: true);
            }
            finally
            {
                _isSaving = false;
            }
        }

        private bool CanSave(object parameter)
        {
            var dto = PurchaseOrderRequestDTO;
            var propertiesToValidate = new[] { nameof(dto.PurchaseOrderCode), nameof(dto.WarehouseCode), nameof(dto.SupplierCode), nameof(dto.OrderDate), nameof(dto.ExpectedDate) };
            foreach (var prop in propertiesToValidate)
            {
                if (!string.IsNullOrEmpty(dto[prop]?.ToString()))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CanConfirmQuantity(object parameter)
        {
            return !string.IsNullOrEmpty(PurchaseOrderRequestDTO?.PurchaseOrderCode) && string.IsNullOrEmpty(PurchaseOrderRequestDTO?.Error);
        }

        private bool CanAddDetailLine(object parameter)
        {
            return !string.IsNullOrEmpty(PurchaseOrderRequestDTO?.PurchaseOrderCode);
        }

        private async Task<bool> IsDetailExistsAsync(string purchaseOrderCode, string productCode)
        {
            try
            {
                var result = await _purchaseOrderDetailService.GetAllPurchaseOrderDetails(purchaseOrderCode, 1, int.MaxValue);
                return result.Success && result.Data?.Data.Any(d => d.ProductCode == productCode) == true;
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi kiểm tra chi tiết đơn đặt hàng: {ex.Message}", "OK", isError: true);
                return false;
            }
        }

        private void AddDetailLine()
        {
            if (string.IsNullOrEmpty(PurchaseOrderRequestDTO.PurchaseOrderCode))
            {
                _notificationService.ShowMessage("Vui lòng nhập mã đơn đặt hàng trước khi thêm chi tiết.", "OK", isError: true);
                return;
            }

            var newDetail = new PurchaseOrderDetailResponseDTO
            {
                PurchaseOrderCode = PurchaseOrderRequestDTO.PurchaseOrderCode,
                ProductCode = string.Empty,
                ProductName = string.Empty,
                Quantity = 0,
                QuantityReceived = 0,
                IsNewRow = true,
                SelectedProduct = null
            };
            LstPurchaseOrderDetails.Add(newDetail);
            _ = _messengerService.SendMessageAsync("FocusNewDetailRow");
        }

        private async Task DeleteDetailLineAsync(PurchaseOrderDetailResponseDTO detail)
        {
            if (detail == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa chi tiết sản phẩm {detail.ProductName}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (!detail.IsNewRow)
                    {
                        var deleteResult = await _purchaseOrderDetailService.DeletePurchaseOrderDetail(detail.PurchaseOrderCode, detail.ProductCode);
                        if (!deleteResult.Success)
                        {
                            _notificationService.ShowMessage(deleteResult.Message ?? "Không thể xóa chi tiết đơn đặt hàng.", "OK", isError: true);
                            return;
                        }
                    }

                    LstPurchaseOrderDetails.Remove(detail);
                    _notificationService.ShowMessage("Xóa chi tiết đơn đặt hàng thành công.", "OK", isError: false);
                }
                catch (Exception ex)
                {
                    _notificationService.ShowMessage($"Lỗi khi xóa chi tiết đơn đặt hàng: {ex.Message}", "OK", isError: true);
                }
            }
        }

        private void NavigateBack()
        {
            var ucPurchaseOrder = App.ServiceProvider!.GetRequiredService<ucPurchaseOrder>();
            _navigationService.NavigateTo(ucPurchaseOrder);
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
            if (e.PropertyName == nameof(PurchaseOrderRequestDTO.PurchaseOrderCode))
            {
                ((RelayCommand)ConfirmQuantityCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)AddDetailLineCommand)?.RaiseCanExecuteChanged();
            }
            else if (new[] { nameof(PurchaseOrderRequestDTO.PurchaseOrderCode), nameof(PurchaseOrderRequestDTO.WarehouseCode), nameof(PurchaseOrderRequestDTO.SupplierCode), nameof(PurchaseOrderRequestDTO.OrderDate), nameof(PurchaseOrderRequestDTO.ExpectedDate) }.Contains(e.PropertyName))
            {
                ((RelayCommand)SaveCommand)?.RaiseCanExecuteChanged();
            }
        }
        private Task ExportAndPreview(object parameter)
        {
            try
            {
                // Path to the template Excel file
                string templatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "DonDatHang.xlsx");

                // Path to save the exported file on the Desktop
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string exportPath = Path.Combine(desktopPath, $"DonDatHang_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
                // Load the Excel template
                using (var workbook = new XLWorkbook(templatePath))
                {
                    var worksheet = workbook.Worksheet(1); // Assuming data is in the first worksheet

                    // Populate header fields
                    worksheet.Cell(3, 7).Value = $"Ngày {PurchaseOrderRequestDTO.OrderDate}";
                    worksheet.Cell(4, 5).Value = Properties.Settings.Default.FullName; // Người in
                    worksheet.Cell(5, 5).Value = Properties.Settings.Default.RoleName; // Chức vụ
                    worksheet.Cell(6, 5).Value = PurchaseOrderRequestDTO.PurchaseOrderDescription; // Nội dung
                    worksheet.Cell(1, 8).Value += PurchaseOrderRequestDTO.PurchaseOrderCode; // Mã đơn
                    worksheet.Cell(4, 8).Value = LstWarehouses.FirstOrDefault(w => w.WarehouseCode == PurchaseOrderRequestDTO.WarehouseCode)?.WarehouseName ?? ""; // Tên kho
                    worksheet.Cell(5, 8).Value = LstSuppliers.FirstOrDefault(s => s.SupplierCode == PurchaseOrderRequestDTO.SupplierCode)?.SupplierName ?? ""; // Tên nhà cung cấp
                    worksheet.Cell(6, 8).Value = PurchaseOrderRequestDTO.ExpectedDate; // Ngày dự kiến giao

                    // Define the starting row for data (based on the template, data starts at row 10)
                    int startRow = 10;
                    int stt = 1;

                    // Populate the data from LstPurchaseOrderDetails
                    foreach (var detail in LstPurchaseOrderDetails)
                    {
                        worksheet.Cell(startRow, 2).Value = stt; // STT
                        worksheet.Cell(startRow, 3).Value = detail.ProductCode; // Mã sản phẩm
                        worksheet.Cell(startRow, 5).Value = detail.ProductName; // Tên sản phẩm
                        worksheet.Cell(startRow, 6).Value = detail.Quantity; // Số lượng đặt hàng
                        worksheet.Cell(startRow, 7).Value = ""; // Ghi chú (currently empty as per template)
                        startRow++;
                        stt++;
                    }

                    // Save the workbook to the Desktop
                    workbook.SaveAs(exportPath);

                    // Notify user of success
                    _notificationService.ShowMessage($"Xuất đơn đặt hàng thành công! File được lưu tại: {exportPath}", "OK", isError: false);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi xuất đơn đặt hàng: {ex.Message}", "OK", isError: true);
            }

            return Task.CompletedTask;
        }
    }
}