using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.CategoryDTO;
using Chrome_WPF.Models.InventoryDTO;
using Chrome_WPF.Models.PagedResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.InventoryService
{
    public interface IInventoryService
    {
        Task<ApiResult<PagedResponse<InventorySummaryDTO>>> GetListProductInventory( int page, int pageSize);
        Task<ApiResult<PagedResponse<ProductWithLocationsDTO>>> GetProductWithLocations (string productCode, int page, int pageSize);
        Task<ApiResult<PagedResponse<InventorySummaryDTO>>> GetListProductInventoryByCategoryIds(string[] categoryIds, int page, int pageSize); 
        Task<ApiResult<PagedResponse<InventorySummaryDTO>>> SearchProductInventory(string textToSearch, int page, int pageSize);
        Task<ApiResult<List<CategoryResponseDTO>>> GetAllCategories();
        Task<ApiResult<List<WarehouseUsageDTO>>> GetInventoryUsedPercent();
        Task<ApiResult<double>> GetTotalPriceOfWarehouse();
    }
}
