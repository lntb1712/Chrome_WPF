using Chrome_WPF.Models.CustomerMasterDTO;
using System;
using System.ComponentModel;

namespace Chrome_WPF.Models.ProductCustomerDTO
{
    public class ProductCustomerResponseDTO : INotifyPropertyChanged
    {
        private string _productCode = string.Empty;
        private string _customerCode = string.Empty;
        private string _customerName = string.Empty;
        private int? _expectedDeliverTime;
        private double? _pricePerUnit;
        private bool _isNewRow;
        private CustomerMasterResponseDTO? _selectedCustomer;

        public string ProductCode
        {
            get => _productCode;
            set
            {
                _productCode = value;
                OnPropertyChanged(nameof(ProductCode));
            }
        }

        public string CustomerCode
        {
            get => _customerCode;
            set
            {
                _customerCode = value;
                OnPropertyChanged(nameof(CustomerCode));
            }
        }

        public string CustomerName
        {
            get => _customerName;
            set
            {
                _customerName = value;
                OnPropertyChanged(nameof(CustomerName));
            }
        }

        public int? ExpectedDeliverTime
        {
            get => _expectedDeliverTime;
            set
            {
                _expectedDeliverTime = value;
                OnPropertyChanged(nameof(ExpectedDeliverTime));
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
        public bool IsNewRow
        {
            get => _isNewRow;
            set
            {
                _isNewRow = value;
                OnPropertyChanged(nameof(IsNewRow));
            }
        }

        public CustomerMasterResponseDTO? SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                if (_selectedCustomer != value)
                {
                    _selectedCustomer = value;
                    OnPropertyChanged(nameof(SelectedCustomer));

                    // Tự động cập nhật mã và tên khi chọn từ ComboBox
                    CustomerCode = value?.CustomerCode ?? string.Empty;
                    CustomerName = value?.CustomerName ?? string.Empty;
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