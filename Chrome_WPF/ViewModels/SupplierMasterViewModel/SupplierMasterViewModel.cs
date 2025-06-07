using Chrome_WPF.Helpers;
using Chrome_WPF.Models.SupplierMasterDTO;
using Chrome_WPF.Services.SupplierMasterService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Views.UserControls.SupplierMaster;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.SupplierMasterViewModel
{
    public class SupplierMasterViewModel : BaseViewModel
    {
        private readonly ISupplierMasterService _supplierMasterService;
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private readonly IMessengerService _messengerService;

        private ObservableCollection<SupplierMasterResponseDTO> _supplierList;
        private ObservableCollection<object> _displayPages;
        private string _searchText;
        private int _currentPage;
        private int _pageSize = 10;
        private int _totalPages;
        private SupplierMasterResponseDTO _selectedSupplier;
        private SupplierMasterRequestDTO? _supplierMasterRequestDTO;
        private int _totalSuppliersCount;

        public ObservableCollection<SupplierMasterResponseDTO> SupplierList
        {
            get => _supplierList;
            set
            {
                _supplierList = value;
                OnPropertyChanged();
            }
        }

        public SupplierMasterRequestDTO? SupplierMasterRequestDTO
        {
            get => _supplierMasterRequestDTO!;
            set
            {
                _supplierMasterRequestDTO = value;
                OnPropertyChanged();
            }
        }

        public SupplierMasterResponseDTO SelectedSupplier
        {
            get => _selectedSupplier;
            set
            {
                _selectedSupplier = value;
                OnPropertyChanged();
                if (SelectedSupplier != null)
                {
                    SupplierMasterRequestDTO = new SupplierMasterRequestDTO
                    {
                        SupplierCode = SelectedSupplier.SupplierCode,
                        SupplierName = SelectedSupplier.SupplierName ?? string.Empty,
                        SupplierPhone = SelectedSupplier.SupplierPhone ?? string.Empty,
                        SupplierAddress = SelectedSupplier.SupplierAddress ?? string.Empty
                    };
                }
                else
                {
                    SupplierMasterRequestDTO = new SupplierMasterRequestDTO();
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
                _ = LoadSuppliersAsync();
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
                _ = LoadSuppliersAsync();
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

        public int TotalSuppliersCount
        {
            get => _totalSuppliersCount;
            set
            {
                _totalSuppliersCount = value;
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

        public SupplierMasterViewModel(
            ISupplierMasterService supplierMasterService,
            INotificationService notificationService,
            INavigationService navigationService,
            IMessengerService messengerService)
        {
            _supplierMasterService = supplierMasterService ?? throw new ArgumentException(nameof(supplierMasterService));
            _notificationService = notificationService ?? throw new ArgumentException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentException(nameof(navigationService));
            _messengerService = messengerService ?? throw new ArgumentException(nameof(messengerService));

            _supplierList = new ObservableCollection<SupplierMasterResponseDTO>();
            _displayPages = new ObservableCollection<object>();
            SupplierMasterRequestDTO = new SupplierMasterRequestDTO();
            _selectedSupplier = null!;
            _currentPage = 1;
            _searchText = string.Empty;

            SearchCommand = new RelayCommand(async _ => await SearchSuppliersAsync());
            AddCommand = new RelayCommand(_ => OpenEditor(null));
            DeleteCommand = new RelayCommand(async supplier => await DeleteSupplierAsync((SupplierMasterResponseDTO)supplier));
            UpdateCommand = new RelayCommand(supplier => OpenEditor((SupplierMasterResponseDTO)supplier));
            RefreshCommand = new RelayCommand(async _ => await LoadSuppliersAsync());
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage());
            SelectPageCommand = new RelayCommand(page => SelectPage((int)page));

            _ = messengerService.RegisterMessageAsync("ReloadSupplierListMessage", async (obj) =>
            {
                await LoadSuppliersAsync();
            });

            _ = LoadSuppliersAsync();
        }

        private async Task SearchSuppliersAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    await LoadSuppliersAsync();
                    return;
                }

                var result = await _supplierMasterService.SearchSupplierMaster(SearchText, CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    SupplierList.Clear();
                    foreach (var supplier in result.Data.Data ?? Enumerable.Empty<SupplierMasterResponseDTO>())
                    {
                        SupplierList.Add(supplier);
                    }
                    TotalPages = result.Data.TotalPages;
                    OnPropertyChanged(nameof(SupplierList));
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tìm kiếm nhà cung cấp.", "OK", isError: true);
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

        private async Task DeleteSupplierAsync(SupplierMasterResponseDTO supplier)
        {
            if (supplier == null) return;
            var result = MessageBox.Show($"Bạn có chắc muốn xóa nhà cung cấp {supplier.SupplierName}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deleteResult = await _supplierMasterService.DeleteSupplierMaster(supplier.SupplierCode);
                    if (deleteResult.Success)
                    {
                        _notificationService.ShowMessage($"Đã xóa nhà cung cấp {supplier.SupplierName} thành công.", "OK", isError: false);
                        await LoadSuppliersAsync();
                    }
                    else
                    {
                        _notificationService.ShowMessage(deleteResult.Message ?? "Lỗi khi xóa nhà cung cấp.", "OK", isError: true);
                    }
                }
                catch (Exception ex)
                {
                    _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
                }
            }
        }

        private void OpenEditor(SupplierMasterResponseDTO? supplier)
        {
            var supplierEditor = App.ServiceProvider!.GetRequiredService<ucSupplierEditor>();
            var dto = supplier == null
                ? new SupplierMasterRequestDTO()
                : new SupplierMasterRequestDTO
                {
                    SupplierCode = supplier.SupplierCode,
                    SupplierName = supplier.SupplierName ?? string.Empty,
                    SupplierPhone = supplier.SupplierPhone ?? string.Empty,
                    SupplierAddress = supplier.SupplierAddress ?? string.Empty
                };

            var viewModel = new SupplierEditorViewModel(
                _supplierMasterService,
                _notificationService,
                _navigationService,
                _messengerService,
                isAddingNew: supplier == null,
                initialDto: dto);

            supplierEditor.DataContext = viewModel;
            _navigationService.NavigateTo(supplierEditor);
        }

        private async Task TotalSuppliers()
        {
            try
            {
                var result = await _supplierMasterService.GetTotalSupplierCount();
                if (result.Success)
                {
                    TotalSuppliersCount = result.Data;
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi lấy tổng số nhà cung cấp.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private async Task LoadSuppliersAsync()
        {
            try
            {
                var result = await _supplierMasterService.GetAllSupplierMaster(CurrentPage, PageSize);
                if (result.Success && result.Data != null)
                {
                    SupplierList.Clear();
                    foreach (var supplier in result.Data.Data ?? Enumerable.Empty<SupplierMasterResponseDTO>())
                    {
                        SupplierList.Add(supplier);
                    }
                    TotalPages = result.Data.TotalPages;
                    _ = TotalSuppliers();
                }
                else
                {
                    _notificationService.ShowMessage(result.Message ?? "Lỗi khi tải danh sách nhà cung cấp.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }
    }
}