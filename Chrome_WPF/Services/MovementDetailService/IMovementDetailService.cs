using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.MovementDetailDTO;
using Chrome_WPF.Models.MovementDTO;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.ProductMasterDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.MovementDetailService
{
    public interface IMovementDetailService
    {
        Task<ApiResult<PagedResponse<MovementDetailResponseDTO>>> GetAllMovementDetailsAsync(string[] warehouseCodes, int page, int pageSize);
        Task<ApiResult<PagedResponse<MovementDetailResponseDTO>>> GetMovementDetailsByMovementCodeAsync(string movementCode, int page, int pageSize);
        Task<ApiResult<PagedResponse<MovementDetailResponseDTO>>> SearchMovementDetailsAsync(string[] warehouseCodes, string movementCode, string textToSearch, int page, int pageSize);
        Task<ApiResult<bool>> AddMovementDetail(MovementDetailRequestDTO movementDetail);
        Task<ApiResult<bool>> UpdateMovementDetail(MovementDetailRequestDTO movementDetail);
        Task<ApiResult<bool>> DeleteMovementDetail(string movementCode, string productCode);
        Task<ApiResult<ProductMasterResponseDTO>> GetProductByLocationCode(string locationCode);
    }

}
