using Chrome_WPF.Helpers;
using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.GroupFunctionDTO;
using Chrome_WPF.Models.GroupManagementDTO;
using Chrome_WPF.Properties;
using Chrome_WPF.Services.GroupManagementService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Views.UserControls.GroupManagement;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Chrome_WPF.ViewModels
{
    public class GroupEditorViewModel : BaseViewModel
    {
        private readonly IGroupManagementService _groupManagementService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private ObservableCollection<GroupFunctionResponseDTO> _lstGroupFunctions;
        private GroupManagementRequestDTO? _groupManagementRequestDTO;
        private bool _isAddingNew;
        private readonly RelayCommand _saveCommand;

        public ObservableCollection<GroupFunctionResponseDTO> LstGroupFunctions
        {
            get => _lstGroupFunctions;
            private set
            {
                _lstGroupFunctions = value;
                OnPropertyChanged(nameof(LstGroupFunctions));
            }
        }

        public GroupManagementRequestDTO GroupManagementRequestDTO
        {
            get => _groupManagementRequestDTO!;
            set
            {
                if (_groupManagementRequestDTO != null)
                {
                    _groupManagementRequestDTO.PropertyChanged -= OnPropertyChangedHandler;
                }
                _groupManagementRequestDTO = value ?? new GroupManagementRequestDTO();
                _groupManagementRequestDTO.PropertyChanged += OnPropertyChangedHandler;
                OnPropertyChanged(nameof(GroupManagementRequestDTO));
                // Tải danh sách chức năng khi GroupManagementRequestDTO được gán
                _ = LoadGroupFunctionsAsync();
            }
        }

        public bool IsAddingNew
        {
            get => _isAddingNew;
            private set
            {
                _isAddingNew = value;
                OnPropertyChanged(nameof(IsAddingNew));
            }
        }

        public RelayCommand SaveCommand => _saveCommand;

        public GroupEditorViewModel(
            IGroupManagementService groupManagementService,
            INotificationService notificationService,
            INavigationService navigationService,
            bool isAddingNew = true,
            GroupManagementRequestDTO? initialDto = null)
        {
            _groupManagementService = groupManagementService ?? throw new ArgumentNullException(nameof(groupManagementService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            IsAddingNew = isAddingNew;
            _lstGroupFunctions = new ObservableCollection<GroupFunctionResponseDTO>();
            _saveCommand = new RelayCommand(SaveAsync, CanSave);
            // Khởi tạo DTO và tải dữ liệu
            GroupManagementRequestDTO = initialDto ?? new GroupManagementRequestDTO();
        }

        private void OnPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            _saveCommand.RaiseCanExecuteChanged();
        }

        private bool CanSave(object? _)
        {
            var dto = GroupManagementRequestDTO;
            var propertiesToValidate = new[] { nameof(dto.GroupId), nameof(dto.GroupName)};

            foreach (var prop in propertiesToValidate)
            {
                if (!string.IsNullOrEmpty(dto[prop]))
                {

                    return false;
                }
            }

            return true;
        }

        private async Task LoadGroupFunctionsAsync()
        {
            try
            {
                ApiResult<List<GroupFunctionResponseDTO>> result;

                // Kiểm tra điều kiện để gọi API phù hợp
                if (IsAddingNew && string.IsNullOrEmpty(GroupManagementRequestDTO.GroupId))
                {
                    result = await _groupManagementService.GetAllGroupFunction();
                }
                else
                {
                    result = await _groupManagementService.GetGroupFunctionWithGroupID(GroupManagementRequestDTO.GroupId);
                }

                // Nếu danh sách trống hoặc API không thành công, thử lấy tất cả chức năng
                if (!result.Success || result.Data == null || result.Data.Count == 0)
                {
                    result = await _groupManagementService.GetAllGroupFunction();
                }

                if (result.Success && result.Data != null)
                {
                    LstGroupFunctions.Clear(); // Xóa dữ liệu cũ để tránh bộ nhớ đệm
                    foreach (var function in result.Data)
                    {
                        LstGroupFunctions.Add(function);
                    }
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải danh sách chức năng.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async void SaveAsync(object? _)
        {
            try
            {
                GroupManagementRequestDTO.UpdateBy = Settings.Default.FullName ?? string.Empty;
                GroupManagementRequestDTO.RequestValidation();

                if (!CanSave(null))
                {
                    _notificationService.ShowMessage("Vui lòng điền đầy đủ thông tin.", "OK", isError: true);
                    return;
                }

                GroupManagementRequestDTO.GroupFunctions = LstGroupFunctions.ToList();
                var result = IsAddingNew
                    ? await _groupManagementService.AddGroupManagement(GroupManagementRequestDTO)
                    : await _groupManagementService.UpdateGroupManagement(GroupManagementRequestDTO);

                if (result.Success)
                {
                    _notificationService.ShowMessage(
                        result.Message ?? (IsAddingNew ? "Thêm nhóm thành công!" : "Cập nhật nhóm thành công!"),
                        "OK", isError: false);

                    if (IsAddingNew)
                    {
                        GroupManagementRequestDTO.ClearValidation();
                        GroupManagementRequestDTO = new GroupManagementRequestDTO();
                        LstGroupFunctions.Clear();
                    }

                    var ucGroupManagement = App.ServiceProvider!.GetRequiredService<ucGroupManagement>();
                    _navigationService.NavigateTo(ucGroupManagement);
                }
                else
                {
                    _notificationService.ShowMessage(
                        result.Message ?? (IsAddingNew ? "Không thể thêm nhóm." : "Không thể cập nhật nhóm."),
                        "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }
    }
}