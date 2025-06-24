using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.MovementDTO
{
    public class MovementResponseDTO
    {
        public string MovementCode { get; set; } = null!;

        public string? OrderTypeCode { get; set; }
        public string? OrderTypeName { get; set; }

        public string? WarehouseCode { get; set; }
        public string? WarehouseName { get; set; }

        public string? FromLocation { get; set; }
        public string? FromLocationName { get; set; }

        public string? ToLocation { get; set; }
        public string? ToLocationName { get; set; }

        public string? Responsible { get; set; }
        public string? FullNameResponible { get; set; }

        public int? StatusId { get; set; }
        public string? StatusName { get; set; }

        public string? MovementDate { get; set; }

        public string? MovementDescription { get; set; }
    }
}
