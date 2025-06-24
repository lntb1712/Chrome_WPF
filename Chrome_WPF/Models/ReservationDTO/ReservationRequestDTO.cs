using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome_WPF.Models.ReservationDTO
{
    public class ReservationRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _reservationCode = string.Empty;
        private string? _warehouseCode;
        private string? _orderTypeCode;
        private string? _orderId;
        private string? _reservationDate;
        private int? _statusId;
        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Mã đặt chỗ không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Mã đặt chỗ chỉ được chứa chữ, số và dấu gạch dưới")]
        public string ReservationCode
        {
            get => _reservationCode;
            set
            {
                _reservationCode = value;
                OnPropertyChanged(nameof(ReservationCode));
            }
        }

        [Required(ErrorMessage = "Mã kho không được để trống")]
        public string? WarehouseCode
        {
            get => _warehouseCode;
            set
            {
                _warehouseCode = value;
                OnPropertyChanged(nameof(WarehouseCode));
            }
        }

        [Required(ErrorMessage = "Mã loại đơn hàng không được để trống")]
        public string? OrderTypeCode
        {
            get => _orderTypeCode;
            set
            {
                _orderTypeCode = value;
                OnPropertyChanged(nameof(OrderTypeCode));
            }
        }

        [Required(ErrorMessage = "Mã đơn hàng không được để trống")]
        public string? OrderId
        {
            get => _orderId;
            set
            {
                _orderId = value;
                OnPropertyChanged(nameof(OrderId));
            }
        }

        [Required(ErrorMessage = "Ngày đặt chỗ không được để trống")]
        [RegularExpression(@"^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[012])/\d{4}$",
            ErrorMessage = "Ngày đặt chỗ phải có định dạng dd/MM/yyyy")]
        public string? ReservationDate
        {
            get => _reservationDate;
            set
            {
                _reservationDate = value;
                OnPropertyChanged(nameof(ReservationDate));
            }
        }

        [Required(ErrorMessage = "Trạng thái không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Trạng thái phải là số nguyên dương")]
        public int? StatusId
        {
            get => _statusId;
            set
            {
                _statusId = value;
                OnPropertyChanged(nameof(StatusId));
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
            OnPropertyChanged(nameof(ReservationCode));
            OnPropertyChanged(nameof(WarehouseCode));
            OnPropertyChanged(nameof(OrderTypeCode));
            OnPropertyChanged(nameof(OrderId));
            OnPropertyChanged(nameof(ReservationDate));
            OnPropertyChanged(nameof(StatusId));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(ReservationCode));
            OnPropertyChanged(nameof(WarehouseCode));
            OnPropertyChanged(nameof(OrderTypeCode));
            OnPropertyChanged(nameof(OrderId));
            OnPropertyChanged(nameof(ReservationDate));
            OnPropertyChanged(nameof(StatusId));
        }
    }
}