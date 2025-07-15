using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.DashboardDTO
{
    public class DashboardRequestDTO
    {
        public string[] warehouseCodes { get; set; } = Array.Empty<string>();
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? Quarter { get; set; }
    }
}
