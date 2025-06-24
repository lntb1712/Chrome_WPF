using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.OrderTypeDTO;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.ReservationDTO;
using Chrome_WPF.Models.StatusMasterDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Chrome_WPF.Services.ReservationService
{
    public class ReservationService : IReservationService
    {
        private readonly HttpClient _httpClient;
        private readonly API_Constant _baseUrl;

        // Constructor nhận vào hằng số API (API_Constant) để cấu hình HttpClient
        public ReservationService(API_Constant baseUrl)
        {
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            _httpClient = _baseUrl.GetHttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AccessToken);
        }

        // Lấy tất cả các đặt chỗ
        public async Task<ApiResult<PagedResponse<ReservationResponseDTO>>> GetAllReservations(string[] warehouseCodes, int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<ReservationResponseDTO>>("Trang và kích thước trang phải lớn hơn 0", false);
            }

            try
            {
                var queryString = string.Join("&", warehouseCodes.Select(w => $"warehouseCodes={Uri.EscapeDataString(w)}"));
                var response = await _httpClient.GetAsync($"Reservation/GetAllReservations?{queryString}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<ReservationResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<ReservationResponseDTO>>(result?.Message ?? "Không thể lấy danh sách đặt chỗ", false);
                    }
                    return result;
                }

                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<ReservationResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách đặt chỗ";
                return HandleErrorResponse<PagedResponse<ReservationResponseDTO>>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<ReservationResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<ReservationResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<ReservationResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        // Lấy tất cả các đặt chỗ theo trạng thái
        public async Task<ApiResult<PagedResponse<ReservationResponseDTO>>> GetAllReservationsWithStatus(string[] warehouseCodes, int statusId, int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<ReservationResponseDTO>>("Trang và kích thước trang phải lớn hơn 0", false);
            }
            if (statusId < 1)
            {
                return new ApiResult<PagedResponse<ReservationResponseDTO>>("Trạng thái phải là số nguyên dương", false);
            }

            try
            {
                var queryString = string.Join("&", warehouseCodes.Select(w => $"warehouseCodes={Uri.EscapeDataString(w)}"));
                var response = await _httpClient.GetAsync($"Reservation/GetAllReservationsWithStatus?{queryString}&statusId={statusId}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<ReservationResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<ReservationResponseDTO>>(result?.Message ?? "Không thể lấy danh sách đặt chỗ theo trạng thái", false);
                    }
                    return result;
                }

                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<ReservationResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách đặt chỗ theo trạng thái";
                return HandleErrorResponse<PagedResponse<ReservationResponseDTO>>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<ReservationResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<ReservationResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<ReservationResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        // Tìm kiếm các đặt chỗ
        public async Task<ApiResult<PagedResponse<ReservationResponseDTO>>> SearchReservationsAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(textToSearch) || page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<ReservationResponseDTO>>("Văn bản tìm kiếm, trang và kích thước trang phải hợp lệ", false);
            }

            try
            {
                var queryString = string.Join("&", warehouseCodes.Select(w => $"warehouseCodes={Uri.EscapeDataString(w)}"));
                var response = await _httpClient.GetAsync($"Reservation/SearchReservationsAsync?{queryString}&textToSearch={Uri.EscapeDataString(textToSearch)}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<ReservationResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<ReservationResponseDTO>>(result?.Message ?? "Không thể tìm kiếm đặt chỗ", false);
                    }
                    return result;
                }

                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<ReservationResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể tìm kiếm đặt chỗ";
                return HandleErrorResponse<PagedResponse<ReservationResponseDTO>>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<ReservationResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<ReservationResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<ReservationResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        // Lấy danh sách loại đơn hàng
        public async Task<ApiResult<List<OrderTypeResponseDTO>>> GetListOrderType(string prefix)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Reservation/GetListOrderType?prefix={Uri.EscapeDataString(prefix ?? "")}").ConfigureAwait(true);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<OrderTypeResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<List<OrderTypeResponseDTO>>(result?.Message ?? "Không thể lấy danh sách loại đơn hàng", false);
                    }
                    return result;
                }

                var errorResult = JsonConvert.DeserializeObject<ApiResult<List<OrderTypeResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách loại đơn hàng";
                return HandleErrorResponse<List<OrderTypeResponseDTO>>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<List<OrderTypeResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<List<OrderTypeResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<List<OrderTypeResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        // Lấy danh sách trạng thái chính
        public async Task<ApiResult<List<StatusMasterResponseDTO>>> GetListStatusMaster()
        {
            try
            {
                var response = await _httpClient.GetAsync("Reservation/GetListStatusMaster").ConfigureAwait(true);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<StatusMasterResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<List<StatusMasterResponseDTO>>(result?.Message ?? "Không thể lấy danh sách trạng thái", false);
                    }
                    return result;
                }

                var errorResult = JsonConvert.DeserializeObject<ApiResult<List<StatusMasterResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách trạng thái";
                return HandleErrorResponse<List<StatusMasterResponseDTO>>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<List<StatusMasterResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<List<StatusMasterResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<List<StatusMasterResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }
        public async Task<ApiResult<ReservationResponseDTO>> GetReservationsByStockOutCodeAsync(string stockOutCode)
        {
            if (string.IsNullOrEmpty(stockOutCode))
            {
                return new ApiResult<ReservationResponseDTO>("Mã xuất kho không được để trống", false);
            }

            try
            {
                var response = await _httpClient.GetAsync($"Reservation/GetReservationsByStockOutCodeAsync?stockOutCode={Uri.EscapeDataString(stockOutCode)}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<ReservationResponseDTO>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<ReservationResponseDTO>(result?.Message ?? "Không thể lấy đặt chỗ theo mã xuất kho", false);
                    }
                    return result;
                }

                var errorResult = JsonConvert.DeserializeObject<ApiResult<ReservationResponseDTO>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy đặt chỗ theo mã xuất kho";
                return HandleErrorResponse<ReservationResponseDTO>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<ReservationResponseDTO>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<ReservationResponseDTO>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<ReservationResponseDTO>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<ReservationResponseDTO>> GetReservationsByMovementCodeAsync(string movementCode)
        {
            if (string.IsNullOrEmpty(movementCode))
            {
                return new ApiResult<ReservationResponseDTO>("Mã di chuyển không được để trống", false);
            }

            try
            {
                var response = await _httpClient.GetAsync($"Reservation/GetReservationsByMovementCodeAsync?movementCode={Uri.EscapeDataString(movementCode)}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<ReservationResponseDTO>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<ReservationResponseDTO>(result?.Message ?? "Không thể lấy đặt chỗ theo mã di chuyển", false);
                    }
                    return result;
                }

                var errorResult = JsonConvert.DeserializeObject<ApiResult<ReservationResponseDTO>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy đặt chỗ theo mã di chuyển";
                return HandleErrorResponse<ReservationResponseDTO>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<ReservationResponseDTO>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<ReservationResponseDTO>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<ReservationResponseDTO>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<ReservationResponseDTO>> GetReservationsByTransferCodeAsync(string transferCode)
        {
            if (string.IsNullOrEmpty(transferCode))
            {
                return new ApiResult<ReservationResponseDTO>("Mã chuyển kho không được để trống", false);
            }

            try
            {
                var response = await _httpClient.GetAsync($"Reservation/GetReservationsByTransferCodeAsync?transferCode={Uri.EscapeDataString(transferCode)}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<ReservationResponseDTO>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<ReservationResponseDTO>(result?.Message ?? "Không thể lấy đặt chỗ theo mã chuyển kho", false);
                    }
                    return result;
                }

                var errorResult = JsonConvert.DeserializeObject<ApiResult<ReservationResponseDTO>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy đặt chỗ theo mã chuyển kho";
                return HandleErrorResponse<ReservationResponseDTO>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<ReservationResponseDTO>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<ReservationResponseDTO>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<ReservationResponseDTO>($"Lỗi không xác định: {ex.Message}", false);
            }
        }
        // Lấy danh sách quyền kho
        public async Task<ApiResult<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission(string[] warehouseCodes)
        {
            try
            {
                var queryString = string.Join("&", warehouseCodes.Select(w => $"warehouseCodes={Uri.EscapeDataString(w)}"));
                var response = await _httpClient.GetAsync($"Reservation/GetListWarehousePermission?{queryString}").ConfigureAwait(true);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<WarehouseMasterResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<List<WarehouseMasterResponseDTO>>(result?.Message ?? "Không thể lấy danh sách quyền kho", false);
                    }
                    return result;
                }

                var errorResult = JsonConvert.DeserializeObject<ApiResult<List<WarehouseMasterResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách quyền kho";
                return HandleErrorResponse<List<WarehouseMasterResponseDTO>>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<List<WarehouseMasterResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<List<WarehouseMasterResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<List<WarehouseMasterResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        // Thêm đặt chỗ mới
        public async Task<ApiResult<bool>> AddReservation(ReservationRequestDTO reservation)
        {
            if (reservation == null)
            {
                return new ApiResult<bool>("Dữ liệu đặt chỗ không được để trống", false);
            }
            if (string.IsNullOrEmpty(reservation.ReservationCode) || string.IsNullOrEmpty(reservation.WarehouseCode) ||
                string.IsNullOrEmpty(reservation.OrderTypeCode) || string.IsNullOrEmpty(reservation.OrderId) ||
                string.IsNullOrEmpty(reservation.ReservationDate))
            {
                return new ApiResult<bool>("Các trường mã đặt chỗ, mã kho, mã loại đơn hàng, mã đơn hàng và ngày đặt chỗ không được để trống", false);
            }

            try
            {
                var json = JsonConvert.SerializeObject(reservation);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("Reservation/AddReservation", content).ConfigureAwait(true);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể thêm đặt chỗ", false);
                    }
                    return result;
                }

                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể thêm đặt chỗ";
                return HandleErrorResponse<bool>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<bool>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<bool>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        // Xóa đặt chỗ
        public async Task<ApiResult<bool>> DeleteReservationAsync(string reservationCode)
        {
            if (string.IsNullOrEmpty(reservationCode))
            {
                return new ApiResult<bool>("Mã đặt chỗ không được để trống", false);
            }

            try
            {
                var response = await _httpClient.DeleteAsync($"Reservation/DeleteReservationAsync?reservationCode={Uri.EscapeDataString(reservationCode)}").ConfigureAwait(true);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể xóa đặt chỗ", false);
                    }
                    return result;
                }

                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể xóa đặt chỗ";
                return HandleErrorResponse<bool>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<bool>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<bool>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        // Cập nhật đặt chỗ
        public async Task<ApiResult<bool>> UpdateReservation(ReservationRequestDTO reservation)
        {
            if (reservation == null || string.IsNullOrEmpty(reservation.ReservationCode))
            {
                return new ApiResult<bool>("Dữ liệu đặt chỗ hoặc mã đặt chỗ không được để trống", false);
            }

            try
            {
                var json = JsonConvert.SerializeObject(reservation);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("Reservation/UpdateReservation", content).ConfigureAwait(true);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể cập nhật đặt chỗ", false);
                    }
                    return result;
                }

                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể cập nhật đặt chỗ";
                return HandleErrorResponse<bool>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<bool>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<bool>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        // Xử lý lỗi phản hồi HTTP
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