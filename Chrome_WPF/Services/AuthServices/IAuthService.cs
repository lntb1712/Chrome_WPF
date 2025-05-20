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
        Task <Dictionary<string, object>> DecodeJWT(LoginResponseDTO loginResponse);
        Task <List<string>> GetPermissionFromToken(LoginResponseDTO loginResponse);
        Task<string> GetName(LoginResponseDTO loginResponse);

    }
}
