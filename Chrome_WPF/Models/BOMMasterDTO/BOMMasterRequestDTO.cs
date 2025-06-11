using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome_WPF.Models.BOMMasterDTO
{
    public class BOMMasterRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _bomCode = string.Empty;
        private string _bomVersion = string.Empty;
        private bool _isActive = false;
        private string _productCode = string.Empty;

        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Mã BOM không được để trống")]
        public string BOMCode
        {
            get => _bomCode;
            set
            {
                _bomCode = value;
                OnPropertyChanged(nameof(BOMCode));
            }
        }

        [Required(ErrorMessage = "Phiên bản BOM không được để trống")]
        public string BOMVersion
        {
            get => _bomVersion;
            set
            {
                _bomVersion = value;
                OnPropertyChanged(nameof(BOMVersion));
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnPropertyChanged(nameof(IsActive));
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
            OnPropertyChanged(nameof(BOMCode));
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(IsActive));
            OnPropertyChanged(nameof(BOMVersion));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(BOMCode));
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(IsActive));
            OnPropertyChanged(nameof(BOMVersion));
        }
    }
}
