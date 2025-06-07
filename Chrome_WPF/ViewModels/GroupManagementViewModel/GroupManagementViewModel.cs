using Chrome_WPF.Helpers;
using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.GroupManagementDTO;
using Chrome_WPF.Services.GroupManagementService;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Views.UserControls.GroupManagement;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.GroupManagementViewModel
{
    public class GroupManagementViewModel : BaseViewModel
    {
        private readonly IGroupManagementService _groupManagementService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IMessengerService _messengerService;

        private ObservableCollection<GroupManagementResponseDTO> _groupList;
        private ObservableCollection<object> _displayPages;
        private string _searchText;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;

        private GroupManagementResponseDTO _selectedGroupManagement;
        private GroupManagementRequestDTO? _groupManagementRequestDTO;
        private bool _isEditorOpen;
        private int _totalGroupsCount;

        public ObservableCollection<GroupManagementResponseDTO> GroupList
        {
            get => _groupList;
            set
            {
                _groupList = value;
                OnPropertyChanged();
            }
        }
        public GroupManagementRequestDTO GroupManagementRequestDTO
        {
            get => _groupManagementRequestDTO!;
            set
            {
                _groupManagementRequestDTO = value;
                OnPropertyChanged();
            }
        }
        public GroupManagementResponseDTO SelectedGroup
        {
            get => _selectedGroupManagement;
            set
            {
                _selectedGroupManagement = value;
                OnPropertyChanged();
                if (SelectedGroup != null)
                {
                    GroupManagementRequestDTO = new GroupManagementRequestDTO
                    {
                        GroupId = SelectedGroup.GroupId,
                        GroupName = SelectedGroup.GroupName!,
                        GroupDescription = SelectedGroup.GroupDescription,
                    };
                }
                else
                {
                    GroupManagementRequestDTO = new GroupManagementRequestDTO();
                }
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
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
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
                _ = LoadGroupsAsync();
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
                _ = LoadGroupsAsync();
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
        public bool IsEditorOpen
        {
            get => _isEditorOpen;
            set
            {
                _isEditorOpen = value;
                OnPropertyChanged();
            }
        }
        public int TotalGroupsCount
        {
            get => _totalGroupsCount;
            set
            {
                _totalGroupsCount = value;
                OnPropertyChanged();
            }
        }


        public ICommand SearchCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand SelectPageCommand { get; }
        public ICommand FilterAllCommand { get; }

        public GroupManagementViewModel(
            IGroupManagementService groupManagementService,
            INotificationService notificationService,
            INavigationService navigationService,
            IMessengerService messengerService)
        {
            _groupManagementService = groupManagementService ?? throw new ArgumentException(nameof(groupManagementService));
            _notificationService = notificationService ?? throw new ArgumentException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentException(nameof(messengerService));

            _groupList = new ObservableCollection<GroupManagementResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            GroupManagementRequestDTO = new GroupManagementRequestDTO();
            _selectedGroupManagement = null!;
            _currentPage = 1;
            _searchText = string.Empty;

            SearchCommand = new RelayCommand(async _ => await SearchGroupsAsync());
            AddCommand = new RelayCommand(_ => OpenEditor(null!));
            DeleteCommand = new RelayCommand(async group => await DeleteGroupAsync((GroupManagementResponseDTO)group));
            UpdateCommand = new RelayCommand(group => OpenEditor((GroupManagementResponseDTO)group));
            RefreshCommand = new RelayCommand(async _ => await LoadGroupsAsync());
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            FilterAllCommand = new RelayCommand(async _ => await LoadGroupsAsync());

            _ = messengerService.RegisterMessageAsync("ReloadGroupListMessage", async (obj) =>
            {
                await LoadGroupsAsync();
            });

            _ = LoadGroupsAsync();
        }

        private async Task SearchGroupsAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    await LoadGroupsAsync();
                    return;
                }

                var result = await _groupManagementService.SearchGroupInList(SearchText, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    GroupList.Clear();
                    foreach (var group in result.Data.Data ?? Enumerable.Empty<GroupManagementResponseDTO>())
                    {
                        GroupList.Add(group);
                    }
                    TotalPages = result.Data.TotalPages;
                    // Đảm bảo DataGrid cập nhật ngay lập tức
                    OnPropertyChanged(nameof(GroupList));
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tìm kiếm tài khoản.", "OK", isError: true);
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
            DisplayPages = new ObservableCollection<object>();
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

        private async Task DeleteGroupAsync(GroupManagementResponseDTO group)
        {
            if (group == null) return;
            var result = MessageBox.Show($"Bạn có chắc muốn xóa nhóm {group.GroupName}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deleteResult = await _groupManagementService.DeleteGroupManagement(group.GroupId);
                    if (deleteResult.Success)
                    {
                        _notificationService.ShowMessage($"Đã xóa nhóm {group.GroupName} thành công.", "OK", isError: false);
                        await LoadGroupsAsync();
                    }
                    else
                    {
                        _notificationService.ShowMessage(deleteResult.Message ?? "Lỗi khi xóa nhóm.", "OK", isError: true);
                    }
                }
                catch (Exception ex)
                {
                    _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
                }
            }
        }

        private void OpenEditor(GroupManagementResponseDTO? group)
        {
            var groupEditor = App.ServiceProvider!.GetRequiredService<ucGroupEditor>();
            var dto = group == null
                ? new GroupManagementRequestDTO()
                : new GroupManagementRequestDTO
                {
                    GroupId = group.GroupId ?? string.Empty,
                    GroupName = group.GroupName ?? string.Empty,
                    GroupDescription = group.GroupDescription ?? string.Empty,
                };

            var viewModel = new GroupEditorViewModel(
                _groupManagementService,
                _notificationService,
                _navigationService,
                _messengerService,
                isAddingNew: group == null,
                initialDto: dto);

            groupEditor.DataContext = viewModel;
            _navigationService.NavigateTo(groupEditor);
        }

        private async Task TotalGroups()
        {
            try
            {
                var result = await _groupManagementService.GetTotalGroupCount();
                if (result.Success)
                {
                    TotalGroupsCount = result.Data;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi lấy tổng số nhóm.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }
        private async Task LoadGroupsAsync()
        {
            try
            {
                var result = await _groupManagementService.GetAllGroupManagement(CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    GroupList.Clear();
                    foreach (var group in result.Data.Data ?? Enumerable.Empty<GroupManagementResponseDTO>())
                    {
                        GroupList.Add(group);
                    }
                    TotalPages = result.Data.TotalPages;
                    _ = TotalGroups();
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách nhóm.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }
    }
}
