using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.LocationMasterDTO;
using Chrome_WPF.Models.MovementDTO;
using Chrome_WPF.Models.OrderTypeDTO;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.PickListDTO;
using Chrome_WPF.Models.PutAwayDTO;
using Chrome_WPF.Models.StatusMasterDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.MovementService
{
    public interface IMovementService
    {
        Task<ApiResult<PagedResponse<MovementResponseDTO>>> GetAllMovements(string[] warehouseCodes, int page, int pageSize);
        Task<ApiResult<PagedResponse<MovementResponseDTO>>> GetAllMovementsWithStatus(string[] warehouseCodes, int statusId, int page, int pageSize);
        Task<ApiResult<PagedResponse<MovementResponseDTO>>> SearchMovementAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize);
        Task<ApiResult<bool>> AddMovement(MovementRequestDTO movement);
        Task<ApiResult<bool>> DeleteMovementAsync(string movementCode);
        Task<ApiResult<bool>> UpdateMovement(MovementRequestDTO movement);
        Task<ApiResult<List<OrderTypeResponseDTO>>> GetListOrderType(string prefix);
        Task<ApiResult<List<AccountManagementResponseDTO>>> GetListResponsibleAsync();
        Task<ApiResult<List<StatusMasterResponseDTO>>> GetListStatusMaster();
        Task<ApiResult<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission(string[] warehouseCodes);
        Task<ApiResult<List<LocationMasterResponseDTO>>> GetListFromLocation(string warehouseCode);
        Task<ApiResult<List<LocationMasterResponseDTO>>> GetListToLocation(string warehouseCode, string toLocation);
        Task<ApiResult<PutAwayResponseDTO>> GetPutAwayContainsMovement(string movementCode);
        Task<ApiResult<PickListResponseDTO>> GetPickListContainsMovement(string movementCode);
    }
}
