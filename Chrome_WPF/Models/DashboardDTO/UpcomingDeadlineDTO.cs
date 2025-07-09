using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.DashboardDTO
{
    public class UpcomingDeadlineDTO
    {
        public string OrderCode { get; set; } = null!;
        public List<string> ProductCodes { get; set; } = new List<string>();
        public string Deadline { get; set; } = null!;
        public string StatusName { get; set; } = null!;
    }
}
