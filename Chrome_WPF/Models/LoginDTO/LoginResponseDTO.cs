using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.LoginDTO
{
    public class LoginResponseDTO
    {
        public string? Token { get; set; }
        public string? Username { get; set; }
        public string? GroupId { get; set; }
    }
}
