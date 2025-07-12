using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.PurchaseOrderDTO
{
    public class PurchaseOrderResponseDTO
    {
        public string PurchaseOrderCode { get; set; } = null!;

        public string? WarehouseCode { get; set; }
        public string? WarehouseName { get; set; }

        public int? StatusId { get; set; }
        public string? StatusName { get; set; }

        public string? OrderDate { get; set; }

        public string? ExpectedDate { get; set; }

        public string? SupplierCode { get; set; }
        public string? SupplierName { get; set; }

        public string? PurchaseOrderDescription { get; set; }
    }
}
