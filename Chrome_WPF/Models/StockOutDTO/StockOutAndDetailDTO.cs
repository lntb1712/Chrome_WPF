using Chrome_WPF.Models.StockOutDetailDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.StockOutDTO
{
    public class StockOutAndDetailDTO
    {
        public string StockOutCode { get; set; } = null!;

        public string? OrderTypeCode { get; set; }
        public string? OrderTypeName { get; set; }

        public string? WarehouseCode { get; set; }
        public string? WarehouseName { get; set; }

        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }

        public string? Responsible { get; set; }
        public string? FullNameResponsible { get; set; }

        public int? StatusId { get; set; }
        public string? StatusName { get; set; }

        public string? StockOutDate { get; set; }

        public string? StockOutDescription { get; set; }

        public List<StockOutDetailReportDTO> stockOutDetails { get; set; } = new List<StockOutDetailReportDTO>();
    }
}
