using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.WarehouseMasterDTO
{
    public class WarehouseMasterResponseDTO
    {
        public string WarehouseCode { get; set; } = null!;

        public string? WarehouseName { get; set; }

        public string? WarehouseDescription { get; set; }

        public string? WarehouseAddress { get; set; }
    }
}
