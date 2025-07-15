using Chrome_WPF.Models.InventoryDTO;
using Chrome_WPF.Models.ManufacturingOrderDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.DashboardDTO
{
    public class DashboardResponseDTO
    {
        public List<InventorySummaryDTO> InventorySummaryDTOs { get; set; } = new List<InventorySummaryDTO>();
        public List<ProgressManufacturingOrderDTO> ProgressManufacturingOrderDTOs { get; set; } = new List<ProgressManufacturingOrderDTO>();
        public int QuantityToCompleteStockIn { get; set; }
        public int QuantityToCompleteStockOut { get; set; }
        public int QuantityToCompleteManufacturingOrder { get; set; }
        public int QuantityToCompletePurchaseOrder { get; set; }
        public int QuantityToCompleteStocktake { get; set; }
        public List<UpcomingDeadlineDTO> UpcomingDeadlines { get; set; } = new List<UpcomingDeadlineDTO>();
        public List<StatusOrderCodeDTO> StatusOrderCodeDTOs { get; set; } = new List<StatusOrderCodeDTO>();
    }
}
