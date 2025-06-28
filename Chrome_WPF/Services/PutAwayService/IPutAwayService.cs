using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.PutAwayDTO;
using Chrome_WPF.Models.StatusMasterDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.PutAwayService
{
    public interface IPutAwayService
    {
        Task<ApiResult<PagedResponse<PutAwayResponseDTO>>> GetAllPutAwaysAsync(string[] warehouseCodes, int page, int pageSize);
        Task<ApiResult<PagedResponse<PutAwayResponseDTO>>> GetAllPutAwaysWithStatusAsync(string[] warehouseCodes, int statusId, int page, int pageSize);
        Task<ApiResult<PagedResponse<PutAwayResponseDTO>>> SearchPutAwaysAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize);
        Task<ApiResult<PutAwayResponseDTO>> GetPutAwayByCodeAsync(string putAwayCode);
        Task<ApiResult<PutAwayAndDetailResponseDTO>> GetPutAwayContainsCodeAsync(string orderCode);
        Task<ApiResult<bool>> AddPutAway(PutAwayRequestDTO putAway);
        Task<ApiResult<bool>> DeletePutAway(string putAwayCode);
        Task<ApiResult<bool>> UpdatePutAway(PutAwayRequestDTO putAway);
        Task<ApiResult<List<StatusMasterResponseDTO>>> GetListStatusMaster();
    }
}
