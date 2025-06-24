using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.ReservationDetailDTO;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.ReservationDetailService
{
    public class ReservationDetailService : IReservationDetailService
    {
        private readonly HttpClient _httpClient;
        private readonly API_Constant _baseUrl;

        public ReservationDetailService(API_Constant baseUrl)
        {
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            _httpClient = _baseUrl.GetHttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AccessToken);
        }

        public async Task<ApiResult<PagedResponse<ReservationDetailResponseDTO>>> GetAllReservationDetails(string reservationCode, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(reservationCode))
            {
                return new ApiResult<PagedResponse<ReservationDetailResponseDTO>>("Mã đặt chỗ không được để trống", false);
            }
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<ReservationDetailResponseDTO>>("Trang và kích thước trang phải lớn hơn 0", false);
            }

            try
            {
                var response = await _httpClient.GetAsync($"Reservation/{Uri.EscapeDataString(reservationCode)}/ReservationDetail/GetAllReservationDetails?page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<ReservationDetailResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<ReservationDetailResponseDTO>>(result?.Message ?? "Không thể lấy danh sách chi tiết đặt chỗ", false);
                    }
                    return result;
                }

                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<ReservationDetailResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách chi tiết đặt chỗ";
                return HandleErrorResponse<PagedResponse<ReservationDetailResponseDTO>>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<ReservationDetailResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<ReservationDetailResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<ReservationDetailResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<ReservationDetailResponseDTO>> GetReservationDetailByIdAsync(int id)
        {
            if (id < 1)
            {
                return new ApiResult<ReservationDetailResponseDTO>("ID chi tiết đặt chỗ phải lớn hơn 0", false);
            }

            try
            {
                var response = await _httpClient.GetAsync($"Reservation/ReservationDetail/GetReservationDetailById?id={id}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<ReservationDetailResponseDTO>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<ReservationDetailResponseDTO>(result?.Message ?? "Không thể lấy chi tiết đặt chỗ", false);
                    }
                    return result;
                }

                var errorResult = JsonConvert.DeserializeObject<ApiResult<ReservationDetailResponseDTO>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy chi tiết đặt chỗ";
                return HandleErrorResponse<ReservationDetailResponseDTO>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<ReservationDetailResponseDTO>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<ReservationDetailResponseDTO>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<ReservationDetailResponseDTO>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<ReservationDetailResponseDTO>>> SearchReservationDetailsAsync(string reservationCode, string textToSearch, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(reservationCode))
            {
                return new ApiResult<PagedResponse<ReservationDetailResponseDTO>>("Mã đặt chỗ không được để trống", false);
            }
            if (string.IsNullOrEmpty(textToSearch))
            {
                return new ApiResult<PagedResponse<ReservationDetailResponseDTO>>("Văn bản tìm kiếm không được để trống", false);
            }
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<ReservationDetailResponseDTO>>("Trang và kích thước trang phải lớn hơn 0", false);
            }

            try
            {
                var response = await _httpClient.GetAsync($"Reservation/{Uri.EscapeDataString(reservationCode)}/ReservationDetail/SearchReservationDetails?textToSearch={Uri.EscapeDataString(textToSearch)}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<ReservationDetailResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<ReservationDetailResponseDTO>>(result?.Message ?? "Không thể tìm kiếm chi tiết đặt chỗ", false);
                    }
                    return result;
                }

                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<ReservationDetailResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể tìm kiếm chi tiết đặt chỗ";
                return HandleErrorResponse<PagedResponse<ReservationDetailResponseDTO>>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<ReservationDetailResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<ReservationDetailResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<ReservationDetailResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        private ApiResult<T> HandleErrorResponse<T>(System.Net.HttpStatusCode statusCode, string errorMessage)
        {
            switch (statusCode)
            {
                case System.Net.HttpStatusCode.Unauthorized:
                    return new ApiResult<T>(errorMessage, false);
                case System.Net.HttpStatusCode.Forbidden:
                    return new ApiResult<T>(errorMessage, false);
                case System.Net.HttpStatusCode.NotFound:
                    return new ApiResult<T>(errorMessage, false);
                case System.Net.HttpStatusCode.InternalServerError:
                    return new ApiResult<T>(errorMessage, false);
                default:
                    return new ApiResult<T>(errorMessage, false);
            }
        }
    }
}