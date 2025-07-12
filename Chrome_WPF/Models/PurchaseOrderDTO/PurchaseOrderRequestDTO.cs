using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome_WPF.Models.PurchaseOrderDTO
{
    public class PurchaseOrderRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _purchaseOrderCode = string.Empty;
        private string _warehouseCode = string.Empty;
        private int _statusId = 1;
        private string _orderDate = string.Empty;
        private string _expectedDate = string.Empty;
        private string _supplierCode = string.Empty;
        private string _purchaseOrderDescription = string.Empty;
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
        public int StatusId
        {
            get => _statusId;
            set
            {
                _statusId = value;
                OnPropertyChanged(nameof(StatusId));
            }
        }

        [Required(ErrorMessage = "Mã kho không được để trống")]
        public string WarehouseCode
        {
            get => _warehouseCode;
            set
            {
                _warehouseCode = value;
                OnPropertyChanged(nameof(WarehouseCode));
            }
        }

        [Required(ErrorMessage = "Ngày đặt hàng không được để trống")]
        public string OrderDate
        {
            get => _orderDate;
            set
            {
                _orderDate = value;
                OnPropertyChanged(nameof(OrderDate));
            }
        }

        [Required(ErrorMessage = "Ngày dự kiến nhận hàng không được để trống")]
        public string ExpectedDate
        {
            get => _expectedDate;
            set
            {
                _expectedDate = value;
                OnPropertyChanged(nameof(ExpectedDate));
            }
        }

        [Required(ErrorMessage = "Mã nhà cung cấp không được để trống")]
        public string SupplierCode
        {
            get => _supplierCode;
            set
            {
                _supplierCode = value;
                OnPropertyChanged(nameof(SupplierCode));
            }
        }

        public string PurchaseOrderDescription
        {
            get => _purchaseOrderDescription;
            set
            {
                _purchaseOrderDescription = value;
                OnPropertyChanged(nameof(PurchaseOrderDescription));
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
            OnPropertyChanged(nameof(WarehouseCode));
            OnPropertyChanged(nameof(OrderDate));
            OnPropertyChanged(nameof(ExpectedDate));
            OnPropertyChanged(nameof(SupplierCode));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(PurchaseOrderCode));
            OnPropertyChanged(nameof(WarehouseCode));
            OnPropertyChanged(nameof(OrderDate));
            OnPropertyChanged(nameof(ExpectedDate));
            OnPropertyChanged(nameof(SupplierCode));
        }
    }
}
