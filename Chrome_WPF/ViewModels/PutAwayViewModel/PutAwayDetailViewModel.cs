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
        private static readonly Dictionary<string, List<PutAwayDetailResponseDTO>> _cache = new Dictionary<string, List<PutAwayDetailResponseDTO>>();

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
        public ICommand SaveCommand { get; }

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
            SaveCommand = new RelayCommand(async _ => await SavePutAwayDetails());

            _ = LoadPutAwayDetailsAsync();
        }

        private async Task LoadPutAwayDetailsAsync()
        {
            try
            {
                // Check cache first
                if (_cache.ContainsKey(PutAwayCode) && string.IsNullOrWhiteSpace(SearchText))
                {
                    PutAwayDetails.Clear();
                    foreach (var detail in _cache[PutAwayCode])
                    {
                        PutAwayDetails.Add(detail);
                    }
                    TotalPages = (int)Math.Ceiling((double)_cache[PutAwayCode].Count / PageSize);
                    return;
                }

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

                    // Cache the results if not a search operation
                    if (string.IsNullOrWhiteSpace(SearchText))
                    {
                        _cache[PutAwayCode] = PutAwayDetails.ToList();
                    }
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

        private async Task SavePutAwayDetails()
        {
            try
            {
                if (!_putAwayDetails.Any())
                {
                    _notificationService.ShowMessage("Không có chi tiết phiếu nhập kho để lưu.", "OK", isError: true);
                    return;
                }

                bool allSuccess = true;
                string errorMessage = string.Empty;

                foreach (var detail in _putAwayDetails)
                {
                    // Convert ResponseDTO to RequestDTO for update
                    var requestDTO = new PutAwayDetailRequestDTO
                    {
                        PutAwayCode = detail.PutAwayCode,
                        ProductCode = detail.ProductCode,
                        LotNo = detail.LotNo,
                        Demand = detail.Demand,
                        Quantity = detail.Quantity
                    };

                    var result = await _putAwayDetailService.UpdatePutAwayDetail(requestDTO);
                    if (!result.Success)
                    {
                        allSuccess = false;
                        errorMessage = result.Message ?? "Lỗi khi cập nhật chi tiết phiếu nhập kho.";
                        break;
                    }
                }

                if (allSuccess)
                {
                    // Update cache
                    _cache[PutAwayCode] = PutAwayDetails.ToList();
                    _notificationService.ShowMessage("Lưu dữ liệu phiếu nhập kho thành công.", "OK", isError: false);

                    // Refresh data to ensure consistency
                    await LoadPutAwayDetailsAsync();
                }
                else
                {
                    _notificationService.ShowMessage(errorMessage, "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }
    }
}