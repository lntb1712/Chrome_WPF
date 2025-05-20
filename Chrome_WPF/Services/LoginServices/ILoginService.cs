using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.LoginDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.LoginServices
{
    public interface ILoginService
    {
        Task<ApiResult<LoginResponseDTO>> AuthenticateAsync(LoginRequestDTO loginRequest);
    }
}
