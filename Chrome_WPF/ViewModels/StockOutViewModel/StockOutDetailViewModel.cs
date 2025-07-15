using Chrome_WPF.Helpers;
using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.CustomerMasterDTO;
using Chrome_WPF.Models.OrderTypeDTO;
using Chrome_WPF.Models.PickListDTO;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Models.ReservationDTO;
using Chrome_WPF.Models.StockInDTO;
using Chrome_WPF.Models.StockOutDetailDTO;
using Chrome_WPF.Models.StockOutDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.PickListService;
using Chrome_WPF.Services.ReservationService;
using Chrome_WPF.Services.StockOutDetailService;
using Chrome_WPF.Services.StockOutService;
using Chrome_WPF.Views.UserControls.StockOut;
using ClosedXML.Excel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Chrome_WPF.ViewModels.StockOutViewModel
{
    public class StockOutDetailViewModel : BaseViewModel
    {
        private readonly IStockOutDetailService _stockOutDetailService;
        private readonly IStockOutService _stockOutService;
        private readonly IReservationService _reservationService;
        private readonly IPickListService _pickListService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IMessengerService _messengerService;

        private ObservableCollection<StockOutDetailResponseDTO> _lstStockOutDetails;
        private ObservableCollection<ProductMasterResponseDTO> _lstProducts;
        private ObservableCollection<OrderTypeResponseDTO> _lstOrderTypes;
        private ObservableCollection<WarehouseMasterResponseDTO> _lstWarehouses;
        private ObservableCollection<CustomerMasterResponseDTO> _lstCustomers;
        private ObservableCollection<AccountManagementResponseDTO> _lstResponsiblePersons;
        private ObservableCollection<ReservationAndDetailResponseDTO> _lstReservations;
        private ObservableCollection<PickAndDetailResponseDTO> _lstPickList;
        private ObservableCollection<object> _displayPages;
        private StockOutRequestDTO _stockOutRequestDTO;
        private bool _isAddingNew;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;
        private bool _isSaving;
        private bool _hasPicklist;
        private bool _hasReservation;

        public bool HasReservation
        {
            get => _hasReservation;
            set
            {
                _hasReservation = value;
                OnPropertyChanged();
                ((RelayCommand)CreateReservationCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)CreatePicklistCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)ConfirmQuantityCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<StockOutDetailResponseDTO> LstStockOutDetails
        {
            get => _lstStockOutDetails;
            set
            {
                if (_lstStockOutDetails != null)
                {
                    _lstStockOutDetails.CollectionChanged -= StockOutDetails_CollectionChanged!;
                }
                _lstStockOutDetails = value;
                if (_lstStockOutDetails != null)
                {
                    _lstStockOutDetails.CollectionChanged += StockOutDetails_CollectionChanged!;
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

        public ObservableCollection<PickAndDetailResponseDTO> LstPickList
        {
            get => _lstPickList;
            set
            {
                _lstPickList = value;
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

        public ObservableCollection<CustomerMasterResponseDTO> LstCustomers
        {
            get => _lstCustomers;
            set
            {
                _lstCustomers = value;
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

        public StockOutRequestDTO StockOutRequestDTO
        {
            get => _stockOutRequestDTO;
            set
            {
                if (_stockOutRequestDTO != null)
                {
                    _stockOutRequestDTO.PropertyChanged -= OnPropertyChangedHandler!;
                }
                _stockOutRequestDTO = value;
                if (_stockOutRequestDTO != null)
                {
                    _stockOutRequestDTO.PropertyChanged += OnPropertyChangedHandler!;
                }
                OnPropertyChanged();
                _ = LoadStockOutDetailsAsync();
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
                    _ = LoadStockOutDetailsAsync();
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
                    _ = LoadStockOutDetailsAsync();
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
        public ICommand RowMouseEnterCommand { get; }
        public ICommand ExportStockOutExcelCommand { get; }

        public StockOutDetailViewModel(
            IStockOutDetailService stockOutDetailService,
            IStockOutService stockOutService,
            IReservationService reservationService,
            IPickListService pickListService,
            INotificationService notificationService,
            INavigationService navigationService,
            IMessengerService messengerService,
            StockOutResponseDTO? stockOut = null)
        {
            _stockOutDetailService = stockOutDetailService ?? throw new ArgumentNullException(nameof(stockOutDetailService));
            _stockOutService = stockOutService ?? throw new ArgumentNullException(nameof(stockOutService));
            _reservationService = reservationService ?? throw new ArgumentNullException(nameof(reservationService));
            _pickListService = pickListService ?? throw new ArgumentNullException(nameof(pickListService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));

            _lstStockOutDetails = new ObservableCollection<StockOutDetailResponseDTO>();
            _lstStockOutDetails.CollectionChanged += StockOutDetails_CollectionChanged!;
            _lstProducts = new ObservableCollection<ProductMasterResponseDTO>();
            _lstOrderTypes = new ObservableCollection<OrderTypeResponseDTO>();
            _lstWarehouses = new ObservableCollection<WarehouseMasterResponseDTO>();
            _lstCustomers = new ObservableCollection<CustomerMasterResponseDTO>();
            _lstResponsiblePersons = new ObservableCollection<AccountManagementResponseDTO>();
            _lstReservations = new ObservableCollection<ReservationAndDetailResponseDTO>();
            _lstPickList = new ObservableCollection<PickAndDetailResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            _isAddingNew = stockOut == null;
            _currentPage = 1;

            _isSaving = false;
            _hasPicklist = false;
            _hasReservation = false;
            _stockOutRequestDTO = stockOut == null ? new StockOutRequestDTO
            {
                StockOutDate = DateTime.Now.ToString("dd/MM/yyyy")
            } : new StockOutRequestDTO
            {
                StockOutCode = stockOut.StockOutCode ?? string.Empty,
                OrderTypeCode = stockOut.OrderTypeCode ?? string.Empty,
                WarehouseCode = stockOut.WarehouseCode ?? string.Empty,
                CustomerCode = stockOut.CustomerCode ?? string.Empty,
                Responsible = stockOut.Responsible ?? string.Empty,
                StockOutDate = stockOut.StockOutDate ?? string.Empty,
                StockOutDescription = stockOut.StockOutDescription??string.Empty,
            };

            SaveCommand = new RelayCommand(async parameter => await SaveStockOutAsync(parameter), CanSave);
            BackCommand = new RelayCommand(_ => NavigateBack());
            AddDetailLineCommand = new RelayCommand(_ => AddDetailLine(), CanAddDetailLine);
            DeleteDetailLineCommand = new RelayCommand(async detail => await DeleteDetailLineAsync((StockOutDetailResponseDTO)detail));
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            ConfirmQuantityCommand = new RelayCommand(async parameter => await CheckQuantityAsync(parameter), CanConfirmQuantity);
            CreateReservationCommand = new RelayCommand(async parameter => await CreateReservationAsync(parameter), CanCreateReservation);
            CreatePicklistCommand = new RelayCommand(async parameter => await CreatePicklistAsync(parameter), CanCreatePicklist);
            RowMouseEnterCommand = new RelayCommand(async parameter => await LoadForecastDataForTooltipAsync((StockOutDetailResponseDTO)parameter));
            ExportStockOutExcelCommand = new RelayCommand(async _ => await ExportStockOutExcel());

            // Fix for CS0019: Operator '+=' cannot be applied to operands of type 'StockOutRequestDTO' and 'method group'
            // The issue is that the code is attempting to use the '+=' operator on an object of type 'StockOutRequestDTO'.
            // This is incorrect because 'StockOutRequestDTO' does not support event subscription directly.
            // The correct approach is to subscribe to the 'PropertyChanged' event of the 'StockOutRequestDTO' instance.

            _stockOutRequestDTO.PropertyChanged += OnPropertyChangedHandler!;
            // Fix for CS1002: ; expected
            // This error typically occurs when a semicolon is missing at the end of a statement.
            // Ensure that all statements are properly terminated with a semicolon.


            _ = InitializeAsync();
        }

        private async Task ExportStockOutExcel()
        {
            try
            {
                string templatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "PhieuXuatKho.xlsx");
                if (!File.Exists(templatePath))
                {
                    _notificationService.ShowMessage("Không tìm thấy file mẫu PhieuXuatKho.xlsx.", "OK", isError: true);
                    return;
                }

                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string exportPath = Path.Combine(desktopPath, $"PhieuXuatKho_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");

                using (var workbook = new XLWorkbook(templatePath))
                {
                    var worksheet = workbook.Worksheet(1); // Sheet đầu tiên

                    // Ghi ngày in vào tiêu đề
                    worksheet.Cell(3, 7).Value = $"Ngày {DateTime.Now.ToString("dd/MM/yyyy")}";
                    worksheet.Cell(4, 5).Value = Properties.Settings.Default.FullName; // Người in
                    worksheet.Cell(5, 5).Value = Properties.Settings.Default.RoleName; // Chức vụ
                    worksheet.Cell(6, 5).Value = StockOutRequestDTO.StockOutDescription ?? "";
                    worksheet.Cell(1, 8).Value += StockOutRequestDTO.StockOutCode;
                    worksheet.Cell(4, 8).Value = LstWarehouses.FirstOrDefault(w => w.WarehouseCode == StockOutRequestDTO.WarehouseCode)?.WarehouseName ?? ""; // Tên kho
                    worksheet.Cell(5, 8).Value = LstCustomers.FirstOrDefault(s => s.CustomerCode == StockOutRequestDTO.CustomerCode)?.CustomerName ?? ""; // Tên nhà cung cấp
                    worksheet.Cell(6, 8).Value = StockOutRequestDTO.StockOutDate; // Ngày dự kiến giao
                    int startRow = 10;
                    int stt = 1;

                    foreach (var item in LstStockOutDetails)
                    {
                        worksheet.Cell(startRow, 2).Value = stt; // STT
                        worksheet.Cell(startRow, 3).Value = item.ProductCode; // Mã sản phẩm
                        worksheet.Cell(startRow, 5).Value = item.ProductName; // Tên sản phẩm
                        worksheet.Cell(startRow, 6).Value = item.Quantity;

                        startRow++;
                        stt++;
                    }

                    workbook.SaveAs(exportPath);

                    _notificationService.ShowMessage($"In phiếu xuất thành công!\nFile được lưu tại: {exportPath}", "OK", isError: false);


                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi xuất in phiếu xuất: {ex.Message}", "OK", isError: true);
            }
        }

        private void StockOutDetails_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                foreach (var item in _lstStockOutDetails)
                {
                    item.PropertyChanged -= StockOutDetail_PropertyChanged!;
                }
            }
            if (e.OldItems != null)
            {
                foreach (StockOutDetailResponseDTO item in e.OldItems)
                {
                    item.PropertyChanged -= StockOutDetail_PropertyChanged!;
                }
            }
            if (e.NewItems != null)
            {
                foreach (StockOutDetailResponseDTO item in e.NewItems)
                {
                    item.PropertyChanged += StockOutDetail_PropertyChanged!;
                }
            }
        }

        private void StockOutDetail_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == nameof(StockOutDetailResponseDTO.SelectedProduct))
                {
                    var detail = (StockOutDetailResponseDTO)sender;
                    if (detail?.SelectedProduct != null)
                    {
                        _ = Task.Delay(500);
                        // Add logic for handling selected product change if necessary
                    }
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadForecastDataForTooltipAsync(StockOutDetailResponseDTO detail)
        {
            if (detail == null || IsAddingNew || string.IsNullOrEmpty(StockOutRequestDTO.StockOutCode) || string.IsNullOrEmpty(detail.ProductCode))
            {
                return;
            }

            try
            {
                var forecastTooltip = new ucForecastTooltip(App.ServiceProvider!.GetRequiredService<ForecastTooltipViewModel>());
                var tooltipViewModel = App.ServiceProvider!.GetRequiredService<ForecastTooltipViewModel>();
                tooltipViewModel.ProductName = detail.ProductName;
                await tooltipViewModel.LoadForecastDataAsync(StockOutRequestDTO.StockOutCode, detail.ProductCode);
                forecastTooltip.DataContext = tooltipViewModel;

                var window = new Window
                {
                    Title = "Dữ liệu dự báo",
                    Content = forecastTooltip,
                    Width = 300,
                    Height = 200,
                    WindowStartupLocation = WindowStartupLocation.Manual,
                    Owner = Application.Current.MainWindow,
                    WindowStyle = WindowStyle.SingleBorderWindow,
                    Background = Brushes.White,
                    BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                    BorderThickness = new Thickness(1)
                };

                var mousePosition = Mouse.GetPosition(Application.Current.MainWindow);
                var screenPosition = Application.Current.MainWindow.PointToScreen(mousePosition);

                window.Left = screenPosition.X + 10;
                window.Top = screenPosition.Y + 10;

                var screen = SystemParameters.WorkArea;
                window.Left = Math.Max(screen.Left, Math.Min(screen.Right - window.Width, window.Left));
                window.Top = Math.Max(screen.Top, Math.Min(screen.Bottom - window.Height, window.Top));

                window.ShowDialog();
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi hiển thị dữ liệu dự báo: {ex.Message}", "OK", isError: true);
            }
            finally
            {
                // Ensure proper disposal if necessary
            }
        }

        private async Task InitializeAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(StockOutRequestDTO?.StockOutCode) && !IsAddingNew)
                {
                    _notificationService.ShowMessage("Mã phiếu xuất kho không hợp lệ. Không thể khởi tạo.", "OK", isError: true);
                    NavigateBack();
                    return;
                }

                // Load static data only if not already loaded
                if (!LstOrderTypes.Any())
                {
                    await LoadOrderTypesAsync();
                    if (IsAddingNew && LstOrderTypes.Any())
                    {
                        StockOutRequestDTO!.OrderTypeCode = LstOrderTypes.First().OrderTypeCode;
                    }
                }
                if (!LstWarehouses.Any())
                {
                    await LoadWarehousesAsync();
                    if (IsAddingNew && LstWarehouses.Any())
                    {
                        StockOutRequestDTO!.WarehouseCode = LstWarehouses.First().WarehouseCode;
                    }
                }
                if (!LstCustomers.Any())
                {
                    await LoadCustomersAsync();
                    if(IsAddingNew && LstCustomers.Any())
                    {
                        StockOutRequestDTO!.CustomerCode = LstCustomers.First().CustomerCode;
                    }    
                }
                if (!IsAddingNew)
                {
                    if (!LstProducts.Any())
                    {
                        await LoadProductsAsync();
                    }
                    await LoadStockOutDetailsAsync();
                    await LoadResponsiblePersonsAsync();
                    await CheckReservationExistenceAsync();
                    if (HasReservation)
                    {
                        await LoadReservationsAsync();
                    }
                    if (HasPicklist)
                    {
                        await LoadPickListAsync();
                    }
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
                if (string.IsNullOrEmpty(StockOutRequestDTO.StockOutCode))
                {
                    HasReservation = false;
                    return;
                }

                var reservationResult = await _reservationService.GetReservationsByStockOutCodeAsync(StockOutRequestDTO.StockOutCode);
                HasReservation = reservationResult.Success && reservationResult.Data != null;
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi kiểm tra đặt chỗ: {ex.Message}", "OK", isError: true);
                HasReservation = false;
            }
        }

        private async Task LoadPickListAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(StockOutRequestDTO.StockOutCode))
                {
                    LstPickList.Clear();
                    HasPicklist = false;
                    return;
                }

                var pickListResult = await _pickListService.GetPickListContainCodeAsync(StockOutRequestDTO.StockOutCode);
                if (pickListResult.Success && pickListResult.Data != null)
                {
                    LstPickList.Clear();
                    LstPickList.Add(pickListResult.Data);
                    HasPicklist = true;
                }
                else
                {
                    LstPickList.Clear();
                    HasPicklist = false;
                    _notificationService.ShowMessage(pickListResult.Message ?? "Không thể tải danh sách lấy hàng.", "OK", isError: true);
                }
                ((RelayCommand)CreateReservationCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)CreatePicklistCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)ConfirmQuantityCommand)?.RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải danh sách lấy hàng: {ex.Message}", "OK", isError: true);
                HasPicklist = false;
            }
        }

        private async Task LoadReservationsAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(StockOutRequestDTO.StockOutCode))
                {
                    LstReservations.Clear();
                    HasReservation = false;
                    return;
                }

                var reservationResult = await _reservationService.GetReservationsByStockOutCodeAsync(StockOutRequestDTO.StockOutCode);
                if (reservationResult.Success && reservationResult.Data != null)
                {
                    LstReservations.Clear();
                    LstReservations.Add(reservationResult.Data);
                    HasReservation = true;
                    await CheckPicklistExistenceAsync();
                }
                else
                {
                    LstReservations.Clear();
                    HasReservation = false;
                    _notificationService.ShowMessage(reservationResult.Message ?? "Không thể tải danh sách đặt chỗ.", "OK", isError: true);
                }
                ((RelayCommand)CreateReservationCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)CreatePicklistCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)ConfirmQuantityCommand)?.RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải danh sách đặt chỗ: {ex.Message}", "OK", isError: true);
                HasReservation = false;
            }
        }

        private async Task CheckPicklistExistenceAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(StockOutRequestDTO.StockOutCode))
                {
                    HasPicklist = false;
                    return;
                }
                var picklistResult = await _pickListService.GetPickListContainCodeAsync(StockOutRequestDTO.StockOutCode);
                HasPicklist = picklistResult.Success && picklistResult.Data != null;
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi kiểm tra picklist: {ex.Message}", "OK", isError: true);
                HasPicklist = false;
            }
        }

        private async Task CreatePicklistAsync(object parameter)
        {
            try
            {
                if (string.IsNullOrEmpty(StockOutRequestDTO.StockOutCode))
                {
                    _notificationService.ShowMessage("Vui lòng nhập mã phiếu xuất kho trước khi tạo picklist.", "OK", isError: true);
                    return;
                }

                if (!LstReservations.Any())
                {
                    _notificationService.ShowMessage("Không có đặt chỗ nào để tạo picklist.", "OK", isError: true);
                    return;
                }
                await SaveStockOutAsync(parameter);
                if (!string.IsNullOrEmpty(StockOutRequestDTO.Error))
                {
                    return;
                }

                var reservation = LstReservations.First();
                var picklist = new PickListRequestDTO
                {
                    PickNo = $"PICK_{StockOutRequestDTO.StockOutCode}",
                    PickDate = DateTime.Now.ToString("dd/MM/yyyy"),
                    ReservationCode = reservation.ReservationCode,
                    Responsible = StockOutRequestDTO.Responsible,
                    StatusId = 1,
                    WarehouseCode = reservation.WarehouseCode
                };

                var result = await _pickListService.AddPickList(picklist);
                if (result.Success)
                {
                    _notificationService.ShowMessage($"Tạo picklist {picklist.PickNo} thành công!", "OK", isError: false);
                    HasPicklist = true;
                    await _messengerService.SendMessageAsync("ReloadPicklistList");
                    await LoadPickListAsync();
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
            return !string.IsNullOrEmpty(StockOutRequestDTO?.StockOutCode) && LstReservations.Any() && !HasPicklist && string.IsNullOrEmpty(StockOutRequestDTO?.Error);
        }

        private bool CanCreateReservation(object parameter)
        {
            return !string.IsNullOrEmpty(StockOutRequestDTO?.StockOutCode) && !HasReservation && string.IsNullOrEmpty(StockOutRequestDTO?.Error);
        }

        private async Task CreateReservationAsync(object parameter)
        {
            try
            {
                if (string.IsNullOrEmpty(StockOutRequestDTO.StockOutCode))
                {
                    _notificationService.ShowMessage("Vui lòng nhập mã phiếu xuất kho trước khi tạo đặt chỗ.", "OK", isError: true);
                    return;
                }

                if (!LstStockOutDetails.Any())
                {
                    _notificationService.ShowMessage("Danh sách chi tiết xuất kho rỗng. Vui lòng thêm chi tiết trước khi tạo đặt chỗ.", "OK", isError: true);
                    return;
                }

                await SaveStockOutAsync(parameter);
                if (!string.IsNullOrEmpty(StockOutRequestDTO.Error))
                {
                    return;
                }

                var reservation = new ReservationRequestDTO
                {
                    ReservationCode = $"RES_{StockOutRequestDTO.StockOutCode}",
                    WarehouseCode = StockOutRequestDTO.WarehouseCode,
                    OrderTypeCode = StockOutRequestDTO.OrderTypeCode,
                    OrderId = StockOutRequestDTO.StockOutCode,
                    ReservationDate = StockOutRequestDTO.StockOutDate,
                    StatusId = 1
                };

                var result = await _reservationService.AddReservation(reservation);
                if (result.Success)
                {
                    _notificationService.ShowMessage("Tạo đặt chỗ thành công!", "OK", isError: false);
                    HasReservation = true;
                    await _messengerService.SendMessageAsync("ReloadReservationList");
                    await LoadReservationsAsync();
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tạo đặt chỗ.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tạo đặt chỗ: {ex.Message}", "OK", isError: true);
            }
        }

        private bool CanConfirmQuantity(object parameter)
        {
            return !string.IsNullOrEmpty(StockOutRequestDTO?.StockOutCode) && HasPicklist && string.IsNullOrEmpty(StockOutRequestDTO?.Error);
        }

        private async Task CheckQuantityAsync(object parameter)
        {
            try
            {
                if (!LstStockOutDetails.Any())
                {
                    _notificationService.ShowMessage("Danh sách chi tiết xuất kho rỗng.", "OK", isError: true);
                    return;
                }

                await SaveStockOutAsync(parameter);
                if (!string.IsNullOrEmpty(StockOutRequestDTO.Error))
                {
                    return;
                }

                bool hasShortage = LstStockOutDetails.Any(d => d.Quantity < d.Demand);
                if (!hasShortage)
                {
                    var confirmResult = await _stockOutDetailService.ConfirmStockOut(StockOutRequestDTO.StockOutCode);
                    if (confirmResult.Success)
                    {
                        _notificationService.ShowMessage("Xác nhận phiếu xuất kho thành công!", "OK", isError: false);
                        await _messengerService.SendMessageAsync("ReloadStockOutList");
                        NavigateBack();
                    }
                    else
                    {
                        _notificationService.ShowMessage(confirmResult.Message ?? "Không thể xác nhận phiếu xuất kho.", "OK", isError: true);
                    }
                    return;
                }

                var viewModel = new BackOrderDialogViewModel(_notificationService, new ObservableCollection<StockOutDetailResponseDTO>(LstStockOutDetails));
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
                        var backOrderResult = await _stockOutDetailService.CreateBackOrder(
                            StockOutRequestDTO.StockOutCode,
                            $"Tạo back order cho phiếu xuất {StockOutRequestDTO.StockOutCode}",
                            viewModel.SelectedDate.ToString()!); // Pass SelectedDate
                        if (!backOrderResult.Success)
                        {
                            _notificationService.ShowMessage(backOrderResult.Message ?? "Không thể tạo backorder.", "OK", isError: true);
                            return;
                        }

                        var confirmResult = await _stockOutDetailService.ConfirmStockOut(StockOutRequestDTO.StockOutCode);
                        if (confirmResult.Success)
                        {
                            _notificationService.ShowMessage("Xác nhận phiếu xuất kho và tạo backorder thành công!", "OK", isError: false);
                            await _messengerService.SendMessageAsync("ReloadStockOutList");
                            NavigateBack();
                        }
                        else
                        {
                            _notificationService.ShowMessage(confirmResult.Message ?? "Không thể xác nhận phiếu xuất kho.", "OK", isError: true);
                        }
                    }
                    else if (viewModel.NoBackorder)
                    {
                        var confirmResult = await _stockOutDetailService.ConfirmStockOut(StockOutRequestDTO.StockOutCode);
                        if (confirmResult.Success)
                        {
                            _notificationService.ShowMessage("Xác nhận phiếu xuất kho thành công, không tạo backorder.", "OK", isError: false);
                            await _messengerService.SendMessageAsync("ReloadStockOutList");
                            NavigateBack();
                        }
                        else
                        {
                            _notificationService.ShowMessage(confirmResult.Message ?? "Không thể xác nhận phiếu xuất kho.", "OK", isError: true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi kiểm tra số lượng: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadStockOutDetailsAsync()
        {
            try
            {
              

                if (string.IsNullOrEmpty(StockOutRequestDTO.StockOutCode))
                {
                    LstStockOutDetails.Clear();
                    TotalPages = 0;
                    return;
                }

                var result = await _stockOutDetailService.GetAllStockOutDetails(StockOutRequestDTO.StockOutCode, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    LstStockOutDetails.Clear();
                    foreach (var detail in result.Data.Data ?? Enumerable.Empty<StockOutDetailResponseDTO>())
                    {
                        detail.SelectedProduct = LstProducts.FirstOrDefault(p => p.ProductCode == detail.ProductCode)!;
                        detail.IsNewRow = false;
                        LstStockOutDetails.Add(detail);
                    }
                    TotalPages = result.Data.TotalPages;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải chi tiết xuất kho.", "OK", isError: true);
                    LstStockOutDetails.Clear();
                    TotalPages = 0;
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải chi tiết xuất kho: {ex.Message}", "OK", isError: true);
                LstStockOutDetails.Clear();
                TotalPages = 0;
            }
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                var result = await _stockOutDetailService.GetListProductToSO(StockOutRequestDTO.StockOutCode);
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

        private async Task LoadOrderTypesAsync()
        {
            try
            {
                var result = await _stockOutService.GetListOrderType("SO");
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
                    LstOrderTypes.Clear();
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải danh sách loại lệnh: {ex.Message}", "OK", isError: true);
                LstOrderTypes.Clear();
            }
        }

        private async Task LoadWarehousesAsync()
        {
            try
            {
                var result = await _stockOutService.GetListWarehousePermission();
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

        private async Task LoadCustomersAsync()
        {
            try
            {
                var result = await _stockOutService.GetListCustomerMasterAsync();
                if (result.Success && result.Data != null)
                {
                    LstCustomers.Clear();
                    foreach (var customer in result.Data)
                    {
                        LstCustomers.Add(customer);
                    }
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải danh sách khách hàng.", "OK", isError: true);
                    LstCustomers.Clear();
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải danh sách khách hàng: {ex.Message}", "OK", isError: true);
                LstCustomers.Clear();
            }
        }

        private async Task LoadResponsiblePersonsAsync()
        {
            try
            {
                var currentResponsible = StockOutRequestDTO.Responsible;
                var result = await _stockOutService.GetListResponsibleAsync(StockOutRequestDTO.WarehouseCode);
                if (result.Success && result.Data != null)
                {
                    LstResponsiblePersons.Clear();
                    foreach (var person in result.Data)
                    {
                        LstResponsiblePersons.Add(person);
                    }
                    if (!string.IsNullOrEmpty(currentResponsible) && LstResponsiblePersons.Any(p => p.UserName == currentResponsible))
                    {
                        StockOutRequestDTO.Responsible = currentResponsible;
                    }
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải danh sách người phụ trách.", "OK", isError: true);
                    LstResponsiblePersons.Clear();
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải danh sách người phụ trách: {ex.Message}", "OK", isError: true);
                LstResponsiblePersons.Clear();
            }
        }

        private async Task SaveStockOutAsync(object parameter)
        {
            if (_isSaving) return;

            try
            {
                _isSaving = true;

                StockOutRequestDTO.RequestValidation();
                if (!string.IsNullOrEmpty(StockOutRequestDTO.Error) || !CanSave(parameter))
                {
                    _notificationService.ShowMessage(StockOutRequestDTO.Error ?? "Vui lòng kiểm tra lại thông tin nhập vào.", "OK", isError: true);
                    return;
                }

                ApiResult<bool> stockOutResult;
                if (IsAddingNew)
                {
                    stockOutResult = await _stockOutService.AddStockOut(StockOutRequestDTO);
                }
                else
                {
                    stockOutResult = await _stockOutService.UpdateStockOut(StockOutRequestDTO);
                }

                if (!stockOutResult.Success)
                {
                    _notificationService.ShowMessage(stockOutResult.Message ?? "Không thể lưu thông tin xuất kho.", "OK", isError: true);
                    return;
                }

                foreach (var detail in LstStockOutDetails.ToList())
                {
                    if (detail?.SelectedProduct == null)
                    {
                        _notificationService.ShowMessage("Vui lòng chọn sản phẩm cho tất cả các dòng.", "OK", isError: true);
                        return;
                    }

                    detail.ProductCode = detail.SelectedProduct.ProductCode ?? throw new InvalidOperationException("Product code cannot be null.");
                    detail.ProductName = detail.SelectedProduct.ProductName ?? string.Empty;

                    var request = new StockOutDetailRequestDTO
                    {
                        StockOutCode = StockOutRequestDTO.StockOutCode,
                        ProductCode = detail.ProductCode,
                        Demand = (double?)detail.Demand,
                        Quantity = (double?)detail.Quantity
                    };

                    request.RequestValidation();
                    if (!string.IsNullOrEmpty(request.Error))
                    {
                        _notificationService.ShowMessage($"Lỗi dữ liệu chi tiết: {request.Error}", "OK", isError: true);
                        return;
                    }

                    ApiResult<bool> detailResult;
                    if (await IsDetailExistsAsync(detail.StockOutCode!, detail.ProductCode))
                    {
                        detailResult = await _stockOutDetailService.UpdateStockOutDetail(request);
                    }
                    else
                    {
                        detailResult = await _stockOutDetailService.AddStockOutDetail(request);
                    }

                    if (!detailResult.Success)
                    {
                        _notificationService.ShowMessage(detailResult.Message ?? "Không thể lưu chi tiết xuất kho.", "OK", isError: true);
                        return;
                    }
                }

                _notificationService.ShowMessage(IsAddingNew ? "Thêm phiếu xuất kho thành công!" : "Cập nhật phiếu xuất kho thành công!", "OK", isError: false);
                if (IsAddingNew)
                {
                    StockOutRequestDTO.ClearValidation();
                    IsAddingNew = false;
                    // Load products only when adding new to ensure dropdowns are populated
                    if (!LstProducts.Any())
                    {
                        await LoadProductsAsync();
                    }
                }

                await _messengerService.SendMessageAsync("ReloadStockOutList");
                // Refresh only necessary data
                await LoadStockOutDetailsAsync();
                await CheckReservationExistenceAsync();
                if (HasReservation)
                {
                    await LoadReservationsAsync();
                }
                if (HasPicklist)
                {
                    await LoadPickListAsync();
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi lưu thông tin xuất kho: {ex.Message}", "OK", isError: true);
            }
            finally
            {
                _isSaving = false;
            }
        }

        private bool CanSave(object parameter)
        {
            var dto = StockOutRequestDTO;
            var propertiesToValidate = new[] { nameof(dto.StockOutCode), nameof(dto.OrderTypeCode), nameof(dto.WarehouseCode), nameof(dto.CustomerCode), nameof(dto.Responsible) };
            foreach (var prop in propertiesToValidate)
            {
                if (!string.IsNullOrEmpty(dto[prop]?.ToString()))
                {
                    return false;
                }
            }
            return true;
        }

        private void NavigateBack()
        {
            var ucStockOutView = App.ServiceProvider!.GetRequiredService<ucStockOut>();
            _navigationService.NavigateTo(ucStockOutView);
        }

        private void AddDetailLine()
        {
            var newDetail = new StockOutDetailResponseDTO
            {
                StockOutCode = StockOutRequestDTO.StockOutCode,
                IsNewRow = true
            };
            LstStockOutDetails.Add(newDetail);
        }

        private bool CanAddDetailLine(object parameter)
        {
            return !string.IsNullOrEmpty(StockOutRequestDTO?.StockOutCode);
        }

        private async Task DeleteDetailLineAsync(StockOutDetailResponseDTO detail)
        {
            if (detail == null) return;

            try
            {
                if (!detail.IsNewRow)
                {
                    var result = await _stockOutDetailService.DeleteStockOutDetail(detail.StockOutCode!, detail.ProductCode);
                    if (!result.Success)
                    {
                        _notificationService.ShowMessage(result.Message ?? "Không thể xóa chi tiết xuất kho.", "OK", isError: true);
                        return;
                    }
                }

                LstStockOutDetails.Remove(detail);
                _notificationService.ShowMessage("Xóa chi tiết xuất kho thành công!", "OK", isError: false);
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi xóa chi tiết xuất kho: {ex.Message}", "OK", isError: true);
            }
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
            CurrentPage = page;
        }

        private void UpdateDisplayPages()
        {
            DisplayPages.Clear();
            if (TotalPages <= 5)
            {
                for (int i = 1; i <= TotalPages; i++)
                {
                    DisplayPages.Add(i);
                }
                return;
            }

            if (CurrentPage <= 3)
            {
                for (int i = 1; i <= 4; i++)
                {
                    DisplayPages.Add(i);
                }
                DisplayPages.Add("...");
                DisplayPages.Add(TotalPages);
            }
            else if (CurrentPage >= TotalPages - 2)
            {
                DisplayPages.Add(1);
                DisplayPages.Add("...");
                for (int i = TotalPages - 3; i <= TotalPages; i++)
                {
                    DisplayPages.Add(i);
                }
            }
            else
            {
                DisplayPages.Add(1);
                DisplayPages.Add("...");
                for (int i = CurrentPage - 1; i <= CurrentPage + 1; i++)
                {
                    DisplayPages.Add(i);
                }
                DisplayPages.Add("...");
                DisplayPages.Add(TotalPages);
            }
        }

        private async Task<bool> IsDetailExistsAsync(string stockOutCode, string productCode)
        {
            try
            {
                var result = await _stockOutDetailService.GetAllStockOutDetails(stockOutCode, 1, int.MaxValue);
                return result.Success && result.Data?.Data.Any(d => d.ProductCode == productCode) == true;
            }
            catch
            {
                return false;
            }
        }

        private async void OnPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StockOutRequestDTO.StockOutCode))
            {
                ((RelayCommand)CreateReservationCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)CreatePicklistCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)ConfirmQuantityCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)AddDetailLineCommand)?.RaiseCanExecuteChanged();
            }
            else if (e.PropertyName == nameof(StockOutRequestDTO.WarehouseCode))
            {
                await LoadResponsiblePersonsAsync();
            }
            else if (new[] { nameof(StockOutRequestDTO.StockOutCode), nameof(StockOutRequestDTO.OrderTypeCode),
                             nameof(StockOutRequestDTO.WarehouseCode), nameof(StockOutRequestDTO.CustomerCode),
                             nameof(StockOutRequestDTO.Responsible), nameof(StockOutRequestDTO.StockOutDate) }
                     .Contains(e.PropertyName))
            {
                ((RelayCommand)SaveCommand)?.RaiseCanExecuteChanged();
            }
        }
    }
}