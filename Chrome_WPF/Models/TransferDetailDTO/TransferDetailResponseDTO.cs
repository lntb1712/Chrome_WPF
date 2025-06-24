using Chrome_WPF.Models.InventoryDTO;
using Chrome_WPF.Models.ProductMasterDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.TransferDetailDTO
{
    public class TransferDetailResponseDTO:INotifyPropertyChanged
    {
        public string TransferCode { get; set; } = null!;

        public string ProductCode { get; set; } = null!;

        public string ProductName { get; set; } = null!;

        public double? Demand { get; set; }

        public double? QuantityInBounded { get; set; }

        public double? QuantityOutBounded { get; set; }

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
        public InventorySummaryDTO? SelectedProduct { get; set; }
    }
}
