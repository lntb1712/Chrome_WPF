using Chrome.Services.ManufacturingOrderService;
using Chrome_WPF.Helpers;
using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.BOMMasterDTO;
using Chrome_WPF.Models.ManufacturingOrderDetailDTO;
using Chrome_WPF.Models.ManufacturingOrderDTO;
using Chrome_WPF.Models.OrderTypeDTO;
using Chrome_WPF.Models.PickListDTO;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Models.ReservationDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using Chrome_WPF.Services.ManufacturingOrderDetailService;
using Chrome_WPF.Services.ManufacturingOrderService;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.PickListService;
using Chrome_WPF.Services.ReservationService;
using Chrome_WPF.Views.UserControls.ManufacturingOrder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.ManufacturingOrderViewModel
{
    public class ManufacturingOrderDetailViewModel : BaseViewModel
    {
        private readonly IManufacturingOrderDetailService _manufacturingOrderDetailService;
        private readonly IManufacturingOrderService _manufacturingOrderService;
        private readonly IReservationService _reservationService;
        private readonly IPickListService _pickListService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IMessengerService _messengerService;

        private ObservableCollection<ManufacturingOrderDetailResponseDTO> _lstManufacturingOrderDetails;
        private ObservableCollection<ProductMasterResponseDTO> _lstProducts;
        private ObservableCollection<BOMMasterResponseDTO> _lstBomMasters;
        private ObservableCollection<OrderTypeResponseDTO> _lstOrderTypes;
        private ObservableCollection<WarehouseMasterResponseDTO> _lstWarehouses;
        private ObservableCollection<AccountManagementResponseDTO> _lstResponsiblePersons;
        private ObservableCollection<ReservationAndDetailResponseDTO> _lstReservations;
        private ObservableCollection<object> _displayPages;
        private ManufacturingOrderRequestDTO _manufacturingOrderRequestDTO;
        private bool _isAddingNew;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;
        private int _lastLoadedPage;
        private bool _isSaving;
        private bool _hasPicklist;

        public ObservableCollection<ManufacturingOrderDetailResponseDTO> LstManufacturingOrderDetails
        {
            get => _lstManufacturingOrderDetails;
            set
            {
                _lstManufacturingOrderDetails = value;
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

        public ObservableCollection<BOMMasterResponseDTO> LstBomMasters
        {
            get => _lstBomMasters;
            set
            {
                _lstBomMasters = value;
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

        public ObservableCollection<AccountManagementResponseDTO> LstResponsiblePersons
        {
            get => _lstResponsiblePersons;
            set
            {
                _lstResponsiblePersons = value;
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

        public ObservableCollection<object> DisplayPages
        {
            get => _displayPages;
            set
            {
                _displayPages = value;
                OnPropertyChanged();
            }
        }

        public ManufacturingOrderRequestDTO ManufacturingOrderRequestDTO
        {
            get => _manufacturingOrderRequestDTO;
            set
            {
                if (_manufacturingOrderRequestDTO != null)
                {
                    _manufacturingOrderRequestDTO.PropertyChanged -= OnManufacturingOrderPropertyChanged!;
                }
                _manufacturingOrderRequestDTO = value;
                if (_manufacturingOrderRequestDTO != null)
                {
                    _manufacturingOrderRequestDTO.PropertyChanged += OnManufacturingOrderPropertyChanged!;
                }
                OnPropertyChanged();
                _ = LoadManufacturingOrderDetailsAsync();
                _ = LoadReservationsAsync();
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
                    _ = LoadManufacturingOrderDetailsAsync();
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
                    _ = LoadManufacturingOrderDetailsAsync();
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

        public bool HasPicklist
        {
            get => _hasPicklist;
            set
            {
                _hasPicklist = value;
                OnPropertyChanged();
                ((RelayCommand)CreatePicklistCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)ConfirmQuantityCommand)?.RaiseCanExecuteChanged();
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
        public ICommand CreateReservationCommand { get; }
        public ICommand CreatePicklistCommand { get; }

        public ManufacturingOrderDetailViewModel(
            IManufacturingOrderDetailService manufacturingOrderDetailService,
            IManufacturingOrderService manufacturingOrderService,
            IReservationService reservationService,
            IPickListService pickListService,
            INotificationService notificationService,
            INavigationService navigationService,
            IMessengerService messengerService,
            ManufacturingOrderResponseDTO? manufacturingOrder = null)
        {
            _manufacturingOrderDetailService = manufacturingOrderDetailService ?? throw new ArgumentNullException(nameof(manufacturingOrderDetailService));
            _manufacturingOrderService = manufacturingOrderService ?? throw new ArgumentNullException(nameof(manufacturingOrderService));
            _reservationService = reservationService ?? throw new ArgumentNullException(nameof(reservationService));
            _pickListService = pickListService ?? throw new ArgumentNullException(nameof(pickListService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));

            _lstManufacturingOrderDetails = new ObservableCollection<ManufacturingOrderDetailResponseDTO>();
            _lstProducts = new ObservableCollection<ProductMasterResponseDTO>();
            _lstBomMasters = new ObservableCollection<BOMMasterResponseDTO>();
            _lstOrderTypes = new ObservableCollection<OrderTypeResponseDTO>();
            _lstWarehouses = new ObservableCollection<WarehouseMasterResponseDTO>();
            _lstResponsiblePersons = new ObservableCollection<AccountManagementResponseDTO>();
            _lstReservations = new ObservableCollection<ReservationAndDetailResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            _isAddingNew = manufacturingOrder == null;
            _currentPage = 1;
            _lastLoadedPage = 0;
            _isSaving = false;
            _hasPicklist = false;
            _manufacturingOrderRequestDTO = manufacturingOrder == null ? new ManufacturingOrderRequestDTO() : new ManufacturingOrderRequestDTO
            {
                ManufacturingOrderCode = manufacturingOrder.ManufacturingOrderCode,
                OrderTypeCode = manufacturingOrder.OrderTypeCode ?? string.Empty,
                ProductCode = manufacturingOrder.ProductCode,
                Bomcode = manufacturingOrder.Bomcode,
                BomVersion = manufacturingOrder.BomVersion,
                Quantity = manufacturingOrder.Quantity,
                QuantityProduced = manufacturingOrder.QuantityProduced,
                ScheduleDate = manufacturingOrder.ScheduleDate,
                Deadline = manufacturingOrder.Deadline,
                Responsible = manufacturingOrder.Responsible ?? string.Empty,
                StatusId = manufacturingOrder.StatusId,
                WarehouseCode = manufacturingOrder.WarehouseCode ?? string.Empty
            };

            SaveCommand = new RelayCommand(async _ => await SaveManufacturingOrderAsync(), CanSave);
            BackCommand = new RelayCommand(_ => NavigateBack());
            AddDetailLineCommand = new RelayCommand(_ => AddDetailLine(), CanAddDetailLine);
            DeleteDetailLineCommand = new RelayCommand( detail =>  DeleteDetailLineAsync((ManufacturingOrderDetailResponseDTO)detail));
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            ConfirmQuantityCommand = new RelayCommand(async _ => await ConfirmQuantityAsync(), CanConfirmQuantity);
            CreateReservationCommand = new RelayCommand(async _ => await CreateReservationAsync(), CanCreateReservation);
            CreatePicklistCommand = new RelayCommand(async _ => await CreatePicklistAsync(), CanCreatePicklist);

            _manufacturingOrderRequestDTO.PropertyChanged += OnManufacturingOrderPropertyChanged!;
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(ManufacturingOrderRequestDTO?.ManufacturingOrderCode) && !IsAddingNew)
                {
                    _notificationService.ShowMessage("Mã lệnh sản xuất không hợp lệ. Không thể khởi tạo.", "OK", isError: true);
                    NavigateBack();
                    return;
                }

                await Task.WhenAll(
                    LoadProductsAsync(),
                    LoadBomMastersAsync(ManufacturingOrderRequestDTO!.ProductCode),
                    LoadOrderTypesAsync(),
                    LoadWarehousesAsync(),
                    LoadResponsiblePersonsAsync(),
                    LoadReservationsAsync());

                if (!IsAddingNew)
                {
                    await LoadManufacturingOrderDetailsAsync();
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Khởi tạo thất bại: {ex.Message}", "OK", isError: true);
                NavigateBack();
            }
        }

        private async Task LoadReservationsAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(ManufacturingOrderRequestDTO.ManufacturingOrderCode))
                {
                    LstReservations.Clear();
                    HasPicklist = false;
                    return;
                }

                var reservationResult = await _reservationService.GetReservationsByManufacturingCodeAsync(ManufacturingOrderRequestDTO.ManufacturingOrderCode);
                if (reservationResult.Success && reservationResult.Data != null)
                {
                    LstReservations.Clear();
                    LstReservations.Add(reservationResult.Data);
                    await CheckPicklistExistenceAsync();
                }
                else
                {
                    LstReservations.Clear();
                    HasPicklist = false;
                    _notificationService.ShowMessage(reservationResult.Message ?? "Không thể tải danh sách đặt chỗ.", "OK", isError: true);
                }
                ((RelayCommand)CreateReservationCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)CreatePicklistCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)ConfirmQuantityCommand)?.RaiseCanExecuteChanged();
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
                var picklistResult = await _pickListService.GetPickListContainCodeAsync(ManufacturingOrderRequestDTO.ManufacturingOrderCode);
                HasPicklist = picklistResult.Success && picklistResult.Data != null;
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi kiểm tra picklist: {ex.Message}", "OK", isError: true);
                HasPicklist = false;
            }
        }

        private async Task CreatePicklistAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(ManufacturingOrderRequestDTO.ManufacturingOrderCode))
                {
                    _notificationService.ShowMessage("Vui lòng nhập mã lệnh sản xuất trước khi tạo picklist.", "OK", isError: true);
                    return;
                }

                if (!LstReservations.Any())
                {
                    _notificationService.ShowMessage("Không có đặt chỗ nào để tạo picklist.", "OK", isError: true);
                    return;
                }

                await SaveManufacturingOrderAsync();
                if (!string.IsNullOrEmpty(ManufacturingOrderRequestDTO.Error))
                {
                    return;
                }

                var reservation = LstReservations.First();
                var picklist = new PickListRequestDTO
                {
                    PickNo = $"PICK_{ManufacturingOrderRequestDTO.ManufacturingOrderCode}",
                    PickDate = DateTime.Now.ToString("dd/MM/yyyy"),
                    ReservationCode = reservation.ReservationCode,
                    StatusId = 1,
                    WarehouseCode = reservation.WarehouseCode,
                };

                var result = await _pickListService.AddPickList(picklist);
                if (result.Success)
                {
                    _notificationService.ShowMessage($"Tạo picklist {picklist.PickNo} thành công!", "OK", isError: false);
                    HasPicklist = true;
                    await _messengerService.SendMessageAsync("ReloadPicklistList");
                    ((RelayCommand)CreatePicklistCommand)?.RaiseCanExecuteChanged();
                    ((RelayCommand)ConfirmQuantityCommand)?.RaiseCanExecuteChanged();
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tạo picklist.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tạo picklist: {ex.Message}", "OK", isError: true);
            }
        }

        private bool CanCreatePicklist(object parameter)
        {
            return !string.IsNullOrEmpty(ManufacturingOrderRequestDTO?.ManufacturingOrderCode) && LstReservations.Any() && !HasPicklist;
        }

        private bool CanCreateReservation(object parameter)
        {
            return !string.IsNullOrEmpty(ManufacturingOrderRequestDTO?.ManufacturingOrderCode) && !LstReservations.Any();
        }

        private async Task CreateReservationAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(ManufacturingOrderRequestDTO.ManufacturingOrderCode))
                {
                    _notificationService.ShowMessage("Vui lòng nhập mã lệnh sản xuất trước khi tạo đặt chỗ.", "OK", isError: true);
                    return;
                }

                if (!LstManufacturingOrderDetails.Any())
                {
                    _notificationService.ShowMessage("Danh sách chi tiết lệnh sản xuất rỗng. Vui lòng thêm chi tiết trước khi tạo đặt chỗ.", "OK", isError: true);
                    return;
                }

                await SaveManufacturingOrderAsync();
                if (string.IsNullOrEmpty(ManufacturingOrderRequestDTO.Error))
                {
                    var reservation = new ReservationRequestDTO
                    {
                        ReservationCode = $"RES_{ManufacturingOrderRequestDTO.ManufacturingOrderCode}",
                        WarehouseCode = ManufacturingOrderRequestDTO.WarehouseCode,
                        OrderTypeCode = ManufacturingOrderRequestDTO.OrderTypeCode,
                        OrderId = ManufacturingOrderRequestDTO.ManufacturingOrderCode,
                        ReservationDate = ManufacturingOrderRequestDTO.ScheduleDate,
                        StatusId = 1
                    };

                    var result = await _reservationService.AddReservation(reservation);
                    if (result.Success)
                    {
                        _notificationService.ShowMessage("Tạo đặt chỗ thành công!", "OK",isError:false);
                        await _messengerService.SendMessageAsync("ReloadReservationList");
                        await LoadReservationsAsync();
                    }
                    else
                    {
                        _notificationService.ShowMessage(result.Message ?? "Không thể tạo đặt chỗ.", "OK", isError: true);
                    }
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tạo đặt chỗ: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadManufacturingOrderDetailsAsync()
        {
            try
            {
                if (_lastLoadedPage == CurrentPage && LstManufacturingOrderDetails.Any())
                {
                    return;
                }

                if (string.IsNullOrEmpty(ManufacturingOrderRequestDTO.ManufacturingOrderCode))
                {
                    LstManufacturingOrderDetails.Clear();
                    return;
                }

                var result = await _manufacturingOrderDetailService.GetManufacturingOrderDetail(ManufacturingOrderRequestDTO.ManufacturingOrderCode);
                if (result.Success && result.Data != null)
                {
                    LstManufacturingOrderDetails.Clear();
                    foreach (var detail in result.Data.Data)
                    {
                        LstManufacturingOrderDetails.Add(detail);
                    }
                    TotalPages = 1;
                    _lastLoadedPage = CurrentPage;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải chi tiết lệnh sản xuất.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải chi tiết lệnh sản xuất: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                var result = await _manufacturingOrderService.GetListProductMasterIsFGAndSFG();
                if (result.Success && result.Data != null)
                {
                    LstProducts.Clear();
                    foreach (var product in result.Data)
                    {
                        LstProducts.Add(new ProductMasterResponseDTO
                        {
                            ProductCode = product.ProductCode!,
                            ProductName = product.ProductName
                        });
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

        private async Task LoadBomMastersAsync(string? productCode)
        {
            try
            {
                LstBomMasters.Clear();
                if (string.IsNullOrEmpty(productCode))
                {
                    ManufacturingOrderRequestDTO.Bomcode = string.Empty;
                    ManufacturingOrderRequestDTO.BomVersion = string.Empty;
                    return;
                }

                var result = await _manufacturingOrderService.GetListBomMasterAsync(productCode);
                if (result.Success && result.Data != null)
                {
                    ManufacturingOrderRequestDTO.Bomcode = result.Data.BOMCode;
                }
                else
                {
                    ManufacturingOrderRequestDTO.Bomcode = string.Empty;
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải danh sách BOM.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                ManufacturingOrderRequestDTO.Bomcode = string.Empty;
                ManufacturingOrderRequestDTO.BomVersion = string.Empty;
                _notificationService.ShowMessage($"Lỗi khi tải danh sách BOM: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadOrderTypesAsync()
        {
            try
            {
                var result = await _manufacturingOrderService.GetListOrderTypeAsync("MO");
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
                var result = await _manufacturingOrderService.GetListWarehousePermissionAsync(warehouseCodes);
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

        private async Task LoadResponsiblePersonsAsync()
        {
            try
            {
                var result = await _manufacturingOrderService.GetListResponsibleAsync();
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

        private async Task SaveManufacturingOrderAsync()
        {
            if (_isSaving) return;

            try
            {
                _isSaving = true;

                ManufacturingOrderRequestDTO.RequestValidation();
                if (!string.IsNullOrEmpty(ManufacturingOrderRequestDTO.Error))
                {
                    _notificationService.ShowMessage($"Lỗi dữ liệu: {ManufacturingOrderRequestDTO.Error}", "OK", isError: true);
                    return;
                }

                ApiResult<bool> manufacturingOrderResult;
                if (IsAddingNew)
                {
                    manufacturingOrderResult = await _manufacturingOrderService.AddManufacturingOrderAsync(ManufacturingOrderRequestDTO);
                }
                else
                {
                    manufacturingOrderResult = await _manufacturingOrderService.UpdateManufacturingOrderAsync(ManufacturingOrderRequestDTO);
                }

                if (!manufacturingOrderResult.Success)
                {
                    _notificationService.ShowMessage(manufacturingOrderResult.Message ?? "Không thể lưu thông tin lệnh sản xuất.", "OK", isError: true);
                    return;
                }

                _notificationService.ShowMessage(IsAddingNew ? "Thêm lệnh sản xuất thành công!" : "Cập nhật lệnh sản xuất thành công!", "OK", isError: false);
                if (IsAddingNew)
                {
                    ManufacturingOrderRequestDTO.ClearValidation();
                    IsAddingNew = false;
                }

                await _messengerService.SendMessageAsync("ReloadManufacturingOrderList");
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi lưu thông tin lệnh sản xuất: {ex.Message}", "OK", isError: true);
            }
            finally
            {
                _isSaving = false;
            }
        }

        private async Task ConfirmQuantityAsync()
        {
            try
            {
                if (!LstManufacturingOrderDetails.Any())
                {
                    _notificationService.ShowMessage("Danh sách chi tiết lệnh sản xuất rỗng.", "OK", isError: true);
                    return;
                }

                await SaveManufacturingOrderAsync();
                bool hasShortage = ManufacturingOrderRequestDTO.QuantityProduced < ManufacturingOrderRequestDTO.Quantity;
                if (!hasShortage)
                {
                    var confirmResult = await _manufacturingOrderService.ConfirmManufacturingOrder(ManufacturingOrderRequestDTO.ManufacturingOrderCode);
                    if (confirmResult.Success)
                    {
                        _notificationService.ShowMessage("Xác nhận lệnh sản xuất thành công!", "OK", isError: false);
                        await _messengerService.SendMessageAsync("ReloadManufacturingOrderList");
                        NavigateBack();
                    }
                    else
                    {
                        _notificationService.ShowMessage(confirmResult.Message ?? "Không thể xác nhận lệnh sản xuất.", "OK", isError: true);
                    }
                    return;
                }
                var viewModel = new BackOrderDialogViewModel(_notificationService, ManufacturingOrderRequestDTO);
                var popup = new BackOrderDialog(viewModel)
                {
                    DataContext = viewModel,
                    Owner = Application.Current.MainWindow
                };
                popup.ShowDialog();
                if (viewModel.IsClosed)
                {
                    if (viewModel.CreateBackorder)
                    {
                        var backOrderResult = await _manufacturingOrderService.CreateBackOrder(ManufacturingOrderRequestDTO.ManufacturingOrderCode);
                        if (!backOrderResult.Success)
                        {
                            _notificationService.ShowMessage(backOrderResult.Message ?? "Không thể tạo backorder.", "OK", isError: true);
                            return;
                        }

                        var confirmResult = await _manufacturingOrderService.ConfirmManufacturingOrder(ManufacturingOrderRequestDTO.ManufacturingOrderCode);
                        if (confirmResult.Success)
                        {
                            _notificationService.ShowMessage("Xác nhận lệnh sản xuất và tạo backorder thành công!", "OK", isError: false);
                            await _messengerService.SendMessageAsync("ReloadManufacturingOrderList");
                            NavigateBack();
                        }
                        else
                        {
                            _notificationService.ShowMessage(confirmResult.Message ?? "Không thể xác nhận lệnh sản xuất.", "OK", isError: true);
                        }
                    }
                    else if (viewModel.NoBackorder)
                    {
                        var confirmResult = await _manufacturingOrderService.ConfirmManufacturingOrder(ManufacturingOrderRequestDTO.ManufacturingOrderCode);
                        if (confirmResult.Success)
                        {
                            _notificationService.ShowMessage("Xác nhận lệnh sản xuất thành công, không tạo backorder.", "OK", isError: false);
                            await _messengerService.SendMessageAsync("ReloadManufacturingOrderList");
                            NavigateBack();
                        }
                        else
                        {
                            _notificationService.ShowMessage(confirmResult.Message ?? "Không thể xác nhận lệnh sản xuất.", "OK", isError: true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi kiểm tra số lượng: {ex.Message}", "OK", isError: true);
            }
        }

        private void AddDetailLine()
        {
            var newDetail = new ManufacturingOrderDetailResponseDTO
            {
                ManufacturingOrderCode = ManufacturingOrderRequestDTO.ManufacturingOrderCode
            };
            LstManufacturingOrderDetails.Add(newDetail);
        }

        private  void  DeleteDetailLineAsync(ManufacturingOrderDetailResponseDTO detail)
        {
            if (detail == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa chi tiết này?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    LstManufacturingOrderDetails.Remove(detail);
                    _notificationService.ShowMessage("Xóa chi tiết thành công.", "OK", isError: false);
                }
                catch (Exception ex)
                {
                    _notificationService.ShowMessage($"Lỗi khi xóa chi tiết: {ex.Message}", "OK", isError: true);
                }
            }
        }

        private bool CanSave(object parameter)
        {
            return !_isSaving && ManufacturingOrderRequestDTO != null && string.IsNullOrEmpty(ManufacturingOrderRequestDTO.Error);
        }

        private bool CanAddDetailLine(object parameter)
        {
            return !string.IsNullOrEmpty(ManufacturingOrderRequestDTO.ManufacturingOrderCode);
        }

        private bool CanConfirmQuantity(object parameter)
        {
            return !string.IsNullOrEmpty(ManufacturingOrderRequestDTO.ManufacturingOrderCode) && !IsAddingNew && HasPicklist;
        }

        private void NavigateBack()
        {
            var manufacturingOrderView = App.ServiceProvider!.GetRequiredService<ucManufacturingOrder>();
            _navigationService.NavigateTo(manufacturingOrderView);
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

        private async void OnManufacturingOrderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ManufacturingOrderRequestDTO.ProductCode))
            {
                await LoadBomMastersAsync(ManufacturingOrderRequestDTO.ProductCode);
            }
            CommandManager.InvalidateRequerySuggested();
        }
    }
}