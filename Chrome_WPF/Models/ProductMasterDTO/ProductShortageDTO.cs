using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.ProductMasterDTO
{
    public class ProductShortageDTO
    {
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public double RequiredQuantity { get; set; }
        public double ShortageQuantity { get; set; }
        public string? WarehouseCode { get; set; }
    }
}
