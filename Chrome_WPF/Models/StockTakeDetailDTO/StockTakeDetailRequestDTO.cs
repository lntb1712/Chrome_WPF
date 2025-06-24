using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome_WPF.Models.StocktakeDTO
{
    public class StockTakeDetailRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _stocktakeCode = string.Empty;
        private string _productCode = string.Empty;
        private string _lotNo = string.Empty;
        private string _locationCode = string.Empty;
        private double? _quantity;
        private double? _countedQuantity;
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

        [Required(ErrorMessage = "Mã sản phẩm không được để trống")]
        public string ProductCode
        {
            get => _productCode;
            set
            {
                if (_productCode != value)
                {
                    _productCode = value;
                    OnPropertyChanged(nameof(ProductCode));
                }
            }
        }

        [Required(ErrorMessage = "Số lô không được để trống")]
        public string LotNo
        {
            get => _lotNo;
            set
            {
                if (_lotNo != value)
                {
                    _lotNo = value;
                    OnPropertyChanged(nameof(LotNo));
                }
            }
        }

        [Required(ErrorMessage = "Vị trí không được để trống")]
        public string LocationCode
        {
            get => _locationCode;
            set
            {
                if (_locationCode != value)
                {
                    _locationCode = value;
                    OnPropertyChanged(nameof(LocationCode));
                }
            }
        }

        [Range(0, double.MaxValue, ErrorMessage = "Tồn kho phải là số không âm")]
        public double? Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }

        [Range(0, double.MaxValue, ErrorMessage = "Số lượng kiểm kê phải là số không âm")]
        public double? CountedQuantity
        {
            get => _countedQuantity;
            set
            {
                if (_countedQuantity != value)
                {
                    _countedQuantity = value;
                    OnPropertyChanged(nameof(CountedQuantity));
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
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(LotNo));
            OnPropertyChanged(nameof(LocationCode));
            OnPropertyChanged(nameof(Quantity));
            OnPropertyChanged(nameof(CountedQuantity));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(StockTakeCode));
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(LotNo));
            OnPropertyChanged(nameof(LocationCode));
            OnPropertyChanged(nameof(Quantity));
            OnPropertyChanged(nameof(CountedQuantity));
        }
    }
}
