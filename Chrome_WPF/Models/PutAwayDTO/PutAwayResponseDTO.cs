using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.PutAwayDTO
{
    public class PutAwayResponseDTO
    {
        public string PutAwayCode { get; set; } = null!;

        public string? OrderTypeCode { get; set; }
        public string? OrderTypeName { get; set; }

        public string? LocationCode { get; set; }
        public string? LocationName { get; set; }

        public string? Responsible { get; set; }
        public string? FullNameResponsible { get; set; }

        public int? StatusId { get; set; }
        public string? StatusName { get; set; }

        public string? PutAwayDate { get; set; }

        public string? PutAwayDescription { get; set; }
    }
}
