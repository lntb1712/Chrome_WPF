using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Models.PurchaseOrderDTO;
using Chrome_WPF.Models.StatusMasterDTO;
using Chrome_WPF.Models.SupplierMasterDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.PurchaseOrderService
{
    public interface IPurchaseOrderService
    {
        Task<ApiResult<PagedResponse<PurchaseOrderResponseDTO>>> GetAllPurchaseOrders(int page, int pageSize);
        Task<ApiResult<PurchaseOrderResponseDTO>> GetPurchaseOrderByCode(string purchaseOrderCode);
        Task<ApiResult<PagedResponse<PurchaseOrderResponseDTO>>> GetAllPurchaseOrdersWithStatus(int statusId, int page, int pageSize);
        Task<ApiResult<PagedResponse<PurchaseOrderResponseDTO>>> SearchPurchaseOrderAsync(string textToSearch, int page, int pageSize);
        Task<ApiResult<List<StatusMasterResponseDTO>>> GetListStatusMaster();
        Task<ApiResult<List<SupplierMasterResponseDTO>>> GetListSupplierMasterAsync();
        Task<ApiResult<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission();
        Task<ApiResult<bool>> AddPurchaseOrder(PurchaseOrderRequestDTO purchaseOrder);
        Task<ApiResult<bool>> UpdatePurchaseOrder(PurchaseOrderRequestDTO purchaseOrder);
        Task<ApiResult<bool>> DeletePurchaseOrderAsync(string purchaseOrderCode);
    }
}
