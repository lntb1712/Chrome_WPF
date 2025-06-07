using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.CustomerMasterDTO
{
    public class CustomerMasterResponseDTO
    {
        public string CustomerCode { get; set; } = null!;

        public string? CustomerName { get; set; }

        public string? CustomerPhone { get; set; }

        public string? CustomerAddress { get; set; }

        public string? CustomerEmail { get; set; }
    }
}
