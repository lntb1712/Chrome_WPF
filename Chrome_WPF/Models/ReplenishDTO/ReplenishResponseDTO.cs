using Newtonsoft.Json;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Chrome_WPF.Models.WarehouseMasterDTO;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Models.LocationMasterDTO;
using Chrome_WPF.Models.StorageProductDTO;

namespace Chrome.DTO.ReplenishDTO
{
    public class ReplenishResponseDTO : INotifyPropertyChanged
    {
        private string _productCode = string.Empty;
        private string _productName = string.Empty;
        private string _warehouseCode = string.Empty;
        private string _warehouseName = string.Empty;
        private double? _minQuantity;
        private double? _maxQuantity;
        private double? _totalOnHand;
        private bool _isNewRow;
        private WarehouseMasterResponseDTO? _selectedWarehouse;
        private ProductMasterResponseDTO? _selectedProduct;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [JsonProperty("ProductCode")]
        public string ProductCode
        {
            get => _productCode;
            set
            {
                _productCode = value;
                OnPropertyChanged(nameof(ProductCode));
            }
        }

        [JsonProperty("ProductName")]
        public string ProductName
        {
            get => _productName;
            set
            {
                _productName = value;
                OnPropertyChanged(nameof(ProductName));
            }
        }

        [JsonProperty("WarehouseCode")]
        public string WarehouseCode
        {
            get => _warehouseCode;
            set
            {
                _warehouseCode = value;
                OnPropertyChanged(nameof(WarehouseCode));
            }
        }

        [JsonProperty("WarehouseName")]
        public string WarehouseName
        {
            get => _warehouseName;
            set
            {
                _warehouseName = value;
                OnPropertyChanged(nameof(WarehouseName));
            }
        }

        [JsonProperty("MinQuantity")]
        public double? MinQuantity
        {
            get => _minQuantity;
            set
            {
                _minQuantity = value;
                OnPropertyChanged(nameof(MinQuantity));
            }
        }

        [JsonProperty("MaxQuantity")]
        public double? MaxQuantity
        {
            get => _maxQuantity;
            set
            {
                _maxQuantity = value;
                OnPropertyChanged(nameof(MaxQuantity));
            }
        }

        [JsonProperty("TotalOnHand")]
        public double? TotalOnHand
        {
            get => _totalOnHand;
            set
            {
                _totalOnHand = value;
                OnPropertyChanged(nameof(TotalOnHand));
            }
        }

        public bool IsNewRow
        {
            get => _isNewRow;
            set
            {
                _isNewRow = value;
                OnPropertyChanged(nameof(IsNewRow));
            }
        }

        public WarehouseMasterResponseDTO? SelectedWarehouse
        {
            get => _selectedWarehouse;
            set
            {
                _selectedWarehouse = value;
                OnPropertyChanged(nameof(SelectedWarehouse));
                WarehouseCode = value?.WarehouseCode ?? string.Empty;
                WarehouseName = value?.WarehouseName ?? string.Empty;
            }
        }

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

        
    }
}