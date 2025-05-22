using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.AccountManagementDTO
{
    public class UserInformationDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string ImageInitial { get; set; } = string.Empty;
    }
}
