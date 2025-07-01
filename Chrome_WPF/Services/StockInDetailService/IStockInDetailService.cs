using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Models.StockInDetailDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.StockInDetailService
{
    public interface IStockInDetailService
    {
        Task<ApiResult<PagedResponse<StockInDetailResponseDTO>>> GetAllStockInDetails(string stockInCode, int page, int pageSize);
        Task<ApiResult<List<ProductMasterResponseDTO>>> GetListProductToSI(string stockInCode);
        Task<ApiResult<bool>> AddStockInDetail(StockInDetailRequestDTO stockInDetail);
        Task<ApiResult<bool>> CreateBackOrder(string stockInCode, string backOrderDescription);
        Task<ApiResult<bool>> DeleteStockInDetail(string stockInCode, string productCode);
        Task<ApiResult<bool>> UpdateStockInDetail( StockInDetailRequestDTO stockInDetail);
        Task<ApiResult<bool>> ConfirmnStockIn(string stockInCode);
        Task<ApiResult<bool>> CheckAndUpdateStockBackOrderStatus(string stockInCode);
    }
}
