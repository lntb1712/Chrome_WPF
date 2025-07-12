using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome_WPF.Models.PurchaseOrderDetailDTO
{
    public class PurchaseOrderDetailRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _purchaseOrderCode = string.Empty;
        private string _productCode = string.Empty;
        private double _quantity = 0;
        private double _quantityReceived = 0;
        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Mã phiếu đặt hàng không được để trống")]
        public string PurchaseOrderCode
        {
            get => _purchaseOrderCode;
            set
            {
                _purchaseOrderCode = value;
                OnPropertyChanged(nameof(PurchaseOrderCode));
            }
        }

        [Required(ErrorMessage = "Mã sản phẩm không được để trống")]
        public string ProductCode
        {
            get => _productCode;
            set
            {
                _productCode = value;
                OnPropertyChanged(nameof(ProductCode));
            }
        }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public double Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
            }
        }
        [Required(ErrorMessage = "Số lượng đã nhận không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public double QuantityReceived
        {
            get => _quantityReceived;
            set
            {
                _quantityReceived = value;
                OnPropertyChanged(nameof(QuantityReceived));
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
            OnPropertyChanged(nameof(PurchaseOrderCode));
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(Quantity));
            OnPropertyChanged(nameof(QuantityReceived));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(PurchaseOrderCode));
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(QuantityReceived));
            OnPropertyChanged(nameof(QuantityReceived));
        }
    }
}
