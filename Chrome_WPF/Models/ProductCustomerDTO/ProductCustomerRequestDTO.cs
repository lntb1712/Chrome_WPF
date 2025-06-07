using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Chrome_WPF.Models.ProductCustomerDTO
{
    public class ProductCustomerRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _customerCode = string.Empty;
        private string _productCode = string.Empty;
        private int? _expectedDeliverTime;
        private double? _pricePerUnit;
        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Mã khách hàng không được để trống")]
        public string CustomerCode
        {
            get => _customerCode;
            set
            {
                _customerCode = value;
                OnPropertyChanged(nameof(CustomerCode));
            }
        }

        [Required(ErrorMessage = "Mã sản phẩm không được để trống")]
        public string ProductCode
        {
            get => _productCode;
            set
            {
                _productCode = value;
                OnPropertyChanged(nameof(ProductCode));
            }
        }

        public int? ExpectedDeliverTime
        {
            get => _expectedDeliverTime;
            set
            {
                _expectedDeliverTime = value;
                OnPropertyChanged(nameof(ExpectedDeliverTime));
            }
        }

        public double? PricePerUnit
        {
            get => _pricePerUnit;
            set
            {
                _pricePerUnit = value;
                OnPropertyChanged(nameof(PricePerUnit));
            }
        }

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                if (!_isValidationRequested)
                    return string.Empty;

                if (columnName == nameof(ExpectedDeliverTime) || columnName == nameof(PricePerUnit))
                {
                    return string.Empty;
                }

                var property = GetType().GetProperty(columnName);
                if (property == null)
                    return string.Empty;

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
            OnPropertyChanged(nameof(CustomerCode));
            OnPropertyChanged(nameof(ProductCode));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(CustomerCode));
            OnPropertyChanged(nameof(ProductCode));
        }
    }
}