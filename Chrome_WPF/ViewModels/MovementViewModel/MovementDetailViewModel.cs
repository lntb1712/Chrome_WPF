using Chrome_WPF.Helpers;
using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.LocationMasterDTO;
using Chrome_WPF.Models.MovementDetailDTO;
using Chrome_WPF.Models.MovementDTO;
using Chrome_WPF.Models.OrderTypeDTO;
using Chrome_WPF.Models.PickListDTO;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Models.PutAwayDTO;
using Chrome_WPF.Models.ReservationDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.MovementDetailService;
using Chrome_WPF.Services.MovementService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.PickListService;
using Chrome_WPF.Services.PutAwayService;
using Chrome_WPF.Services.ReservationService;
using Chrome_WPF.Views.UserControls.Movement;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.MovementViewModel
{
    public class MovementDetailViewModel : BaseViewModel
    {
        private readonly IMovementService _movementService;
        private readonly IMovementDetailService _movementDetailService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IMessengerService _messengerService;
        private readonly IPutAwayService _putAwayService;
        private readonly IPickListService _pickListService;
        private readonly IReservationService _reservationService;

        private ObservableCollection<MovementDetailResponseDTO> _lstMovementDetails;
        private ObservableCollection<ProductMasterResponseDTO> _lstProducts;
        private ObservableCollection<OrderTypeResponseDTO> _lstOrderTypes;
        private ObservableCollection<WarehouseMasterResponseDTO> _lstWarehouses;
        private ObservableCollection<LocationMasterResponseDTO> _lstFromLocations;
        private ObservableCollection<LocationMasterResponseDTO> _lstToLocations;
        private ObservableCollection<AccountManagementResponseDTO> _lstResponsiblePersons;
        private ObservableCollection<PutAwayAndDetailResponseDTO> _lstPutAway;
        private ObservableCollection<ReservationAndDetailResponseDTO> _lstReservations;
        private ObservableCollection<PickAndDetailResponseDTO> _lstPickList;
        private ObservableCollection<object> _displayPages;
        private MovementRequestDTO _movementRequestDTO;
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
        public ObservableCollection<MovementDetailResponseDTO> LstMovementDetails
        {
            get => _lstMovementDetails;
            set
            {
                _lstMovementDetails = value;
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

        public ObservableCollection<WarehouseMasterResponseDTO> LstWarehouses
        {
            get => _lstWarehouses;
            set
            {
                _lstWarehouses = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LocationMasterResponseDTO> LstFromLocations
        {
            get => _lstFromLocations;
            set
            {
                _lstFromLocations = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LocationMasterResponseDTO> LstToLocations
        {
            get => _lstToLocations;
            set
            {
                _lstToLocations = value;
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

        public MovementRequestDTO MovementRequestDTO
        {
            get => _movementRequestDTO;
            set
            {
                if (_movementRequestDTO != null)
                {
                    _movementRequestDTO.PropertyChanged -= OnMovementRequestDTOPropertyChanged!;
                }
                _movementRequestDTO = value;
                if (_movementRequestDTO != null)
                {
                    _movementRequestDTO.PropertyChanged += OnMovementRequestDTOPropertyChanged!;
                }
                OnPropertyChanged();
                _ = LoadMovementDetailsAsync();
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
                    _ = LoadMovementDetailsAsync();
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
                    _ = LoadMovementDetailsAsync();
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

        public MovementDetailViewModel(
            IMovementService movementService,
            IMovementDetailService movementDetailService,
            INotificationService notificationService,
            INavigationService navigationService,
            IMessengerService messengerService,
            IPutAwayService putAwayService,
            IPickListService pickListService,
            IReservationService reservationService,
            MovementResponseDTO movement = null!)
        {
            _movementService = movementService ?? throw new ArgumentNullException(nameof(movementService));
            _movementDetailService = movementDetailService ?? throw new ArgumentNullException(nameof(movementDetailService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));
            _putAwayService = putAwayService ?? throw new ArgumentNullException(nameof(putAwayService));
            _pickListService = pickListService ?? throw new ArgumentNullException(nameof(pickListService));
            _reservationService = reservationService ?? throw new ArgumentNullException(nameof(reservationService));

            _lstMovementDetails = new ObservableCollection<MovementDetailResponseDTO>();
            _lstProducts = new ObservableCollection<ProductMasterResponseDTO>();
            _lstOrderTypes = new ObservableCollection<OrderTypeResponseDTO>();
            _lstWarehouses = new ObservableCollection<WarehouseMasterResponseDTO>();
            _lstFromLocations = new ObservableCollection<LocationMasterResponseDTO>();
            _lstToLocations = new ObservableCollection<LocationMasterResponseDTO>();
            _lstResponsiblePersons = new ObservableCollection<AccountManagementResponseDTO>();
            _lstReservations = new ObservableCollection<ReservationAndDetailResponseDTO>();
            _lstPutAway = new ObservableCollection<PutAwayAndDetailResponseDTO>();
            _lstPickList = new ObservableCollection<PickAndDetailResponseDTO>();
            _displayPages = new ObservableCollection<object>();

            _isAddingNew = movement == null;
            _currentPage = 1;
            _lastLoadedPage = 0;
            _isSaving = false;
            _movementRequestDTO = movement == null ? new MovementRequestDTO() : new MovementRequestDTO
            {
                MovementCode = movement.MovementCode,
                OrderTypeCode = movement.OrderTypeCode,
                WarehouseCode = movement.WarehouseCode,
                FromLocation = movement.FromLocation,
                ToLocation = movement.ToLocation,
                Responsible = movement.Responsible,
                MovementDate = movement.MovementDate,
                StatusId = movement.StatusId,
                MovementDescription = movement.MovementDescription
            };

            SaveCommand = new RelayCommand(async parameter => await SaveMovementAsync(parameter), CanSave);
            BackCommand = new RelayCommand(_ => NavigateBack());
            AddDetailLineCommand = new RelayCommand(_ => AddDetailLine(), CanAddDetailLine);
            DeleteDetailLineCommand = new RelayCommand(async detail => await DeleteDetailLineAsync((MovementDetailResponseDTO)detail));
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));

            _movementRequestDTO.PropertyChanged += OnMovementRequestDTOPropertyChanged!;
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(MovementRequestDTO?.MovementCode) && !IsAddingNew)
                {
                    _notificationService.ShowMessage("Mã phiếu di chuyển không hợp lệ. Không thể khởi tạo.", "OK", isError: true);
                    NavigateBack();
                    return;
                }

                await Task.WhenAll(
                    LoadOrderTypesAsync(),
                    LoadWarehousesAsync(),

                    CheckReservationExistenceAsync(),
                    CheckPicklistExistenceAsync(),
                    CheckPutAwayHasValue());

                if (!IsAddingNew)
                {
                    await Task.WhenAll(
                     LoadFromLocationsAsync(),
                     LoadToLocationsAsync(),
                     LoadProductsAsync(),
                     LoadMovementDetailsAsync(),
                     LoadResponsiblePersonsAsync());
                }
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
                if (string.IsNullOrEmpty(MovementRequestDTO.MovementCode))
                {
                    HasReservation = false;
                    return;
                }

                var reservationResult = await _reservationService.GetReservationsByMovementCodeAsync(MovementRequestDTO.MovementCode);
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
                if (string.IsNullOrEmpty(MovementRequestDTO.MovementCode))
                {
                    LstReservations.Clear();
                    HasPicklist = false;
                    return;
                }

                var reservationResult = await _reservationService.GetReservationsByMovementCodeAsync(MovementRequestDTO.MovementCode);
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
                var picklistResult = await _pickListService.GetPickListContainCodeAsync(MovementRequestDTO.MovementCode);
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
                if (string.IsNullOrEmpty(MovementRequestDTO.MovementCode))
                {
                    LstPickList.Clear();
                    HasPicklist = false;
                    return;
                }

                var pickListResult = await _pickListService.GetPickListContainCodeAsync(MovementRequestDTO.MovementCode);
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
                var putAwayResult = await _putAwayService.GetPutAwayContainsCodeAsync(MovementRequestDTO.MovementCode);
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
                if (string.IsNullOrEmpty(MovementRequestDTO.MovementCode))
                {
                    LstPutAway.Clear();
                    return;
                }
                var result = await _putAwayService.GetPutAwayContainsCodeAsync(MovementRequestDTO.MovementCode);
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
        private async Task LoadMovementDetailsAsync()
        {
            try
            {
                if (_lastLoadedPage == CurrentPage && LstMovementDetails.Any())
                {
                    return;
                }

                if (string.IsNullOrEmpty(MovementRequestDTO.MovementCode))
                {
                    LstMovementDetails.Clear();
                    return;
                }

                var result = await _movementDetailService.GetMovementDetailsByMovementCodeAsync(MovementRequestDTO.MovementCode, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    LstMovementDetails.Clear();
                    foreach (var detail in result.Data.Data ?? Enumerable.Empty<MovementDetailResponseDTO>())
                    {
                        detail.SelectedProduct = LstProducts.FirstOrDefault(p => p.ProductCode == detail.ProductCode);
                        detail.IsNewRow = false;
                        LstMovementDetails.Add(detail);
                    }
                    TotalPages = result.Data.TotalPages;
                    _lastLoadedPage = CurrentPage;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải chi tiết di chuyển.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải chi tiết di chuyển: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(MovementRequestDTO.FromLocation))
                {
                    LstProducts.Clear();
                    return;
                }

                var result = await _movementDetailService.GetProductByLocationCode(MovementRequestDTO.FromLocation);
                if (result.Success && result.Data != null)
                {

                    LstProducts.Add(result.Data);

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
                var result = await _movementService.GetListOrderType("MV");
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
                var warehouseCodes = Properties.Settings.Default.WarehousePermission?.Cast<string>().ToArray() ?? Array.Empty<string>();
                var result = await _movementService.GetListWarehousePermission(warehouseCodes);
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

        private async Task LoadFromLocationsAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(MovementRequestDTO.WarehouseCode))
                {
                    LstFromLocations.Clear();
                    return;
                }

                var result = await _movementService.GetListFromLocation(MovementRequestDTO.WarehouseCode);
                if (result.Success && result.Data != null)
                {
                    LstFromLocations.Clear();
                    foreach (var location in result.Data)
                    {
                        LstFromLocations.Add(location);
                    }
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải danh sách vị trí nguồn.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải danh sách vị trí nguồn: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadToLocationsAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(MovementRequestDTO.FromLocation))
                {
                    LstToLocations.Clear();
                    return;
                }

                var result = await _movementService.GetListToLocation(MovementRequestDTO.WarehouseCode!, MovementRequestDTO.FromLocation!);
                if (result.Success && result.Data != null)
                {
                    LstToLocations.Clear();
                    foreach (var location in result.Data)
                    {
                        LstToLocations.Add(location);
                    }
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải danh sách vị trí đích.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải danh sách vị trí đích: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadResponsiblePersonsAsync()
        {
            try
            {
                var result = await _movementService.GetListResponsibleAsync(MovementRequestDTO.WarehouseCode!);
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

        private async Task SaveMovementAsync(object parameter)
        {
            if (_isSaving) return;

            try
            {
                _isSaving = true;

                MovementRequestDTO.RequestValidation();
                if (!CanSave(parameter))
                {
                    _notificationService.ShowMessage("Vui lòng kiểm tra lại thông tin nhập vào.", "OK", isError: true);
                    return;
                }
                ApiResult<bool> movementResult;
                if (IsAddingNew)
                {
                    movementResult = await _movementService.AddMovement(MovementRequestDTO);
                }
                else
                {
                    movementResult = await _movementService.UpdateMovement(MovementRequestDTO);
                }

                if (!movementResult.Success)
                {
                    _notificationService.ShowMessage(movementResult.Message ?? "Không thể lưu thông tin di chuyển.", "OK", isError: true);
                    return;
                }

                foreach (var detail in LstMovementDetails.ToList())
                {
                    if (detail.SelectedProduct == null)
                    {
                        _notificationService.ShowMessage("Vui lòng chọn sản phẩm cho tất cả các dòng.", "OK", isError: true);
                        return;
                    }

                    detail.ProductCode = detail.SelectedProduct.ProductCode;
                    detail.ProductName = detail.SelectedProduct.ProductName ?? string.Empty;

                    var request = new MovementDetailRequestDTO
                    {
                        MovementCode = MovementRequestDTO.MovementCode,
                        ProductCode = detail.ProductCode,
                        Demand = detail.Demand
                    };

                    request.RequestValidation();
                    if (!string.IsNullOrEmpty(request.Error))
                    {
                        _notificationService.ShowMessage($"Lỗi dữ liệu chi tiết: {request.Error}", "OK", isError: true);
                        return;
                    }

                    ApiResult<bool> detailResult;
                    if (await IsDetailExistsAsync(detail.MovementCode, detail.ProductCode))
                    {
                        detailResult = await _movementDetailService.UpdateMovementDetail(request);
                    }
                    else
                    {
                        detailResult = await _movementDetailService.AddMovementDetail(request);
                    }

                    if (!detailResult.Success)
                    {
                        _notificationService.ShowMessage(detailResult.Message ?? "Không thể lưu chi tiết di chuyển.", "OK", isError: true);
                        return;
                    }
                }

                _notificationService.ShowMessage(IsAddingNew ? "Thêm phiếu di chuyển thành công!" : "Cập nhật phiếu di chuyển thành công!", "OK", isError: false);
                if (IsAddingNew)
                {
                    MovementRequestDTO.ClearValidation();
                    IsAddingNew = false;

                    if (!LstProducts.Any())
                    {
                        await LoadProductsAsync();
                    }
                }
                await LoadMovementDetailsAsync();
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
                await _messengerService.SendMessageAsync("ReloadMovementList");
               
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi lưu thông tin di chuyển: {ex.Message}", "OK", isError: true);
            }
            finally
            {
                _isSaving = false;
            }
        }

        

        private async Task<bool> IsDetailExistsAsync(string movementCode, string productCode)
        {
            try
            {
                var result = await _movementDetailService.GetMovementDetailsByMovementCodeAsync(movementCode, 1, int.MaxValue);
                return result.Success && result.Data?.Data.Any(d => d.ProductCode == productCode) == true;
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi kiểm tra chi tiết di chuyển: {ex.Message}", "OK", isError: true);
                return false;
            }
        }

        private void AddDetailLine()
        {
            if (string.IsNullOrEmpty(MovementRequestDTO.MovementCode))
            {
                _notificationService.ShowMessage("Vui lòng nhập mã phiếu di chuyển trước khi thêm chi tiết.", "OK", isError: true);
                return;
            }

            var newDetail = new MovementDetailResponseDTO
            {
                MovementCode = MovementRequestDTO.MovementCode,
                ProductCode = string.Empty,
                ProductName = string.Empty,
                Demand = 0,
                IsNewRow = true,
                SelectedProduct = null
            };
            LstMovementDetails.Add(newDetail);
            _messengerService.SendMessageAsync("FocusNewDetailRow");
        }

        private async Task DeleteDetailLineAsync(MovementDetailResponseDTO detail)
        {
            if (detail == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa chi tiết sản phẩm '{detail.ProductName}'?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (!detail.IsNewRow)
                    {
                        var deleteResult = await _movementDetailService.DeleteMovementDetail(detail.MovementCode, detail.ProductCode);
                        if (!deleteResult.Success)
                        {
                            _notificationService.ShowMessage(deleteResult.Message ?? "Không thể xóa chi tiết di chuyển.", "OK", isError: true);
                            return;
                        }
                    }
                    LstMovementDetails.Remove(detail);
                    _notificationService.ShowMessage("Xóa chi tiết di chuyển thành công.", "OK", isError: false);
                }
                catch (Exception ex)
                {
                    _notificationService.ShowMessage($"Lỗi khi xóa chi tiết di chuyển: {ex.Message}", "OK", isError: true);
                }
            }
        }

        private bool CanSave(object parameter)
        {
            var dto = MovementRequestDTO;
            var propertiesToValidate = new[] { nameof(dto.MovementCode), nameof(dto.WarehouseCode), nameof(dto.OrderTypeCode), nameof(dto.FromLocation), nameof(dto.ToLocation), nameof(dto.Responsible),nameof(dto.MovementDate) };

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
            return !string.IsNullOrEmpty(MovementRequestDTO?.MovementCode);
        }

      

        private void NavigateBack()
        {

            var movementList = App.ServiceProvider!.GetRequiredService<ucMovement>();
            _navigationService.NavigateTo(movementList);
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

        private async void OnMovementRequestDTOPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(MovementRequestDTO));
            if (!_isSaving)
            {
                ((RelayCommand)SaveCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)AddDetailLineCommand)?.RaiseCanExecuteChanged();

                try
                {
                    if (e.PropertyName == nameof(MovementRequestDTO.WarehouseCode))
                    {
                        // Clear locations when warehouse changes
                        LstFromLocations.Clear();
                        LstToLocations.Clear();
                        MovementRequestDTO.FromLocation = null;
                        MovementRequestDTO.ToLocation = null;
                        LstProducts.Clear();

                        // Validate and escape parameter
                        string warehouseCode = MovementRequestDTO.WarehouseCode!;
                        if (string.IsNullOrEmpty(warehouseCode))
                        {
                            _notificationService.ShowMessage("Vui lòng chọn kho trước.", "OK", isError: true);
                            return;
                        }

                        // Escape parameter to handle special characters
                        warehouseCode = Uri.EscapeDataString(warehouseCode);

                        // Load new locations
                        await Task.WhenAll(
                            LoadFromLocationsAsync(),
                            LoadResponsiblePersonsAsync(),
                            LoadProductsAsync());
                    }
                    else if (e.PropertyName == nameof(MovementRequestDTO.FromLocation))
                    {
                        // Clear products when from location changes
                        LstProducts.Clear();
                        LstToLocations.Clear();
                        MovementRequestDTO.ToLocation = null;

                        string fromLocation = MovementRequestDTO.FromLocation!;
                        if (string.IsNullOrEmpty(fromLocation))
                        {
                            _notificationService.ShowMessage("Vui lòng chọn vị trí nguồn trước.", "OK", isError: false);
                            return;
                        }

                        // Escape parameter
                        fromLocation = Uri.EscapeDataString(fromLocation);

                        // Load new data
                        await Task.WhenAll(
                            LoadToLocationsAsync(),
                            LoadProductsAsync());
                    }
                }
                catch (Exception ex)
                {
                    _notificationService.ShowMessage($"Lỗi khi tải dữ liệu: {ex.Message}", "OK", isError: true);
                }
            }
        }
    }
}