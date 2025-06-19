using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.CustomerMasterDTO;
using Chrome_WPF.Models.OrderTypeDTO;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Models.StatusMasterDTO;
using Chrome_WPF.Models.StockOutDetailDTO;
using Chrome_WPF.Models.StockOutDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.StockOutDetailService
{
    public interface IStockOutDetailService
    {
        Task<ApiResult<PagedResponse<StockOutDetailResponseDTO>>> GetAllStockOutDetails(string stockOutCode, int page, int pageSize);
        Task<ApiResult<List<ProductMasterResponseDTO>>> GetListProductToSO(string stockOutCode);
        Task<ApiResult<bool>> AddStockOutDetail(StockOutDetailRequestDTO stockOutDetail);
        Task<ApiResult<bool>> CreateBackOrder(string stockOutCode, string backOrderDescription);
        Task<ApiResult<bool>> DeleteStockOutDetail(string stockOutCode, string productCode);
        Task<ApiResult<bool>> UpdateStockOutDetail(StockOutDetailRequestDTO stockOutDetail);
        Task<ApiResult<bool>> ConfirmStockOut(string stockOutCode);
        Task<ApiResult<bool>> CheckAndUpdateBackOrderStatus(string stockOutCode);
    }
}
