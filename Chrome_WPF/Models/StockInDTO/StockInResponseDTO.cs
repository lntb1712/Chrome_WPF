using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.StockInDTO
{
    public class StockInResponseDTO
    {
        public string StockInCode { get; set; } = null!;

        public string? OrderTypeCode { get; set; }
        public string? OrderTypeName { get; set; }

        public string? WarehouseCode { get; set; }
        public string? WarehouseName { get; set; }

        public string? PurchaseOrderCode { get; set; }
        public string? SupplierCode { get; set; }
        public string? SupplierName { get; set; }

        public string? Responsible { get; set; }
        public string? FullNameResponsible { get; set; }

        public int? StatusId { get; set; }
        public string? StatusName { get; set; }

        public string? OrderDeadline { get; set; }

        public string? StockInDescription { get; set; }
    }
}
