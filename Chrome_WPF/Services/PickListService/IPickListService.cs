using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.PickListDTO;
using Chrome_WPF.Models.StatusMasterDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.PickListService
{
    public interface IPickListService
    {
        Task<ApiResult<PagedResponse<PickListResponseDTO>>> GetAllPickListsAsync(string[] warehouseCodes, int page, int pageSize);
        Task<ApiResult<PagedResponse<PickListResponseDTO>>> GetAllPickListsWithStatusAsync(string[] warehouseCodes, int statusId, int page, int pageSize);
        Task<ApiResult<PagedResponse<PickListResponseDTO>>> SearchPickListsAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize);
        Task<ApiResult<PickListResponseDTO>> GetPickListByCodeAsync(string pickNo);
        Task<ApiResult<bool>> AddPickList(PickListRequestDTO pickList);
        Task<ApiResult<bool>> DeletePickList(string pickNo);
        Task<ApiResult<bool>> UpdatePickList(PickListRequestDTO pickList);
        Task<ApiResult<List<StatusMasterResponseDTO>>> GetListStatusMaster();
        Task<ApiResult<PickAndDetailResponseDTO>> GetPickListContainCodeAsync(string orderCode);
    }
}
