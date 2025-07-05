using Chrome_WPF.Models.ProductMasterDTO;
using System.ComponentModel;

namespace Chrome_WPF.Models.StockOutDetailDTO
{
    public class StockOutDetailResponseDTO : INotifyPropertyChanged
    {
        private ProductMasterResponseDTO? _selectedProduct;
        private string? _productCode;
        private string? _productName;
        private decimal _demand;
        private decimal _quantity;
        private bool _isNewRow;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ProductMasterResponseDTO SelectedProduct
        {
            get => _selectedProduct!;
            set
            {
                _selectedProduct = value;
                ProductCode = value?.ProductCode ?? string.Empty;
                ProductName = value?.ProductName ?? string.Empty;
                OnPropertyChanged(nameof(SelectedProduct));
            }
        }

        public string ProductCode
        {
            get => _productCode!;
            set
            {
                _productCode = value;
                OnPropertyChanged(nameof(ProductCode));
            }
        }

        public string ProductName
        {
            get => _productName!;
            set
            {
                _productName = value;
                OnPropertyChanged(nameof(ProductName));
            }
        }

        public decimal Demand
        {
            get => _demand;
            set
            {
                _demand = value;
                OnPropertyChanged(nameof(Demand));
            }
        }

        public decimal Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
            }
        }

        public bool IsNewRow
        {
            get => _isNewRow;
            set
            {
                _isNewRow = value;
                OnPropertyChanged(nameof(IsNewRow));
            }
        }

        public string? StockOutCode { get; set; }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}