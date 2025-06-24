using Chrome_WPF.Helpers;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.ReservationDetailDTO;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.ReservationDetailService;
using Chrome_WPF.Views.UserControls.Reservation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.ReservationViewModel
{
    public class ReservationDetailViewModel : BaseViewModel
    {
        private readonly IReservationDetailService _reservationDetailService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;

        private ObservableCollection<ReservationDetailResponseDTO> _reservationDetails;
        private ObservableCollection<object> _displayPages;
        private string _reservationCode;
        private string _searchText;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;

        public ObservableCollection<ReservationDetailResponseDTO> ReservationDetails
        {
            get => _reservationDetails;
            set
            {
                _reservationDetails = value;
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

        public string ReservationCode
        {
            get => _reservationCode;
            set
            {
                _reservationCode = value;
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
                _ = LoadReservationDetailsAsync();
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
                _ = LoadReservationDetailsAsync();
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
                _ = LoadReservationDetailsAsync();
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

        public ICommand BackCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand SelectPageCommand { get; }

        public ReservationDetailViewModel(
            IReservationDetailService reservationDetailService,
            INotificationService notificationService,
            INavigationService navigationService,
            string reservationCode = "")
        {
            _reservationDetailService = reservationDetailService ?? throw new ArgumentNullException(nameof(reservationDetailService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            _reservationDetails = new ObservableCollection<ReservationDetailResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            _reservationCode = reservationCode;
            _searchText = string.Empty;
            _currentPage = 1;

            BackCommand = new RelayCommand(_ => NavigateBack());
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));

            _ = LoadReservationDetailsAsync();
        }

        private async Task LoadReservationDetailsAsync()
        {
            try
            {
                ApiResult<PagedResponse<ReservationDetailResponseDTO>> result;
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    result = await _reservationDetailService.GetAllReservationDetails(ReservationCode, CurrentPage, PageSize);
                }
                else
                {
                    result = await _reservationDetailService.SearchReservationDetailsAsync(ReservationCode, SearchText, CurrentPage, PageSize);
                }

                if (result.Success && result.Data != null)
                {
                    ReservationDetails.Clear();
                    foreach (var detail in result.Data.Data ?? Enumerable.Empty<ReservationDetailResponseDTO>())
                    {
                        ReservationDetails.Add(detail);
                    }
                    TotalPages = result.Data.TotalPages;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách chi tiết đặt chỗ.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private void NavigateBack()
        {
            var ucReservation = App.ServiceProvider!.GetRequiredService<ucReservation>();
            _navigationService.NavigateTo(ucReservation);
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
    }
}