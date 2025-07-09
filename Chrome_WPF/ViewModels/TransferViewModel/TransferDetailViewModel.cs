using Chrome_WPF.Helpers;
using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.InventoryDTO;
using Chrome_WPF.Models.OrderTypeDTO;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.PickListDTO;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Models.PutAwayDTO;
using Chrome_WPF.Models.ReservationDTO;
using Chrome_WPF.Models.StockInDTO;
using Chrome_WPF.Models.StockOutDTO;
using Chrome_WPF.Models.TransferDetailDTO;
using Chrome_WPF.Models.TransferDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.PickListService;
using Chrome_WPF.Services.PutAwayService;
using Chrome_WPF.Services.ReservationService;
using Chrome_WPF.Services.TransferDetailService;
using Chrome_WPF.Services.TransferService;
using Chrome_WPF.Views.UserControls.StockIn;
using Chrome_WPF.Views.UserControls.Transfer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.TransferViewModel
{
    public class TransferDetailViewModel : BaseViewModel
    {
        private readonly ITransferService _transferService;
        private readonly ITransferDetailService _transferDetailService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IMessengerService _messengerService;
        private readonly IPutAwayService _putAwayService;
        private readonly IPickListService _pickListService;
        private readonly IReservationService _reservationService;

        private ObservableCollection<TransferDetailResponseDTO> _lstTransferDetails;
        private ObservableCollection<InventorySummaryDTO> _lstProducts;
        private ObservableCollection<OrderTypeResponseDTO> _lstOrderTypes;
        private ObservableCollection<WarehouseMasterResponseDTO> _lstWarehouses;
        private ObservableCollection<WarehouseMasterResponseDTO> _lstToWarehouses; // Thêm mới
        private ObservableCollection<AccountManagementResponseDTO> _lstFromResponsiblePersons;
        private ObservableCollection<AccountManagementResponseDTO> _lstToResponsiblePersons;
        private ObservableCollection<PutAwayAndDetailResponseDTO> _lstPutAway;
        private ObservableCollection<ReservationAndDetailResponseDTO> _lstReservations;
        private ObservableCollection<PickAndDetailResponseDTO> _lstPickList;
        private ObservableCollection<object> _displayPages;
        private TransferRequestDTO _transferRequestDTO;
        private bool _isAddingNew;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;
        private int _lastLoadedPage;
        private bool _isSaving; 
        private bool _hasPutAway;
        private bool _hasPicklist;
        private bool _hasReservation;
        public bool HasPutAway
        {
            get => _hasPutAway;
            set
            {
                _hasPutAway = value;
                OnPropertyChanged();
            }
        }
        public bool HasReservation
        {
            get => _hasReservation;
            set
            {
                _hasReservation = value;
                OnPropertyChanged();
            }
        }
        public bool HasPicklist
        {
            get => _hasPicklist;
            set
            {
                _hasPicklist = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<TransferDetailResponseDTO> LstTransferDetails
        {
            get => _lstTransferDetails;
            set
            {
                _lstTransferDetails = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<InventorySummaryDTO> LstProducts
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

        public ObservableCollection<WarehouseMasterResponseDTO> LstToWarehouses // Thêm mới
        {
            get => _lstToWarehouses;
            set
            {
                _lstToWarehouses = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<AccountManagementResponseDTO> LstFromResponsiblePersons
        {
            get => _lstFromResponsiblePersons;
            set
            {
                _lstFromResponsiblePersons = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<AccountManagementResponseDTO> LstToResponsiblePersons
        {
            get => _lstToResponsiblePersons;
            set
            {
                _lstToResponsiblePersons = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ReservationAndDetailResponseDTO> LstReservations
        {
            get => _lstReservations;
            set
            {
                _lstReservations = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<PickAndDetailResponseDTO> LstPickList
        {
            get => _lstPickList;
            set
            {
                _lstPickList = value;
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

        public ObservableCollection<object> DisplayPages
        {
            get => _displayPages;
            set
            {
                _displayPages = value;
                OnPropertyChanged();
            }
        }

        public TransferRequestDTO TransferRequestDTO
        {
            get => _transferRequestDTO;
            set
            {
                if (_transferRequestDTO != null)
                {
                    _transferRequestDTO.PropertyChanged -= OnTransferRequestDTOPropertyChanged!;
                }
                _transferRequestDTO = value;
                if (_transferRequestDTO != null)
                {
                    _transferRequestDTO.PropertyChanged += OnTransferRequestDTOPropertyChanged!;
                }
                OnPropertyChanged();
                _ = LoadTransferDetailsAsync();
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
                    _ = LoadTransferDetailsAsync();
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
                    _ = LoadTransferDetailsAsync();
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
        public ICommand ConfirmCommand { get; }

        public TransferDetailViewModel(
            ITransferService transferService,
            ITransferDetailService transferDetailService,
            INotificationService notificationService,
            INavigationService navigationService,
            IMessengerService messengerService,
            IPutAwayService putAwayService,
            IPickListService pickListService,
            IReservationService reservationService,
            TransferResponseDTO transfer = null!)
        {
            _transferService = transferService ?? throw new ArgumentNullException(nameof(transferService));
            _transferDetailService = transferDetailService ?? throw new ArgumentNullException(nameof(transferDetailService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));
            _putAwayService = putAwayService ?? throw new ArgumentNullException(nameof(putAwayService));
            _pickListService = pickListService ?? throw new ArgumentNullException(nameof(pickListService));
            _reservationService = reservationService ?? throw new ArgumentNullException(nameof(reservationService));

            _lstTransferDetails = new ObservableCollection<TransferDetailResponseDTO>();
            _lstProducts = new ObservableCollection<InventorySummaryDTO>();
            _lstOrderTypes = new ObservableCollection<OrderTypeResponseDTO>();
            _lstWarehouses = new ObservableCollection<WarehouseMasterResponseDTO>();
            _lstToWarehouses = new ObservableCollection<WarehouseMasterResponseDTO>(); // Khởi tạo mới
            _lstFromResponsiblePersons = new ObservableCollection<AccountManagementResponseDTO>();
            _lstToResponsiblePersons = new ObservableCollection<AccountManagementResponseDTO>();
            _lstReservations = new ObservableCollection<ReservationAndDetailResponseDTO>();
            _lstPutAway = new ObservableCollection<PutAwayAndDetailResponseDTO>();
            _lstPickList = new ObservableCollection<PickAndDetailResponseDTO>();
            _displayPages = new ObservableCollection<object>();

            _isAddingNew = transfer == null;
            _currentPage = 1;
            _lastLoadedPage = 0;
            _isSaving = false;
            _hasPicklist = false;
            _hasReservation = false;
            _hasPutAway = false;

            _transferRequestDTO = transfer == null ? new TransferRequestDTO() : new TransferRequestDTO
            {
                TransferCode = transfer.TransferCode,
                OrderTypeCode = transfer.OrderTypeCode,
                FromWarehouseCode = transfer.FromWarehouseCode!,
                ToWarehouseCode = transfer.ToWarehouseCode!,
                FromResponsible = transfer.FromResponsible!,
                ToResponsible = transfer.ToResponsible!,
                StatusId = transfer.StatusId,
                TransferDate = transfer.TransferDate!,
                TransferDescription = transfer.TransferDescription
            };

            SaveCommand = new RelayCommand(async parameter => await SaveTransferAsync(parameter), CanSave);
            BackCommand = new RelayCommand(_ => NavigateBack());
            AddDetailLineCommand = new RelayCommand(_ => AddDetailLine(), CanAddDetailLine);
            DeleteDetailLineCommand = new RelayCommand(async detail => await DeleteDetailLineAsync((TransferDetailResponseDTO)detail));
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            ConfirmCommand = new RelayCommand(async parameter => await ConfirmTransferAsync(parameter), CanConfirm);

            _transferRequestDTO.PropertyChanged += OnTransferRequestDTOPropertyChanged!;
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(TransferRequestDTO?.TransferCode) && !IsAddingNew)
                {
                    _notificationService.ShowMessage("Mã phiếu chuyển kho không hợp lệ. Không thể khởi tạo.", "OK", isError: true);
                    NavigateBack();
                    return;
                }

                await Task.WhenAll(
                    LoadOrderTypesAsync(),
                    LoadWarehousesAsync(),
                    LoadFromResponsiblePersonsAsync(),
                    LoadToResponsiblePersonsAsync(),
                    LoadProductsAsync(),
                    CheckReservationExistenceAsync(),
                    CheckPicklistExistenceAsync(),
                    CheckPutAwayHasValue());

                if (!IsAddingNew)
                {
                    await LoadTransferDetailsAsync();
                }
                if(HasReservation)
                {
                    await LoadReservationsAsync();
                }    
                if(HasPicklist)
                {
                    await LoadPickListAsync();
                }    
                if(HasPutAway)
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
        private async Task CheckReservationExistenceAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(TransferRequestDTO.TransferCode))
                {
                    HasReservation = false;
                    return;
                }

                var reservationResult = await _reservationService.GetReservationsByTransferCodeAsync(TransferRequestDTO.TransferCode);
                HasReservation = reservationResult.Success && reservationResult.Data != null;
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi kiểm tra đặt chỗ: {ex.Message}", "OK", isError: true);
                HasReservation = false;
            }
        }

        private async Task LoadReservationsAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(TransferRequestDTO.TransferCode))
                {
                    LstReservations.Clear();
                    HasPicklist = false;
                    return;
                }

                var reservationResult = await _reservationService.GetReservationsByTransferCodeAsync(TransferRequestDTO.TransferCode);
                if (reservationResult.Success && reservationResult.Data != null)
                {
                    LstReservations.Clear();
                    LstReservations.Add(reservationResult.Data);
                    // Check for picklist existence
                    await CheckPicklistExistenceAsync();
                }
                else
                {
                    LstReservations.Clear();
                    HasPicklist = false;
                    _notificationService.ShowMessage(reservationResult.Message ?? "Không thể tải danh sách đặt chỗ.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải danh sách đặt chỗ: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task CheckPicklistExistenceAsync()
        {
            try
            {
                var picklistResult = await _pickListService.GetPickListContainCodeAsync(TransferRequestDTO.TransferCode);
                HasPicklist = picklistResult.Success && picklistResult.Data != null;
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi kiểm tra picklist: {ex.Message}", "OK", isError: true);
                HasPicklist = false;
            }
        }

        private async Task LoadPickListAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(TransferRequestDTO.TransferCode))
                {
                    LstPickList.Clear();
                    HasPicklist = false;
                    return;
                }

                var pickListResult = await _pickListService.GetPickListContainCodeAsync(TransferRequestDTO.TransferCode);
                if (pickListResult.Success && pickListResult.Data != null)
                {
                    LstPickList.Clear();
                    LstPickList.Add(pickListResult.Data);
                    // Check for picklist existence
                    await CheckPicklistExistenceAsync();
                }
                else
                {
                    LstPickList.Clear();
                    HasPicklist = false;
                    _notificationService.ShowMessage(pickListResult.Message ?? "Không thể tải danh sách lấy hàng.", "OK", isError: true);
                }
             
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải danh sách đặt chỗ: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task CheckPutAwayHasValue()
        {
            try
            {
                var putAwayResult = await _putAwayService.GetPutAwayContainsCodeAsync(TransferRequestDTO.TransferCode);
                HasPutAway = putAwayResult.Success && putAwayResult.Data != null;
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi kiểm tra picklist: {ex.Message}", "OK", isError: true);
                HasPutAway = false;
            }
        }
        
        private async Task LoadPutAway()
        {
            try
            {
                if (string.IsNullOrEmpty(TransferRequestDTO.TransferCode))
                {
                    LstPutAway.Clear();
                    return;
                }
                var result = await _putAwayService.GetListPutAwayContainsCodeAsync(TransferRequestDTO.TransferCode);
                if (result.Success && result.Data != null)
                {
                    LstPutAway.Clear();
                    foreach(var item in result.Data)
                    {
                        LstPutAway.Add(item);
                    }    

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

        private async Task LoadWarehousesAsync()
        {
            try
            {
                var warehouseCodes = Properties.Settings.Default.WarehousePermission?.Cast<string>().ToArray() ?? Array.Empty<string>();
                var result = await _transferService.GetListWarehousePermission(warehouseCodes);
                if (result.Success && result.Data != null)
                {
                    LstWarehouses.Clear();
                    LstToWarehouses.Clear();
                    foreach (var warehouse in result.Data)
                    {
                        LstWarehouses.Add(warehouse);
                    }
                    // Populate LstToWarehouses, excluding FromWarehouseCode
                    UpdateToWarehouses();
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

        private void UpdateToWarehouses()
        {
            LstToWarehouses.Clear();
            if (string.IsNullOrEmpty(TransferRequestDTO.FromWarehouseCode))
            {
                foreach (var warehouse in LstWarehouses)
                {
                    LstToWarehouses.Add(warehouse);
                }
            }
            else
            {
                foreach (var warehouse in LstWarehouses.Where(w => w.WarehouseCode != TransferRequestDTO.FromWarehouseCode))
                {
                    LstToWarehouses.Add(warehouse);
                }
            }
        }

        private async Task LoadTransferDetailsAsync()
        {
            try
            {
                if (_lastLoadedPage == CurrentPage && LstTransferDetails.Any())
                {
                    return;
                }

                if (string.IsNullOrEmpty(TransferRequestDTO.TransferCode))
                {
                    LstTransferDetails.Clear();
                    return;
                }

                var result = await _transferDetailService.GetTransferDetailsByTransferCodeAsync(TransferRequestDTO.TransferCode, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    LstTransferDetails.Clear();
                    foreach (var detail in result.Data.Data ?? Enumerable.Empty<TransferDetailResponseDTO>())
                    {
                        detail.SelectedProduct = LstProducts.FirstOrDefault(p => p.ProductCode == detail.ProductCode);
                        detail.IsNewRow = false;
                        LstTransferDetails.Add(detail);
                    }
                    TotalPages = result.Data.TotalPages;
                    _lastLoadedPage = CurrentPage;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải chi tiết chuyển kho.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải chi tiết chuyển kho: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(TransferRequestDTO.FromWarehouseCode))
                {
                    LstProducts.Clear();
                    return;
                }

                var result = await _transferDetailService.GetProductByWarehouseCode(TransferRequestDTO.FromWarehouseCode);
                if (result.Success && result.Data != null)
                {
                    LstProducts.Clear();
                    foreach (var product in result.Data)
                    {
                        LstProducts.Add(product);
                        Console.WriteLine($"Product: {product.ProductCode}, {product.ProductName}");
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
                var result = await _transferService.GetListOrderType("TF");
                if (result.Success && result.Data != null)
                {
                    LstOrderTypes.Clear();
                    foreach (var orderType in result.Data)
                    {
                        LstOrderTypes.Add(orderType);
                        Console.WriteLine($"OrderType: {orderType.OrderTypeCode}, {orderType.OrderTypeName}");
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

        private async Task LoadFromResponsiblePersonsAsync()
        {
            try
            {
                // Kiểm tra nếu FromWarehouseCode rỗng hoặc null
                if (string.IsNullOrWhiteSpace(TransferRequestDTO?.FromWarehouseCode))
                {
                    LstFromResponsiblePersons.Clear();
                    return;
                }

                // Gọi API để lấy danh sách người phụ trách dựa trên FromWarehouseCode
                var result = await _transferService.GetListFromResponsibleAsync(TransferRequestDTO.FromWarehouseCode);
                if (result.Success && result.Data != null)
                {
                    LstFromResponsiblePersons.Clear();
                    foreach (var person in result.Data)
                    {
                        LstFromResponsiblePersons.Add(person);
                    }

                    
                }
                else
                {
                    LstFromResponsiblePersons.Clear();
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải danh sách người xuất.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                LstFromResponsiblePersons.Clear();
                _notificationService.ShowMessage($"Lỗi khi tải danh sách người xuất: {ex.Message}", "Error", isError:true);
            }
        }

        private async Task LoadToResponsiblePersonsAsync()
        {
            try
            {
                // Kiểm tra nếu ToWarehouseCode rỗng hoặc null
                if (string.IsNullOrWhiteSpace(TransferRequestDTO?.ToWarehouseCode))
                {
                    LstToResponsiblePersons.Clear();
                    return;
                }

                // Gọi API để lấy danh sách người nhận dựa trên ToWarehouseCode
                var result = await _transferService.GetListToResponsibleAsync(TransferRequestDTO.ToWarehouseCode);
                if (result.Success && result.Data != null)
                {
                    LstToResponsiblePersons.Clear();
                    foreach (var person in result.Data)
                    {
                        LstToResponsiblePersons.Add(person);
                    }

                    
                }
                else
                {
                    LstToResponsiblePersons.Clear();
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải danh sách người nhận.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                LstToResponsiblePersons.Clear();
                _notificationService.ShowMessage($"Lỗi khi tải danh sách người nhận: {ex.Message}", "Error", isError: true);
            }
        }

        private async Task SaveTransferAsync(object parameter)
        {
            if (_isSaving) return;

            try
            {
                _isSaving = true;

                TransferRequestDTO.RequestValidation();
                if (!CanSave(parameter))
                {
                    _notificationService.ShowMessage("Vui lòng kiểm tra lại thông tin nhập vào.", "OK", isError: true);
                    return;
                }

                // Validate that FromWarehouseCode and ToWarehouseCode are different
                if (TransferRequestDTO.FromWarehouseCode == TransferRequestDTO.ToWarehouseCode)
                {
                    _notificationService.ShowMessage("Kho xuất và kho nhận không được trùng nhau.", "OK", isError: true);
                    return;
                }

                ApiResult<bool> transferResult;
                if (IsAddingNew)
                {
                    transferResult = await _transferService.AddTransfer(TransferRequestDTO);
                }
                else
                {
                    transferResult = await _transferService.UpdateTransfer(TransferRequestDTO);
                }

                if (!transferResult.Success)
                {
                    _notificationService.ShowMessage(transferResult.Message ?? "Không thể lưu thông tin chuyển kho.", "OK", isError: true);
                    return;
                }

                foreach (var detail in LstTransferDetails.ToList())
                {
                    if (detail.SelectedProduct == null)
                    {
                        _notificationService.ShowMessage("Vui lòng chọn sản phẩm cho tất cả các dòng.", "OK", isError: true);
                        return;
                    }

                    detail.ProductCode = detail.SelectedProduct.ProductCode;
                    detail.ProductName = detail.SelectedProduct.ProductName ?? string.Empty;

                    var request = new TransferDetailRequestDTO
                    {
                        TransferCode = TransferRequestDTO.TransferCode,
                        ProductCode = detail.ProductCode,
                        Demand = detail.Demand,
                        QuantityInBounded = detail.QuantityInBounded,
                        QuantityOutBounded = detail.QuantityOutBounded,
                    };

                    request.RequestValidation();
                    if (!string.IsNullOrEmpty(request.Error))
                    {
                        _notificationService.ShowMessage($"Lỗi dữ liệu chi tiết: {request.Error}", "OK", isError: true);
                        return;
                    }

                    ApiResult<bool> detailResult;
                    if (await IsDetailExistsAsync(detail.TransferCode, detail.ProductCode))
                    {
                        detailResult = await _transferDetailService.UpdateTransferDetail(request);
                    }
                    else
                    {
                        detailResult = await _transferDetailService.AddTransferDetail(request);
                    }

                    if (!detailResult.Success)
                    {
                        _notificationService.ShowMessage(detailResult.Message ?? "Không thể lưu chi tiết chuyển kho.", "OK", isError: true);
                        return;
                    }
                }

                _notificationService.ShowMessage(IsAddingNew ? "Thêm phiếu chuyển kho thành công!" : "Cập nhật phiếu chuyển kho thành công!", "OK", isError: false);
                if (IsAddingNew)
                {
                    TransferRequestDTO.ClearValidation();
                    IsAddingNew = false;
                    // Load products only when adding new to ensure dropdowns are populated
                    if (!LstProducts.Any())
                    {
                        await LoadProductsAsync();
                    }
                }

                await LoadTransferDetailsAsync();
                await CheckPicklistExistenceAsync();
                await CheckReservationExistenceAsync();
                await CheckPutAwayHasValue();
                if (HasReservation)
                {
                    await LoadReservationsAsync();
                }
                if (HasPicklist)
                {
                    await LoadPickListAsync();
                }
                if (HasPutAway)
                {
                    await LoadPutAway();
                }
                await _messengerService.SendMessageAsync("ReloadTransferList");
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi lưu thông tin chuyển kho: {ex.Message}", "OK", isError: true);
            }
            finally
            {
                _isSaving = false;
            }
        }

        private async Task ConfirmTransferAsync(object parameter)
        {
            try
            {
                if (!LstTransferDetails.Any())
                {
                    _notificationService.ShowMessage("Danh sách chi tiết chuyển kho rỗng.", "OK", isError: true);
                    return;
                }

                await SaveTransferAsync(parameter);
                if (!string.IsNullOrEmpty(TransferRequestDTO.Error))
                {
                    return;
                }

                var confirmResult = await _transferService.UpdateTransfer(new TransferRequestDTO
                {
                    TransferCode = TransferRequestDTO.TransferCode,
                    OrderTypeCode = TransferRequestDTO.OrderTypeCode,
                    FromWarehouseCode = TransferRequestDTO.FromWarehouseCode,
                    ToWarehouseCode = TransferRequestDTO.ToWarehouseCode,
                    FromResponsible = TransferRequestDTO.FromResponsible,
                    ToResponsible = TransferRequestDTO.ToResponsible,
                    StatusId = 3, // Assuming 3 is completed status
                    TransferDate = TransferRequestDTO.TransferDate,
                    TransferDescription = TransferRequestDTO.TransferDescription
                });

                if (confirmResult.Success)
                {
                    _notificationService.ShowMessage("Xác nhận phiếu chuyển kho thành công!", "OK", isError: false);
                    await _messengerService.SendMessageAsync("ReloadTransferList");
                    NavigateBack();
                }
                else
                {
                    _notificationService.ShowMessage(confirmResult.Message ?? "Không thể xác nhận phiếu chuyển kho.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi xác nhận phiếu chuyển kho: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task<bool> IsDetailExistsAsync(string transferCode, string productCode)
        {
            try
            {
                var result = await _transferDetailService.GetTransferDetailsByTransferCodeAsync(transferCode, 1, int.MaxValue);
                return result.Success && result.Data?.Data.Any(d => d.ProductCode == productCode) == true;
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi kiểm tra chi tiết chuyển kho: {ex.Message}", "OK", isError: true);
                return false;
            }
        }

        private void AddDetailLine()
        {
            if (string.IsNullOrEmpty(TransferRequestDTO.TransferCode))
            {
                _notificationService.ShowMessage("Vui lòng nhập mã phiếu chuyển kho trước khi thêm chi tiết.", "OK", isError: true);
                return;
            }

            var newDetail = new TransferDetailResponseDTO
            {
                TransferCode = TransferRequestDTO.TransferCode,
                ProductCode = string.Empty,
                ProductName = string.Empty,
                Demand = 0,
                QuantityInBounded = 0,
                QuantityOutBounded = 0,
                IsNewRow = true,
                SelectedProduct = LstProducts.FirstOrDefault() // Set default product
            };
            LstTransferDetails.Add(newDetail);
            _messengerService.SendMessageAsync("FocusNewDetailRow");
        }

        private async Task DeleteDetailLineAsync(TransferDetailResponseDTO detail)
        {
            if (detail == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa chi tiết sản phẩm '{detail.ProductName}'?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (!detail.IsNewRow)
                    {
                        var deleteResult = await _transferDetailService.DeleteTransferDetail(detail.TransferCode, detail.ProductCode);
                        if (!deleteResult.Success)
                        {
                            _notificationService.ShowMessage(deleteResult.Message ?? "Không thể xóa chi tiết chuyển kho.", "OK", isError: true);
                            return;
                        }
                    }
                    LstTransferDetails.Remove(detail);
                    _notificationService.ShowMessage("Xóa chi tiết chuyển kho thành công.", "OK", isError: false);
                }
                catch (Exception ex)
                {
                    _notificationService.ShowMessage($"Lỗi khi xóa chi tiết chuyển kho: {ex.Message}", "OK", isError: true);
                }
            }
        }

        private bool CanSave(object parameter)
        {
            var dto = TransferRequestDTO;
            var propertiesToValidate = new[] { nameof(dto.TransferCode), nameof(dto.OrderTypeCode), nameof(dto.FromWarehouseCode), nameof(dto.ToWarehouseCode), nameof(dto.FromResponsible), nameof(dto.ToResponsible),nameof(dto.TransferDate) };

            foreach (var prop in propertiesToValidate)
            {
                if (!string.IsNullOrEmpty((string)dto[prop]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CanAddDetailLine(object parameter)
        {
            return !string.IsNullOrEmpty(TransferRequestDTO?.TransferCode);
        }

        private bool CanConfirm(object parameter)
        {
            return !string.IsNullOrEmpty(TransferRequestDTO?.TransferCode) && LstTransferDetails.Any();
        }

        private void NavigateBack()
        {
            if (_transferRequestDTO != null)
            {
                _transferRequestDTO.PropertyChanged -= OnTransferRequestDTOPropertyChanged!;
            }

            var transferList = App.ServiceProvider!.GetRequiredService<ucTransfer>();
            _navigationService.NavigateTo(transferList);
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

        private async void OnTransferRequestDTOPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TransferRequestDTO));
            if (!_isSaving)
            {
                ((RelayCommand)SaveCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)AddDetailLineCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)ConfirmCommand)?.RaiseCanExecuteChanged();

                if (e.PropertyName == nameof(TransferRequestDTO.FromWarehouseCode))
                {
                    LstProducts.Clear();
                    LstFromResponsiblePersons.Clear();
                    TransferRequestDTO.FromResponsible = null!;
                    UpdateToWarehouses(); // Cập nhật LstToWarehouses khi FromWarehouseCode thay đổi
                    await Task.WhenAll(LoadFromResponsiblePersonsAsync(), LoadProductsAsync());
                }
                else if (e.PropertyName == nameof(TransferRequestDTO.ToWarehouseCode))
                {
                    LstToResponsiblePersons.Clear();
                    TransferRequestDTO.ToResponsible = null!;
                    await LoadToResponsiblePersonsAsync();
                }
            }
        }
    }
}