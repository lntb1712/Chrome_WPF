using Chrome_WPF.Helpers;
using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.OrderTypeDTO;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Models.PutAwayDTO;
using Chrome_WPF.Models.StockInDetailDTO;
using Chrome_WPF.Models.StockInDTO;
using Chrome_WPF.Models.StockOutDTO;
using Chrome_WPF.Models.SupplierMasterDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.PutAwayService;
using Chrome_WPF.Services.StockInDetailService;
using Chrome_WPF.Services.StockInService;
using Chrome_WPF.Views.UserControls.StockIn;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.StockInViewModel
{
    public class StockInDetailViewModel : BaseViewModel
    {
        private readonly IStockInDetailService _stockInDetailService;
        private readonly IStockInService _stockInService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IMessengerService _messengerService;
        private readonly IPutAwayService _putAwayService;

        private ObservableCollection<StockInDetailResponseDTO> _lstStockInDetails;
        private ObservableCollection<ProductMasterResponseDTO> _lstProducts;
        private ObservableCollection<OrderTypeResponseDTO> _lstOrderTypes;
        private ObservableCollection<WarehouseMasterResponseDTO> _lstWarehouses;
        private ObservableCollection<SupplierMasterResponseDTO> _lstSuppliers;
        private ObservableCollection<AccountManagementResponseDTO> _lstResponsiblePersons;
        private ObservableCollection<PutAwayAndDetailResponseDTO> _lstPutAway;
        private ObservableCollection<object> _displayPages;
        private StockInRequestDTO _stockInRequestDTO;
        private bool _isAddingNew;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;
        private int _lastLoadedPage; 
        private bool _hasPutAway;
        public bool HasPutAway
        {
            get => _hasPutAway;
            set
            {
                _hasPutAway = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<PutAwayAndDetailResponseDTO> LstPutAway
        {
            get => _lstPutAway;
            set
            {
                _lstPutAway = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<StockInDetailResponseDTO> LstStockInDetails
        {
            get => _lstStockInDetails;
            set
            {
                _lstStockInDetails = value;
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

        public ObservableCollection<OrderTypeResponseDTO> LstOrderTypes
        {
            get => _lstOrderTypes;
            set
            {
                _lstOrderTypes = value;
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

        public ObservableCollection<AccountManagementResponseDTO> LstResponsiblePersons
        {
            get => _lstResponsiblePersons;
            set
            {
                _lstResponsiblePersons = value;
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

        public StockInRequestDTO StockInRequestDTO
        {
            get => _stockInRequestDTO;
            set
            {
                if (_stockInRequestDTO != null)
                {
                    _stockInRequestDTO.PropertyChanged -= OnPropertyChangedHandler!;
                }
                _stockInRequestDTO = value;
                if (_stockInRequestDTO != null)
                {
                    _stockInRequestDTO.PropertyChanged += OnPropertyChangedHandler!;
                }
                OnPropertyChanged();
                _ = LoadStockInDetailsAsync();
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
                    _ = LoadStockInDetailsAsync();
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
                    _ = LoadStockInDetailsAsync();
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
        public ICommand CreatePutAwayCommand { get; }

        public StockInDetailViewModel(
            IStockInDetailService stockInDetailService,
            IStockInService stockInService,
            INotificationService notificationService,
            INavigationService navigationService,
            IMessengerService messengerService,
            IPutAwayService putAwayService,

            StockInResponseDTO stockIn = null!)
        {
            _stockInDetailService = stockInDetailService ?? throw new ArgumentNullException(nameof(stockInDetailService));
            _stockInService = stockInService ?? throw new ArgumentNullException(nameof(stockInService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));
            _putAwayService = putAwayService ?? throw new ArgumentNullException(nameof(putAwayService));

            _lstStockInDetails = new ObservableCollection<StockInDetailResponseDTO>();
            _lstProducts = new ObservableCollection<ProductMasterResponseDTO>();
            _lstOrderTypes = new ObservableCollection<OrderTypeResponseDTO>();
            _lstWarehouses = new ObservableCollection<WarehouseMasterResponseDTO>();
            _lstSuppliers = new ObservableCollection<SupplierMasterResponseDTO>();
            _lstResponsiblePersons = new ObservableCollection<AccountManagementResponseDTO>();
            _lstPutAway = new ObservableCollection<PutAwayAndDetailResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            _isAddingNew = stockIn == null;
            _currentPage = 1;
            _lastLoadedPage = 0;
            _hasPutAway = false;
            _stockInRequestDTO = stockIn == null ? new StockInRequestDTO() : new StockInRequestDTO
            {
                StockInCode = stockIn.StockInCode,
                OrderTypeCode = stockIn.OrderTypeCode!,
                WarehouseCode = stockIn.WarehouseCode!,
                SupplierCode = stockIn.SupplierCode!,
                Responsible = stockIn.Responsible!,
                OrderDeadLine = stockIn.OrderDeadline!
            };

            SaveCommand = new RelayCommand(async _ => await SaveStockInAsync(), CanSave);
            BackCommand = new RelayCommand(_ => NavigateBack());
            AddDetailLineCommand = new RelayCommand(_ => AddDetailLine(), CanAddDetailLine);
            DeleteDetailLineCommand = new RelayCommand(async detail => await DeleteDetailLineAsync((StockInDetailResponseDTO)detail));
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            ConfirmQuantityCommand = new RelayCommand(_ => CheckQuantity());
            CreatePutAwayCommand = new RelayCommand(async _ => await CreatePutAwayList());

            _stockInRequestDTO.PropertyChanged += OnPropertyChangedHandler!;
            _ = InitializeAsync();
        }

        private async Task CreatePutAwayList()
        {
            try
            {
                if (string.IsNullOrEmpty(StockInRequestDTO.StockInCode))
                {
                    LstPutAway.Clear();
                    return;
                }
                var result = await _stockInDetailService.CreatePutAway(StockInRequestDTO.StockInCode);
                if (result.Success)
                {
                    _notificationService.ShowMessage("Tạo phiếu cất hàng và chi tiết cất hàng thành công", "OK", isError: false);
                    await _messengerService.SendMessageAsync("ReloadStockInList");
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tạo phiếu cất hàng", "OK", isError: true);
                }
            
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi : {ex.Message}", "OK", isError: true);
            }
        }

        private async Task CheckPutAwayHasValue()
        {
            try
            {
                var putAwayResult  = await _putAwayService.GetPutAwayContainsCodeAsync(StockInRequestDTO.StockInCode);
                HasPutAway = putAwayResult.Success && putAwayResult.Data != null;
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi kiểm tra picklist: {ex.Message}", "OK", isError: true);
                HasPutAway = false;
            }
        }
        private async void CheckQuantity()
        {
            try
            {
                if (!LstStockInDetails.Any())
                {
                    _notificationService.ShowMessage("Danh sách chi tiết nhập kho rỗng.", "OK", isError: true);
                    return;
                }
                _ = SaveStockInAsync();
                // Kiểm tra xem có sản phẩm nào thiếu số lượng không
                bool hasShortage = LstStockInDetails.Any(d => d.Quantity < d.Demand);
                if (!hasShortage)
                {
                    // Tất cả sản phẩm đủ số lượng, xác nhận phiếu nhập kho
                    var confirmResult = await _stockInDetailService.ConfirmnStockIn(StockInRequestDTO.StockInCode);
                    if (confirmResult.Success)
                    {
                        _notificationService.ShowMessage("Xác nhận phiếu nhập kho thành công!", "OK", isError: false);
                        await _messengerService.SendMessageAsync("ReloadStockInList");
                    }
                    else
                    {
                        _notificationService.ShowMessage(confirmResult.Message ?? "Không thể xác nhận phiếu nhập kho.", "OK", isError: true);
                    }
                    return;
                }

                // Có sản phẩm thiếu, hiển thị popup để người dùng chọn
                var viewModel = new BackOrderDialogViewModel(_notificationService, new ObservableCollection<StockInDetailResponseDTO>(LstStockInDetails));
                var popup = new BackOrderDialog(viewModel)
                {
                    Owner = Application.Current.MainWindow // Căn giữa dialog
                };
                popup.ShowDialog();

                if (viewModel.IsClosed)
                {
                    if (viewModel.CreateBackorder) // Người dùng chọn tạo backorder
                    {
                        // Tạo backorder và xác nhận phiếu nhập kho
                        var backOrderResult = await _stockInDetailService.CreateBackOrder(StockInRequestDTO.StockInCode, $"Tạo back order cho phiếu nhập {StockInRequestDTO.StockInCode}");
                        if (!backOrderResult.Success)
                        {
                            _notificationService.ShowMessage(backOrderResult.Message ?? "Không thể tạo backorder.", "OK", isError: true);
                            return;
                        }

                        var confirmResult = await _stockInDetailService.ConfirmnStockIn(StockInRequestDTO.StockInCode);
                        if (confirmResult.Success)
                        {
                            _notificationService.ShowMessage("Xác nhận phiếu nhập kho và tạo backorder thành công!", "OK", isError: false);
                            await _messengerService.SendMessageAsync("ReloadStockInList");
                        }
                        else
                        {
                            _notificationService.ShowMessage(confirmResult.Message ?? "Không thể xác nhận phiếu nhập kho.", "OK", isError: true);
                        }
                    }
                    else if (viewModel.NoBackorder) // Người dùng không tạo backorder
                    {
                        // Xác nhận phiếu nhập kho mà không tạo backorder
                        var confirmResult = await _stockInDetailService.ConfirmnStockIn(StockInRequestDTO.StockInCode);
                        if (confirmResult.Success)
                        {
                            _notificationService.ShowMessage("Xác nhận phiếu nhập kho thành công, không tạo backorder.", "OK", isError: false);
                            await _messengerService.SendMessageAsync("ReloadStockInList");
                        }
                        else
                        {
                            _notificationService.ShowMessage(confirmResult.Message ?? "Không thể xác nhận phiếu nhập kho.", "OK", isError: true);
                        }
                    }
                    // Trường hợp Discard không cần xử lý thêm vì popup đã đóng
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi kiểm tra số lượng: {ex.Message}", "OK", isError: true);
            }
        }
        private async Task InitializeAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(StockInRequestDTO?.StockInCode) && !IsAddingNew)
                {
                    _notificationService.ShowMessage("Mã phiếu nhập kho không hợp lệ. Không thể khởi tạo.", "OK", isError: true);
                    NavigateBack();
                    return;
                }

                await LoadProductsAsync();
               

                await Task.WhenAll(
                    LoadOrderTypesAsync(),
                    LoadWarehousesAsync(),
                    LoadSuppliersAsync(),
                    LoadResponsiblePersonsAsync(),
                    CheckPutAwayHasValue());

                if (!IsAddingNew)
                {
                    await LoadStockInDetailsAsync();
                }
                if (HasPutAway)
                {
                    await LoadPutAway();
                }
                
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Khởi tạo thất bại: {ex.Message}", "OK", isError: true);
                NavigateBack();
            }
        }
        private async Task LoadPutAway()
        {
            try
            {
                if (string.IsNullOrEmpty(StockInRequestDTO.StockInCode))
                {
                    LstPutAway.Clear();
                    return;
                }
                var result = await _putAwayService.GetPutAwayContainsCodeAsync(StockInRequestDTO.StockInCode);
                if (result.Success && result.Data != null)
                {
                    LstPutAway.Clear();
                    LstPutAway.Add(result.Data);
                    
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải danh sách cất hàng", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải danh sách cất hàng: {ex.Message}", "OK", isError: true);
            }
        }
        private async Task LoadStockInDetailsAsync()
        {
            try
            {
                if (_lastLoadedPage == CurrentPage && LstStockInDetails.Any())
                {
                    return;
                }

                if (string.IsNullOrEmpty(StockInRequestDTO.StockInCode))
                {
                    LstStockInDetails.Clear();
                    return;
                }

                var result = await _stockInDetailService.GetAllStockInDetails(StockInRequestDTO.StockInCode, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    LstStockInDetails.Clear();
                    foreach (var detail in result.Data.Data ?? Enumerable.Empty<StockInDetailResponseDTO>())
                    {
                        detail.SelectedProduct = LstProducts.FirstOrDefault(p => p.ProductCode == detail.ProductCode);
                        detail.IsNewRow = false;
                        LstStockInDetails.Add(detail);
                    }
                    TotalPages = result.Data.TotalPages;
                    _lastLoadedPage = CurrentPage;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải chi tiết nhập kho.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải chi tiết nhập kho: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                var result = await _stockInDetailService.GetListProductToSI(StockInRequestDTO.StockInCode);
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
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải danh sách sản phẩm: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadOrderTypesAsync()
        {
            try
            {
                var result = await _stockInService.GetListOrderType("SI");
                if (result.Success && result.Data != null)
                {
                    LstOrderTypes.Clear();
                    foreach (var orderType in result.Data)
                    {
                        LstOrderTypes.Add(orderType);
                    }
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải danh sách loại lệnh.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải danh sách loại lệnh: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadWarehousesAsync()
        {
            try
            {
                var result = await _stockInService.GetListWarehousePermission();
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
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải danh sách kho: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadSuppliersAsync()
        {
            try
            {
                var result = await _stockInService.GetListSupplierMasterAsync();
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
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải danh sách nhà cung cấp: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadResponsiblePersonsAsync()
        {
            try
            {
                var result = await _stockInService.GetListResponsibleAsync();
                if (result.Success && result.Data != null)
                {
                    LstResponsiblePersons.Clear();
                    foreach (var person in result.Data)
                    {
                        LstResponsiblePersons.Add(person);
                    }
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải danh sách người phụ trách.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải danh sách người phụ trách: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task SaveStockInAsync()
        {
            try
            {
                StockInRequestDTO.RequestValidation();
                if (!string.IsNullOrEmpty(StockInRequestDTO.Error))
                {
                    _notificationService.ShowMessage($"Lỗi dữ liệu: {StockInRequestDTO.Error}", "OK", isError: true);
                    return;
                }

                ApiResult<bool> stockInResult;
                if (IsAddingNew)
                {
                    stockInResult = await _stockInService.AddStockIn(StockInRequestDTO);
                }
                else
                {
                    stockInResult = await _stockInService.UpdateStockIn(StockInRequestDTO);
                }

                if (!stockInResult.Success)
                {
                    _notificationService.ShowMessage(stockInResult.Message ?? "Không thể lưu thông tin nhập kho.", "OK", isError: true);
                    return;
                }

                foreach (var detail in LstStockInDetails)
                {
                    if (detail.SelectedProduct != null)
                    {
                        detail.ProductCode = detail.SelectedProduct.ProductCode;
                        detail.ProductName = detail.SelectedProduct.ProductName!;
                    }

                    var request = new StockInDetailRequestDTO
                    {
                        StockInCode = StockInRequestDTO.StockInCode,
                        ProductCode = detail.ProductCode,
                        Demand = detail.Demand,
                        Quantity = detail.Quantity
                    };

                    request.RequestValidation();
                    if (!string.IsNullOrEmpty(request.Error))
                    {
                        _notificationService.ShowMessage($"Lỗi dữ liệu chi tiết: {request.Error}", "OK", isError: true);
                        return;
                    }

                    ApiResult<bool> detailResult;
                    if (await IsDetailExistsAsync(detail.StockInCode, detail.ProductCode))
                    {
                        detailResult = await _stockInDetailService.UpdateStockInDetail(request);
                    }
                    else
                    {
                        detailResult = await _stockInDetailService.AddStockInDetail(request);
                    }

                    if (!detailResult.Success)
                    {
                        _notificationService.ShowMessage(detailResult.Message ?? "Không thể lưu chi tiết nhập kho.", "OK", isError: true);
                        return;
                    }
                }

                _notificationService.ShowMessage(IsAddingNew ? "Thêm phiếu nhập kho thành công!" : "Cập nhật phiếu nhập kho thành công!", "OK", isError: false);
                if (IsAddingNew)
                {
                    StockInRequestDTO.ClearValidation();
                    StockInRequestDTO = new StockInRequestDTO();
                    LstStockInDetails.Clear();
                }
                await _messengerService.SendMessageAsync("ReloadStockInList");
               
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi lưu nhập kho: {ex.Message}", "OK", isError: true);
            }
        }

        private bool CanSave(object parameter)
        {
            var dto = StockInRequestDTO;
            var propertiesToValidate = new[] { nameof(dto.StockInCode), nameof(dto.OrderTypeCode), nameof(dto.WarehouseCode), nameof(dto.SupplierCode) };
            foreach (var prop in propertiesToValidate)
            {
                if (!string.IsNullOrEmpty(dto[prop]))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CanAddDetailLine(object parameter)
        {
            return !string.IsNullOrEmpty(StockInRequestDTO.StockInCode);
        }

        private async Task<bool> IsDetailExistsAsync(string stockInCode, string productCode)
        {
            try
            {
                var result = await _stockInDetailService.GetAllStockInDetails(stockInCode, 1, int.MaxValue);
                return result.Success && result.Data?.Data.Any(d => d.ProductCode == productCode) == true;
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi kiểm tra chi tiết nhập kho: {ex.Message}", "OK", isError: true);
                return false;
            }
        }

        private void AddDetailLine()
        {
            if (string.IsNullOrEmpty(StockInRequestDTO.StockInCode))
            {
                _notificationService.ShowMessage("Vui lòng nhập mã phiếu nhập kho trước khi thêm chi tiết.", "OK", isError: true);
                return;
            }

            var newDetail = new StockInDetailResponseDTO
            {
                StockInCode = StockInRequestDTO.StockInCode,
                ProductCode = string.Empty,
                ProductName = string.Empty,
                LotNo = string.Empty,
                Demand = 0,
                Quantity = 0,
                IsNewRow = true,
                SelectedProduct = null
            };
            LstStockInDetails.Add(newDetail);
            _messengerService.SendMessageAsync("FocusNewDetailRow");
        }

        private async Task DeleteDetailLineAsync(StockInDetailResponseDTO detail)
        {
            if (detail == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa chi tiết sản phẩm {detail.ProductName}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deleteResult = await _stockInDetailService.DeleteStockInDetail(detail.StockInCode, detail.ProductCode);
                    if (deleteResult.Success)
                    {
                        LstStockInDetails.Remove(detail);
                        _notificationService.ShowMessage("Xóa chi tiết nhập kho thành công.", "OK", isError: false);
                    }
                    else
                    {
                        _notificationService.ShowMessage(deleteResult.Message ?? "Không thể xóa chi tiết nhập kho.", "OK", isError: true);
                    }
                }
                catch (Exception ex)
                {
                    _notificationService.ShowMessage($"Lỗi khi xóa chi tiết nhập kho: {ex.Message}", "OK", isError: true);
                }
            }
        }

        private void NavigateBack()
        {
            var ucStockIn = App.ServiceProvider!.GetRequiredService<ucStockIn>();
            _navigationService.NavigateTo(ucStockIn);
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
            ((RelayCommand)SaveCommand)?.RaiseCanExecuteChanged();
            ((RelayCommand)AddDetailLineCommand)?.RaiseCanExecuteChanged();
        }
    }
}