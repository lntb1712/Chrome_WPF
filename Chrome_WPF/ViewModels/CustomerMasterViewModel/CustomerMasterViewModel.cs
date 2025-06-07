using Chrome_WPF.Helpers;
using Chrome_WPF.Models.CustomerMasterDTO;
using Chrome_WPF.Services.CustomerMasterService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Views.UserControls.CustomerMaster;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.CustomerMasterViewModel
{
    public class CustomerMasterViewModel : BaseViewModel
    {
        private readonly ICustomerMasterService _customerMasterService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IMessengerService _messengerService;

        private ObservableCollection<CustomerMasterResponseDTO> _customerList;
        private ObservableCollection<object> _displayPages;
        private string _searchText;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;
        private CustomerMasterResponseDTO _selectedCustomer;
        private CustomerMasterRequestDTO? _customerMasterRequestDTO;
        private int _totalCustomersCount;

        public ObservableCollection<CustomerMasterResponseDTO> CustomerList
        {
            get => _customerList;
            set
            {
                _customerList = value;
                OnPropertyChanged();
            }
        }

        public CustomerMasterRequestDTO CustomerMasterRequestDTO
        {
            get => _customerMasterRequestDTO!;
            set
            {
                _customerMasterRequestDTO = value;
                OnPropertyChanged();
            }
        }

        public CustomerMasterResponseDTO? SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value!;
                OnPropertyChanged();
                if (SelectedCustomer != null)
                {
                    CustomerMasterRequestDTO = new CustomerMasterRequestDTO
                    {
                        CustomerCode = SelectedCustomer.CustomerCode,
                        CustomerName = SelectedCustomer.CustomerName ?? string.Empty,
                        CustomerPhone = SelectedCustomer.CustomerPhone ?? string.Empty,
                        CustomerAddress = SelectedCustomer.CustomerAddress ?? string.Empty,
                        CustomerEmail = SelectedCustomer.CustomerEmail ?? string.Empty
                    };
                }
                else
                {
                    CustomerMasterRequestDTO = new CustomerMasterRequestDTO();
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
                _ = LoadCustomersAsync();
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
                _ = LoadCustomersAsync();
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

        public int TotalCustomersCount
        {
            get => _totalCustomersCount;
            set
            {
                _totalCustomersCount = value;
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

        public CustomerMasterViewModel(
            ICustomerMasterService customerMasterService,
            INotificationService notificationService,
            INavigationService navigationService,
            IMessengerService messengerService)
        {
            _customerMasterService = customerMasterService ?? throw new ArgumentException(nameof(customerMasterService));
            _notificationService = notificationService ?? throw new ArgumentException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentException(nameof(messengerService));

            _customerList = new ObservableCollection<CustomerMasterResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            CustomerMasterRequestDTO = new CustomerMasterRequestDTO();
            _selectedCustomer = null!;
            _currentPage = 1;
            _searchText = string.Empty;

            SearchCommand = new RelayCommand(async _ => await SearchCustomersAsync());
            AddCommand = new RelayCommand(_ => OpenEditor(null));
            DeleteCommand = new RelayCommand(async customer => await DeleteCustomerAsync((CustomerMasterResponseDTO)customer));
            UpdateCommand = new RelayCommand(customer => OpenEditor((CustomerMasterResponseDTO)customer));
            RefreshCommand = new RelayCommand(async _ => await LoadCustomersAsync());
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));

            _ = messengerService.RegisterMessageAsync("ReloadCustomerListMessage", async (obj) =>
            {
                await LoadCustomersAsync();
            });

            _ = LoadCustomersAsync();
        }

        private async Task SearchCustomersAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    await LoadCustomersAsync();
                    return;
                }

                var result = await _customerMasterService.SearchCustomerMaster(SearchText, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    CustomerList.Clear();
                    foreach (var customer in result.Data.Data ?? Enumerable.Empty<CustomerMasterResponseDTO>())
                    {
                        CustomerList.Add(customer);
                    }
                    TotalPages = result.Data.TotalPages;
                    OnPropertyChanged(nameof(CustomerList));
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tìm kiếm khách hàng.", "OK", isError: true);
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

        private async Task DeleteCustomerAsync(CustomerMasterResponseDTO customer)
        {
            if (customer == null) return;
            var result = MessageBox.Show($"Bạn có chắc muốn xóa khách hàng {customer.CustomerName}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deleteResult = await _customerMasterService.DeleteCustomerMaster(customer.CustomerCode);
                    if (deleteResult.Success)
                    {
                        _notificationService.ShowMessage($"Đã xóa khách hàng {customer.CustomerName} thành công.", "OK", isError: false);
                        await LoadCustomersAsync();
                    }
                    else
                    {
                        _notificationService.ShowMessage(deleteResult.Message ?? "Lỗi khi xóa khách hàng.", "OK", isError: true);
                    }
                }
                catch (Exception ex)
                {
                    _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
                }
            }
        }

        private void OpenEditor(CustomerMasterResponseDTO? customer)
        {
            var customerEditor = App.ServiceProvider!.GetRequiredService<ucCustomerEditor>();
            var dto = customer == null
                ? new CustomerMasterRequestDTO()
                : new CustomerMasterRequestDTO
                {
                    CustomerCode = customer.CustomerCode,
                    CustomerName = customer.CustomerName ?? string.Empty,
                    CustomerPhone = customer.CustomerPhone ?? string.Empty,
                    CustomerAddress = customer.CustomerAddress ?? string.Empty,
                    CustomerEmail = customer.CustomerEmail ?? string.Empty
                };

            var viewModel = new CustomerEditorViewModel(
                _customerMasterService,
                _notificationService,
                _navigationService,
                _messengerService,
                isAddingNew: customer == null,
                initialDto: dto);

            customerEditor.DataContext = viewModel;
            _navigationService.NavigateTo(customerEditor);
        }

        private async Task TotalCustomers()
        {
            try
            {
                var result = await _customerMasterService.GetTotalCustomerCount();
                if (result.Success)
                {
                    TotalCustomersCount = result.Data;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi lấy tổng số khách hàng.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadCustomersAsync()
        {
            try
            {
                var result = await _customerMasterService.GetAllCustomerMaster(CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    CustomerList.Clear();
                    foreach (var customer in result.Data.Data ?? Enumerable.Empty<CustomerMasterResponseDTO>())
                    {
                        CustomerList.Add(customer);
                    }
                    TotalPages = result.Data.TotalPages;
                    _ = TotalCustomers();
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách khách hàng.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }
    }
}