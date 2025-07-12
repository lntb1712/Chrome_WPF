using Chrome.Services.ManufacturingOrderService;
using Chrome_WPF.Helpers;
using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.BOMMasterDTO;
using Chrome_WPF.Models.ManufacturingOrderDetailDTO;
using Chrome_WPF.Models.ManufacturingOrderDTO;
using Chrome_WPF.Models.MovementDTO;
using Chrome_WPF.Models.OrderTypeDTO;
using Chrome_WPF.Models.PickListDTO;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Models.PutAwayDTO;
using Chrome_WPF.Models.ReservationDTO;
using Chrome_WPF.Models.StockInDTO;
using Chrome_WPF.Models.StockOutDetailDTO;
using Chrome_WPF.Models.StockOutDTO;
using Chrome_WPF.Models.TransferDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using Chrome_WPF.Services.ManufacturingOrderDetailService;
using Chrome_WPF.Services.ManufacturingOrderService;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.PickListService;
using Chrome_WPF.Services.PutAwayService;
using Chrome_WPF.Services.ReservationService;
using Chrome_WPF.ViewModels.StockOutViewModel;
using Chrome_WPF.Views.UserControls.ManufacturingOrder;
using Chrome_WPF.Views.UserControls.StockOut;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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
        private readonly IPutAwayService _putAwayService;

        private ObservableCollection<ManufacturingOrderDetailResponseDTO> _lstManufacturingOrderDetails;
        private ObservableCollection<ProductMasterResponseDTO> _lstProducts;
        private ObservableCollection<BOMMasterResponseDTO> _lstBomMasters;
        private ObservableCollection<OrderTypeResponseDTO> _lstOrderTypes;
        private ObservableCollection<WarehouseMasterResponseDTO> _lstWarehouses;
        private ObservableCollection<AccountManagementResponseDTO> _lstResponsiblePersons;
        private ObservableCollection<PutAwayAndDetailResponseDTO> _lstPutAway;
        private ObservableCollection<ReservationAndDetailResponseDTO> _lstReservations;
        private ObservableCollection<PickAndDetailResponseDTO> _lstPickList;
        private ObservableCollection<ProductShortageDTO> _inventoryShortages;
        private ObservableCollection<object> _displayPages;
        private ManufacturingOrderRequestDTO _manufacturingOrderRequestDTO;
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
        public ObservableCollection<ProductShortageDTO> InventoryShortages
        {
            get => _inventoryShortages;
            set
            {
                _inventoryShortages = value;
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

        public ICommand RowMouseEnterCommand { get; }

        public ManufacturingOrderDetailViewModel(
            IManufacturingOrderDetailService manufacturingOrderDetailService,
            IManufacturingOrderService manufacturingOrderService,
            IReservationService reservationService,
            IPickListService pickListService,
            IPutAwayService putAwayService,
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
            _putAwayService = putAwayService ?? throw new ArgumentNullException(nameof(putAwayService));

            _lstManufacturingOrderDetails = new ObservableCollection<ManufacturingOrderDetailResponseDTO>();
            _lstProducts = new ObservableCollection<ProductMasterResponseDTO>();
            _lstBomMasters = new ObservableCollection<BOMMasterResponseDTO>();
            _lstOrderTypes = new ObservableCollection<OrderTypeResponseDTO>();
            _lstWarehouses = new ObservableCollection<WarehouseMasterResponseDTO>();
            _lstResponsiblePersons = new ObservableCollection<AccountManagementResponseDTO>();
            _lstReservations = new ObservableCollection<ReservationAndDetailResponseDTO>();
            _lstPickList = new ObservableCollection<PickAndDetailResponseDTO>();
            _lstPutAway = new ObservableCollection<PutAwayAndDetailResponseDTO>();
            _inventoryShortages = new ObservableCollection<ProductShortageDTO>();
            _displayPages = new ObservableCollection<object>();
            _isAddingNew = manufacturingOrder == null;
            _currentPage = 1;
            _lastLoadedPage = 0;
            _isSaving = false;
            _hasPicklist = false;
            _manufacturingOrderRequestDTO = manufacturingOrder == null ? new ManufacturingOrderRequestDTO
            {
                ScheduleDate =DateTime.Now.ToString("dd/MM/yyyy"),
                Deadline = DateTime.Today.AddDays(7).ToString("dd/MM/yyyy")
            } : new ManufacturingOrderRequestDTO
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

            SaveCommand = new RelayCommand(async parameter => await SaveManufacturingOrderAsync(parameter), CanSave);
            BackCommand = new RelayCommand(_ => NavigateBack());
            AddDetailLineCommand = new RelayCommand(_ => AddDetailLine(), CanAddDetailLine);
            DeleteDetailLineCommand = new RelayCommand(detail => DeleteDetailLineAsync((ManufacturingOrderDetailResponseDTO)detail));
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            ConfirmQuantityCommand = new RelayCommand(async parameter => await ConfirmQuantityAsync(parameter), CanConfirmQuantity);
            RowMouseEnterCommand = new RelayCommand(async parameter => await LoadForecastDataForTooltipAsync((ManufacturingOrderDetailResponseDTO)parameter));
            _manufacturingOrderRequestDTO.PropertyChanged += OnManufacturingOrderPropertyChanged!;
            _ = InitializeAsync();
        }

        private async Task LoadForecastDataForTooltipAsync(ManufacturingOrderDetailResponseDTO detail)
        {
            if (detail == null || IsAddingNew || string.IsNullOrEmpty(ManufacturingOrderRequestDTO.ManufacturingOrderCode) || string.IsNullOrEmpty(detail.ComponentCode))
            {
                return;
            }

            try
            {
                // Create and configure ucForecastTooltip
                var forecastTooltip = new ucForecastManufacturingTooltip(App.ServiceProvider!.GetRequiredService<ForecastManufacturingTooltipViewModel>());
                var tooltipViewModel = App.ServiceProvider!.GetRequiredService<ForecastManufacturingTooltipViewModel>();
                tooltipViewModel.ProductName = detail.ComponentName;
                await tooltipViewModel.LoadForecastDataAsync(ManufacturingOrderRequestDTO.ManufacturingOrderCode, detail.ComponentCode);
                forecastTooltip.DataContext = tooltipViewModel;

                // Create Window
                var window = new Window
                {
                    Title = $"Dữ liệu dự báo ",
                    Content = forecastTooltip,
                    Width = 300,
                    Height = 200,
                    WindowStartupLocation = WindowStartupLocation.Manual, // Manual positioning
                    Owner = Application.Current.MainWindow,
                    WindowStyle = WindowStyle.SingleBorderWindow,
                    Background = Brushes.White,
                    BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                    BorderThickness = new Thickness(1)
                };

                // Get the current mouse position relative to the main window
                var mousePosition = Mouse.GetPosition(Application.Current.MainWindow);

                // Convert the mouse position to screen coordinates
                var screenPosition = Application.Current.MainWindow.PointToScreen(mousePosition);

                // Set the window position (e.g., slightly offset from the mouse cursor)
                window.Left = screenPosition.X + 10; // Offset by 10 pixels to the right
                window.Top = screenPosition.Y + 10;  // Offset by 10 pixels down

                // Ensure the window stays within screen bounds
                var screen = SystemParameters.WorkArea; // Get the primary screen's working area
                if (window.Left + window.Width > screen.Right)
                {
                    window.Left = screen.Right - window.Width - 100; // Adjust if it goes off the right edge
                }
                if (window.Top + window.Height > screen.Bottom)
                {
                    window.Top = screen.Bottom - window.Height - 100; // Adjust if it goes off the bottom edge
                }


                // Show the Window
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi hiển thị dữ liệu dự báo: {ex.Message}", "OK", isError: true);
            }
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
                if (!LstOrderTypes.Any())
                {
                    await LoadOrderTypesAsync();
                    if (IsAddingNew && LstOrderTypes.Any())
                    {
                        ManufacturingOrderRequestDTO!.OrderTypeCode = LstOrderTypes.First().OrderTypeCode;
                    }
                }
                if (!LstWarehouses.Any())
                {
                    await LoadWarehousesAsync();
                    if (IsAddingNew && LstWarehouses.Any())
                    {
                        ManufacturingOrderRequestDTO!.WarehouseCode = LstWarehouses.First().WarehouseCode;
                    }
                }
                await Task.WhenAll(
                    LoadProductsAsync(),
                    LoadBomMastersAsync(ManufacturingOrderRequestDTO!.ProductCode),
                    CheckReservationExistenceAsync(),
                    CheckPicklistExistenceAsync(),
                    CheckPutAwayHasValue());

                if (!IsAddingNew)
                {
                    await Task.WhenAll(
                        LoadManufacturingOrderDetailsAsync(),
                        LoadResponsiblePersonsAsync()
                        );
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
                if (string.IsNullOrEmpty(ManufacturingOrderRequestDTO.ManufacturingOrderCode))
                {
                    HasReservation = false;
                    return;
                }

                var reservationResult = await _reservationService.GetReservationsByManufacturingCodeAsync(ManufacturingOrderRequestDTO.ManufacturingOrderCode);
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
        private async Task LoadPickListAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(ManufacturingOrderRequestDTO.ManufacturingOrderCode))
                {
                    LstPickList.Clear();
                    HasPicklist = false;
                    return;
                }

                var pickListResult = await _pickListService.GetPickListContainCodeAsync(ManufacturingOrderRequestDTO.ManufacturingOrderCode);
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
                var putAwayResult = await _putAwayService.GetPutAwayContainsCodeAsync(ManufacturingOrderRequestDTO.ManufacturingOrderCode);
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
                if (string.IsNullOrEmpty(ManufacturingOrderRequestDTO.ManufacturingOrderCode))
                {
                    LstPutAway.Clear();
                    return;
                }
                var result = await _putAwayService.GetPutAwayContainsCodeAsync(ManufacturingOrderRequestDTO.ManufacturingOrderCode);
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
                var result = await _manufacturingOrderService.GetListResponsibleAsync(ManufacturingOrderRequestDTO.WarehouseCode!);
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

        private async Task SaveManufacturingOrderAsync(object parameter)
        {
            if (_isSaving) return;

            try
            {
                _isSaving = true;

                ManufacturingOrderRequestDTO.RequestValidation();
                if (!CanSave(parameter))
                {
                    _notificationService.ShowMessage("Vui lòng kiểm tra lại thông tin nhập vào.", "OK", isError: true);
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
                    MessageBox.Show(manufacturingOrderResult.Message ?? "Không thể lưu thông tin lệnh sản xuất.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _notificationService.ShowMessage(IsAddingNew ? "Thêm lệnh sản xuất thành công!" : "Cập nhật lệnh sản xuất thành công!", "OK", isError: false);
                if (IsAddingNew)
                {
                    ManufacturingOrderRequestDTO.ClearValidation();
                    IsAddingNew = false;
                    if (!LstProducts.Any())
                    {
                        await LoadProductsAsync();
                    }
                }
                await LoadManufacturingOrderDetailsAsync();
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

        private async Task ConfirmQuantityAsync(object parameter)
        {
            try
            {
                if (!LstManufacturingOrderDetails.Any())
                {
                    _notificationService.ShowMessage("Danh sách chi tiết lệnh sản xuất rỗng.", "OK", isError: true);
                    return;
                }

                await SaveManufacturingOrderAsync(parameter);
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
                var popup = new Views.UserControls.ManufacturingOrder.BackOrderDialog(viewModel)
                {
                    DataContext = viewModel,
                    Owner = Application.Current.MainWindow
                };
                popup.ShowDialog();
                if (viewModel.IsClosed)
                {
                    if (viewModel.CreateBackorder)
                    {
                        var backOrderResult = await _manufacturingOrderService.CreateBackOrder(ManufacturingOrderRequestDTO.ManufacturingOrderCode, viewModel.ScheduleDate.ToString()!, viewModel.Deadline.ToString()!);
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

        private void DeleteDetailLineAsync(ManufacturingOrderDetailResponseDTO detail)
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
            var dto = ManufacturingOrderRequestDTO;
            var propertiesToValidate = new[] { nameof(dto.ManufacturingOrderCode), nameof(dto.OrderTypeCode), nameof(dto.WarehouseCode), nameof(dto.Bomcode), nameof(dto.Responsible), nameof(dto.ScheduleDate), nameof(dto.Deadline), nameof(dto.Quantity) };
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
            OnPropertyChanged(nameof(MovementRequestDTO));
            if (!_isSaving)
            {
                ((RelayCommand)SaveCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)AddDetailLineCommand)?.RaiseCanExecuteChanged();
                if (e.PropertyName == nameof(ManufacturingOrderRequestDTO.ProductCode))
                {
                    await LoadBomMastersAsync(ManufacturingOrderRequestDTO.ProductCode);
                }
                if (e.PropertyName == nameof(ManufacturingOrderRequestDTO.WarehouseCode))
                {
                    await LoadResponsiblePersonsAsync();
                }
            }
            
        }
    }
}