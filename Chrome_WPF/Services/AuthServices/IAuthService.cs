using Chrome_WPF.Models.LoginDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.AuthServices
{
    public interface IAuthService
    {
        Task <Dictionary<string, object>> DecodeJWT(string token);
        Task <List<string>> GetPermissionFromToken(string token);
        Task<string> GetName(string token);
            
    }
}
