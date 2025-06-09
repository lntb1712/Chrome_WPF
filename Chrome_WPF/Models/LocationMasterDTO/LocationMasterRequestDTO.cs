using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome_WPF.Models.LocationMasterDTO
{
    public class LocationMasterRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _locationCode = string.Empty;
        private string _locationName = string.Empty;
        private string _warehouseCode = string.Empty;
        private string _storageProductId = string.Empty;

        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Mã vị trí không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Mã vị trí chỉ được chứa chữ, số và dấu gạch dưới")]
        public string LocationCode
        {
            get => _locationCode;
            set
            {
                _locationCode = value;
                OnPropertyChanged(nameof(LocationCode));
            }
        }

        [Required(ErrorMessage = "Tên vị trí không được để trống")]
        public string LocationName
        {
            get => _locationName;
            set
            {
                _locationName = value;
                OnPropertyChanged(nameof(LocationName));
            }
        }

        [Required(ErrorMessage = "Mã kho không được để trống")]
        public string WarehouseCode
        {
            get => _warehouseCode;
            set
            {
                _warehouseCode = value;
                OnPropertyChanged(nameof(WarehouseCode));
            }
        }

        public string StorageProductId
        {
            get => _storageProductId;
            set
            {
                _storageProductId = value;
                OnPropertyChanged(nameof(StorageProductId));
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
            OnPropertyChanged(nameof(LocationCode));
            OnPropertyChanged(nameof(LocationName));
            OnPropertyChanged(nameof(WarehouseCode));
            OnPropertyChanged(nameof(StorageProductId));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(LocationCode));
            OnPropertyChanged(nameof(LocationName));
            OnPropertyChanged(nameof(WarehouseCode));
            OnPropertyChanged(nameof(StorageProductId));
        }
    }
}
