using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.InventoryDTO
{
    public class LocationUsageDTO
    {
        public string? LocationCode { get; set; }
        public string? LocationName { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public double? Quantity { get; set; }
        public double UsedPercentage { get; set; }
    }
}
