using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.WarehouseMasterDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.WarehouseMasterService
{
    public interface IWarehouseMasterService
    {
        Task<ApiResult<PagedResponse<WarehouseMasterResponseDTO>>> GetAllWarehouseMaster(int page, int pageSize);
        Task<ApiResult<PagedResponse<WarehouseMasterResponseDTO>>> SearchWarehouseMaster(string textToSearch, int page, int pageSize);
        Task<ApiResult<WarehouseMasterResponseDTO>> GetWarehouseWithWarehouseCode(string warehouseCode);
        Task<ApiResult<bool>> AddWarehouseMaster(WarehouseMasterRequestDTO warehouseMaster);
        Task<ApiResult<bool>> DeleteWarehouseMaster(string warehouseCode);
        Task<ApiResult<bool>> UpdateWarehouseMaster(WarehouseMasterRequestDTO warehouseMaster);
        Task<ApiResult<int>> GetTotalWarehouseCount();

    }
}
