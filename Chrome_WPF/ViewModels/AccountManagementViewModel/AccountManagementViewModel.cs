using Chrome_WPF.Helpers;
using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.GroupManagementDTO;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Services.AccountManagementService;
using Chrome_WPF.Services.GroupManagementService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Views;
using Chrome_WPF.Views.UserControls.AccountManagement;
using ClosedXML.Excel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        private ObservableCollection<GroupManagementTotalDTO> _lstGroupManagement;
        private ObservableCollection<object> _displayPages;
        private string _searchText;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;
        private string _selectedGroupId;
        private AccountManagementResponseDTO _selectedAccount;
        private AccountManagementRequestDTO? _accountManagementRequestDTO;
        private bool _isEditorOpen;
        private int _totalAccountCount;


        public ObservableCollection<AccountManagementResponseDTO> AccountList
        {
            get => _accountList;
            set
            {
                _accountList = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<GroupManagementTotalDTO> LstGroupManagement
        {
            get => _lstGroupManagement;
            set
            {
                _lstGroupManagement = value;
                OnPropertyChanged();
            }
        }
        public AccountManagementRequestDTO AccountManagementRequestDTO
        {
            get => _accountManagementRequestDTO!;
            set
            {
                _accountManagementRequestDTO = value;
                OnPropertyChanged();
            }
        }

        public AccountManagementResponseDTO SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                _selectedAccount = value;
                OnPropertyChanged();
                if (SelectedAccount != null)
                {
                    AccountManagementRequestDTO = new AccountManagementRequestDTO
                    {
                        UserName = SelectedAccount.UserName,
                        FullName = SelectedAccount.FullName,
                        GroupID = SelectedAccount.GroupID,
                        Password = SelectedAccount.Password
                    };
                }
                else
                {
                    AccountManagementRequestDTO = new AccountManagementRequestDTO();
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
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged();
                    UpdateDisplayPages();
                    _ = LoadAccountsAsync();
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
                _ = LoadAccountsAsync();
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
                    _ = LoadAccountsByRole(value);
                }
                else
                {
                    _ = LoadAccountsAsync();
                }
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

        public int TotalAccountCount
        {
            get => _totalAccountCount;
            set
            {
                _totalAccountCount = value;
                OnPropertyChanged();
            }
        }


        // Commands
        public ICommand SearchCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand SelectPageCommand { get; }
        public ICommand FilterByTypeCommand { get; }
        public ICommand ExportAndPreviewCommand { get; }
        public ICommand FilterAllCommand { get; }

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
            _lstGroupManagement = new ObservableCollection<GroupManagementTotalDTO>();
            AccountManagementRequestDTO = new AccountManagementRequestDTO(); // Initialize the field here
            _currentPage = 1;
            _searchText = string.Empty;
            _selectedGroupId = string.Empty;
            _selectedAccount = null!;
            _displayPages = new ObservableCollection<object>();

            SearchCommand = new RelayCommand(async _ => await SearchAccountsAsync());
            RefreshCommand = new RelayCommand(async _ => await LoadAccountsAsync());
            AddCommand = new RelayCommand(_ => OpenEditor(null!));
            DeleteCommand = new RelayCommand(async account => await DeleteAccountAsync((AccountManagementResponseDTO)account), account => account != null);
            UpdateCommand = new RelayCommand(account => OpenEditor((AccountManagementResponseDTO)account));
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));
            FilterByTypeCommand = new RelayCommand(groupId => SelectedGroupId = (string)groupId);
            ExportAndPreviewCommand = new RelayCommand(async p=> await ExportAndPreview(p));
            FilterAllCommand = new RelayCommand(async _ => await LoadAccountsAsync());
            _ = LoadAccountsAsync();
            _ = LoadGroupsAsync();
            _ = GetTotalAccount();
        }

        private async Task GetTotalAccount()
        {
            try
            {
                var result = await _accountManagementService.GetTotalAccount();
                if (result.Success)
                {
                    TotalAccountCount = result.Data;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi lấy tổng số tài khoản.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
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
                    _ = GetTotalAccount();
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
                    foreach (var account in result.Data.Data)
                    {
                        AccountList.Add(account);
                    }
                    TotalPages = result.Data.TotalPages;
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
                    // Đảm bảo DataGrid cập nhật ngay lập tức
                    OnPropertyChanged(nameof(AccountList));
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
                        LstGroupManagement.Add(kvp);
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
            var accountEditor = App.ServiceProvider!.GetRequiredService<ucAccountEditor>();
            // Truyền isAddingNew dựa trên account có null hay không
            accountEditor.DataContext = new AccountEditorViewModel(
                _accountManagementService,
                _groupManagementService,
                _notificationService,
                _navigationService,
                isAddingNew: account == null) // Nếu account null thì isAddingNew = true
            {
                AccountManagementRequestDTO = account == null ? new AccountManagementRequestDTO() : new AccountManagementRequestDTO
                {
                    UserName = account.UserName,
                    Password = account.Password,
                    FullName = account.FullName,
                    GroupID = account.GroupID,
                    UpdateBy = account.UpdateBy
                }
            };
            _navigationService.NavigateTo(accountEditor);

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
                        _=LoadAccountsAsync();
                        _notificationService.ShowMessage("Xóa tài khoản thành công!", "OK");
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
        private async Task ExportAndPreview(object parameter)
        {
            try
            {
                // Path to the template Excel file
                string templatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "BaoCaoNhanSu.xlsx");

                // Path to save the exported file on the Desktop
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string exportPath = Path.Combine(desktopPath, $"BaoCaoNhanSu_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
                var response = await _groupManagementService.GetGroupManagementWithGroupID(Properties.Settings.Default.Role);
                // Load the Excel template
                using (var workbook = new XLWorkbook(templatePath))
                {
                    var worksheet = workbook.Worksheet(1); // Assuming data is in the first worksheet

                    // Define the starting row for data (based on the template, data starts at row 8)
                    int startRow = 10;

                    // Populate the header fields (if needed)
                    worksheet.Cell(3, 7).Value = $"Ngày {DateTime.Now.Day} Tháng {DateTime.Now.Month} Năm {DateTime.Now.Year}";
                    worksheet.Cell(4, 5).Value = Properties.Settings.Default.FullName; // Replace with actual data
                    worksheet.Cell(5, 5).Value = response.Data!.GroupName; // Replace with actual data
                    worksheet.Cell(6, 5).Value = $"Báo cáo nhân sự tháng {DateTime.Now.Month}/{DateTime.Now.Year}"; // Replace with actual data

                    // Populate the data from AccountList
                    int row = startRow;
                    int stt = 1;

                    foreach (var account in AccountList)
                    {
                        worksheet.Cell(row, 2).Value = stt; // STT
                        worksheet.Cell(row, 3).Value = account.UserName.ToString(); // Mã nhân viên
                        worksheet.Cell(row, 4).Value = account.FullName.ToString(); // Họ và tên
                        worksheet.Cell(row, 5).Value = LstGroupManagement.FirstOrDefault(g => g.GroupID == account.GroupID)?.GroupName ?? ""; // Nhóm người dùng
                        worksheet.Cell(row, 6).Value = account.UpdateTime.ToString() ?? ""; // Ngày cập nhật
                        worksheet.Cell(row, 7).Value = account.UpdateBy.ToString() ?? ""; // Người cập nhật
                        worksheet.Cell(row, 8).Value = ""; // Ghi chú (empty as per template)
                        row++;
                        stt++;
                    }

                    // Save the workbook to the Desktop
                    workbook.SaveAs(exportPath);

                    // Notify user of success
                    _notificationService.ShowMessage($"Xuất báo cáo thành công! File được lưu tại: {exportPath}", "OK", isError: false);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi khi xuất báo cáo: {ex.Message}", "OK", isError: true);
            }
        }
    }
}