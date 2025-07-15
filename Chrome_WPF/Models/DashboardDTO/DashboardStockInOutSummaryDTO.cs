using Chrome_WPF.Models.InventoryDTO;
using Chrome_WPF.Models.ManufacturingOrderDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.DashboardDTO
{
    public class DashboardStockInOutSummaryDTO
    {
        public int StockInThisMonth { get; set; }
        public int StockInThisQuarter { get; set; }
        public int StockInThisYear { get; set; }
        public int StockOutThisMonth { get; set; }
        public int StockOutThisQuarter { get; set; }
        public int StockOutThisYear { get; set; }
        public int PurchaseOrderThisMonth { get; set; }
        public int PurchaseOrderThisQuarter { get; set; }
        public int PurchaseOrderThisYear { get; set; }
        public int StocktakeThisMonth { get; set; }
        public int StocktakeThisQuarter { get; set; }
        public int StocktakeThisYear { get; set; }
        public List<MonthlyStockInOutDTO> MonthlyStockInOuts { get; set; }
    }
}
