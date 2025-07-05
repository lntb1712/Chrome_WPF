using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.StockOutDetailDTO
{
    public class ForecastStockOutDetailDTO
    {
        public string StockOutCode { get; set; } = null!;

        public string ProductCode { get; set; } = null!;

        public double? QuantityOnHand { get; set; }

        public double? QuantityToOutBound { get; set; }

        public double? QuantityToInBound { get; set; }

        public double? AvailableQty { get; set; }
    }
}
