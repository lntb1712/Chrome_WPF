using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.LocationMasterDTO;
using Chrome_WPF.Models.PagedResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.LocationMasterService
{
    public interface ILocationMasterService
    {
        Task<ApiResult<PagedResponse<LocationMasterResponseDTO>>> GetAllLocationMaster(string warehouseCode, int page, int pageSize);
        Task<ApiResult<LocationMasterResponseDTO>> GetLocationWithLocationCode(string warehouseCode,string locationCode);
        Task<ApiResult<bool>> AddLocationMaster(LocationMasterRequestDTO locationMaster);
        Task<ApiResult<bool>> DeleteLocationMaster(string warehouseCode, string locationCode);
        Task<ApiResult<bool>> UpdateLocationMaster(LocationMasterRequestDTO locationMaster);
        Task<ApiResult<int>> GetTotalLocationCount(string warehouseCode);
    }
}
