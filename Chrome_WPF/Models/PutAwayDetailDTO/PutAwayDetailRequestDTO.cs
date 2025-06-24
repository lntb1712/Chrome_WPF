using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome_WPF.Models.PutAwayDetailDTO
{
    public class PutAwayDetailRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _putAwayCode = string.Empty;
        private string _productCode = string.Empty;
        private string? _lotNo;
        private double? _demand;
        private double? _quantity;
        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Mã để hàng không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Mã để hàng chỉ được chứa chữ, số và dấu gạch dưới")]
        public string PutAwayCode
        {
            get => _putAwayCode;
            set
            {
                _putAwayCode = value;
                OnPropertyChanged(nameof(PutAwayCode));
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

        public string? LotNo
        {
            get => _lotNo;
            set
            {
                _lotNo = value;
                OnPropertyChanged(nameof(LotNo));
            }
        }

        [Required(ErrorMessage = "Số lượng yêu cầu không được để trống")]
        [Range(0.0, double.MaxValue, ErrorMessage = "Số lượng yêu cầu phải là số không âm")]
        public double? Demand
        {
            get => _demand;
            set
            {
                _demand = value;
                OnPropertyChanged(nameof(Demand));
            }
        }

        [Required(ErrorMessage = "Số lượng thực tế không được để trống")]
        [Range(0.0, double.MaxValue, ErrorMessage = "Số lượng thực tế phải là số không âm")]
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
            OnPropertyChanged(nameof(PutAwayCode));
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(LotNo));
            OnPropertyChanged(nameof(Demand));
            OnPropertyChanged(nameof(Quantity));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(PutAwayCode));
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(LotNo));
            OnPropertyChanged(nameof(Demand));
            OnPropertyChanged(nameof(Quantity));
        }
    }
}