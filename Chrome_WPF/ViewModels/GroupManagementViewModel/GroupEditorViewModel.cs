using Chrome_WPF.Helpers;
using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.FunctionDTO;
using Chrome_WPF.Models.GroupFunctionDTO;
using Chrome_WPF.Models.GroupManagementDTO;
using Chrome_WPF.Properties;
using Chrome_WPF.Services.GroupManagementService;
using Chrome_WPF.Services.MessengerService;
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
        private readonly IMessengerService _messengerService;

        private ObservableCollection<GroupFunctionResponseDTO> _lstGroupFunctions;
        private GroupManagementRequestDTO? _groupManagementRequestDTO;
        private ObservableCollection<ApplicableLocationResponseDTO> _lstApplicableLocations;
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
        public ObservableCollection<ApplicableLocationResponseDTO> LstApplicableLocations
        {
            get => _lstApplicableLocations;
            set
            {
                _lstApplicableLocations = value;
                OnPropertyChanged(nameof(LstApplicableLocations));
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
            IMessengerService messengerService,
            bool isAddingNew = true,
            GroupManagementRequestDTO? initialDto = null)
        {
            _groupManagementService = groupManagementService ?? throw new ArgumentNullException(nameof(groupManagementService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentException(nameof(messengerService));
            
            IsAddingNew = isAddingNew;
            _lstGroupFunctions = new ObservableCollection<GroupFunctionResponseDTO>();
            _lstApplicableLocations = new ObservableCollection<ApplicableLocationResponseDTO>();
            _saveCommand = new RelayCommand(SaveAsync, CanSave);
            // Khởi tạo DTO và tải dữ liệu
            GroupManagementRequestDTO = initialDto ?? new GroupManagementRequestDTO();
            _ = LoadApplicableLocationsAsync();
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

        private async Task LoadApplicableLocationsAsync()
        {
            try
            {
                var result = await _groupManagementService.GetListApplicableSelected();
                if (result.Success && result.Data != null)
                {
                    LstApplicableLocations.Clear();
                    foreach (var location in result.Data)
                    {
                        LstApplicableLocations.Add(location);
                    }
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải danh sách vị trí áp dụng.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadGroupFunctionsAsync()
        {
            try
            {
                List<GroupFunctionResponseDTO> groupFunctions = new List<GroupFunctionResponseDTO>();

                // Lấy danh sách ApplicableLocations từ service
                var applicableLocationsResult = await _groupManagementService.GetListApplicableSelected();
                var applicableLocations = applicableLocationsResult.Success && applicableLocationsResult.Data != null
                    ? applicableLocationsResult.Data
                    : new List<ApplicableLocationResponseDTO>();

                if (!IsAddingNew && !string.IsNullOrEmpty(GroupManagementRequestDTO.GroupId))
                {
                    var result = await _groupManagementService.GetGroupFunctionWithGroupID(GroupManagementRequestDTO.GroupId);

                    if (result.Success && result.Data != null && result.Data.Count > 0)
                    {
                        groupFunctions = result.Data;
                    }
                }

                // Nếu không có groupFunctions hoặc đang thêm mới
                if (groupFunctions.Count == 0)
                {
                    var resultFunctionNew = await _groupManagementService.GetAllFunctions();

                    if (resultFunctionNew.Success && resultFunctionNew.Data != null)
                    {
                        groupFunctions = resultFunctionNew.Data
                            .Select(f => new GroupFunctionResponseDTO
                            {
                                FunctionId = f.FunctionId!,
                                FunctionName = f.FunctionName!,
                                IsEnable = f.IsEnable,
                                LstApplicableLocations = new ObservableCollection<ApplicableLocationResponseDTO>(applicableLocations)
                            })
                            .ToList();
                    }
                    else
                    {
                        _notificationService.ShowMessage(resultFunctionNew.Message ?? "Không thể tải danh sách chức năng.", "OK", isError: true);
                        return;
                    }
                }
                else
                {
                    // Gán LstApplicableLocations cho các groupFunctions hiện có
                    foreach (var function in groupFunctions)
                    {
                        function.LstApplicableLocations = new ObservableCollection<ApplicableLocationResponseDTO>(applicableLocations);
                    }
                }

                // Đưa dữ liệu vào danh sách hiển thị
                LstGroupFunctions.Clear();
                foreach (var function in groupFunctions)
                {
                    LstGroupFunctions.Add(function);
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

                    await _messengerService.SendMessageAsync("ReloadGroupListMessage");

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