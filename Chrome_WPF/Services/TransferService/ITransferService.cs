using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.OrderTypeDTO;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.StatusMasterDTO;
using Chrome_WPF.Models.TransferDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.TransferService
{
    public interface ITransferService
    {
        Task<ApiResult<PagedResponse<TransferResponseDTO>>> GetAllTransfers(string[] warehouseCodes, int page, int pageSize);
        Task<ApiResult<PagedResponse<TransferResponseDTO>>> GetAllTransfersWithStatus(string[] warehouseCodes, int statusId, int page, int pageSize);
        Task<ApiResult<PagedResponse<TransferResponseDTO>>> SearchTransfersAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize);
        Task<ApiResult<bool>> AddTransfer(TransferRequestDTO transfer);
        Task<ApiResult<bool>> DeleteTransferAsync(string transferCode);
        Task<ApiResult<bool>> UpdateTransfer(TransferRequestDTO transfer);
        Task<ApiResult<List<OrderTypeResponseDTO>>> GetListOrderType(string prefix);
        Task<ApiResult<List<AccountManagementResponseDTO>>> GetListFromResponsibleAsync(string warehouseCode);
        Task<ApiResult<List<AccountManagementResponseDTO>>> GetListToResponsibleAsync(string warehouseCode);
        Task<ApiResult<List<StatusMasterResponseDTO>>> GetListStatusMaster();
        Task<ApiResult<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission(string[] warehouseCodes);
    }
}
