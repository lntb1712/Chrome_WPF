using Chrome_WPF.Helpers;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.PickListDetailDTO;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.PickListDetailService;
using Chrome_WPF.Views.UserControls.PickList;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.PickListViewModel
{
    public class PickListDetailViewModel : BaseViewModel
    {
        private readonly IPickListDetailService _pickListDetailService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;

        private ObservableCollection<PickListDetailResponseDTO> _pickListDetails;
        private ObservableCollection<object> _displayPages;
        private string _pickNo;
        private string _searchText;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;
        private static readonly Dictionary<string, List<PickListDetailResponseDTO>> _cache = new Dictionary<string, List<PickListDetailResponseDTO>>();

        public ObservableCollection<PickListDetailResponseDTO> PickListDetails
        {
            get => _pickListDetails;
            set
            {
                _pickListDetails = value;
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

        public string PickNo
        {
            get => _pickNo;
            set
            {
                _pickNo = value;
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
                _ = LoadPickListDetailsAsync();
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
                _ = LoadPickListDetailsAsync();
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
                _ = LoadPickListDetailsAsync();
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

        public PickListDetailViewModel(
            IPickListDetailService pickListDetailService,
            INotificationService notificationService,
            INavigationService navigationService,
            string pickNo = "")
        {
            _pickListDetailService = pickListDetailService ?? throw new ArgumentNullException(nameof(pickListDetailService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            _pickListDetails = new ObservableCollection<PickListDetailResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            _pickNo = pickNo;
            _searchText = string.Empty;
            _currentPage = 1;

            BackCommand = new RelayCommand(_ => NavigateBack());
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            SaveCommand = new RelayCommand(async _ => await SavePickListDetails());

            _ = LoadPickListDetailsAsync();
        }

        private async Task LoadPickListDetailsAsync()
        {
            try
            {
                // Check cache first
                if (_cache.ContainsKey(PickNo) && string.IsNullOrWhiteSpace(SearchText))
                {
                    PickListDetails.Clear();
                    foreach (var detail in _cache[PickNo])
                    {
                        PickListDetails.Add(detail);
                    }
                    TotalPages = (int)Math.Ceiling((double)_cache[PickNo].Count / PageSize);
                    return;
                }

                ApiResult<PagedResponse<PickListDetailResponseDTO>> result;
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    result = await _pickListDetailService.GetPickListDetailsByPickNoAsync(_pickNo, CurrentPage, PageSize);
                }
                else
                {
                    result = await _pickListDetailService.SearchPickListDetailsAsync(new string[] { }, PickNo, SearchText, CurrentPage, PageSize);
                }

                if (result.Success && result.Data != null)
                {
                    PickListDetails.Clear();
                    foreach (var detail in result.Data.Data ?? Enumerable.Empty<PickListDetailResponseDTO>())
                    {
                        PickListDetails.Add(detail);
                    }
                    TotalPages = result.Data.TotalPages;

                    // Cache the results if not a search operation
                    if (string.IsNullOrWhiteSpace(SearchText))
                    {
                        _cache[PickNo] = PickListDetails.ToList();
                    }
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách chi tiết phiếu lấy hàng.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private void NavigateBack()
        {
            var ucPickList = App.ServiceProvider!.GetRequiredService<ucPickList>();
            _navigationService.NavigateTo(ucPickList);
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

        private async Task SavePickListDetails()
        {
            try
            {
                if (!_pickListDetails.Any())
                {
                    _notificationService.ShowMessage("Không có chi tiết phiếu lấy hàng để lưu.", "OK", isError: true);
                    return;
                }

                bool allSuccess = true;
                string errorMessage = string.Empty;

                foreach (var detail in _pickListDetails)
                {
                    // Convert ResponseDTO to RequestDTO for update
                    var requestDTO = new PickListDetailRequestDTO
                    {
                        PickNo = detail.PickNo,
                        ProductCode = detail.ProductCode,
                        LotNo = detail.LotNo,
                        Demand = detail.Demand,
                        Quantity = detail.Quantity,
                        LocationCode = detail.LocationCode
                    };

                    var result = await _pickListDetailService.UpdatePickListDetail(requestDTO);
                    if (!result.Success)
                    {
                        allSuccess = false;
                        errorMessage = result.Message ?? "Lỗi khi cập nhật chi tiết phiếu lấy hàng.";
                        break;
                    }
                }

                if (allSuccess)
                {
                    // Update cache
                    _cache[PickNo] = PickListDetails.ToList();
                    _notificationService.ShowMessage("Lưu dữ liệu phiếu lấy hàng thành công.", "OK", isError: false);

                    // Refresh data to ensure consistency
                    await LoadPickListDetailsAsync();
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