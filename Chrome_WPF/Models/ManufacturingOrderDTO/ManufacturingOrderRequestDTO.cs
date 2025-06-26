using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome_WPF.Models.ManufacturingOrderDTO
{
    public class ManufacturingOrderRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _manufacturingOrderCode = string.Empty;
        private string? _orderTypeCode;
        private string _productCode = string.Empty;
        private string _bomcode = string.Empty;
        private string? _bomVersion;
        private int? _quantity;
        private int? _quantityProduced;
        private string? _scheduleDate;
        private string? _deadline;
        private string? _responsible;
        private int? _statusId;
        private string? _warehouseCode;
        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Mã lệnh sản xuất không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Mã lệnh sản xuất chỉ được chứa chữ, số và dấu gạch dưới")]
        public string ManufacturingOrderCode
        {
            get => _manufacturingOrderCode;
            set
            {
                _manufacturingOrderCode = value;
                OnPropertyChanged(nameof(ManufacturingOrderCode));
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

        [Required(ErrorMessage = "Mã sản phẩm không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Mã sản phẩm chỉ được chứa chữ, số và dấu gạch dưới")]
        public string ProductCode
        {
            get => _productCode;
            set
            {
                _productCode = value;
                OnPropertyChanged(nameof(ProductCode));
            }
        }

        [Required(ErrorMessage = "Mã BOM không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Mã BOM chỉ được chứa chữ, số và dấu gạch dưới")]
        public string Bomcode
        {
            get => _bomcode;
            set
            {
                _bomcode = value;
                OnPropertyChanged(nameof(Bomcode));
            }
        }

        public string? BomVersion
        {
            get => _bomVersion;
            set
            {
                _bomVersion = value;
                OnPropertyChanged(nameof(BomVersion));
            }
        }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải là một số nguyên dương")]
        public int? Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
            }
        }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng sản xuất phải là một số nguyên không âm")]
        public int? QuantityProduced
        {
            get => _quantityProduced;
            set
            {
                _quantityProduced = value;
                OnPropertyChanged(nameof(QuantityProduced));
            }
        }

        [Required(ErrorMessage = "Ngày bắt đầu sản xuất không được để trống")]
        [RegularExpression(@"^(?:\d{1,2}/\d{1,2}/\d{4}|\d{1,2}/\d{1,2}/\d{4}\s+\d{1,2}:\d{2}:\d{2}\s+(?:AM|PM))$",
            ErrorMessage = "Ngày bắt đầu sản xuất phải có định dạng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt")]
        public string? ScheduleDate
        {
            get => _scheduleDate;
            set
            {
                _scheduleDate = value;
                OnPropertyChanged(nameof(ScheduleDate));
            }
        }

        [Required(ErrorMessage = "Ngày hết hạn không được để trống")]
        [RegularExpression(@"^(?:\d{1,2}/\d{1,2}/\d{4}|\d{1,2}/\d{1,2}/\d{4}\s+\d{1,2}:\d{2}:\d{2}\s+(?:AM|PM))$",
            ErrorMessage = "Ngày hết hạn phải có định dạng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt")]
        public string? Deadline
        {
            get => _deadline;
            set
            {
                _deadline = value;
                OnPropertyChanged(nameof(Deadline));
            }
        }

        [Required(ErrorMessage = "Người phụ trách không được để trống")]
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

        [Required(ErrorMessage = "Mã kho không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Mã kho chỉ được chứa chữ, số và dấu gạch dưới")]
        public string? WarehouseCode
        {
            get => _warehouseCode;
            set
            {
                _warehouseCode = value;
                OnPropertyChanged(nameof(WarehouseCode));
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

                // Additional custom validation for dates
                if (isValid && (columnName == nameof(ScheduleDate) || columnName == nameof(Deadline)))
                {
                    string[] formats = { "dd/MM/yyyy", "M/d/yyyy h:mm:ss tt" };
                    if (value != null && !DateTime.TryParseExact(value.ToString(), formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out _))
                    {
                        results.Add(new ValidationResult($"{columnName} không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt."));
                        isValid = false;
                    }
                }

                // Validate ScheduleDate <= Deadline
                if (isValid && columnName == nameof(Deadline) && !string.IsNullOrEmpty(ScheduleDate) && !string.IsNullOrEmpty(Deadline))
                {
                    string[] formats = { "dd/MM/yyyy", "M/d/yyyy h:mm:ss tt" };
                    if (DateTime.TryParseExact(ScheduleDate, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime scheduleDate) &&
                        DateTime.TryParseExact(Deadline, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime deadline))
                    {
                        if (scheduleDate > deadline)
                        {
                            results.Add(new ValidationResult("Ngày bắt đầu sản xuất không thể lớn hơn ngày hết hạn."));
                            isValid = false;
                        }
                    }
                }

                return isValid ? string.Empty : results.FirstOrDefault()?.ErrorMessage ?? string.Empty;
            }
        }

        public void RequestValidation()
        {
            _isValidationRequested = true;
            OnPropertyChanged(nameof(ManufacturingOrderCode));
            OnPropertyChanged(nameof(OrderTypeCode));
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(Bomcode));
            OnPropertyChanged(nameof(BomVersion));
            OnPropertyChanged(nameof(Quantity));
            OnPropertyChanged(nameof(QuantityProduced));
            OnPropertyChanged(nameof(ScheduleDate));
            OnPropertyChanged(nameof(Deadline));
            OnPropertyChanged(nameof(Responsible));
            OnPropertyChanged(nameof(StatusId));
            OnPropertyChanged(nameof(WarehouseCode));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(ManufacturingOrderCode));
            OnPropertyChanged(nameof(OrderTypeCode));
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(Bomcode));
            OnPropertyChanged(nameof(BomVersion));
            OnPropertyChanged(nameof(Quantity));
            OnPropertyChanged(nameof(QuantityProduced));
            OnPropertyChanged(nameof(ScheduleDate));
            OnPropertyChanged(nameof(Deadline));
            OnPropertyChanged(nameof(Responsible));
            OnPropertyChanged(nameof(StatusId));
            OnPropertyChanged(nameof(WarehouseCode));
        }
    }
}