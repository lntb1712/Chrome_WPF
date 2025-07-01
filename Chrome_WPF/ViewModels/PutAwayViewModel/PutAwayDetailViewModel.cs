using Chrome_WPF.Helpers;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.PutAwayDetailDTO;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.PutAwayDetailService;
using Chrome_WPF.Views.UserControls.PutAway;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.PutAwayViewModel
{
    public class PutAwayDetailViewModel : BaseViewModel
    {
        private readonly IPutAwayDetailService _putAwayDetailService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;

        private ObservableCollection<PutAwayDetailResponseDTO> _putAwayDetails;
        private ObservableCollection<object> _displayPages;
        private string _putAwayCode;
        private string _searchText;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;
       

        public ObservableCollection<PutAwayDetailResponseDTO> PutAwayDetails
        {
            get => _putAwayDetails;
            set
            {
                _putAwayDetails = value;
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

        public string PutAwayCode
        {
            get => _putAwayCode;
            set
            {
                _putAwayCode = value;
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
                _ = LoadPutAwayDetailsAsync();
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
                _ = LoadPutAwayDetailsAsync();
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
                _ = LoadPutAwayDetailsAsync();
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
        public ICommand RefreshCommand { get; }

        public PutAwayDetailViewModel(
            IPutAwayDetailService putAwayDetailService,
            INotificationService notificationService,
            INavigationService navigationService,
            string putAwayCode = "")
        {
            _putAwayDetailService = putAwayDetailService ?? throw new ArgumentNullException(nameof(putAwayDetailService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            _putAwayDetails = new ObservableCollection<PutAwayDetailResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            _putAwayCode = putAwayCode;
            _searchText = string.Empty;
            _currentPage = 1;

            BackCommand = new RelayCommand(_ => NavigateBack());
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            RefreshCommand = new RelayCommand(async _ => await LoadPutAwayDetailsAsync());

            _ = LoadPutAwayDetailsAsync();
        }

        private async Task LoadPutAwayDetailsAsync()
        {
            try
            {


                ApiResult<PagedResponse<PutAwayDetailResponseDTO>> result;
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    result = await _putAwayDetailService.GetPutAwayDetailsByPutawayCodeAsync(_putAwayCode, CurrentPage, PageSize);
                }
                else
                {
                    result = await _putAwayDetailService.SearchPutAwayDetailsAsync(new string[] { }, PutAwayCode, SearchText, CurrentPage, PageSize);
                }

                if (result.Success && result.Data != null)
                {
                    PutAwayDetails.Clear();
                    foreach (var detail in result.Data.Data ?? Enumerable.Empty<PutAwayDetailResponseDTO>())
                    {
                        PutAwayDetails.Add(detail);
                    }
                    TotalPages = result.Data.TotalPages;

    
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách chi tiết phiếu nhập kho.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private void NavigateBack()
        {
            var ucPutAway = App.ServiceProvider!.GetRequiredService<ucPutAway>();
            _navigationService.NavigateTo(ucPutAway);
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