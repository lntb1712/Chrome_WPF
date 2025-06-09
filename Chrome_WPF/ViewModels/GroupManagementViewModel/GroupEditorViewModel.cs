using Chrome_WPF.Helpers;
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
        private ObservableCollection<ApplicableLocationResponseDTO> _lstListBoxLocations;
        private GroupFunctionResponseDTO _selectedGroupFunction;
        private bool _isAddingNew;
        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _selectAllLocationsCommand;

        public ObservableCollection<GroupFunctionResponseDTO> LstGroupFunctions
        {
            get => _lstGroupFunctions;
            private set
            {
                _lstGroupFunctions = value;
                OnPropertyChanged(nameof(LstGroupFunctions));
            }
        }

        public ObservableCollection<ApplicableLocationResponseDTO> LstListBoxLocations
        {
            get => _lstListBoxLocations;
            set
            {
                if (_lstListBoxLocations != null)
                {
                    foreach (var location in _lstListBoxLocations)
                        location.PropertyChanged -= ListBoxLocation_PropertyChanged;
                }

                _lstListBoxLocations = value;

                if (_lstListBoxLocations != null)
                {
                    foreach (var location in _lstListBoxLocations)
                        location.PropertyChanged += ListBoxLocation_PropertyChanged;
                }

                OnPropertyChanged(nameof(LstListBoxLocations));
            }
        }

        public GroupManagementRequestDTO? GroupManagementRequestDTO
        {
            get => _groupManagementRequestDTO;
            set
            {
                if (_groupManagementRequestDTO != null)
                    _groupManagementRequestDTO.PropertyChanged -= OnPropertyChangedHandler;

                _groupManagementRequestDTO = value ?? new GroupManagementRequestDTO();
                _groupManagementRequestDTO.PropertyChanged += OnPropertyChangedHandler;

                OnPropertyChanged(nameof(GroupManagementRequestDTO));
                _ = LoadGroupFunctionsAsync();
            }
        }

        public GroupFunctionResponseDTO SelectedGroupFunction
        {
            get => _selectedGroupFunction;
            set
            {
                _selectedGroupFunction = value;
                OnPropertyChanged(nameof(SelectedGroupFunction));
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
        public RelayCommand SelectAllLocationsCommand => _selectAllLocationsCommand;

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
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));

            IsAddingNew = isAddingNew;
            _selectedGroupFunction = new GroupFunctionResponseDTO();
            _lstGroupFunctions = new ObservableCollection<GroupFunctionResponseDTO>();
            _lstListBoxLocations = new ObservableCollection<ApplicableLocationResponseDTO>();
            _saveCommand = new RelayCommand(async p => await SaveAsync(), CanSave);
            _selectAllLocationsCommand = new RelayCommand(SelectAllLocations);
            GroupManagementRequestDTO = initialDto ?? new GroupManagementRequestDTO();
        }

        private void OnPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            _saveCommand.RaiseCanExecuteChanged();
        }

        private void ListBoxLocation_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ApplicableLocationResponseDTO.IsSelected) && sender is ApplicableLocationResponseDTO location)
            {
                foreach (var function in LstGroupFunctions)
                {
                    var funcLocation = function.ApplicableLocations
                        .FirstOrDefault(l => l.ApplicableLocation == location.ApplicableLocation);

                    if (funcLocation != null)
                    {
                        funcLocation.IsSelected = location.IsSelected;
                    }
                    else if (location.IsSelected)
                    {
                        function.ApplicableLocations.Add(new ApplicableLocationResponseDTO
                        {
                            ApplicableLocation = location.ApplicableLocation,
                            IsSelected = true
                        });
                    }
                }
            }
        }

        private bool CanSave(object? _) =>
            string.IsNullOrWhiteSpace(GroupManagementRequestDTO![nameof(GroupManagementRequestDTO.GroupId)]) &&
            string.IsNullOrWhiteSpace(GroupManagementRequestDTO[nameof(GroupManagementRequestDTO.GroupName)]);

        private void SelectAllLocations(object? _)
        {
            foreach (var location in LstListBoxLocations)
                location.IsSelected = true;
        }

        private async Task LoadGroupFunctionsAsync()
        {
            try
            {
                List<GroupFunctionResponseDTO> groupFunctions = new();

                if (!IsAddingNew && !string.IsNullOrWhiteSpace(GroupManagementRequestDTO?.GroupId))
                {
                    var result = await _groupManagementService.GetGroupFunctionWithGroupID(GroupManagementRequestDTO.GroupId);
                    if (result.Success && result.Data != null)
                        groupFunctions = result.Data;
                }

                if (groupFunctions.Count == 0)
                {
                    var resultFunctionNew = await _groupManagementService.GetAllFunctions();
                    if (resultFunctionNew.Success && resultFunctionNew.Data != null)
                    {
                        groupFunctions = resultFunctionNew.Data
                            .Select(f => new GroupFunctionResponseDTO
                            {
                                GroupId = GroupManagementRequestDTO!.GroupId,
                                FunctionId = f.FunctionId!,
                                FunctionName = f.FunctionName!,
                                IsEnable = f.IsEnable,
                                ApplicableLocations = new ObservableCollection<ApplicableLocationResponseDTO>(
                                    f.ApplicableLocations?.Select(a => new ApplicableLocationResponseDTO
                                    {
                                        ApplicableLocation = a.ApplicableLocation,
                                        IsSelected = a.IsSelected
                                    }) ?? new List<ApplicableLocationResponseDTO>())
                            }).ToList();
                    }
                    else
                    {
                        _notificationService.ShowMessage(resultFunctionNew.Message ?? "Không thể tải danh sách chức năng.", "OK", isError: true);
                        return;
                    }
                }

                var allLocations = groupFunctions
                    .SelectMany(f => f.ApplicableLocations)
                    .GroupBy(l => l.ApplicableLocation)
                    .Select(g => new ApplicableLocationResponseDTO
                    {
                        ApplicableLocation = g.Key,
                        IsSelected = g.Any(l => l.IsSelected)
                    }).ToList();

                LstListBoxLocations = new ObservableCollection<ApplicableLocationResponseDTO>(allLocations);

                LstGroupFunctions.Clear();
                foreach (var function in groupFunctions)
                    LstGroupFunctions.Add(function);
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError:true);
            }
        }

        private async Task SaveAsync()
        {
            try
            {
                GroupManagementRequestDTO!.RequestValidation();
                if (!CanSave(null))
                {
                    _notificationService.ShowMessage("Vui lòng điền đầy đủ thông tin.", "OK", isError: true);
                    return;
                }

                GroupManagementRequestDTO.GroupFunctions = LstGroupFunctions
                    .Select(f => new GroupFunctionResponseDTO
                    {
                        GroupId = GroupManagementRequestDTO.GroupId,
                        FunctionId = f.FunctionId,
                        FunctionName = f.FunctionName,
                        IsEnable = f.IsEnable,
                        ApplicableLocations = new ObservableCollection<ApplicableLocationResponseDTO>(
                            LstListBoxLocations.Select(l => new ApplicableLocationResponseDTO
                            {
                                ApplicableLocation = l.ApplicableLocation,
                                IsSelected = l.IsSelected
                            }))
                    }).ToList();

                var result = IsAddingNew
                    ? await _groupManagementService.AddGroupManagement(GroupManagementRequestDTO)
                    : await _groupManagementService.UpdateGroupManagement(GroupManagementRequestDTO);

                _notificationService.ShowMessage(result.Message ?? (IsAddingNew ? "Thêm nhóm thành công!" : "Cập nhật nhóm thành công!"), "OK", isError:false);

            
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError:true);
            }
        }
    }
}
