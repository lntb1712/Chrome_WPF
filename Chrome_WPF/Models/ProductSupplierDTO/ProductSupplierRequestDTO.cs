using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.ProductSupplierDTO
{
    public class ProductSupplierRequestDTO :INotifyPropertyChanged,IDataErrorInfo
    {
        private string _supplierCode = string.Empty;
        private string _productCode = string.Empty;
        private int _leadTime = 0;
        private double? _pricePerUnit = 0;

        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage ="Mã nhà cung cấp không được để trống")]
        public string SupplierCode
        {
            get=> _supplierCode;
            set
            {
                _supplierCode = value;
                OnPropertyChanged(nameof(SupplierCode));
            }
        }
        [Required(ErrorMessage ="Mã sản phẩm không được để trống")]
        public string ProductCode
        {
            get => _productCode;
            set
            {
                _productCode = value;
                OnPropertyChanged(nameof(ProductCode));
            }
        }

        public int LeadTime
        {
            get => _leadTime;
            set
            {
                _leadTime = value;
                OnPropertyChanged(nameof(LeadTime));
            }
        }

        public double? PricePerUnit
        {
            get => _pricePerUnit;
            set
            {
                _pricePerUnit = value;
                OnPropertyChanged(nameof(PricePerUnit));
            }
        }

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                if(!_isValidationRequested) 
                    return string.Empty;
                if(columnName == nameof(LeadTime)
                   || columnName == nameof(PricePerUnit))
                {
                    return string.Empty;
                }
                var property = GetType().GetProperty(columnName);
                if (property==null) return string.Empty;

                var value = property.GetValue(this);
                var context = new ValidationContext(this) { MemberName=columnName };
                var results= new List<ValidationResult>();  

                bool isValid = Validator.TryValidateProperty(value, context,results);

                return isValid ? string.Empty : results.FirstOrDefault()?.ErrorMessage ?? string.Empty;

            }
        }

        public void RequestValidation()
        {
            _isValidationRequested = true;
            OnPropertyChanged(nameof(SupplierCode));
            OnPropertyChanged(nameof(ProductCode));
        }

        public void ClearValidation()
        {
            _isValidationRequested= false;
            OnPropertyChanged(nameof(SupplierCode));
            OnPropertyChanged(nameof(ProductCode));
        }
    }
}
