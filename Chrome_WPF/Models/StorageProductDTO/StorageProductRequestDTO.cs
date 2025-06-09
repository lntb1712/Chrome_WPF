using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome_WPF.Models.StorageProductDTO
{
    public class StorageProductRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _storageProductId = string.Empty;
        private string _storageProductName = string.Empty;
        private string _productCode = string.Empty;
        private double? _maxQuantity;

        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Mã sản phẩm lưu kho không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Mã sản phẩm lưu kho chỉ được chứa chữ, số và dấu gạch dưới")]
        public string StorageProductId
        {
            get => _storageProductId;
            set
            {
                _storageProductId = value;
                OnPropertyChanged(nameof(StorageProductId));
            }
        }

        [Required(ErrorMessage = "Tên sản phẩm lưu kho không được để trống")]
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

        [Range(0.00001, double.MaxValue, ErrorMessage = "Số lượng tối đa phải lớn hơn 0")]
        public double? MaxQuantity
        {
            get => _maxQuantity;
            set
            {
                _maxQuantity = value;
                OnPropertyChanged(nameof(MaxQuantity));
            }
        }

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                if (!_isValidationRequested)
                    return string.Empty;

                var property = GetType().GetProperty(columnName);
                if (property == null) return string.Empty;

                var value = property.GetValue(this);
                var context = new ValidationContext(this) { MemberName = columnName };
                var results = new List<ValidationResult>();

                bool isValid = Validator.TryValidateProperty(value, context, results);

                return isValid ? string.Empty : results.FirstOrDefault()?.ErrorMessage ?? string.Empty;
            }
        }

        public void RequestValidation()
        {
            _isValidationRequested = true;
            OnPropertyChanged(nameof(StorageProductId));
            OnPropertyChanged(nameof(StorageProductName));
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(MaxQuantity));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(StorageProductId));
            OnPropertyChanged(nameof(StorageProductName));
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(MaxQuantity));
        }
    }
}
