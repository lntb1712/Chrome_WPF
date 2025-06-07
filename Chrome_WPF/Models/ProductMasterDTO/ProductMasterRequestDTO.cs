using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.ProductMasterDTO
{
    public class ProductMasterRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _productCode = string.Empty;
        private string _productName = string.Empty;
        private string _productDescription = string.Empty;
        private string _productImg = string.Empty;
        private string _categoryId = string.Empty;
        private double _baseQuantity = 0.00;
        private string _uom = string.Empty;
        private string _baseUOM = string.Empty;
        private double _valuation = 0.00;

        private bool _isValidationRequested;


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        public string ProductName
        {
            get => _productName;
            set
            {
                _productName = value;
                OnPropertyChanged(nameof(ProductName));
            }
        }

        public string ProductDescription
        {
            get => _productDescription;
            set
            {
                _productDescription = value;
                OnPropertyChanged(nameof(ProductDescription));
            }
        }

        public string ProductImg
        {
            get => _productImg;
            set
            {
                _productImg = value;
                OnPropertyChanged(nameof(ProductImg));
            }
        }

        public string CategoryId
        {
            get => _categoryId;
            set
            {
                _categoryId = value;
                OnPropertyChanged(nameof(CategoryId));
            }
        }

        public double BaseQuantity
        {
            get => _baseQuantity;
            set
            {
                _baseQuantity = value;
                OnPropertyChanged(nameof(BaseQuantity));
            }
        }

        public string UOM
        {
            get => _uom;
            set
            {
                _uom = value;
                OnPropertyChanged(nameof(UOM));
            }
        }

        public string BaseUOM
        {
            get => _baseUOM;
            set
            {
                _baseUOM = value;
                OnPropertyChanged(nameof(BaseUOM));
            }
        }
        public double Valuation
        {
            get => _valuation;
            set
            {
                _valuation = value;
                OnPropertyChanged(nameof(Valuation));
            }
        }

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                if (!_isValidationRequested)
                    return string.Empty;
                if (columnName == nameof(ProductDescription) 
                    || columnName == nameof(CategoryId)
                    || columnName == nameof(ProductImg)
                    || columnName == nameof(BaseQuantity)
                    || columnName == nameof(UOM)
                    || columnName == nameof(BaseUOM)
                    || columnName == nameof(Valuation))
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
            OnPropertyChanged(nameof(ProductCode));
            OnPropertyChanged(nameof(ProductName));
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(ProductName));
            OnPropertyChanged(nameof(ProductCode));
        } 


    }
}
