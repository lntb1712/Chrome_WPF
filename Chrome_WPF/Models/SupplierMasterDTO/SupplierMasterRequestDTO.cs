using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.SupplierMasterDTO
{
    public class SupplierMasterRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _supplierCode = string.Empty;
        private string _supplierName = string.Empty;
        private string _supplierPhone = string.Empty;
        private string _supplierAddress = string.Empty;

        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        [Required(ErrorMessage = "Mã nhà cung cấp không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Mã nhà cung cấp chỉ được chứa chữ, số và dấu gạch dưới")]
        public string SupplierCode
        {
            get => _supplierCode;
            set
            {
                _supplierCode = value;
                OnPropertyChanged(nameof(SupplierCode));
            }
        }

        [Required(ErrorMessage = "Tên nhà cung cấp không được để trống")]
        public string SupplierName
        {
            get => _supplierName;
            set
            {
                _supplierName = value;
                OnPropertyChanged(nameof(SupplierName));
            }
        }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^(?:\+84|0)(?:3|5|7|8|9)\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ. Vui lòng nhập số điện thoại Việt Nam (bắt đầu bằng +84 hoặc 0, theo sau là 9 chữ số).")]
        public string SupplierPhone
        {
            get => _supplierPhone;
            set
            {
                _supplierPhone = value;
                OnPropertyChanged(nameof(SupplierPhone));
            }
        }

        [Required(ErrorMessage = "Địa chỉ của nhà cung cấp không được để trống")]
        public string SupplierAddress
        {
            get => _supplierAddress;
            set
            {
                _supplierAddress = value;
                OnPropertyChanged(nameof(SupplierAddress));
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
            OnPropertyChanged(nameof(SupplierCode));
            OnPropertyChanged(nameof(SupplierName));
            OnPropertyChanged(nameof(SupplierAddress));
            OnPropertyChanged(nameof(SupplierPhone));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(SupplierCode));
            OnPropertyChanged(nameof(SupplierName));
            OnPropertyChanged(nameof(SupplierAddress));
            OnPropertyChanged(nameof(SupplierPhone));
        }
    }
}
