using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.PickListDetailDTO
{
    public class PickListDetailResponseDTO
    {
        public string PickNo { get; set; } = null!;

        public string ProductCode { get; set; } = null!;
        public string? ProductName { get; set; }

        public string? LotNo { get; set; }

        public double? Demand { get; set; }

        public double? Quantity { get; set; }

        public string? LocationCode { get; set; }
        public string? LocationName { get; set; }
    }
}
