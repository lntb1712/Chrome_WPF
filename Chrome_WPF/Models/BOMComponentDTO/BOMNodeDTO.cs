using System;
using System.ComponentModel;

namespace Chrome_WPF.Models.BOMComponentDTO
{
    public class BOMNodeDTO : INotifyPropertyChanged
    {
        private string _bomCode = string.Empty;
        private string _productCode = string.Empty;
        private string _productName = string.Empty;
        private string _bomVersion = string.Empty;
        private string _componentCode = string.Empty;
        private string _componentName = string.Empty;
        private float _totalQuantity;
        private int _level;

        public string BOMCode
        {
            get => _bomCode;
            set
            {
                _bomCode = value;
                OnPropertyChanged(nameof(BOMCode));
            }
        }

        public string ProductCode
        {
            get => _productCode;
            set
            {
                _productCode = value;
                OnPropertyChanged(nameof(ProductCode));
            }
        }
        public string ProductName
        {
            get => _productName;
            set
            {
                _productName = value;
                OnPropertyChanged(nameof(ProductName));
            }
        }

        public string BOMVersion
        {
            get => _bomVersion;
            set
            {
                _bomVersion = value;
                OnPropertyChanged(nameof(BOMVersion));
            }
        }

        public string ComponentCode
        {
            get => _componentCode;
            set
            {
                _componentCode = value;
                OnPropertyChanged(nameof(ComponentCode));
            }
        }
        public string ComponentName
        {
            get => _componentName;
            set
            {
                _componentName=value;
                OnPropertyChanged(nameof(ComponentName));
            }
        }

        public float TotalQuantity
        {
            get => _totalQuantity;
            set
            {
                _totalQuantity = value;
                OnPropertyChanged(nameof(TotalQuantity));
            }
        }

        public int Level
        {
            get => _level;
            set
            {
                _level = value;
                OnPropertyChanged(nameof(Level));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
