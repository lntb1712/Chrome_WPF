using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome_WPF.Models.StocktakeDTO
{
    public class StockTakeRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _stocktakeCode = string.Empty;
        private string? _stocktakeDate;
        private string? _warehouseCode;
        private string? _responsible;
        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Mã kiểm kê không được để trống")]
        public string StockTakeCode
        {
            get => _stocktakeCode;
            set
            {
                if (_stocktakeCode != value)
                {
                    _stocktakeCode = value;
                    OnPropertyChanged(nameof(StockTakeCode));
                }
            }
        }

        public string? StocktakeDate
        {
            get => _stocktakeDate;
            set
            {
                if (_stocktakeDate != value)
                {
                    _stocktakeDate = value;
                    OnPropertyChanged(nameof(StocktakeDate));
                }
            }
        }

        [Required(ErrorMessage = "Kho kiểm kê không được để trống")]
        public string? WarehouseCode
        {
            get => _warehouseCode;
            set
            {
                if (_warehouseCode != value)
                {
                    _warehouseCode = value;
                    OnPropertyChanged(nameof(WarehouseCode));
                }
            }
        }

        public string? Responsible
        {
            get => _responsible;
            set
            {
                if (_responsible != value)
                {
                    _responsible = value;
                    OnPropertyChanged(nameof(Responsible));
                }
            }
        }

     

        // Validation xử lý
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

        // Hàm trigger validate toàn bộ
        public void RequestValidation()
        {
            _isValidationRequested = true;
            OnPropertyChanged(nameof(StockTakeCode));
            OnPropertyChanged(nameof(StocktakeDate));
            OnPropertyChanged(nameof(WarehouseCode));
            OnPropertyChanged(nameof(Responsible));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(StockTakeCode));
            OnPropertyChanged(nameof(StocktakeDate));
            OnPropertyChanged(nameof(WarehouseCode));
            OnPropertyChanged(nameof(Responsible));
        }
    }
}

