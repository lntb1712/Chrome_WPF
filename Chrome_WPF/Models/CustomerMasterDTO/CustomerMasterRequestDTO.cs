using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome_WPF.Models.CustomerMasterDTO
{
    public class CustomerMasterRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _customerCode = string.Empty;
        private string _customerName = string.Empty;
        private string _customerPhone = string.Empty;
        private string _customerAddress = string.Empty;
        private string _customerEmail = string.Empty;
        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Mã khách hàng không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Mã khách hàng chỉ được chứa chữ, số và dấu gạch dưới")]
        public string CustomerCode
        {
            get => _customerCode;
            set
            {
                _customerCode = value;
                OnPropertyChanged(nameof(CustomerCode));
            }
        }

        [Required(ErrorMessage = "Tên khách hàng không được để trống")]
        public string CustomerName
        {
            get => _customerName;
            set
            {
                _customerName = value;
                OnPropertyChanged(nameof(CustomerName));
            }
        }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^(?:\+84|0)(?:3|5|7|8|9)\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ. Vui lòng nhập số điện thoại Việt Nam (bắt đầu bằng +84 hoặc 0, theo sau là 9 chữ số).")]
        public string CustomerPhone
        {
            get => _customerPhone;
            set
            {
                _customerPhone = value;
                OnPropertyChanged(nameof(CustomerPhone));
            }
        }

        [Required(ErrorMessage = "Địa chỉ của khách hàng không được để trống")]
        public string CustomerAddress
        {
            get => _customerAddress;
            set
            {
                _customerAddress = value;
                OnPropertyChanged(nameof(CustomerAddress));
            }
        }

        [EmailAddress(ErrorMessage = "Email không hợp lệ. Vui lòng nhập định dạng email hợp lệ (ví dụ: example@domain.com).")]
        public string CustomerEmail
        {
            get => _customerEmail;
            set
            {
                _customerEmail = value;
                OnPropertyChanged(nameof(CustomerEmail));
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
            OnPropertyChanged(nameof(CustomerCode));
            OnPropertyChanged(nameof(CustomerName));
            OnPropertyChanged(nameof(CustomerPhone));
            OnPropertyChanged(nameof(CustomerAddress));
            OnPropertyChanged(nameof(CustomerEmail));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(CustomerCode));
            OnPropertyChanged(nameof(CustomerName));
            OnPropertyChanged(nameof(CustomerPhone));
            OnPropertyChanged(nameof(CustomerAddress));
            OnPropertyChanged(nameof(CustomerEmail));
        }
    }
}