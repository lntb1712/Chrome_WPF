using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome_WPF.Models.MovementDTO
{
    public class MovementDetailRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _movementCode = string.Empty;
        private string _productCode = string.Empty;
        private double? _demand;
        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Mã di chuyển không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Mã di chuyển chỉ được chứa chữ, số và dấu gạch dưới")]
        public string MovementCode
        {
            get => _movementCode;
            set
            {
                _movementCode = value;
                OnPropertyChanged(nameof(MovementCode));
            }
        }

        [Required(ErrorMessage = "Mã sản phẩm không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Mã sản phẩm chỉ được chứa chữ, số và dấu gạch dưới")]
        public string ProductCode
        {
            get => _productCode;
            set
            {
                _productCode = value;
                OnPropertyChanged(nameof(ProductCode));
            }
        }

        [Required(ErrorMessage = "Số lượng yêu cầu không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số lượng yêu cầu phải là một số dương")]
        public double? Demand
        {
            get => _demand;
            set
            {
                _demand = value;
                OnPropertyChanged(nameof(Demand));
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
            OnPropertyChanged(nameof(MovementCode));
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(Demand));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(MovementCode));
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(Demand));
        }
    }
}