using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Models.PurchaseOrderDetailDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.PurchaseOrderDetailService
{
    public interface IPurchaseOrderDetailService
    {
        Task<ApiResult<PagedResponse<PurchaseOrderDetailResponseDTO>>> GetAllPurchaseOrderDetails (string purchaseOrderCode, int page, int pageSize);
        Task<ApiResult<PurchaseOrderDetailResponseDTO>> GetPurchaseOrderDetailByCode(string purchaseOrderCode, string productCode);
        Task<ApiResult<bool>> AddPurchaseOrderDetail(PurchaseOrderDetailRequestDTO purchaseOrderDetailRequestDTO);
        Task<ApiResult<bool>> UpdatePurchaseOrderDetail(PurchaseOrderDetailRequestDTO purchaseOrderDetailRequestDTO);
        Task<ApiResult<bool>> DeletePurchaseOrderDetail(string purchaseOrderCode, string productCode);
        Task<ApiResult<List<ProductMasterResponseDTO>>> GetListProductToPO(string purchaseOrderCode);
        Task<ApiResult<bool>> CreateBackOrder(string purchaseOrderCode, string backOrderDescription, string dateBackOrder);
        Task<ApiResult<bool>> ConfirmPurchaseOrderDetail(string purchaseOrderCode);
        Task<ApiResult<bool>> CheckAndUpdateStockBackOrderStatus(string purchaseOrderCode);


    }
}
