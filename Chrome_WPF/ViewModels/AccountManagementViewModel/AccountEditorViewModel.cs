using Chrome_WPF.Helpers;
using Chrome_WPF.Models;
using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.GroupManagementDTO;
using Chrome_WPF.Services.AccountManagementService;
using Chrome_WPF.Services.GroupManagementService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Views.UserControls;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels
{
    public class AccountEditorViewModel : BaseViewModel
    {
        private readonly IAccountManagementService _accountManagementService;
        private readonly IGroupManagementService _groupManagementService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private AccountManagementRequestDTO _accountManagementRequestDTO;
        private ObservableCollection<GroupManagementResponseDTO> _lstGroups;
        private GroupManagementResponseDTO _selectedGroup;
        private bool _isAddingNew;
        private readonly RelayCommand _saveCommand;
        private string _passwordError;
        public string PasswordError
        {
            get => _passwordError;
            set
            {
                _passwordError = value;
                OnPropertyChanged(nameof(PasswordError));
            }
        }
        public GroupManagementResponseDTO SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                _selectedGroup = value;
                // Cập nhật GroupID khi SelectedGroup thay đổi
                if (_accountManagementRequestDTO != null)
                {
                    _accountManagementRequestDTO.GroupID = value?.GroupId!;
                }
                OnPropertyChanged(nameof(SelectedGroup));
                OnPropertyChanged(nameof(AccountManagementRequestDTO)); // Kích hoạt validate
            }
        }


        public AccountManagementRequestDTO AccountManagementRequestDTO
        {
            get => _accountManagementRequestDTO;
            set
            {
                if (_accountManagementRequestDTO != null)
                {

                    _accountManagementRequestDTO.PropertyChanged -= OnPropertyChangedHandler!;
                }
                _accountManagementRequestDTO = value;
                _accountManagementRequestDTO.PropertyChanged += OnPropertyChangedHandler!;
                // Cập nhật SelectedGroup dựa trên GroupID
                UpdateSelectedGroup();
                OnPropertyChanged(nameof(AccountManagementRequestDTO));
            }
        }
        // Đồng bộ SelectedGroup với GroupID
        private void UpdateSelectedGroup()
        {
            if (_accountManagementRequestDTO != null && LstGroups != null)
            {
                _selectedGroup = LstGroups.FirstOrDefault(g => g.GroupId == _accountManagementRequestDTO.GroupID)!;
                OnPropertyChanged(nameof(SelectedGroup));
            }
        }
        public ObservableCollection<GroupManagementResponseDTO> LstGroups
        {
            get => _lstGroups;
            set
            {
                _lstGroups = value;
                OnPropertyChanged(nameof(LstGroups));
            }
        }

        public bool IsAddingNew
        {
            get => _isAddingNew;
            set
            {
                _isAddingNew = value;
                OnPropertyChanged(nameof(IsAddingNew));
            }
        }

        public ICommand SaveCommand => _saveCommand;

        public AccountEditorViewModel(
            IAccountManagementService accountManagementService,
            IGroupManagementService groupManagementService,
            INotificationService notificationService,
            INavigationService navigationService,
            bool isAddingNew = true)
        {
            _accountManagementService = accountManagementService ?? throw new ArgumentNullException(nameof(accountManagementService));
            _groupManagementService = groupManagementService ?? throw new ArgumentNullException(nameof(groupManagementService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentException(nameof(navigationService));
            _accountManagementRequestDTO = new AccountManagementRequestDTO();
            _lstGroups = new ObservableCollection<GroupManagementResponseDTO>();
            _selectedGroup = null!;
            _isAddingNew = isAddingNew;
            _passwordError = string.Empty;

            _saveCommand = new RelayCommand(Save, CanSave);
            _accountManagementRequestDTO.PropertyChanged += OnPropertyChangedHandler!;

            _ = LoadGroupsAsync();
        }

        private async Task LoadGroupsAsync()
        {
            try
            {
                var result = await _groupManagementService.GetAllGroupManagement(1, int.MaxValue);
                if (result.Success && result.Data != null)
                {
                    LstGroups.Clear();
                    foreach (var group in result.Data.Data)
                    {
                        LstGroups.Add(group);
                    } 
                   
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách nhóm người dùng", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async void Save(object parameter)
        {
            try
            {
                AccountManagementRequestDTO.UpdateBy = Properties.Settings.Default.FullName ?? string.Empty;
                AccountManagementRequestDTO.RequestValidation();
                if (!CanSave(parameter))
                {
                    _notificationService.ShowMessage("Vui lòng kiểm tra lại thông tin nhập vào.", "OK", isError: true);
                    return;
                }

                ApiResult<bool> result;
                if (IsAddingNew)
                {
                    result = await _accountManagementService.AddAccountManagement(AccountManagementRequestDTO);
                }
                else
                {
                    result = await _accountManagementService.UpdateAccountManagement(AccountManagementRequestDTO);
                }

                if (result.Success)
                {
                    _notificationService.ShowMessage(result.Message ?? (IsAddingNew ? "Thêm tài khoản thành công!" : "Cập nhật tài khoản thành công!"), "OK", isError: false);
                    if (IsAddingNew)
                    {
                        AccountManagementRequestDTO.ClearValidation();
                        AccountManagementRequestDTO = new AccountManagementRequestDTO();
                        SelectedGroup = null!;
                    }
                    var ucAccountManagement = App.ServiceProvider!.GetRequiredService<ucAccountManagement>();
                    _navigationService.NavigateTo<ucAccountManagement>();
                    
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? (IsAddingNew ? "Lỗi khi thêm tài khoản." : "Lỗi khi cập nhật tài khoản."), "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private bool CanSave(object parameter)
        {
            var dto = AccountManagementRequestDTO;
            var propertiesToValidate = new[] { nameof(dto.UserName), nameof(dto.Password), nameof(dto.FullName), nameof(dto.GroupID) };

            foreach (var prop in propertiesToValidate)
            {
                if (!string.IsNullOrEmpty(dto[prop]) )
                {
                    
                    return false;
                }
            }

            return true;
        }

        private void OnPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            _saveCommand.RaiseCanExecuteChanged();
        }
    }
}