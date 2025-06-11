using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome_WPF.Models.BOMComponentDTO
{
    public class BOMComponentRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _bomCode = string.Empty;
        private string _componentCode = string.Empty;
        private string _bomVersion = string.Empty;
        private double? _consumpQuantity;
        private double? _scrapRate;
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

        [Required(ErrorMessage = "Mã thành phần không được để trống")]
        public string ComponentCode
        {
            get => _componentCode;
            set
            {
                _componentCode = value;
                OnPropertyChanged(nameof(ComponentCode));
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

        [Required(ErrorMessage = "Định mức tiêu hao không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Định mức tiêu hao phải lớn hơn 0")]
        public double? ConsumpQuantity
        {
            get => _consumpQuantity;
            set
            {
                _consumpQuantity = value;
                OnPropertyChanged(nameof(ConsumpQuantity));
            }
        }

        [Range(0, 1, ErrorMessage = "Tỉ lệ hao hụt phải trong khoảng 0 đến 1")]
        public double? ScrapRate
        {
            get => _scrapRate;
            set
            {
                _scrapRate = value;
                OnPropertyChanged(nameof(ScrapRate));
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
            OnPropertyChanged(nameof(ComponentCode));
            OnPropertyChanged(nameof(BOMVersion));
            OnPropertyChanged(nameof(ConsumpQuantity));
            OnPropertyChanged(nameof(ScrapRate));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(BOMCode));
            OnPropertyChanged(nameof(ComponentCode));
            OnPropertyChanged(nameof(BOMVersion));
            OnPropertyChanged(nameof(ConsumpQuantity));
            OnPropertyChanged(nameof(ScrapRate));
        }
    }
}
