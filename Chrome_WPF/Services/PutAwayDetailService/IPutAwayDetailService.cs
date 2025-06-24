using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.PutAwayDetailDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.PutAwayDetailService
{
    public interface IPutAwayDetailService
    {
        Task<ApiResult<PagedResponse<PutAwayDetailResponseDTO>>> GetAllPutAwayDetailsAsync(string[] warehouseCodes, int page, int pageSize);
        Task<ApiResult<PagedResponse<PutAwayDetailResponseDTO>>> GetPutAwayDetailsByPutawayCodeAsync(string putAwayCode, int page, int pageSize);
        Task<ApiResult<PagedResponse<PutAwayDetailResponseDTO>>> SearchPutAwayDetailsAsync(string[] warehouseCodes, string putAwayCode, string textToSearch, int page, int pageSize);
        Task<ApiResult<bool>> UpdatePutAwayDetail(PutAwayDetailRequestDTO putAwayDetail);
        Task<ApiResult<bool>> DeletePutAwayDetail(string putAwayCode, string productCode);
    }
}
