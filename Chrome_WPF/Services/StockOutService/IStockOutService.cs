using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.CustomerMasterDTO;
using Chrome_WPF.Models.OrderTypeDTO;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.StatusMasterDTO;
using Chrome_WPF.Models.StockOutDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.StockOutService
{
    public interface IStockOutService
    {
        Task<ApiResult<PagedResponse<StockOutResponseDTO>>> GetAllStockOuts(int page, int pageSize);
        Task<ApiResult<PagedResponse<StockOutResponseDTO>>> GetAllStockOutsWithStatus(int statusId, int page, int pageSize);
        Task<ApiResult<PagedResponse<StockOutResponseDTO>>> SearchStockOutAsync(string textToSearch, int page, int pageSize);
        Task<ApiResult<List<OrderTypeResponseDTO>>> GetListOrderType(string prefix);
        Task<ApiResult<List<CustomerMasterResponseDTO>>> GetListCustomerMasterAsync();
        Task<ApiResult<List<AccountManagementResponseDTO>>> GetListResponsibleAsync();
        Task<ApiResult<List<StatusMasterResponseDTO>>> GetListStatusMaster();
        Task<ApiResult<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission();
        Task<ApiResult<bool>> AddStockOut(StockOutRequestDTO stockOut);
        Task<ApiResult<bool>> UpdateStockOut(StockOutRequestDTO stockOut);
        Task<ApiResult<bool>> DeleteStockOutAsync(string stockOutCode);
    }
}
