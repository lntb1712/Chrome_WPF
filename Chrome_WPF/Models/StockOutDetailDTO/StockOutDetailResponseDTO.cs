using Chrome_WPF.Models.ProductMasterDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.StockOutDetailDTO
{
    public class StockOutDetailResponseDTO: INotifyPropertyChanged
    {
        public string StockOutCode { get; set; } = null!;

        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;

        public double? Demand { get; set; }

        public double? Quantity { get; set; }
        private bool _isNewRow;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsNewRow
        {
            get => _isNewRow;
            set { _isNewRow = value; OnPropertyChanged(nameof(IsNewRow)); }
        }
        public ProductMasterResponseDTO? SelectedProduct { get; set; }
    }
}
