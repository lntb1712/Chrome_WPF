using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.BOMMasterDTO;
using Chrome_WPF.Models.PagedResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.BOMMasterService
{
    public interface IBOMMasterService
    {
        Task<ApiResult<PagedResponse<BOMMasterResponseDTO>>>GetAllBOMMaster(int page,int pageSize);
        Task<ApiResult<PagedResponse<BOMMasterResponseDTO>>>SearchBOMMaster(string textToSearch,int page, int pageSize);
        Task<ApiResult<BOMMasterResponseDTO>> GetBOMMasterByCode(string bomCode, string bomVersion);
        Task<ApiResult<int>> GetTotalBOMMasterCount();
        Task<ApiResult<bool>> AddBOMMaster(BOMMasterRequestDTO bOMMasterRequestDTO);
        Task<ApiResult<bool>> UpdateBOMMaster(BOMMasterRequestDTO bOMMasterRequestDTO);
        Task<ApiResult<bool>> DeleteBOMMaster(string bomCode, string bomVersion);
    }
}
