using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome_WPF.Models.MovementDTO
{
    public class MovementRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _movementCode = string.Empty;
        private string? _orderTypeCode;
        private string? _warehouseCode;
        private string? _fromLocation;
        private string? _toLocation;
        private string? _responsible;
        private int? _statusId;
        private string? _movementDate;
        private string? _movementDescription;
        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Mã di chuyển không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Mã di chuyển chỉ được chứa chữ, số và dấu gạch dưới")]
        public string MovementCode
        {
            get => _movementCode;
            set
            {
                _movementCode = value;
                OnPropertyChanged(nameof(MovementCode));
            }
        }

        [Required(ErrorMessage = "Mã loại lệnh không được để trống")]
        public string? OrderTypeCode
        {
            get => _orderTypeCode;
            set
            {
                _orderTypeCode = value;
                OnPropertyChanged(nameof(OrderTypeCode));
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

        [Required(ErrorMessage = "Vị trí nguồn không được để trống")]
        public string? FromLocation
        {
            get => _fromLocation;
            set
            {
                _fromLocation = value;
                OnPropertyChanged(nameof(FromLocation));
            }
        }

        [Required(ErrorMessage = "Vị trí đích không được để trống")]
        public string? ToLocation
        {
            get => _toLocation;
            set
            {
                _toLocation = value;
                OnPropertyChanged(nameof(ToLocation));
            }
        }
        [Required(ErrorMessage = "Tên nhân viên không được để trống")]
        public string? Responsible
        {
            get => _responsible;
            set
            {
                _responsible = value;
                OnPropertyChanged(nameof(Responsible));
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

        [Required(ErrorMessage = "Ngày di chuyển không được để trống")]
        public string? MovementDate
        {
            get => _movementDate;
            set
            {
                _movementDate = value;
                OnPropertyChanged(nameof(MovementDate));
            }
        }

        public string? MovementDescription
        {
            get => _movementDescription;
            set
            {
                _movementDescription = value;
                OnPropertyChanged(nameof(MovementDescription));
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
            OnPropertyChanged(nameof(MovementCode));
            OnPropertyChanged(nameof(OrderTypeCode));
            OnPropertyChanged(nameof(WarehouseCode));
            OnPropertyChanged(nameof(FromLocation));
            OnPropertyChanged(nameof(ToLocation));
            OnPropertyChanged(nameof(Responsible));
            OnPropertyChanged(nameof(StatusId));
            OnPropertyChanged(nameof(MovementDate));
            OnPropertyChanged(nameof(MovementDescription));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(MovementCode));
            OnPropertyChanged(nameof(OrderTypeCode));
            OnPropertyChanged(nameof(WarehouseCode));
            OnPropertyChanged(nameof(FromLocation));
            OnPropertyChanged(nameof(ToLocation));
            OnPropertyChanged(nameof(Responsible));
            OnPropertyChanged(nameof(StatusId));
            OnPropertyChanged(nameof(MovementDate));
            OnPropertyChanged(nameof(MovementDescription));
        }
    }
}