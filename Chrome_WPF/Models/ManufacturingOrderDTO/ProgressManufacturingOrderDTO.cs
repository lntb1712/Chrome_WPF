using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.ManufacturingOrderDTO
{
    public class ProgressManufacturingOrderDTO
    {
        public string ManufacturingOrderCode { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public int? Quantity { get; set; }

        public int? QuantityProduced { get; set; }
        public string? StatusName { get; set; } = null!;
        public string? WarehouseCode { get; set; }
        public string WarehouseName { get; set; } = null!;
        public double? Progress { get; set; } = null!;
    }
}
