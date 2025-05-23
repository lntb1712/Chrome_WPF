using Chrome_WPF.Models.GroupFunctionDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.GroupManagementDTO
{
    public class GroupManagementRequestDTO
    {
        public string GroupId { get; set; } = null!;
        public string? GroupName { get; set; }
        public string? GroupDescription { get; set; }
        public string? UpdateBy { get; set; }
        public string? UpdateTime { get; set; }
        public List<GroupFunctionResponseDTO> GroupFunctions { get; set; } = new List<GroupFunctionResponseDTO>();
    }
}
