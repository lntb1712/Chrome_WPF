using Chrome_WPF.Helpers;
using Chrome_WPF.Models.SupplierMasterDTO;
using Chrome_WPF.Services.SupplierMasterService;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Views.UserControls.SupplierMaster;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Chrome_WPF.ViewModels.SupplierMasterViewModel
{
    public class SupplierEditorViewModel : BaseViewModel
    {
        private readonly ISupplierMasterService _supplierMasterService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IMessengerService _messengerService;

        private SupplierMasterRequestDTO? _supplierMasterRequestDTO;
        private bool _isAddingNew;
        private readonly RelayCommand _saveCommand;

        public SupplierMasterRequestDTO SupplierMasterRequestDTO
        {
            get => _supplierMasterRequestDTO!;
            set
            {
                if (_supplierMasterRequestDTO != null)
                {
                    _supplierMasterRequestDTO.PropertyChanged -= OnPropertyChangedHandler;
                }
                _supplierMasterRequestDTO = value ?? new SupplierMasterRequestDTO();
                _supplierMasterRequestDTO.PropertyChanged += OnPropertyChangedHandler;
                OnPropertyChanged(nameof(SupplierMasterRequestDTO));
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

        public SupplierEditorViewModel(
            ISupplierMasterService supplierMasterService,
            INotificationService notificationService,
            INavigationService navigationService,
            IMessengerService messengerService,
            bool isAddingNew = true,
            SupplierMasterRequestDTO? initialDto = null)
        {
            _supplierMasterService = supplierMasterService ?? throw new ArgumentNullException(nameof(supplierMasterService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));

            IsAddingNew = isAddingNew;
            _saveCommand = new RelayCommand(SaveAsync, CanSave);
            SupplierMasterRequestDTO = initialDto ?? new SupplierMasterRequestDTO();
        }

        private void OnPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            _saveCommand.RaiseCanExecuteChanged();
        }

        private bool CanSave(object? _)
        {
            var dto = SupplierMasterRequestDTO;
            var propertiesToValidate = new[]
            {
                nameof(dto.SupplierCode),
                nameof(dto.SupplierName),
                nameof(dto.SupplierPhone),
                nameof(dto.SupplierAddress)
            };

            foreach (var prop in propertiesToValidate)
            {
                if (!string.IsNullOrEmpty(dto[prop]))
                {

                    return false;
                }
            }

            return true;
        }

        private async void SaveAsync(object? _)
        {
            try
            {
                SupplierMasterRequestDTO.RequestValidation();

                if (!CanSave(null))
                {
                    _notificationService.ShowMessage("Vui lòng điền đầy đủ và đúng định dạng thông tin.", "OK", isError: true);
                    return;
                }

                var result = IsAddingNew
                    ? await _supplierMasterService.AddSupplierMaster(SupplierMasterRequestDTO)
                    : await _supplierMasterService.UpdateSupplierMaster(SupplierMasterRequestDTO);

                if (result.Success)
                {
                    _notificationService.ShowMessage(
                        result.Message ?? (IsAddingNew ? "Thêm nhà cung cấp thành công!" : "Cập nhật nhà cung cấp thành công!"),
                        "OK", isError: false);

                    if (IsAddingNew)
                    {
                        SupplierMasterRequestDTO.ClearValidation();
                        SupplierMasterRequestDTO = new SupplierMasterRequestDTO();
                    }

                    await _messengerService.SendMessageAsync("ReloadSupplierListMessage");
                }
                else
                {
                    _notificationService.ShowMessage(
                        result.Message ?? (IsAddingNew ? "Không thể thêm nhà cung cấp." : "Không thể cập nhật nhà cung cấp."),
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