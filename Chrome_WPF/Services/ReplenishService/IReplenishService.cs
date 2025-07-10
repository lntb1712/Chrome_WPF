using Chrome.DTO.ReplenishDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Models.ReplenishDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.ReplenishService
{
    public interface IReplenishService
    {
        Task<ApiResult<PagedResponse<ReplenishResponseDTO>>> GetReplenishListAsync(string warehouseCode, int page, int pageSize);
        Task<ApiResult<PagedResponse<ReplenishResponseDTO>>> SearchReplenishAsync(string warehouseCode,string textToSearch, int page, int pageSize);
        Task<ApiResult<List<string>>> CheckReplenishWarningsAsync(string warehouseCode);
        Task<ApiResult<List<ProductMasterResponseDTO>>> GetListProductForReplenish();
        Task<ApiResult<int>> GetTotalReplenishCountAsync(string warehouseCode);
        Task<ApiResult<bool>> AddReplenishAsync(ReplenishRequestDTO replenishRequest);
        Task<ApiResult<bool>> UpdateReplenishAsync(ReplenishRequestDTO replenishRequest);
        Task<ApiResult<bool>> DeleteReplenishAsync(string warehouseCode, string productCode);

    }
}
