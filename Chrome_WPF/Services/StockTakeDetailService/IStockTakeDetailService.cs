using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.StockTakeDetailDTO;
using Chrome_WPF.Models.StocktakeDTO;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.StockTakeDetailService
{
    public interface IStockTakeDetailService
    {
        Task<ApiResult<PagedResponse<StockTakeDetailResponseDTO>>> GetAllStockTakeDetailsAsync(string[] warehouseCodes, int page, int pageSize);
        Task<ApiResult<PagedResponse<StockTakeDetailResponseDTO>>> GetStockTakeDetailsByStockTakeCodeAsync(string stockTakeCode, int page, int pageSize);
        Task<ApiResult<PagedResponse<StockTakeDetailResponseDTO>>> SearchStockTakeDetailsAsync(string[] warehouseCodes, string stockTakeCode, string textToSearch, int page, int pageSize);
        Task<ApiResult<bool>> UpdateStockTakeDetail(StockTakeDetailRequestDTO stockTakeDetail);
        Task<ApiResult<bool>> DeleteStockTakeDetail(string stockTakeCode, string productCode, string lotNo, string locationCode);
    }
}