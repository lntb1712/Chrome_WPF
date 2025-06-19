using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome_WPF.Models.StockOutDetailDTO
{
    public class StockOutDetailRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _stockOutCode = string.Empty;
        private string _productCode = string.Empty;
        private double? _demand;
        private double? _quantity;

        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Mã xuất kho không được để trống")]
        public string StockOutCode
        {
            get => _stockOutCode;
            set
            {
                _stockOutCode = value;
                OnPropertyChanged(nameof(StockOutCode));
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

        [Range(0, double.MaxValue, ErrorMessage = "Nhu cầu phải là số không âm")]
        public double? Demand
        {
            get => _demand;
            set
            {
                _demand = value;
                OnPropertyChanged(nameof(Demand));
            }
        }

        [Range(0, double.MaxValue, ErrorMessage = "Số lượng phải là số không âm")]
        public double? Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
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
            OnPropertyChanged(nameof(StockOutCode));
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(Demand));
            OnPropertyChanged(nameof(Quantity));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(StockOutCode));
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(Demand));
            OnPropertyChanged(nameof(Quantity));
        }
    }
}