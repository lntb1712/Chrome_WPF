using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome_WPF.Models.TransferDTO
{
    public class TransferRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _transferCode = string.Empty;
        private string _orderTypeCode = string.Empty;
        private string _fromWarehouseCode = string.Empty;
        private string _toWarehouseCode = string.Empty;
        private string _toResponsible = string.Empty;
        private string _fromResponsible = string.Empty;
        private int? _statusId = 1;
        private string _transferDate = string.Empty;
        private string _transferDescription = string.Empty;

        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Mã chuyển kho không được để trống")]
        public string TransferCode
        {
            get => _transferCode;
            set
            {
                _transferCode = value;
                OnPropertyChanged(nameof(TransferCode));
            }
        }

        public string? OrderTypeCode
        {
            get => _orderTypeCode;
            set
            {
                _orderTypeCode = value ?? string.Empty;
                OnPropertyChanged(nameof(OrderTypeCode));
            }
        }

        [Required(ErrorMessage = "Mã kho xuất không được để trống")]
        public string FromWarehouseCode
        {
            get => _fromWarehouseCode;
            set
            {
                _fromWarehouseCode = value;
                OnPropertyChanged(nameof(FromWarehouseCode));
            }
        }

        [Required(ErrorMessage = "Mã kho nhận không được để trống")]
        public string ToWarehouseCode
        {
            get => _toWarehouseCode;
            set
            {
                _toWarehouseCode = value;
                OnPropertyChanged(nameof(ToWarehouseCode));
            }
        }

        [Required(ErrorMessage = "Tên nhân viên nhận không được để trống")]
        public string ToResponsible
        {
            get => _toResponsible;
            set
            {
                _toResponsible = value;
                OnPropertyChanged(nameof(ToResponsible));
            }
        }

        [Required(ErrorMessage = "Tên nhân viên xuất không được để trống")]
        public string FromResponsible
        {
            get => _fromResponsible;
            set
            {
                _fromResponsible = value;
                OnPropertyChanged(nameof(FromResponsible));
            }
        }

        public int? StatusId
        {
            get => _statusId;
            set
            {
                _statusId = value;
                OnPropertyChanged(nameof(StatusId));
            }
        }

        [Required(ErrorMessage = "Ngày chuyển kho không được để trống")]
        public string TransferDate
        {
            get => _transferDate;
            set
            {
                _transferDate = value;
                OnPropertyChanged(nameof(TransferDate));
            }
        }

        public string? TransferDescription
        {
            get => _transferDescription;
            set
            {
                _transferDescription = value ?? string.Empty;
                OnPropertyChanged(nameof(TransferDescription));
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
            OnPropertyChanged(nameof(TransferCode));
            OnPropertyChanged(nameof(OrderTypeCode));
            OnPropertyChanged(nameof(FromWarehouseCode));
            OnPropertyChanged(nameof(ToWarehouseCode));
            OnPropertyChanged(nameof(ToResponsible));
            OnPropertyChanged(nameof(FromResponsible));
            OnPropertyChanged(nameof(StatusId));
            OnPropertyChanged(nameof(TransferDate));
            OnPropertyChanged(nameof(TransferDescription));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(TransferCode));
            OnPropertyChanged(nameof(OrderTypeCode));
            OnPropertyChanged(nameof(FromWarehouseCode));
            OnPropertyChanged(nameof(ToWarehouseCode));
            OnPropertyChanged(nameof(ToResponsible));
            OnPropertyChanged(nameof(FromResponsible));
            OnPropertyChanged(nameof(StatusId));
            OnPropertyChanged(nameof(TransferDate));
            OnPropertyChanged(nameof(TransferDescription));
        }
    }
}