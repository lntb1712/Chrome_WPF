using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.OrderTypeDTO;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.ReservationDTO;
using Chrome_WPF.Models.StatusMasterDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.ReservationService
{
    public interface IReservationService
    {
        Task<ApiResult<PagedResponse<ReservationResponseDTO>>> GetAllReservations(string[] warehouseCodes, int page, int pageSize);
        Task<ApiResult<PagedResponse<ReservationResponseDTO>>> GetAllReservationsWithStatus(string[] warehouseCodes, int statusId, int page, int pageSize);
        Task<ApiResult<PagedResponse<ReservationResponseDTO>>> SearchReservationsAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize);
        Task<ApiResult<List<OrderTypeResponseDTO>>> GetListOrderType(string prefix);
        Task<ApiResult<List<StatusMasterResponseDTO>>> GetListStatusMaster();
        Task<ApiResult<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission(string[] warehouseCodes);
        Task<ApiResult<ReservationAndDetailResponseDTO>> GetReservationsByMovementCodeAsync(string movementCode);
        Task<ApiResult<ReservationAndDetailResponseDTO>> GetReservationsByStockOutCodeAsync(string stockOutCode);
        Task<ApiResult<ReservationAndDetailResponseDTO>> GetReservationsByTransferCodeAsync(string transferCode);
        Task<ApiResult<ReservationAndDetailResponseDTO>> GetReservationsByManufacturingCodeAsync(string manufacturingCode);
        Task<ApiResult<bool>> AddReservation(ReservationRequestDTO reservation);
        Task<ApiResult<bool>> DeleteReservationAsync(string reservationCode);
        Task<ApiResult<bool>> UpdateReservation(ReservationRequestDTO reservation);
    }
}