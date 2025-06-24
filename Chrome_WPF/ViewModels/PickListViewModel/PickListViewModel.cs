using Chrome_WPF.Helpers;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.PickListDTO;
using Chrome_WPF.Models.StatusMasterDTO;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.PickListDetailService;
using Chrome_WPF.Services.PickListService;
using Chrome_WPF.Views.UserControls.PickList;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.PickListViewModel
{
    public class PickListViewModel : BaseViewModel
    {
        private readonly IPickListService _pickListService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;

        private ObservableCollection<PickListResponseDTO> _pickList;
        private ObservableCollection<object> _displayPages;
        private ObservableCollection<StatusMasterResponseDTO> _statuses;
        private string _searchText;
        private int _selectedStatusId;
        private int _selectedStatusIndex;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;
        private string _applicableWarehouseCodes = string.Empty; // Initialize with a default value
        private List<string> _warehouseCodes;

        public ObservableCollection<PickListResponseDTO> PickList
        {
            get => _pickList;
            set
            {
                _pickList = value;
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
                _ = LoadPickListsAsync();
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
                _ = LoadPickListsAsync();
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
                _ = LoadPickListsAsync();
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
                _ = LoadPickListsAsync();
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

        public PickListViewModel(
            IPickListService pickListService,
            INotificationService notificationService,
            INavigationService navigationService)
        {
            _pickListService = pickListService ?? throw new ArgumentNullException(nameof(pickListService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            _pickList = new ObservableCollection<PickListResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            _statuses = new ObservableCollection<StatusMasterResponseDTO>();
            _searchText = string.Empty;
            _currentPage = 1;
            _warehouseCodes = new List<string>();
            

            // Load warehouse permissions
            var savedPermissions = Properties.Settings.Default.WarehousePermission;
            if (savedPermissions != null)
            {
                _warehouseCodes = savedPermissions.Cast<string>().ToList();
                ApplicableWarehouseCodes = string.Join(", ", _warehouseCodes);
            }

            SearchCommand = new RelayCommand(async _ => await SearchPickListsAsync());
            RefreshCommand = new RelayCommand(async _ => await LoadPickListsAsync());
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            ViewDetailCommand = new RelayCommand(pickList => OpenDetail((PickListResponseDTO)pickList));

            _ = LoadStatusesAsync();
            _ = LoadPickListsAsync();
        }

        private async Task LoadStatusesAsync()
        {
            try
            {
                var result = await _pickListService.GetListStatusMaster();
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

        private async Task LoadPickListsAsync()
        {
            try
            {
                ApiResult<PagedResponse<PickListResponseDTO>> result;
                if (SelectedStatusId > 0)
                {
                    result = await _pickListService.GetAllPickListsWithStatusAsync(_warehouseCodes.ToArray(), SelectedStatusId, CurrentPage, PageSize);
                }
                else
                {
                    result = await _pickListService.GetAllPickListsAsync(_warehouseCodes.ToArray(), CurrentPage, PageSize);
                }

                if (result.Success && result.Data != null)
                {
                    PickList.Clear();
                    foreach (var pickList in result.Data.Data ?? Enumerable.Empty<PickListResponseDTO>())
                    {
                        PickList.Add(pickList);
                    }
                    TotalPages = result.Data.TotalPages;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách phiếu lấy hàng.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task SearchPickListsAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    await LoadPickListsAsync();
                    return;
                }

                var result = await _pickListService.SearchPickListsAsync(_warehouseCodes.ToArray(), SearchText, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    PickList.Clear();
                    foreach (var pickList in result.Data.Data ?? Enumerable.Empty<PickListResponseDTO>())
                    {
                        PickList.Add(pickList);
                    }
                    TotalPages = result.Data.TotalPages;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tìm kiếm phiếu lấy hàng.", "OK", isError: true);
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

        private void OpenDetail(PickListResponseDTO pickList)
        {
            if (pickList == null) return;

            var pickListDetail = App.ServiceProvider!.GetRequiredService<ucPickListDetail>();
            var viewModel = new PickListDetailViewModel(
                App.ServiceProvider!.GetRequiredService<IPickListDetailService>(),
                App.ServiceProvider!.GetRequiredService<INotificationService>(),
                App.ServiceProvider!.GetRequiredService<INavigationService>(),
                pickList.PickNo);

            pickListDetail.DataContext = viewModel;
            _navigationService.NavigateTo(pickListDetail);
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
