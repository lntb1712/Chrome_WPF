using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.PickListDetailDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.PickListDetailService
{
    public interface IPickListDetailService
    {
        Task<ApiResult<PagedResponse<PickListDetailResponseDTO>>> GetAllPickListDetailsAsync(string[] warehouseCodes, int page, int pageSize);
        Task<ApiResult<PagedResponse<PickListDetailResponseDTO>>> GetPickListDetailsByPickNoAsync(string pickNo, int page, int pageSize);
        Task<ApiResult<PagedResponse<PickListDetailResponseDTO>>> SearchPickListDetailsAsync(string[] warehouseCodes, string pickNo, string textToSearch, int page, int pageSize);
        Task<ApiResult<bool>> UpdatePickListDetail(PickListDetailRequestDTO pickListDetail);
        Task<ApiResult<bool>> DeletePickListDetail(string pickNo, string productCode);
    }
}
