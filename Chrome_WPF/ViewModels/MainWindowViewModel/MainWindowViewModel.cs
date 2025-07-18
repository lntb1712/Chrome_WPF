using Chrome_WPF.Helpers;
using Chrome_WPF.Services.BOMComponentService;
using Chrome_WPF.Services.InventoryService;
using Chrome_WPF.Services.ReplenishService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.MainWindowViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly AuthViewModel _authViewModel;
        private readonly IReplenishService _replenishService;
        private readonly IBOMComponentService _bOMComponentService;
        private readonly IInventoryService _inventoryService;
        private List<string> _replenishWarnings = new List<string>();
        private int _warningCount;

        private string _selectedWarning;
        public string SelectedWarning
        {
            get => _selectedWarning;
            set
            {
                _selectedWarning = value;
                OnPropertyChanged(nameof(SelectedWarning));
            }
        }
        public List<string> ReplenishWarnings
        {
            get => _replenishWarnings;
            set
            {
                _replenishWarnings = value;
                OnPropertyChanged(nameof(ReplenishWarnings));
            }
        }

        public int WarningCount
        {
            get => _warningCount;
            set
            {
                _warningCount = value;
                OnPropertyChanged(nameof(WarningCount));
            }
        }
        public ICommand CreateReplenishCommand { get;  }
        public MainWindowViewModel(AuthViewModel authViewModel, IReplenishService replenishService , IBOMComponentService bOMComponentService , IInventoryService inventoryService)
        {
            _authViewModel = authViewModel ?? throw new ArgumentNullException(nameof(authViewModel));
            _replenishService = replenishService ?? throw new ArgumentNullException(nameof(replenishService));
            _bOMComponentService = bOMComponentService ?? throw new ArgumentNullException(nameof(bOMComponentService));
            _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
            _selectedWarning = "";
            CreateReplenishCommand = new RelayCommand(async p => await ExecuteCreateReplenishmentOrderAsync(p), CanExecuteCreateReplenishmentOrder);
        }

        public async Task InitializeAsync()
        {
            if (_authViewModel.CanReplenish)
            {
                var warehousePermissions = Properties.Settings.Default.WarehousePermission?.Cast<string>().ToList();
                await CheckReplenishWarnings(warehousePermissions!);
            }
        }

        private async Task CheckReplenishWarnings(List<string> warehousePermissions)
        {
            if (warehousePermissions == null || !warehousePermissions.Any())
            {
                MessageBox.Show("Không có quyền truy cập kho nào.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var allWarnings = new List<string>();
            var errorMessages = new List<string>();

            foreach (var warehouseCode in warehousePermissions)
            {
                var response = await _replenishService.CheckReplenishWarningsAsync(warehouseCode);
                if (response.Success && response.Data != null && response.Data.Any())
                {
                    allWarnings.AddRange(response.Data);
                }
                else if (!response.Success)
                {
                    errorMessages.Add($"Kho {warehouseCode}: {response.Message}");
                }
            }

            ReplenishWarnings = allWarnings;
            WarningCount = allWarnings.Count;

            if (allWarnings.Any())
            {
                var warningMessage = "Cảnh báo bổ sung hàng:\n" + string.Join("\n", allWarnings);
                MessageBox.Show(warningMessage, "Cảnh báo bổ sung hàng", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            if (errorMessages.Any())
            {
                var errorMessage = "Lỗi khi kiểm tra bổ sung hàng:\n\n" + string.Join("\n", errorMessages);
                MessageBox.Show(errorMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool CanExecuteCreateReplenishmentOrder(object parameter)
        {
            return !string.IsNullOrEmpty(SelectedWarning);
        }
        private Task ExecuteCreateReplenishmentOrderAsync(object parameter)
        {
            if (SelectedWarning != null)
            {
                // Extract product code from the warning string
                string productCode = ExtractProductCode(SelectedWarning);
                if (string.IsNullOrEmpty(productCode))
                {
                    MessageBox.Show("Không thể xác định mã sản phẩm từ cảnh báo.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return Task.CompletedTask;
                }

                //if (productCode.StartsWith("SFG"))
                //{
                    
                //    var 

                //}
            }

            return Task.CompletedTask;
        }
        private string ExtractProductCode(string warning)
        {
            try
            {
                var match = Regex.Match(warning, @"Mã:\s*(\w+)");
                return match.Success ? match.Groups[1].Value : null!;
            }
            catch
            {
                return null!;
            }
        }
    }
}
