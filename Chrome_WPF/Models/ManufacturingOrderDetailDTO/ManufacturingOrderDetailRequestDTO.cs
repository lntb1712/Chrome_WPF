using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome.DTO.ManufacturingOrderDetailDTO
{
    public class ManufacturingOrderDetailRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _manufacturingOrderCode = string.Empty;
        private string _componentCode = string.Empty;
        private double? _toConsumeQuantity;
        private double? _consumedQuantity;
        private double? _scraptRate;
        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Mã lệnh sản xuất không được để trống")]
        
        public string ManufacturingOrderCode
        {
            get => _manufacturingOrderCode;
            set
            {
                _manufacturingOrderCode = value;
                OnPropertyChanged(nameof(ManufacturingOrderCode));
            }
        }

        [Required(ErrorMessage = "Mã thành phần không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Mã thành phần chỉ được chứa chữ, số và dấu gạch dưới")]
        public string ComponentCode
        {
            get => _componentCode;
            set
            {
                _componentCode = value;
                OnPropertyChanged(nameof(ComponentCode));
            }
        }

        [Required(ErrorMessage = "Số lượng cần tiêu thụ không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Số lượng cần tiêu thụ phải là một số không âm")]
        public double? ToConsumeQuantity
        {
            get => _toConsumeQuantity;
            set
            {
                _toConsumeQuantity = value;
                OnPropertyChanged(nameof(ToConsumeQuantity));
            }
        }

        [Range(0, double.MaxValue, ErrorMessage = "Số lượng đã tiêu thụ phải là một số không âm")]
        public double? ConsumedQuantity
        {
            get => _consumedQuantity;
            set
            {
                _consumedQuantity = value;
                OnPropertyChanged(nameof(ConsumedQuantity));
            }
        }

        [Range(0, 100, ErrorMessage = "Tỷ lệ phế phẩm phải nằm trong khoảng từ 0 đến 100")]
        public double? ScraptRate
        {
            get => _scraptRate;
            set
            {
                _scraptRate = value;
                OnPropertyChanged(nameof(ScraptRate));
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

                // Additional custom validation for ConsumedQuantity <= ToConsumeQuantity
                if (isValid && columnName == nameof(ConsumedQuantity) && ToConsumeQuantity.HasValue && ConsumedQuantity.HasValue)
                {
                    if (ConsumedQuantity > ToConsumeQuantity)
                    {
                        results.Add(new ValidationResult("Số lượng đã tiêu thụ không thể lớn hơn số lượng cần tiêu thụ."));
                        isValid = false;
                    }
                }

                return isValid ? string.Empty : results.FirstOrDefault()?.ErrorMessage ?? string.Empty;
            }
        }

        public void RequestValidation()
        {
            _isValidationRequested = true;
            OnPropertyChanged(nameof(ManufacturingOrderCode));
            OnPropertyChanged(nameof(ComponentCode));
            OnPropertyChanged(nameof(ToConsumeQuantity));
            OnPropertyChanged(nameof(ConsumedQuantity));
            OnPropertyChanged(nameof(ScraptRate));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(ManufacturingOrderCode));
            OnPropertyChanged(nameof(ComponentCode));
            OnPropertyChanged(nameof(ToConsumeQuantity));
            OnPropertyChanged(nameof(ConsumedQuantity));
            OnPropertyChanged(nameof(ScraptRate));
        }
    }
}