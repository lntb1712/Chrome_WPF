using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.DashboardDTO
{
    public class StatusOrderCodeDTO
    {
        public string OrderTypeCode { get; set; } = null!;
        public int CountStatusStart { get; set; }
        public int CountStatusInProgress { get; set; }
        public int CountStatusCompleted { get; set; }
    }
}
