using Chrome_WPF.Models.ProductMasterDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.StorageProductDTO
{
    public class StorageProductResponseDTO : INotifyPropertyChanged
    {
        private string _storageProductId = string.Empty;
        private string _storageProductName = string.Empty;
        private string _productCode = string.Empty;
        private string _productName = string.Empty;
        private double? _maxQuantity;
        private bool _isNewRow;
        private ProductMasterResponseDTO? _selectedProduct;

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
        public string ProductCode
        {
            get => _productCode;
            set
            {
                _productCode = value;
                OnPropertyChanged(nameof(ProductCode));
            }
        }
        public string ProductName
        {
            get => _productName;
            set
            {
                _productName = value;
                OnPropertyChanged(nameof(ProductName));
            }
        }
        public double? MaxQuantity
        {
            get => _maxQuantity;
            set
            {
                _maxQuantity = value;
                OnPropertyChanged(nameof(MaxQuantity));
            }
        }
        public bool IsNewRow
        {
            get => _isNewRow;
            set { _isNewRow = value; OnPropertyChanged(nameof(IsNewRow)); }
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
