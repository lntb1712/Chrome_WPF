using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome_WPF.Models.PickListDTO
{
    public class PickListRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _pickNo = string.Empty;
        private string? _reservationCode;
        private string? _warehouseCode;
        private string? _responsible;
        private string? _pickDate;
        private int? _statusId;
        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Mã phiếu lấy hàng không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Mã phiếu lấy hàng chỉ được chứa chữ, số và dấu gạch dưới")]
        public string PickNo
        {
            get => _pickNo;
            set
            {
                _pickNo = value;
                OnPropertyChanged(nameof(PickNo));
            }
        }

        [Required(ErrorMessage = "Mã đặt chỗ không được để trống")]
        public string? ReservationCode
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
        [Required(ErrorMessage = "Nguo không được để trống")]
        public string? Responsible
        {
            get => _responsible;
            set
            {
                _warehouseCode = value;
                OnPropertyChanged(nameof(_responsible));
            }
        }

        [Required(ErrorMessage = "Ngày lấy hàng không được để trống")]
        public string? PickDate
        {
            get => _pickDate;
            set
            {
                _pickDate = value;
                OnPropertyChanged(nameof(PickDate));
            }
        }

        [Required(ErrorMessage = "Trạng thái không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Trạng thái phải là một số nguyên dương")]
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
            OnPropertyChanged(nameof(PickNo));
            OnPropertyChanged(nameof(ReservationCode));
            OnPropertyChanged(nameof(WarehouseCode));
            OnPropertyChanged(nameof(PickDate));
            OnPropertyChanged(nameof(StatusId));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(PickNo));
            OnPropertyChanged(nameof(ReservationCode));
            OnPropertyChanged(nameof(WarehouseCode));
            OnPropertyChanged(nameof(PickDate));
            OnPropertyChanged(nameof(StatusId));
        }
    }
}