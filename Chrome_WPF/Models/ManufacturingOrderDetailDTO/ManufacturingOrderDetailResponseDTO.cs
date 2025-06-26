using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.ManufacturingOrderDetailDTO
{
    public class ManufacturingOrderDetailResponseDTO
    {
        public string ManufacturingOrderCode { get; set; } = null!;

        public string ComponentCode { get; set; } = null!;
        public string ComponentName { get; set; } = null!;

        public double? ToConsumeQuantity { get; set; }

        public double? ConsumedQuantity { get; set; }

        public double? ScraptRate { get; set; }
    }
}
