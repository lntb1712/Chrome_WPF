using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.InventoryDTO;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.TransferDetailDTO;
using Chrome_WPF.Models.TransferDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.TransferDetailService
{
    public interface ITransferDetailService
    {
        Task<ApiResult<PagedResponse<TransferDetailResponseDTO>>> GetAllTransferDetailsAsync(string[] warehouseCodes, int page, int pageSize);
        Task<ApiResult<PagedResponse<TransferDetailResponseDTO>>> GetTransferDetailsByTransferCodeAsync(string transferCode, int page, int pageSize);
        Task<ApiResult<PagedResponse<TransferDetailResponseDTO>>> SearchTransferDetailsAsync(string[] warehouseCodes, string transferCode, string textToSearch, int page, int pageSize);
        Task<ApiResult<bool>> AddTransferDetail(TransferDetailRequestDTO transferDetail);
        Task<ApiResult<bool>> UpdateTransferDetail(TransferDetailRequestDTO transferDetail);
        Task<ApiResult<bool>> DeleteTransferDetail(string transferCode, string productCode);
        Task<ApiResult<List<InventorySummaryDTO>>> GetProductByWarehouseCode(string warehouseCode);
    }
}
