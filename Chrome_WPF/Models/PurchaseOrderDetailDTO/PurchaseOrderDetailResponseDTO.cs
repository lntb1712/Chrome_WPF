using Chrome_WPF.Models.ProductMasterDTO;
using System.ComponentModel;

namespace Chrome_WPF.Models.PurchaseOrderDetailDTO
{
    public class PurchaseOrderDetailResponseDTO : INotifyPropertyChanged
    {
        private string _purchaseOrderCode = string.Empty;
        private string _productCode = string.Empty;
        private string _productName = string.Empty;
        private double? _quantity;
        private double? _quantityReceived;
        private bool _isNewRow;

        public string PurchaseOrderCode
        {
            get => _purchaseOrderCode;
            set { _purchaseOrderCode = value; OnPropertyChanged(nameof(PurchaseOrderCode)); }
        }

        public string ProductCode
        {
            get => _productCode;
            set { _productCode = value; OnPropertyChanged(nameof(ProductCode)); }
        }

        public string ProductName
        {
            get => _productName;
            set { _productName = value; OnPropertyChanged(nameof(ProductName)); }
        }

        public double? Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(nameof(Quantity)); }
        }
        public double? QuantityReceived
        {
            get => _quantityReceived;
            set { _quantityReceived = value; OnPropertyChanged(nameof(QuantityReceived)); }
        }

        public bool IsNewRow
        {
            get => _isNewRow;
            set { _isNewRow = value; OnPropertyChanged(nameof(IsNewRow)); }
        }
        public ProductMasterResponseDTO? SelectedProduct { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
