using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.DashboardDTO
{
    public class DailyStockInOutDTO
    {
        public string Date { get; set; } = null!;
        public int StockInCount { get; set; }
        public int StockOutCount { get; set; }
    }
}
