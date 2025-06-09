using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.PutAwayRulesDTO
{
    public class PutAwayRulesRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _putAwayRuleCode = string.Empty;
        private string? _warehouseToApply = string.Empty;
        private string? _productCode = string.Empty;
        private string? _locationCode = string.Empty;
        private string? _storageProductId = string.Empty;

        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        [Required(ErrorMessage = "Mã quy tắc để hàng không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Mã sản phẩm lưu kho chỉ được chứa chữ, số và dấu gạch dưới")]
        public string PutAwayRuleCode
        {
            get => _putAwayRuleCode;
            set
            {
                _putAwayRuleCode = value;
                OnPropertyChanged(nameof(PutAwayRuleCode));
            }
        }
        [Required(ErrorMessage = "Kho áp dụng không được để trống")]
        public string? WarehouseToApply
        {
            get => _warehouseToApply;
            set
            {
                _warehouseToApply = value;
                OnPropertyChanged(nameof(WarehouseToApply));
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
        [Required(ErrorMessage = "Mã vị trí không được để trống")]
        public string? LocationCode
        {
            get => _locationCode;
            set
            {
                _locationCode = value;
                OnPropertyChanged(nameof(LocationCode));
            }
        }
        [Required(ErrorMessage = "Mã định mức không được để trống")]
        public string? StorageProductId
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
            OnPropertyChanged(nameof(PutAwayRuleCode));
            OnPropertyChanged(nameof(WarehouseToApply));
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(LocationCode));
            OnPropertyChanged(nameof(StorageProductId));

        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(PutAwayRuleCode));
            OnPropertyChanged(nameof(WarehouseToApply));
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(LocationCode));
            OnPropertyChanged(nameof(StorageProductId));
        }
    }
}
