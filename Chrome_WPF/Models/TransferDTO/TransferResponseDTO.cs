using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.TransferDTO
{
    public class TransferResponseDTO
    {
        public string TransferCode { get; set; } = null!;

        public string? OrderTypeCode { get; set; }
        public string? OrderTypeName { get; set; }

        public string? FromWarehouseCode { get; set; }
        public string? FromWarehouseName { get; set; }

        public string? ToWarehouseCode { get; set; }
        public string? ToWarehouseName { get; set; }

        public string? ToResponsible { get; set; }
        public string? FullNameToResponsible { get; set; }

        public string? FromResponsible { get; set; }
        public string? FullNameFromResponsible { get; set; }

        public int? StatusId { get; set; }
        public string? StatusName { get; set; }

        public string? TransferDate { get; set; }

        public string? TransferDescription { get; set; }    
    }
}
