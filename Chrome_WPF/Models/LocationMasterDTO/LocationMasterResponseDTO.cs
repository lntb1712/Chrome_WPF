using Chrome_WPF.Models.StorageProductDTO;
using Chrome_WPF.Models.SupplierMasterDTO;
using System;
using System.ComponentModel;

namespace Chrome_WPF.Models.LocationMasterDTO
{
    public class LocationMasterResponseDTO : INotifyPropertyChanged
    {
        private string _locationCode = string.Empty;
        private string _locationName = string.Empty;
        private string _warehouseCode = string.Empty;
        private string _warehouseName = string.Empty;
        private string _storageProductId = string.Empty;
        private string _storageProductName = string.Empty;
        private bool? _isEmpty;
        private bool _isNewRow;
        private StorageProductResponseDTO? _selectedStorageProduct;
        public string LocationCode
        {
            get => _locationCode;
            set
            {
                _locationCode = value;
                OnPropertyChanged(nameof(LocationCode));
            }
        }

        public string LocationName
        {
            get => _locationName;
            set
            {
                _locationName = value;
                OnPropertyChanged(nameof(LocationName));
            }
        }

        public string WarehouseCode
        {
            get => _warehouseCode;
            set
            {
                _warehouseCode = value;
                OnPropertyChanged(nameof(WarehouseCode));
            }
        }

        public string WarehouseName
        {
            get => _warehouseName;
            set
            {
                _warehouseName = value;
                OnPropertyChanged(nameof(WarehouseName));
            }
        }

        public string StorageProductId
        {
            get => _storageProductId;
            set
            {
                _storageProductId = value;
                OnPropertyChanged(nameof(StorageProductId));
            }
        }

        public string StorageProductName
        {
            get => _storageProductName;
            set
            {
                _storageProductName = value;
                OnPropertyChanged(nameof(StorageProductName));
            }
        }

        public bool? IsEmpty
        {
            get => _isEmpty;
            set
            {
                _isEmpty = value;
                OnPropertyChanged(nameof(IsEmpty));
            }
        }
        public bool IsNewRow
        {
            get => _isNewRow;
            set { _isNewRow = value; OnPropertyChanged(nameof(IsNewRow)); }
        }
        private string _locationCodeSuffix = string.Empty;
        public string LocationCodeSuffix
        {
            get => _locationCodeSuffix;
            set
            {
                _locationCodeSuffix = value;
                OnPropertyChanged(nameof(LocationCodeSuffix));
                UpdateLocationCode();
            }
        }

        private void UpdateLocationCode()
        {
            if (!string.IsNullOrEmpty(WarehouseCode) && !string.IsNullOrEmpty(LocationCodeSuffix))
            {
                _locationCode = $"{WarehouseCode}/{LocationCodeSuffix}";
            }
            else
            {
                _locationCode = LocationCodeSuffix;
            }
            OnPropertyChanged(nameof(LocationCode));
        }

        private void UpdateLocationCodeSuffix()
        {
            if (!string.IsNullOrEmpty(LocationCode) && LocationCode.StartsWith($"{WarehouseCode}/"))
            {
                _locationCodeSuffix = LocationCode.Substring($"{WarehouseCode}/".Length);
            }
            else
            {
                _locationCodeSuffix = LocationCode;
            }
            OnPropertyChanged(nameof(LocationCodeSuffix));
        }
        public StorageProductResponseDTO? SelectedStorageProduct
        {
            get => _selectedStorageProduct;
            set
            {
                if (_selectedStorageProduct != value)
                {
                    _selectedStorageProduct = value;
                    OnPropertyChanged(nameof(_selectedStorageProduct));

                    // Tự động cập nhật mã và tên khi chọn từ ComboBox
                    StorageProductId = value?.StorageProductId ?? string.Empty;
                    StorageProductName = value?.StorageProductName ?? string.Empty;
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
