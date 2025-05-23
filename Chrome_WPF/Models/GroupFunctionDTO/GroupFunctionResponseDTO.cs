using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.GroupFunctionDTO
{
    public class GroupFunctionResponseDTO
    {
        public string GroupId { get; set; } = null!;
        public string FunctionId { get; set; } = null!;
        public string FunctionName { get; set; } = null!;
        public bool? IsEnable { get; set; }
        public string? UpdateBy { get; set; }
        public string? UpdateTime { get; set; }
    }
}
