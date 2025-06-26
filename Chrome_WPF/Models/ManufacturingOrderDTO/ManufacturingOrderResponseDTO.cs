using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.ManufacturingOrderDTO
{
    public class ManufacturingOrderResponseDTO
    {
        public string ManufacturingOrderCode { get; set; } = null!;

        public string? OrderTypeCode { get; set; }
        public string? OrderTypeName { get; set; }

        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;

        public string Bomcode { get; set; } = null!;

        public string BomVersion { get; set; } = null!;

        public int? Quantity { get; set; }

        public int? QuantityProduced { get; set; }

        public string? ScheduleDate { get; set; }

        public string? Deadline { get; set; }

        public string? Responsible { get; set; }
        public string? FullNameResponsible { get; set; }

        public string? Lotno { get; set; }

        public int? StatusId { get; set; }
        public string StatusName { get; set; } = null!;

        public string? WarehouseCode { get; set; }
        public string WarehouseName { get; set; } = null!;
    }
}
