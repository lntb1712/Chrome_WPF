using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome_WPF.Models.TransferDTO
{
    public class TransferDetailRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _transferCode = string.Empty;
        private string _productCode = string.Empty;
        private double? _demand;
        private double? _quantityInBounded;
        private double? _quantityOutBounded;
        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Mã chuyển kho không được để trống")]
        public string TransferCode
        {
            get => _transferCode;
            set
            {
                if (_transferCode != value)
                {
                    _transferCode = value;
                    OnPropertyChanged(nameof(TransferCode));
                }
            }
        }

        [Required(ErrorMessage = "Mã sản phẩm không được để trống")]
        public string ProductCode
        {
            get => _productCode;
            set
            {
                if (_productCode != value)
                {
                    _productCode = value;
                    OnPropertyChanged(nameof(ProductCode));
                }
            }
        }


        [Range(0, double.MaxValue, ErrorMessage = "Nhu cầu phải là số không âm")]
        public double? Demand
        {
            get => _demand;
            set
            {
                if (_demand != value)
                {
                    _demand = value;
                    OnPropertyChanged(nameof(Demand));
                }
            }
        }

        public double? QuantityInBounded
        {
            get => _quantityInBounded;
            set
            {
                if (_quantityInBounded != value)
                {
                    _quantityInBounded = value;
                    OnPropertyChanged(nameof(QuantityInBounded));
                }
            }
        }

        public double? QuantityOutBounded
        {
            get => _quantityOutBounded;
            set
            {
                if (_quantityOutBounded != value)
                {
                    _quantityOutBounded = value;
                    OnPropertyChanged(nameof(QuantityOutBounded));
                }
            }
        }

        
        // Validation xử lý
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

        // Phương thức yêu cầu validate toàn bộ
        public void RequestValidation()
        {
            _isValidationRequested = true;
            OnPropertyChanged(nameof(TransferCode));
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(Demand));
            OnPropertyChanged(nameof(QuantityInBounded));
            OnPropertyChanged(nameof(QuantityOutBounded));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(TransferCode));
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(Demand));
            OnPropertyChanged(nameof(QuantityInBounded));
            OnPropertyChanged(nameof(QuantityOutBounded));
        }
    }
}
