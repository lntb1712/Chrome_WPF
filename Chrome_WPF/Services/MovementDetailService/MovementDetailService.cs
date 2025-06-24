using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.MovementDetailDTO;
using Chrome_WPF.Models.MovementDTO;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.ProductMasterDTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.MovementDetailService
{
    public class MovementDetailService : IMovementDetailService
    {
        private readonly HttpClient _httpClient;
        private readonly API_Constant _baseUrl;

        public MovementDetailService(API_Constant baseUrl)
        {
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            _httpClient = _baseUrl.GetHttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AccessToken);
        }

        public async Task<ApiResult<PagedResponse<MovementDetailResponseDTO>>> GetAllMovementDetailsAsync(string[] warehouseCodes, int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<MovementDetailResponseDTO>>("Trang và kích thước trang phải lớn hơn 0", false);
            }
            try
            {
                var warehouseCodesQuery = string.Join("&warehouseCodes=", warehouseCodes.Select(Uri.EscapeDataString));
                var response = await _httpClient.GetAsync($"MovementDetail/GetAllMovementDetails?warehouseCodes={warehouseCodesQuery}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<MovementDetailResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<MovementDetailResponseDTO>>(result?.Message ?? "Không thể lấy danh sách chi tiết lệnh di chuyển", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<MovementDetailResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách chi tiết lệnh di chuyển";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<MovementDetailResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<MovementDetailResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<MovementDetailResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<MovementDetailResponseDTO>>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<PagedResponse<MovementDetailResponseDTO>>((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<MovementDetailResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<MovementDetailResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<MovementDetailResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<MovementDetailResponseDTO>>> GetMovementDetailsByMovementCodeAsync(string movementCode, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(movementCode))
            {
                return new ApiResult<PagedResponse<MovementDetailResponseDTO>>("Mã lệnh di chuyển không được để trống", false);
            }
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<MovementDetailResponseDTO>>("Trang và kích thước trang phải lớn hơn 0", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"MovementDetail/GetMovementDetailsByMovementCode?movementCode={Uri.EscapeDataString(movementCode)}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<MovementDetailResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<MovementDetailResponseDTO>>(result?.Message ?? "Không thể lấy chi tiết lệnh di chuyển", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<MovementDetailResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy chi tiết lệnh di chuyển";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<MovementDetailResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<MovementDetailResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<MovementDetailResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<MovementDetailResponseDTO>>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<PagedResponse<MovementDetailResponseDTO>>((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<MovementDetailResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<MovementDetailResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<MovementDetailResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<MovementDetailResponseDTO>>> SearchMovementDetailsAsync(string[] warehouseCodes, string movementCode, string textToSearch, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(movementCode))
            {
                return new ApiResult<PagedResponse<MovementDetailResponseDTO>>("Mã lệnh di chuyển không được để trống", false);
            }
            if (string.IsNullOrEmpty(textToSearch))
            {
                return new ApiResult<PagedResponse<MovementDetailResponseDTO>>("Văn bản tìm kiếm không được để trống", false);
            }
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<MovementDetailResponseDTO>>("Trang và kích thước trang phải lớn hơn 0", false);
            }
            try
            {
                var warehouseCodesQuery = string.Join("&warehouseCodes=", warehouseCodes.Select(Uri.EscapeDataString));
                var response = await _httpClient.GetAsync($"MovementDetail/SearchMovementDetails?warehouseCodes={warehouseCodesQuery}&movementCode={Uri.EscapeDataString(movementCode)}&textToSearch={Uri.EscapeDataString(textToSearch)}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<MovementDetailResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<MovementDetailResponseDTO>>(result?.Message ?? "Không thể tìm kiếm chi tiết lệnh di chuyển", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<MovementDetailResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể tìm kiếm chi tiết lệnh di chuyển";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<MovementDetailResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<MovementDetailResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<MovementDetailResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<MovementDetailResponseDTO>>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<PagedResponse<MovementDetailResponseDTO>>((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<MovementDetailResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<MovementDetailResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<MovementDetailResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<bool>> AddMovementDetail(MovementDetailRequestDTO movementDetail)
        {
            if (movementDetail == null)
            {
                return new ApiResult<bool>("Dữ liệu chi tiết lệnh di chuyển không được để trống", false);
            }
            if (string.IsNullOrEmpty(movementDetail.MovementCode) || string.IsNullOrEmpty(movementDetail.ProductCode))
            {
                return new ApiResult<bool>("Mã lệnh di chuyển và mã sản phẩm không được để trống", false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(movementDetail);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("MovementDetail/AddMovementDetail", content).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể thêm chi tiết lệnh di chuyển", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể thêm chi tiết lệnh di chuyển";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<bool>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<bool>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<bool>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<bool>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<bool>((string)errorMessage, false);
                }
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

        public async Task<ApiResult<bool>> UpdateMovementDetail(MovementDetailRequestDTO movementDetail)
        {
            if (movementDetail == null)
            {
                return new ApiResult<bool>("Dữ liệu chi tiết lệnh di chuyển không được để trống", false);
            }
            if (string.IsNullOrEmpty(movementDetail.MovementCode) || string.IsNullOrEmpty(movementDetail.ProductCode))
            {
                return new ApiResult<bool>("Mã lệnh di chuyển và mã sản phẩm không được để trống", false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(movementDetail);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("MovementDetail/UpdateMovementDetail", content).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể cập nhật chi tiết lệnh di chuyển", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể cập nhật chi tiết lệnh di chuyển";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<bool>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<bool>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<bool>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<bool>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<bool>((string)errorMessage, false);
                }
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

        public async Task<ApiResult<bool>> DeleteMovementDetail(string movementCode, string productCode)
        {
            if (string.IsNullOrEmpty(movementCode) || string.IsNullOrEmpty(productCode))
            {
                return new ApiResult<bool>("Mã lệnh di chuyển và mã sản phẩm không được để trống", false);
            }
            try
            {
                var response = await _httpClient.DeleteAsync($"MovementDetail/DeleteMovementDetail?movementCode={Uri.EscapeDataString(movementCode)}&productCode={Uri.EscapeDataString(productCode)}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể xóa chi tiết lệnh di chuyển", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể xóa chi tiết lệnh di chuyển";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<bool>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<bool>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<bool>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<bool>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<bool>((string)errorMessage, false);
                }
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

        public async Task<ApiResult<ProductMasterResponseDTO>> GetProductByLocationCode(string locationCode)
        {
            if (string.IsNullOrEmpty(locationCode))
            {
                return new ApiResult<ProductMasterResponseDTO>("Mã vị trí không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"MovementDetail/GetProductByLocationCode?locationCode={Uri.EscapeDataString(locationCode)}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<ProductMasterResponseDTO>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<ProductMasterResponseDTO>(result?.Message ?? "Không thể lấy thông tin sản phẩm theo mã vị trí", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<ProductMasterResponseDTO>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy thông tin sản phẩm theo mã vị trí";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<ProductMasterResponseDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<ProductMasterResponseDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<ProductMasterResponseDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<ProductMasterResponseDTO>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<ProductMasterResponseDTO>((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<ProductMasterResponseDTO>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<ProductMasterResponseDTO>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<ProductMasterResponseDTO>($"Lỗi không xác định: {ex.Message}", false);
            }
        }
    }
}
