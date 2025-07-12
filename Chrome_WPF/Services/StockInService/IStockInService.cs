using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.OrderTypeDTO;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.PurchaseOrderDTO;
using Chrome_WPF.Models.StatusMasterDTO;
using Chrome_WPF.Models.StockInDTO;
using Chrome_WPF.Models.SupplierMasterDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.StockInService
{
    public interface IStockInService
    {
        Task<ApiResult<PagedResponse<StockInResponseDTO>>> GetAllStockIns( int page, int pageSize);
        Task<ApiResult<StockInResponseDTO>> GetStockInByCode(string stockInCode);
        Task<ApiResult<PagedResponse<StockInResponseDTO>>> GetAllStockInsWithStatus( int statusId, int page, int pageSize);
        Task<ApiResult<PagedResponse<StockInResponseDTO>>> SearchStockInAsync(string textToSearch, int page, int pageSize);
        Task<ApiResult<List<OrderTypeResponseDTO>>> GetListOrderType(string prefix);
        Task<ApiResult<List<PurchaseOrderResponseDTO>>> GetListPurchaseOrder(int[]? statusFilters);
        Task<ApiResult<List<AccountManagementResponseDTO>>> GetListResponsibleAsync(string warehouseCode);
        Task<ApiResult<List<StatusMasterResponseDTO>>> GetListStatusMaster();
        Task<ApiResult<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission();
        Task<ApiResult<bool>> AddStockIn(StockInRequestDTO stockIn);
        Task<ApiResult<bool>> UpdateStockIn(StockInRequestDTO stockIn);
        Task<ApiResult<bool>> DeleteStockInAsync(string stockInCode);
    }
}
