using Chrome_WPF.Models.SupplierMasterDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.ProductSupplierDTO
{
    public class ProductSupplierResponseDTO : INotifyPropertyChanged
    {
        private string _productCode = string.Empty;
        private string _supplierCode =string.Empty;
        private string _supplierName = string.Empty;
        private double _quantity =0.00;
        private int _leadTime;
        private string _updateTime = string.Empty;
        private bool _isNewRow;
        private SupplierMasterResponseDTO? _selectedSupplier;

    

        public string ProductCode
        {
            get => _productCode;
            set
            {
                _productCode = value;
                OnPropertyChanged(nameof(ProductCode));
            }
        }

        public string SupplierCode
        {
            get => _supplierCode;
            set
            {
                _supplierCode = value;
                OnPropertyChanged(nameof(SupplierCode));
            }
        }

        public string SupplierName
        {
            get => _supplierName;
            set
            {
                _supplierName = value;
                OnPropertyChanged(nameof(SupplierName));
            }
        }

        public double Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
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

        public string UpdateTime
        {
            get => _updateTime;
            set
            {
                _updateTime = value;
                OnPropertyChanged(nameof(UpdateTime));
            }
        }
        public bool IsNewRow
        {
            get => _isNewRow;
            set { _isNewRow = value; OnPropertyChanged(nameof(IsNewRow)); }
        }
        public SupplierMasterResponseDTO? SelectedSupplier
        {
            get => _selectedSupplier;
            set
            {
                if (_selectedSupplier != value)
                {
                    _selectedSupplier = value;
                    OnPropertyChanged(nameof(SelectedSupplier));

                    // Tự động cập nhật mã và tên khi chọn từ ComboBox
                    SupplierCode = value?.SupplierCode ?? string.Empty;
                    SupplierName = value?.SupplierName ?? string.Empty;
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
