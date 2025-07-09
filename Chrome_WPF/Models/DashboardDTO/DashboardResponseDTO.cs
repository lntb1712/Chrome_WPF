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
        public List<InventorySummaryDTO> inventorySummaryDTOs { get; set; } = new List<InventorySummaryDTO>();
        public List<ProgressManufacturingOrderDTO> progressManufacturingOrderDTOs { get; set; } = new List<ProgressManufacturingOrderDTO>();
        public int QuantityToCompleteStockIn { get; set; }
        public int QuantityToCompleteStockOut { get; set; }
        public int QuantityToCompleteManufacturingOrder { get; set; }
        public List<UpcomingDeadlineDTO> upcomingDeadlines { get; set; } = new List<UpcomingDeadlineDTO>();
        public List<StatusOrderCodeDTO> statusOrderCodeDTOs { get; set; } = new List<StatusOrderCodeDTO>();
    }
}
