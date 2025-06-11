using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.BOMComponentDTO;
using Chrome_WPF.Services.BOMMasterService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.BOMComponentService
{
    public interface IBOMComponentService
    {
        Task<ApiResult<List<BOMNodeDTO>>> GetRecursiveBOMAsync(string bomCode, string bomVersion);
        Task<ApiResult<List<BOMComponentResponseDTO>>> GetAllBOMComponent(string bomCode, string bomVersion);
        Task<ApiResult<BOMComponentResponseDTO>> GetBOMComponent (string bomCode, string bomVersion,string componentCode);
        Task<ApiResult<bool>> AddBomComponent(BOMComponentRequestDTO bOMComponentRequestDTO);
        Task<ApiResult<bool>> UpdateBomComponent(BOMComponentRequestDTO bOMComponentRequestDTO);
        Task<ApiResult<bool>> DeleteBomComponent(string bomCode, string bomVersion,string componentCode);
    }
}
