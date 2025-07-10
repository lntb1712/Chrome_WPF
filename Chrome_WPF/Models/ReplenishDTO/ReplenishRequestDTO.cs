using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.ReplenishDTO
{
    public class ReplenishRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string? _warehouseCode = string.Empty;
        private string? _productCode = string.Empty;
        private double _minQuantity = 0.0;
        private double _maxQuantity = 0.0;
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        [Required(ErrorMessage = "Kho không được để trống")]
        public string? WarehouseCode
        {
            get => _warehouseCode;
            set
            {
                _warehouseCode = value;
                OnPropertyChanged(nameof(WarehouseCode));
            }
        }
        [Required(ErrorMessage = "Mã sản phẩm không được để trống")]
        public string? ProductCode
        {
            get => _productCode;
            set
            {
                _productCode = value;
                OnPropertyChanged(nameof(ProductCode));
            }
        }
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public double MinQuantity
        {
            get => _minQuantity;
            set
            {
                _minQuantity = value;
                OnPropertyChanged(nameof(MinQuantity));
            }
        }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public double MaxQuantity
        {
            get => _maxQuantity;
            set
            {
                _maxQuantity = value;
                OnPropertyChanged(nameof(MaxQuantity));
            }
        }

        public string Error => string.Empty;
        private bool _isValidationRequested;
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
            OnPropertyChanged(nameof(WarehouseCode));
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(MinQuantity));
            OnPropertyChanged(nameof(MaxQuantity));
        }

        public void ClearValidationRequest()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(WarehouseCode));
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(MinQuantity));
            OnPropertyChanged(nameof(MaxQuantity));

        }
    }
}
