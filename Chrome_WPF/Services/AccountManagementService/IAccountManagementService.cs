using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.AccountManagementService
{
    public interface IAccountManagementService
    {
        Task<ApiResult<PagedResponse<AccountManagementResponseDTO>>> GetAllAccount(int page, int pageSize);
        Task<ApiResult<PagedResponse<AccountManagementResponseDTO>>> GetAllAccountWithRole(string groupID, int page, int pageSize);
        Task<ApiResult<PagedResponse<AccountManagementResponseDTO>>> SearchAccountInList(string textToSearch, int page, int pageSize);
        Task<ApiResult<bool>> AddAccountManagement(AccountManagementRequestDTO accountManagementRequestDTO);
        Task<ApiResult<bool>> UpdateAccountManagement(AccountManagementRequestDTO accountManagementRequestDTO);  
        Task<ApiResult<bool>> DeleteAccountManagement(string userName);
        Task<ApiResult<int>> GetTotalAccount();
    }
}
