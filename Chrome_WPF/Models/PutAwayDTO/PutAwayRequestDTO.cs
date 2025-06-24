using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome_WPF.Models.PutAwayDTO
{
    public class PutAwayRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _putAwayCode = string.Empty;
        private string? _orderTypeCode;
        private string? _locationCode;
        private string? _responsible;
        private int? _statusId;
        private string? _putAwayDate;
        private string? _putAwayDescription;
        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Mã để hàng không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Mã để hàng chỉ được chứa chữ, số và dấu gạch dưới")]
        public string PutAwayCode
        {
            get => _putAwayCode;
            set
            {
                _putAwayCode = value;
                OnPropertyChanged(nameof(PutAwayCode));
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

        [Required(ErrorMessage = "Ngày để hàng không được để trống")]
        public string? PutAwayDate
        {
            get => _putAwayDate;
            set
            {
                _putAwayDate = value;
                OnPropertyChanged(nameof(PutAwayDate));
            }
        }

        public string? PutAwayDescription
        {
            get => _putAwayDescription;
            set
            {
                _putAwayDescription = value;
                OnPropertyChanged(nameof(PutAwayDescription));
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
            OnPropertyChanged(nameof(PutAwayCode));
            OnPropertyChanged(nameof(OrderTypeCode));
            OnPropertyChanged(nameof(LocationCode));
            OnPropertyChanged(nameof(Responsible));
            OnPropertyChanged(nameof(StatusId));
            OnPropertyChanged(nameof(PutAwayDate));
            OnPropertyChanged(nameof(PutAwayDescription));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(PutAwayCode));
            OnPropertyChanged(nameof(OrderTypeCode));
            OnPropertyChanged(nameof(LocationCode));
            OnPropertyChanged(nameof(Responsible));
            OnPropertyChanged(nameof(StatusId));
            OnPropertyChanged(nameof(PutAwayDate));
            OnPropertyChanged(nameof(PutAwayDescription));
        }
    }
}