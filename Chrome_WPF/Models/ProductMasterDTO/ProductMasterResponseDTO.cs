using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.ProductMasterDTO
{
    public class ProductMasterResponseDTO
    {
        public string ProductCode { get; set; } = null!;
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductImg { get; set; }
        public string? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public double? BaseQuantity { get; set; }
        public string? Uom { get; set; }
        public string? BaseUom { get; set; }
        public float? TotalOnHand { get; set; }
        public string? UpdateTime { get; set; }
        public string? UpdateBy { get; set; }
    }
}
