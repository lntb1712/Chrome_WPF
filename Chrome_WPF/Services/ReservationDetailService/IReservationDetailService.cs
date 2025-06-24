using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.ReservationDetailDTO;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.ReservationDetailService
{
    public interface IReservationDetailService
    {
        Task<ApiResult<PagedResponse<ReservationDetailResponseDTO>>> GetAllReservationDetails(string reservationCode, int page, int pageSize);
        Task<ApiResult<ReservationDetailResponseDTO>> GetReservationDetailByIdAsync(int id);
        Task<ApiResult<PagedResponse<ReservationDetailResponseDTO>>> SearchReservationDetailsAsync(string reservationCode, string textToSearch, int page, int pageSize);
    }
}