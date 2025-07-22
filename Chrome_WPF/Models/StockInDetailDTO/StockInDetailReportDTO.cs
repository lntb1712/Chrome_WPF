using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.StockInDetailDTO
{
    public class StockInDetailReportDTO
    {
        public string StockInCode { get; set; } = null!;

        public string ProductCode { get; set; } = null!;

        public string ProductName { get; set; } = null!;
        public string UOM { get; set; } = null!;

        public string? LotNo { get; set; }

        public double? Demand { get; set; }

        public double? Quantity { get; set; }
    }
}
