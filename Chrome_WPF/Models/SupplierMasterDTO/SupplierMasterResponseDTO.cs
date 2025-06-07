using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.SupplierMasterDTO
{
    public class SupplierMasterResponseDTO
    {
        public string SupplierCode { get; set; } = null!;

        public string? SupplierName { get; set; }

        public string? SupplierPhone { get; set; }

        public string? SupplierAddress { get; set; }
    }
}
