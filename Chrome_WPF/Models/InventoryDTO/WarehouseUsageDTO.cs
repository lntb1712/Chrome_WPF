using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.InventoryDTO
{
    public class WarehouseUsageDTO
    {
        public string? WarehouseCode { get; set; }
        public string? WarehouseName { get; set; }
        public List<LocationUsageDTO> locationUsageDTOs { get; set; } = new List<LocationUsageDTO>();
    }
}
