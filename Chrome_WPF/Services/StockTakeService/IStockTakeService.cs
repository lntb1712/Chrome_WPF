using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.StatusMasterDTO;
using Chrome_WPF.Models.StocktakeDTO;
using Chrome_WPF.Models.StockTakeDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.StockTakeService
{
    public interface IStockTakeService
    {
        Task<ApiResult<PagedResponse<StockTakeResponseDTO>>> GetAllStockTakesAsync(string[] warehouseCodes, int page, int pageSize);
        Task<ApiResult<PagedResponse<StockTakeResponseDTO>>> GetStockTakesByStatusAsync(string[] warehouseCodes, int statusId, int page, int pageSize);
        Task<ApiResult<PagedResponse<StockTakeResponseDTO>>> SearchStockTakesAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize);
        Task<ApiResult<StockTakeResponseDTO>> GetStockTakeByCodeAsync(string stockTakeCode);
        Task<ApiResult<bool>> AddStockTake(StockTakeRequestDTO stockTake);
        Task<ApiResult<bool>> UpdateStockTake(StockTakeRequestDTO stockTake);
        Task<ApiResult<bool>> DeleteStockTakeAsync(string stockTakeCode);
        Task<ApiResult<bool>> ConfirmnStockTake(StockTakeRequestDTO stockTake);
        Task<ApiResult<List<AccountManagementResponseDTO>>> GetListResponsibleAsync(string warehouseCode);
        Task<ApiResult<List<StatusMasterResponseDTO>>> GetListStatusMaster();
        Task<ApiResult<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission(string[] warehouseCodes);
    }
}