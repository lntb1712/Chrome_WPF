using Chrome_WPF.Helpers;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.ReservationDTO;
using Chrome_WPF.Models.StatusMasterDTO;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.ReservationDetailService;
using Chrome_WPF.Services.ReservationService;
using Chrome_WPF.Views.UserControls.Reservation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.ReservationViewModel
{
    public class ReservationViewModel : BaseViewModel
    {
        private readonly IReservationService _reservationService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;

        private ObservableCollection<ReservationResponseDTO> _reservationList;
        private ObservableCollection<object> _displayPages;
        private ObservableCollection<StatusMasterResponseDTO> _statuses;
        private string _searchText;
        private int _selectedStatusId;
        private int _selectedStatusIndex;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;
        private string _applicableWarehouseCodes;
        private List<string> _warehouseCodes;

        public ObservableCollection<ReservationResponseDTO> ReservationList
        {
            get => _reservationList;
            set
            {
                _reservationList = value;
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

        public ObservableCollection<StatusMasterResponseDTO> Statuses
        {
            get => _statuses;
            set
            {
                _statuses = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                _ = LoadReservationsAsync();
            }
        }

        public int SelectedStatusId
        {
            get => _selectedStatusId;
            set
            {
                _selectedStatusId = value;
                OnPropertyChanged();
            }
        }

        public int SelectedStatusIndex
        {
            get => _selectedStatusIndex;
            set
            {
                _selectedStatusIndex = value;
                OnPropertyChanged();
                UpdateSelectedStatus();
                _ = LoadReservationsAsync();
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
                UpdateDisplayPages();
                _ = LoadReservationsAsync();
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
                _ = LoadReservationsAsync();
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

        public string ApplicableWarehouseCodes
        {
            get => _applicableWarehouseCodes;
            set
            {
                _applicableWarehouseCodes = value;
                OnPropertyChanged();
            }
        }

        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand SelectPageCommand { get; }
        public ICommand ViewDetailCommand { get; }

        public ReservationViewModel(
            IReservationService reservationService,
            INotificationService notificationService,
            INavigationService navigationService)
        {
            _reservationService = reservationService ?? throw new ArgumentNullException(nameof(reservationService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            _reservationList = new ObservableCollection<ReservationResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            _statuses = new ObservableCollection<StatusMasterResponseDTO>();
            _searchText = string.Empty;
            _currentPage = 1;
            _warehouseCodes = new List<string>();
            _applicableWarehouseCodes = string.Empty;

            // Load warehouse permissions
            var savedPermissions = Properties.Settings.Default.WarehousePermission;
            if (savedPermissions != null)
            {
                _warehouseCodes = savedPermissions.Cast<string>().ToList();
                ApplicableWarehouseCodes = string.Join(", ", _warehouseCodes);
            }

            SearchCommand = new RelayCommand(async _ => await SearchReservationsAsync());
            RefreshCommand = new RelayCommand(async _ => await LoadReservationsAsync());
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            ViewDetailCommand = new RelayCommand(reservation => OpenDetail((ReservationResponseDTO)reservation));

            _ = LoadStatusesAsync();
            _ = LoadReservationsAsync();
        }

        private async Task LoadStatusesAsync()
        {
            try
            {
                var result = await _reservationService.GetListStatusMaster();
                if (result.Success && result.Data != null)
                {
                    Statuses.Clear();
                    Statuses.Add(new StatusMasterResponseDTO { StatusId = 0, StatusName = "Tất cả" });
                    foreach (var status in result.Data)
                    {
                        Statuses.Add(status);
                    }
                    SelectedStatusIndex = 0;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách trạng thái.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadReservationsAsync()
        {
            try
            {
                ApiResult<PagedResponse<ReservationResponseDTO>> result;
                if (SelectedStatusId > 0)
                {
                    result = await _reservationService.GetAllReservationsWithStatus(_warehouseCodes.ToArray(), SelectedStatusId, CurrentPage, PageSize);
                }
                else
                {
                    result = await _reservationService.GetAllReservations(_warehouseCodes.ToArray(), CurrentPage, PageSize);
                }

                if (result.Success && result.Data != null)
                {
                    ReservationList.Clear();
                    foreach (var reservation in result.Data.Data ?? Enumerable.Empty<ReservationResponseDTO>())
                    {
                        ReservationList.Add(reservation);
                    }
                    TotalPages = result.Data.TotalPages;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách đặt chỗ.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task SearchReservationsAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    await LoadReservationsAsync();
                    return;
                }

                var result = await _reservationService.SearchReservationsAsync(_warehouseCodes.ToArray(), SearchText, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    ReservationList.Clear();
                    foreach (var reservation in result.Data.Data ?? Enumerable.Empty<ReservationResponseDTO>())
                    {
                        ReservationList.Add(reservation);
                    }
                    TotalPages = result.Data.TotalPages;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tìm kiếm đặt chỗ.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
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

        private void OpenDetail(ReservationResponseDTO reservation)
        {
            if (reservation == null) return;

            var reservationDetail = App.ServiceProvider!.GetRequiredService<ucReservationDetail>();
            var viewModel = new ReservationDetailViewModel(
                App.ServiceProvider!.GetRequiredService<IReservationDetailService>(),
                App.ServiceProvider!.GetRequiredService<INotificationService>(),
                App.ServiceProvider!.GetRequiredService<INavigationService>(),
                reservation.ReservationCode);

            reservationDetail.DataContext = viewModel;
            _navigationService.NavigateTo(reservationDetail);
        }

        private void UpdateSelectedStatus()
        {
            if (SelectedStatusIndex >= 0 && SelectedStatusIndex < Statuses.Count)
            {
                SelectedStatusId = Statuses[SelectedStatusIndex].StatusId;
            }
            else
            {
                SelectedStatusId = 0;
            }
        }
    }
}