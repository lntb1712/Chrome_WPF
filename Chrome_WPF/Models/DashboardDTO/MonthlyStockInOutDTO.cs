using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.DashboardDTO
{
    public class MonthlyStockInOutDTO
    {
        public string Month { get; set; }
        public int StockInCount { get; set; }
        public int StockOutCount { get; set; }
        public int PurchaseOrderCount { get; set; }
        public int StocktakeCount { get; set; }
    }
}
