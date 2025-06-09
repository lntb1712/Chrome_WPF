using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome_WPF.Models.WarehouseMasterDTO
{
    public class WarehouseMasterRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _warehouseCode = string.Empty;
        private string _warehouseName = string.Empty;
        private string _warehouseDescription = string.Empty;
        private string _warehouseAddress = string.Empty;

        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Mã kho không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Mã kho chỉ được chứa chữ, số và dấu gạch dưới")]
        public string WarehouseCode
        {
            get => _warehouseCode;
            set
            {
                _warehouseCode = value;
                OnPropertyChanged(nameof(WarehouseCode));
            }
        }

        [Required(ErrorMessage = "Tên kho không được để trống")]
        public string WarehouseName
        {
            get => _warehouseName;
            set
            {
                _warehouseName = value;
                OnPropertyChanged(nameof(WarehouseName));
            }
        }

        public string WarehouseDescription
        {
            get => _warehouseDescription;
            set
            {
                _warehouseDescription = value;
                OnPropertyChanged(nameof(WarehouseDescription));
            }
        }

        [Required(ErrorMessage = "Địa chỉ kho không được để trống")]
        public string WarehouseAddress
        {
            get => _warehouseAddress;
            set
            {
                _warehouseAddress = value;
                OnPropertyChanged(nameof(WarehouseAddress));
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
            OnPropertyChanged(nameof(WarehouseCode));
            OnPropertyChanged(nameof(WarehouseName));
            OnPropertyChanged(nameof(WarehouseDescription));
            OnPropertyChanged(nameof(WarehouseAddress));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(WarehouseCode));
            OnPropertyChanged(nameof(WarehouseName));
            OnPropertyChanged(nameof(WarehouseDescription));
            OnPropertyChanged(nameof(WarehouseAddress));
        }
    }
}
