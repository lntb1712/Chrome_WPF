using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.StockTakeDetailDTO
{
    public class StockTakeDetailResponseDTO
    {
        public string StocktakeCode { get; set; } = null!;

        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;

        public string Lotno { get; set; } = null!;

        public string LocationCode { get; set; } = null!;
        public string LocationName { get; set; } = null!;

        public double? Quantity { get; set; }

        public double? CountedQuantity { get; set; }
    }
}
