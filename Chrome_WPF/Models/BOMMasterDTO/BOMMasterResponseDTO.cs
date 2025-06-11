using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Chrome_WPF.Models.BOMMasterDTO
{
    public class BOMMasterResponseDTO : INotifyPropertyChanged
    {
        private string _bomCode = string.Empty;
        private string? _productCode;
        private string? _productName;

        public string BOMCode
        {
            get => _bomCode;
            set
            {
                _bomCode = value;
                OnPropertyChanged(nameof(BOMCode));
            }
        }

        public string? ProductCode
        {
            get => _productCode;
            set
            {
                _productCode = value;
                OnPropertyChanged(nameof(ProductCode));
            }
        }

        public string? ProductName
        {
            get => _productName;
            set
            {
                _productName = value;
                OnPropertyChanged(nameof(ProductName));
            }
        }

        public ObservableCollection<BOMVersionResponseDTO>? BOMVersionResponses { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
