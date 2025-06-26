using Chrome_WPF.Helpers;
using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.StockTakeDetailDTO;
using Chrome_WPF.Models.StocktakeDTO;
using Chrome_WPF.Models.StockTakeDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.StockTakeDetailService;
using Chrome_WPF.Services.StockTakeService;
using Chrome_WPF.Views.UserControls.StockTake;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.StockTakeViewModel
{
    public class StockTakeDetailViewModel : BaseViewModel
    {
        private readonly IStockTakeService _stockTakeService;
        private readonly IStockTakeDetailService _stockTakeDetailService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IMessengerService _messengerService;

        private ObservableCollection<StockTakeDetailResponseDTO> _lstStockTakeDetails;
        private ObservableCollection<WarehouseMasterResponseDTO> _lstWarehouses;
        private ObservableCollection<AccountManagementResponseDTO> _lstResponsiblePersons;
        private ObservableCollection<object> _displayPages;
        private StockTakeRequestDTO _stockTakeRequestDTO;
        private bool _isAddingNew;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;
        private int _lastLoadedPage;
        private bool _isSaving;

        public ObservableCollection<StockTakeDetailResponseDTO> LstStockTakeDetails
        {
            get => _lstStockTakeDetails;
            set
            {
                _lstStockTakeDetails = value;
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

        public ObservableCollection<object> DisplayPages
        {
            get => _displayPages;
            set
            {
                _displayPages = value;
                OnPropertyChanged();
            }
        }

        public StockTakeRequestDTO StockTakeRequestDTO
        {
            get => _stockTakeRequestDTO;
            set
            {
                if (_stockTakeRequestDTO != null)
                {
                    _stockTakeRequestDTO.PropertyChanged -= OnStockTakeRequestDTOPropertyChanged!;
                }
                _stockTakeRequestDTO = value;
                if (_stockTakeRequestDTO != null)
                {
                    _stockTakeRequestDTO.PropertyChanged += OnStockTakeRequestDTOPropertyChanged!;
                }
                OnPropertyChanged();
                _ = LoadStockTakeDetailsAsync();
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
                    _ = LoadStockTakeDetailsAsync();
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
                    _ = LoadStockTakeDetailsAsync();
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
        public ICommand DeleteDetailLineCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand SelectPageCommand { get; }
        public ICommand ConfirmCommand { get; }

        public StockTakeDetailViewModel(
            IStockTakeService stockTakeService,
            IStockTakeDetailService stockTakeDetailService,
            INotificationService notificationService,
            INavigationService navigationService,
            IMessengerService messengerService,
            StockTakeResponseDTO stockTake = null!)
        {
            _stockTakeService = stockTakeService ?? throw new ArgumentNullException(nameof(stockTakeService));
            _stockTakeDetailService = stockTakeDetailService ?? throw new ArgumentNullException(nameof(stockTakeDetailService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));

            _lstStockTakeDetails = new ObservableCollection<StockTakeDetailResponseDTO>();
            _lstWarehouses = new ObservableCollection<WarehouseMasterResponseDTO>();
            _lstResponsiblePersons = new ObservableCollection<AccountManagementResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            _isAddingNew = stockTake == null;
            _currentPage = 1;
            _lastLoadedPage = 0;
            _isSaving = false;
            _stockTakeRequestDTO = stockTake == null ? new StockTakeRequestDTO() : new StockTakeRequestDTO
            {
                StockTakeCode = stockTake.StocktakeCode,
                StocktakeDate = stockTake.StocktakeDate,
                WarehouseCode = stockTake.WarehouseCode,
                Responsible = stockTake.Responsible
            };

            SaveCommand = new RelayCommand(async _ => await SaveStockTakeAsync(), CanSave);
            BackCommand = new RelayCommand(_ => NavigateBack());
            DeleteDetailLineCommand = new RelayCommand(async detail => await DeleteDetailLineAsync((StockTakeDetailResponseDTO)detail));
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            ConfirmCommand = new RelayCommand(async _ => await ConfirmStockTakeAsync(), CanConfirm);

            _stockTakeRequestDTO.PropertyChanged += OnStockTakeRequestDTOPropertyChanged!;
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(StockTakeRequestDTO?.StockTakeCode) && !IsAddingNew)
                {
                    _notificationService.ShowMessage("Mã lệnh kiểm đếm không hợp lệ. Không thể khởi tạo.", "OK", isError: true);
                    NavigateBack();
                    return;
                }

                await Task.WhenAll(
                    LoadWarehousesAsync(),
                    LoadResponsiblePersonsAsync());

                if (!IsAddingNew)
                {
                    await LoadStockTakeDetailsAsync();
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Khởi tạo thất bại: {ex.Message}", "OK", isError: true);
                NavigateBack();
            }
        }

        private async Task LoadStockTakeDetailsAsync()
        {
            try
            {
                if (_lastLoadedPage == CurrentPage && LstStockTakeDetails.Any())
                {
                    return;
                }

                if (string.IsNullOrEmpty(StockTakeRequestDTO.StockTakeCode))
                {
                    LstStockTakeDetails.Clear();
                    return;
                }

                var warehouseCodes = Properties.Settings.Default.WarehousePermission?.Cast<string>().ToArray() ?? Array.Empty<string>();
                var result = await _stockTakeDetailService.GetStockTakeDetailsByStockTakeCodeAsync(StockTakeRequestDTO.StockTakeCode, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    LstStockTakeDetails.Clear();
                    foreach (var detail in result.Data.Data ?? Enumerable.Empty<StockTakeDetailResponseDTO>())
                    {
                        LstStockTakeDetails.Add(detail);
                    }
                    TotalPages = result.Data.TotalPages;
                    _lastLoadedPage = CurrentPage;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải chi tiết kiểm đếm.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi tải chi tiết kiểm đếm: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadWarehousesAsync()
        {
            try
            {
                var warehouseCodes = Properties.Settings.Default.WarehousePermission?.Cast<string>().ToArray() ?? Array.Empty<string>();
                var result = await _stockTakeService.GetListWarehousePermission(warehouseCodes);
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
                var result = await _stockTakeService.GetListResponsibleAsync(StockTakeRequestDTO.WarehouseCode ?? string.Empty);
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

        private async Task SaveStockTakeAsync()
        {
            if (_isSaving) return;

            try
            {
                _isSaving = true;

                StockTakeRequestDTO.RequestValidation();
                if (!string.IsNullOrEmpty(StockTakeRequestDTO.Error))
                {
                    _notificationService.ShowMessage($"Lỗi dữ liệu: {StockTakeRequestDTO.Error}", "OK", isError: true);
                    return;
                }

                ApiResult<bool> stockTakeResult;
                if (IsAddingNew)
                {
                    stockTakeResult = await _stockTakeService.AddStockTake(StockTakeRequestDTO);
                }
                else
                {
                    stockTakeResult = await _stockTakeService.UpdateStockTake(StockTakeRequestDTO);
                }

                if (!stockTakeResult.Success)
                {
                    _notificationService.ShowMessage(stockTakeResult.Message ?? "Không thể lưu thông tin kiểm đếm.", "OK", isError: true);
                    return;
                }

                foreach (var detail in LstStockTakeDetails.ToList())
                {
                    var request = new StockTakeDetailRequestDTO
                    {
                        StockTakeCode = StockTakeRequestDTO.StockTakeCode,
                        ProductCode = detail.ProductCode,
                        LotNo = detail.Lotno,
                        LocationCode = detail.LocationCode,
                        Quantity = detail.Quantity,
                        CountedQuantity = detail.CountedQuantity
                    };

                    request.RequestValidation();
                    if (!string.IsNullOrEmpty(request.Error))
                    {
                        _notificationService.ShowMessage($"Lỗi dữ liệu chi tiết: {request.Error}", "OK", isError: true);
                        return;
                    }

                    ApiResult<bool> detailResult;
                    if (await IsDetailExistsAsync(detail.StocktakeCode, detail.ProductCode, detail.Lotno, detail.LocationCode))
                    {
                        detailResult = await _stockTakeDetailService.UpdateStockTakeDetail(request);
                    }
                    else
                    {
                        detailResult = await _stockTakeDetailService.UpdateStockTakeDetail(request); // Assuming update for consistency
                    }

                    if (!detailResult.Success)
                    {
                        _notificationService.ShowMessage(detailResult.Message ?? "Không thể lưu chi tiết kiểm đếm.", "OK", isError: true);
                        return;
                    }
                }

                _notificationService.ShowMessage(IsAddingNew ? "Thêm lệnh kiểm đếm thành công!" : "Cập nhật lệnh kiểm đếm thành công!", "OK", isError: false);
                if (IsAddingNew)
                {
                    StockTakeRequestDTO.ClearValidation();
                    IsAddingNew = false;
                }
                await _messengerService.SendMessageAsync("ReloadStockTakeList");
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi lưu thông tin kiểm đếm: {ex.Message}", "OK", isError: true);
            }
            finally
            {
                _isSaving = false;
            }
        }

        private async Task ConfirmStockTakeAsync()
        {
            try
            {
                if (!LstStockTakeDetails.Any())
                {
                    _notificationService.ShowMessage("Danh sách chi tiết kiểm đếm rỗng.", "OK", isError: true);
                    return;
                }

                await SaveStockTakeAsync();
                if (!string.IsNullOrEmpty(StockTakeRequestDTO.Error))
                {
                    return;
                }

                var confirmResult = await _stockTakeService.ConfirmnStockTake(new StockTakeRequestDTO
                {
                    StockTakeCode = StockTakeRequestDTO.StockTakeCode,
                    StocktakeDate = StockTakeRequestDTO.StocktakeDate,
                    WarehouseCode = StockTakeRequestDTO.WarehouseCode,
                    Responsible = StockTakeRequestDTO.Responsible,
                });

                if (confirmResult.Success)
                {
                    _notificationService.ShowMessage("Xác nhận lệnh kiểm đếm thành công!", "OK", isError: false);
                    await _messengerService.SendMessageAsync("ReloadStockTakeList");
                    NavigateBack();
                }
                else
                {
                    _notificationService.ShowMessage(confirmResult.Message ?? "Không thể xác nhận lệnh kiểm đếm.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi xác nhận lệnh kiểm đếm: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task<bool> IsDetailExistsAsync(string stockTakeCode, string productCode, string lotNo, string locationCode)
        {
            try
            {
                var result = await _stockTakeDetailService.GetStockTakeDetailsByStockTakeCodeAsync(stockTakeCode, 1, int.MaxValue);
                return result.Success && result.Data?.Data.Any(d => d.ProductCode == productCode && d.Lotno == lotNo && d.LocationCode == locationCode) == true;
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi kiểm tra chi tiết kiểm đếm: {ex.Message}", "OK", isError: true);
                return false;
            }
        }

        private async Task DeleteDetailLineAsync(StockTakeDetailResponseDTO detail)
        {
            if (detail == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa chi tiết sản phẩm '{detail.ProductName}' tại vị trí '{detail.LocationName}'?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deleteResult = await _stockTakeDetailService.DeleteStockTakeDetail(detail.StocktakeCode, detail.ProductCode, detail.Lotno, detail.LocationCode);
                    if (!deleteResult.Success)
                    {
                        _notificationService.ShowMessage(deleteResult.Message ?? "Không thể xóa chi tiết kiểm đếm.", "OK", isError: true);
                        return;
                    }
                    LstStockTakeDetails.Remove(detail);
                    _notificationService.ShowMessage("Xóa chi tiết kiểm đếm thành công.", "OK", isError: false);
                }
                catch (Exception ex)
                {
                    _notificationService.ShowMessage($"Lỗi khi xóa chi tiết kiểm đếm: {ex.Message}", "OK", isError: true);
                }
            }
        }

        private bool CanSave(object parameter)
        {
            var dto = StockTakeRequestDTO;
            var propertiesToValidate = new[] { nameof(dto.StockTakeCode), nameof(dto.WarehouseCode) };

            foreach (var prop in propertiesToValidate)
            {
                if (!string.IsNullOrEmpty(dto[prop]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CanConfirm(object parameter)
        {
            return !string.IsNullOrEmpty(StockTakeRequestDTO?.StockTakeCode);
        }

        private void NavigateBack()
        {
            if (_stockTakeRequestDTO != null)
            {
                _stockTakeRequestDTO.PropertyChanged -= OnStockTakeRequestDTOPropertyChanged!;
            }

            var stockTakeList = App.ServiceProvider!.GetRequiredService<ucStockTake>();
            _navigationService.NavigateTo(stockTakeList);
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

        private void OnStockTakeRequestDTOPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(StockTakeRequestDTO));
            if (!_isSaving)
            {
                ((RelayCommand)SaveCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)ConfirmCommand)?.RaiseCanExecuteChanged();
                if (e.PropertyName == nameof(StockTakeRequestDTO.WarehouseCode))
                {
                    LstResponsiblePersons.Clear();
                    StockTakeRequestDTO.Responsible = null!;
                    _= LoadResponsiblePersonsAsync();
                }
            }
        }
    }
}