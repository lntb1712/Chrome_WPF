using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome_WPF.Models.StockOutDTO
{
    public class StockOutRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _stockOutCode = string.Empty;
        private string _orderTypeCode = string.Empty;
        private string _warehouseCode = string.Empty;
        private string _customerCode = string.Empty;
        private string _responsible = string.Empty;
        private int _statusId = 1;
        private string _stockOutDate = string.Empty;
        private string _stockOutDescription = string.Empty;

        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Mã xuất kho không được để trống")]
        public string StockOutCode
        {
            get => _stockOutCode;
            set
            {
                _stockOutCode = value;
                OnPropertyChanged(nameof(StockOutCode));
            }
        }

        [Required(ErrorMessage = "Mã loại lệnh không được để trống")]
        public string OrderTypeCode
        {
            get => _orderTypeCode;
            set
            {
                _orderTypeCode = value;
                OnPropertyChanged(nameof(OrderTypeCode));
            }
        }

        [Required(ErrorMessage = "Mã kho xuất không được để trống")]
        public string WarehouseCode
        {
            get => _warehouseCode;
            set
            {
                _warehouseCode = value;
                OnPropertyChanged(nameof(WarehouseCode));
            }
        }

        [Required(ErrorMessage = "Mã khách hàng không được để trống")]
        public string CustomerCode
        {
            get => _customerCode;
            set
            {
                _customerCode = value;
                OnPropertyChanged(nameof(CustomerCode));
            }
        }

        [Required(ErrorMessage = "Tên nhân viên chịu trách nhiệm không được để trống")]
        public string Responsible
        {
            get => _responsible;
            set
            {
                _responsible = value;
                OnPropertyChanged(nameof(Responsible));
            }
        }

        public int StatusId
        {
            get => _statusId;
            set
            {
                _statusId = value;
                OnPropertyChanged(nameof(StatusId));
            }
        }

        [Required(ErrorMessage = "Ngày xuất kho không được để trống")]
        public string StockOutDate
        {
            get => _stockOutDate;
            set
            {
                _stockOutDate = value;
                OnPropertyChanged(nameof(StockOutDate));
            }
        }

        public string StockOutDescription
        {
            get => _stockOutDescription;
            set
            {
                _stockOutDescription = value;
                OnPropertyChanged(nameof(StockOutDescription));
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
            OnPropertyChanged(nameof(StockOutCode));
            OnPropertyChanged(nameof(OrderTypeCode));
            OnPropertyChanged(nameof(WarehouseCode));
            OnPropertyChanged(nameof(CustomerCode));
            OnPropertyChanged(nameof(Responsible));
            OnPropertyChanged(nameof(StockOutDate));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(StockOutCode));
            OnPropertyChanged(nameof(OrderTypeCode));
            OnPropertyChanged(nameof(WarehouseCode));
            OnPropertyChanged(nameof(CustomerCode));
            OnPropertyChanged(nameof(Responsible));
            OnPropertyChanged(nameof(StockOutDate));
        }
    }
}