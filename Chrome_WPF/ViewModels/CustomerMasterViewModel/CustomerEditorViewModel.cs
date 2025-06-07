using Chrome_WPF.Helpers;
using Chrome_WPF.Models.CustomerMasterDTO;
using Chrome_WPF.Services.CustomerMasterService;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Views.UserControls.CustomerMaster;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Chrome_WPF.ViewModels.CustomerMasterViewModel
{
    public class CustomerEditorViewModel : BaseViewModel
    {
        private readonly ICustomerMasterService _customerMasterService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IMessengerService _messengerService;

        private CustomerMasterRequestDTO? _customerMasterRequestDTO;
        private bool _isAddingNew;
        private readonly RelayCommand _saveCommand;

        public CustomerMasterRequestDTO CustomerMasterRequestDTO
        {
            get => _customerMasterRequestDTO!;
            set
            {
                if (_customerMasterRequestDTO != null)
                {
                    _customerMasterRequestDTO.PropertyChanged -= OnPropertyChangedHandler;
                }
                _customerMasterRequestDTO = value ?? new CustomerMasterRequestDTO();
                _customerMasterRequestDTO.PropertyChanged += OnPropertyChangedHandler;
                OnPropertyChanged(nameof(CustomerMasterRequestDTO));
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

        public CustomerEditorViewModel(
            ICustomerMasterService customerMasterService,
            INotificationService notificationService,
            INavigationService navigationService,
            IMessengerService messengerService,
            bool isAddingNew = true,
            CustomerMasterRequestDTO? initialDto = null)
        {
            _customerMasterService = customerMasterService ?? throw new ArgumentNullException(nameof(customerMasterService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));

            IsAddingNew = isAddingNew;
            _saveCommand = new RelayCommand(SaveAsync, CanSaveCustomer);
            CustomerMasterRequestDTO = initialDto ?? new CustomerMasterRequestDTO();
        }

        private void OnPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            _saveCommand.RaiseCanExecuteChanged();
        }

        private bool CanSaveCustomer(object? _)
        {
            var dto = CustomerMasterRequestDTO;
            var propertiesToValidate = new[]
            {
                nameof(dto.CustomerCode),
                nameof(dto.CustomerName),
                nameof(dto.CustomerPhone),
                nameof(dto.CustomerAddress)
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
                CustomerMasterRequestDTO.RequestValidation();

                if (!CanSaveCustomer(null))
                {
                    _notificationService.ShowMessage("Vui lòng điền đầy đủ và đúng định dạng thông tin.", "OK", isError: true);
                    return;
                }

                var result = IsAddingNew
                    ? await _customerMasterService.AddCustomerMaster(CustomerMasterRequestDTO)
                    : await _customerMasterService.UpdateCustomerMaster(CustomerMasterRequestDTO);

                if (result.Success)
                {
                    _notificationService.ShowMessage(
                        result.Message ?? (IsAddingNew ? "Thêm khách hàng thành công!" : "Cập nhật khách hàng thành công!"),
                        "OK", isError: false);

                    if (IsAddingNew)
                    {
                        CustomerMasterRequestDTO.ClearValidation();
                        CustomerMasterRequestDTO = new CustomerMasterRequestDTO();
                    }

                    await _messengerService.SendMessageAsync("ReloadCustomerListMessage");
                }
                else
                {
                    _notificationService.ShowMessage(
                        result.Message ?? (IsAddingNew ? "Không thể thêm khách hàng." : "Không thể cập nhật khách hàng."),
                        "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        public void NavigateBack()
        {
            var customerMaster = App.ServiceProvider!.GetRequiredService<ucCustomerMaster>();
            _navigationService.NavigateTo(customerMaster);
        }
    }
}