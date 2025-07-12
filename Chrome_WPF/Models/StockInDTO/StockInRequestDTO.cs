using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.StockInDTO
{
    public class StockInRequestDTO:INotifyPropertyChanged,IDataErrorInfo
    {
        private string _stockInCode = string.Empty;
        private string _orderTypeCode = string.Empty;
        private string _warehouseCode = string.Empty;
        private string _purchaseOrderCode = string.Empty;
        private string _responsible = string.Empty;
        private int _statusId = 1;
        private string _orderDeadLine = string.Empty;
        private string _stockInDescription = string.Empty;

        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Mã nhập kho không được để trống")]
        public string StockInCode
        {
            get => _stockInCode;
            set
            {
                _stockInCode = value;
                OnPropertyChanged(nameof(StockInCode));
            }
        }
        [Required(ErrorMessage = "Mã loại lệnh không được để trống")]
        public string OrderTypeCode
        {
            get =>_orderTypeCode;
            set
            {
                _orderTypeCode = value;
                OnPropertyChanged(nameof(OrderTypeCode));
            }
        }
        [Required(ErrorMessage = "Mã kho nhập không được để trống")]
        public string WarehouseCode
        {
            get => _warehouseCode;
            set
            {
                _warehouseCode = value;
                OnPropertyChanged(nameof(WarehouseCode));
            }
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

        [Required(ErrorMessage = "Tên nhân viên chịu trách nhiệm không được để trống")]
        public string Responsible
        {
            get => _responsible;
            set
            {
                _responsible = value;
                OnPropertyChanged(nameof(Responsible));
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
        [Required(ErrorMessage = "Hạn nhập hàng không được để trống")]
        public string OrderDeadLine
        {
            get => _orderDeadLine;
            set
            {
                _orderDeadLine = value;
                OnPropertyChanged(nameof(OrderDeadLine));
            }
        }

        public string StockInDescription
        {
            get=> _stockInDescription;
            set
            {
                _stockInDescription = value;
                OnPropertyChanged(nameof(StockInDescription));
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

        public void RequestValidation ()
        {
            _isValidationRequested = true;
            OnPropertyChanged(nameof(StockInCode));
            OnPropertyChanged(nameof(OrderTypeCode));
            OnPropertyChanged(nameof(WarehouseCode));
            OnPropertyChanged(nameof(PurchaseOrderCode));
            OnPropertyChanged(nameof(Responsible));
            OnPropertyChanged(nameof(OrderDeadLine));
        }

        public void ClearValidation ()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(StockInCode));
            OnPropertyChanged(nameof(OrderTypeCode));
            OnPropertyChanged(nameof(WarehouseCode));
            OnPropertyChanged(nameof(PurchaseOrderCode));
            OnPropertyChanged(nameof(Responsible));
            OnPropertyChanged(nameof(OrderDeadLine));
        }
    }
}
