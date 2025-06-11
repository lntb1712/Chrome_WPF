using Chrome_WPF.Models.ProductMasterDTO;
using DocumentFormat.OpenXml.Bibliography;
using System;
using System.ComponentModel;

namespace Chrome_WPF.Models.BOMComponentDTO
{
    public class BOMComponentResponseDTO : INotifyPropertyChanged
    {
        private string _bomCode = string.Empty;
        private string _componentCode = string.Empty;
        private string _componentName = string.Empty;
        private string _bomVersion = string.Empty;
        private double? _consumpQuantity;
        private double? _scrapRate;
        private bool _isNewRow;
        private ProductMasterResponseDTO? _selectedComponent;

        public string BOMCode
        {
            get => _bomCode;
            set
            {
                _bomCode = value;
                OnPropertyChanged(nameof(BOMCode));
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
                _componentName = value;
                OnPropertyChanged(nameof(ComponentName));
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

        public double? ConsumpQuantity
        {
            get => _consumpQuantity;
            set
            {
                _consumpQuantity = value;
                OnPropertyChanged(nameof(ConsumpQuantity));
            }
        }

        public double? ScrapRate
        {
            get => _scrapRate;
            set
            {
                _scrapRate = value;
                OnPropertyChanged(nameof(ScrapRate));
            }
        }
        public bool IsNewRow
        {
            get => _isNewRow;
            set { _isNewRow = value; OnPropertyChanged(nameof(IsNewRow)); }
        }
        public ProductMasterResponseDTO? SelectedComponent
        {
            get => _selectedComponent;
            set
            {
                _selectedComponent = value;
               
                OnPropertyChanged(nameof(SelectedComponent));
                ComponentCode = value?.ProductCode ?? string.Empty;
                ComponentName = value?.ProductName ?? string.Empty;
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;


        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
