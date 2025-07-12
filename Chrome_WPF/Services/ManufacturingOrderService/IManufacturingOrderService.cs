using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.BOMMasterDTO;
using Chrome_WPF.Models.ManufacturingOrderDTO;
using Chrome_WPF.Models.OrderTypeDTO;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Models.StatusMasterDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chrome.Services.ManufacturingOrderService
{
    public interface IManufacturingOrderService
    {
        Task<ApiResult<PagedResponse<ManufacturingOrderResponseDTO>>> GetAllManufacturingOrdersAsync(string[] warehouseCodes, int page, int pageSize);
        Task<ApiResult<PagedResponse<ManufacturingOrderResponseDTO>>> GetAllManufacturingOrdersWithStatusAsync(string[] warehouseCodes, int statusId, int page, int pageSize);
        Task<ApiResult<ManufacturingOrderResponseDTO>> GetManufacturingOrderByCodeAsync(string manufacturingCode);
        Task<ApiResult<PagedResponse<ManufacturingOrderResponseDTO>>> SearchManufacturingOrdersAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize);
        Task<ApiResult<bool>> AddManufacturingOrderAsync(ManufacturingOrderRequestDTO manufacturingOrder);
        Task<ApiResult<bool>> UpdateManufacturingOrderAsync(ManufacturingOrderRequestDTO manufacturingOrder);
        Task<ApiResult<bool>> DeleteManufacturingOrderAsync(string manufacturingCode);
        Task<ApiResult<bool>> ConfirmManufacturingOrder(string manufacturingCode);
        Task<ApiResult<bool>> CreateBackOrder(string manufacturingCode,string scheduleDateBackOrder, string deadLineBackOrder);
        Task<ApiResult<bool>> CheckAndUpdateBackOrderStatus(string manufacturingCode);
        Task<ApiResult<List<OrderTypeResponseDTO>>> GetListOrderTypeAsync(string prefix);
        Task<ApiResult<List<AccountManagementResponseDTO>>> GetListResponsibleAsync(string warehouseCode);
        Task<ApiResult<List<StatusMasterResponseDTO>>> GetListStatusMasterAsync();
        Task<ApiResult<List<WarehouseMasterResponseDTO>>> GetListWarehousePermissionAsync(string[] warehouseCodes);
        Task<ApiResult<BOMMasterResponseDTO>> GetListBomMasterAsync(string productCode);
        Task<ApiResult<List<ProductMasterResponseDTO>>> GetListProductMasterIsFGAndSFG();
    }
}