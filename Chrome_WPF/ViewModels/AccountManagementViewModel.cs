using Chrome_WPF.Helpers;
using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Services.AccountManagementService;
using Chrome_WPF.Services.GroupManagementService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Views;
using Chrome_WPF.Views.UserControls.AccountManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels
{
    public class AccountManagementViewModel : BaseViewModel
    {
        private readonly IAccountManagementService _accountManagementService;
        private readonly IGroupManagementService _groupManagementService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private ObservableCollection<AccountManagementResponseDTO> _accountList;
        private Dictionary<string, int> _lstGroupManagement;
        private string _searchText;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;
        private string _selectedGroupId;
        private AccountManagementResponseDTO _selectedAccount;
        private bool _isEditorOpen;
        private ObservableCollection<object> _displayPages;

        public ObservableCollection<AccountManagementResponseDTO> AccountList
        {
            get => _accountList;
            set
            {
                _accountList = value;
                OnPropertyChanged();
                SelectFirstAccount();
            }
        }

        public Dictionary<string, int> LstGroupManagement
        {
            get => _lstGroupManagement;
            set
            {
                _lstGroupManagement = value;
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
                Task.Delay(300).ContinueWith(_ => SearchAccountsAsync());
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
                    LoadAccountsAsync().GetAwaiter().GetResult();
                }
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
                LoadAccountsAsync().GetAwaiter().GetResult();
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

        public string SelectedGroupId
        {
            get => _selectedGroupId;
            set
            {
                _selectedGroupId = value;
                OnPropertyChanged();
                CurrentPage = 1;
                if (!string.IsNullOrEmpty(value))
                {
                    LoadAccountsByRole(value).GetAwaiter().GetResult();
                }
                else
                {
                    LoadAccountsAsync().GetAwaiter().GetResult();
                }
            }
        }

        public AccountManagementResponseDTO SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                _selectedAccount = value;
                OnPropertyChanged();
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

        public ObservableCollection<object> DisplayPages
        {
            get => _displayPages;
            set
            {
                _displayPages = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand SearchCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand SelectPageCommand { get; }
        public ICommand FilterByTypeCommand { get; }

        public AccountManagementViewModel(
            IAccountManagementService accountManagementService,
            IGroupManagementService groupManagementService,
            INotificationService notificationService,
            INavigationService navigationService)
        {
            _accountManagementService = accountManagementService ?? throw new ArgumentNullException(nameof(accountManagementService));
            _groupManagementService = groupManagementService ?? throw new ArgumentNullException(nameof(groupManagementService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _accountList = new ObservableCollection<AccountManagementResponseDTO>();
            _lstGroupManagement = new Dictionary<string, int>();
            _currentPage = 1;
            _searchText = string.Empty;
            _selectedGroupId = string.Empty;
            _selectedAccount = null!;
            _displayPages = new ObservableCollection<object>();

            SearchCommand = new RelayCommand(async _ => await SearchAccountsAsync());
            AddCommand = new RelayCommand(_ => OpenEditor(null!));
            DeleteCommand = new RelayCommand(async account => await DeleteAccountAsync((AccountManagementResponseDTO)account));
            UpdateCommand = new RelayCommand(account => OpenEditor((AccountManagementResponseDTO)account));
            PreviousPageCommand = new RelayCommand(_ => PreviousPage(), _ => CurrentPage > 1);
            NextPageCommand = new RelayCommand(_ => NextPage(), _ => CurrentPage < TotalPages);
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            FilterByTypeCommand = new RelayCommand(groupId => SelectedGroupId = (string)groupId);

            _=LoadAccountsAsync();
            _=LoadGroupsAsync();
        }

        private async Task LoadAccountsAsync()
        {
            try
            {
                var result = await _accountManagementService.GetAllAccount(CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    AccountList.Clear();
                    foreach (var account in result.Data.Data ?? Enumerable.Empty<AccountManagementResponseDTO>())
                    {
                        AccountList.Add(account);
                    }
                    TotalPages = result.Data.TotalPages;
                    SelectFirstAccount();
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách tài khoản.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadAccountsByRole(string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                await LoadAccountsAsync();
                return;
            }

            try
            {
                var result = await _accountManagementService.GetAllAccountWithRole(groupId, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    AccountList.Clear();
                    foreach (var account in result.Data.Data ?? Enumerable.Empty<AccountManagementResponseDTO>())
                    {
                        AccountList.Add(account);
                    }
                    TotalPages = result.Data.TotalPages;
                    SelectFirstAccount();
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải tài khoản theo vai trò.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task SearchAccountsAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    await LoadAccountsAsync();
                    return;
                }

                var result = await _accountManagementService.SearchAccountInList(SearchText, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    AccountList.Clear();
                    foreach (var account in result.Data.Data ?? Enumerable.Empty<AccountManagementResponseDTO>())
                    {
                        AccountList.Add(account);
                    }
                    TotalPages = result.Data.TotalPages;
                    SelectFirstAccount();
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

        private async Task LoadGroupsAsync()
        {
            try
            {
                var result = await _groupManagementService.GetTotalUserInGroup();
                if (result.Success && result.Data != null)
                {
                    LstGroupManagement.Clear();
                    foreach (var kvp in result.Data)
                    {
                        LstGroupManagement[kvp.Key] = kvp.Value;
                    }
                    OnPropertyChanged(nameof(LstGroupManagement));
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

        private void OpenEditor(AccountManagementResponseDTO account)
        {
            _navigationService.NavigateTo<ucAccountEditor>();
        }

        private async Task DeleteAccountAsync(AccountManagementResponseDTO account)
        {
            if (account == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa tài khoản {account.UserName}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deleteResult = await _accountManagementService.DeleteAccountManagement(account.UserName);
                    if (deleteResult.Success)
                    {
                        AccountList.Remove(account);
                        _notificationService.ShowMessage("Xóa tài khoản thành công!", "OK");
                        SelectFirstAccount();
                    }
                    else
                    {
                        _notificationService.ShowMessage(deleteResult.Message ?? "Lỗi khi xóa tài khoản.", "OK", isError: true);
                    }
                }
                catch (Exception ex)
                {
                    _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
                }
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

        private void SelectFirstAccount()
        {
            foreach (var account in AccountList)
            {
                account.IsSelected = false;
            }

            var firstAccount = AccountList.FirstOrDefault();
            if (firstAccount != null)
            {
                firstAccount.IsSelected = true;
                SelectedAccount = firstAccount;
            }
            else
            {
                SelectedAccount = null!;
            }
        }

        private void UpdateDisplayPages()
        {
            DisplayPages = new ObservableCollection<object>();

            if (TotalPages <= 0) return;

            const int maxPagesToShow = 5; // Số trang tối đa hiển thị (không tính "...")
            if (TotalPages <= maxPagesToShow)
            {
                for (int i = 1; i <= TotalPages; i++)
                {
                    DisplayPages.Add(i);
                }
            }
            else
            {
                DisplayPages.Add(1);
                if (TotalPages > 1) DisplayPages.Add(2);

                if (TotalPages > maxPagesToShow)
                {
                    DisplayPages.Add("...");
                }

                if (TotalPages > 1)
                {
                    DisplayPages.Add(TotalPages);
                }
            }
        }
    }
}