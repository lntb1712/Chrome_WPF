using Chrome_WPF.Models.LocationMasterDTO;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Models.StorageProductDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.PutAwayRulesDTO
{
    public class PutAwayRulesResponseDTO : INotifyPropertyChanged
    {
        private string _putAwayRuleCode = string.Empty;
        private string? _warehouseToApply = string.Empty;
        private string? _warehouseToApplyName = string.Empty;
        private string? _productCode = string.Empty;
        private string? _productName = string.Empty;
        private string? _locationCode = string.Empty;
        private string? _locationName = string.Empty;
        private string? _storageProductId = string.Empty;
        private string? _storageProductName = string.Empty;

        private bool _isNewRow;
        private WarehouseMasterResponseDTO? _selectedWarehouse;
        private LocationMasterResponseDTO? _selectedLocation;
        private ProductMasterResponseDTO? _selectedProduct;
        private StorageProductResponseDTO? _selectedStorageProduct;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string PutAwayRuleCode
        {
            get => _putAwayRuleCode;
            set
            {
                _putAwayRuleCode = value;
                OnPropertyChanged(nameof(PutAwayRuleCode));
            }
        }
        public string? WarehouseToApply
        {
            get => _warehouseToApply;
            set
            {
                _warehouseToApply = value;
                OnPropertyChanged(nameof(WarehouseToApply));
            }
        }
        public string? WarehouseToApplyName
        {
            get => _warehouseToApplyName;
            set
            {
                _warehouseToApplyName = value;
                OnPropertyChanged(nameof(WarehouseToApplyName));
            }
        }
        public string? ProductCode
        {
            get => _productCode;
            set
            {
                _productCode = value;
                OnPropertyChanged(nameof(ProductCode));
            }
        }
        public string? ProductName
        {
            get => _productName;
            set
            {
                _productName = value;
                OnPropertyChanged(nameof(ProductName));
            }
        }
        [JsonProperty("LocationCode")]
        public string? LocationCode
        {
            get => _locationCode;
            set
            {
                _locationCode = value;
                OnPropertyChanged(nameof(LocationCode));
            }
        }
        [JsonProperty("LocationName")]
        public string? LocationName
        {
            get => _locationName;
            set
            {
                _locationName = value;
                OnPropertyChanged(nameof(LocationName));
            }
        }
        public string? StorageProductId
        {
            get => _storageProductId;
            set
            {
                _storageProductId = value;
                OnPropertyChanged(nameof(StorageProductId));
            }
        }
        public string? StorageProductName
        {
            get => _storageProductName;
            set
            {
                _storageProductName = value;
                OnPropertyChanged(nameof(StorageProductName));
            }
        }
        public bool IsNewRow
        {
            get => _isNewRow;
            set { _isNewRow = value; OnPropertyChanged(nameof(IsNewRow)); }
        }
        public WarehouseMasterResponseDTO? SelectedWarehouse
        {
            get => _selectedWarehouse;
            set
            {
                _selectedWarehouse = value;
                OnPropertyChanged(nameof(SelectedWarehouse));
                WarehouseToApply = value?.WarehouseCode ?? string.Empty;
                WarehouseToApplyName = value?.WarehouseName ?? string.Empty;
            }
        }
        public LocationMasterResponseDTO? SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                _selectedLocation = value;
                OnPropertyChanged(nameof(SelectedLocation));
                LocationCode = value?.LocationCode ?? string.Empty;
                LocationName = value?.LocationName ?? string.Empty;
            }
        }
        // Thêm thuộc tính mới
        public ObservableCollection<LocationMasterResponseDTO> AvailableLocations { get; set; } = new ObservableCollection<LocationMasterResponseDTO>();
        public ProductMasterResponseDTO? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged(nameof(SelectedProduct));
                ProductCode = value?.ProductCode ?? string.Empty;
                ProductName = value?.ProductName ?? string.Empty;
            }
        }
        public StorageProductResponseDTO? SelectedStorageProduct
        {
            get => _selectedStorageProduct;
            set
            {
                _selectedStorageProduct = value;
                OnPropertyChanged(nameof(SelectedStorageProduct));
                StorageProductId = value?.StorageProductId ?? string.Empty;
                StorageProductName = value?.StorageProductName ?? string.Empty;
            }
        }
    }
}
