using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.PutAwayDetailDTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.PutAwayDetailService
{
    public class PutAwayDetailService : IPutAwayDetailService
    {
        private readonly HttpClient _httpClient;
        private readonly API_Constant _baseUrl;

        public PutAwayDetailService(API_Constant baseUrl)
        {
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            _httpClient = _baseUrl.GetHttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AccessToken);
        }

        public async Task<ApiResult<PagedResponse<PutAwayDetailResponseDTO>>> GetAllPutAwayDetailsAsync(string[] warehouseCodes, int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>("Trang và kích thước trang phải lớn hơn 0", false);
            }
            try
            {
                var warehouseCodesQuery = string.Join("&warehouseCodes=", warehouseCodes.Select(Uri.EscapeDataString));
                var response = await _httpClient.GetAsync($"PutAway/{Uri.EscapeDataString("all")}/PutAwayDetail/GetAllPutAwayDetails?warehouseCodes={warehouseCodesQuery}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<PutAwayDetailResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>(result?.Message ?? "Không thể lấy danh sách chi tiết lệnh để hàng", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<PutAwayDetailResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách chi tiết lệnh để hàng";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<PutAwayDetailResponseDTO>>> GetPutAwayDetailsByPutawayCodeAsync(string putAwayCode, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(putAwayCode))
            {
                return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>("Mã lệnh để hàng không được để trống", false);
            }
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>("Trang và kích thước trang phải lớn hơn 0", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"PutAway/{Uri.EscapeDataString(putAwayCode)}/PutAwayDetail/GetPutAwayDetailsByPutawayCode?page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<PutAwayDetailResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>(result?.Message ?? "Không thể lấy chi tiết lệnh để hàng", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<PutAwayDetailResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy chi tiết lệnh để hàng";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<PutAwayDetailResponseDTO>>> SearchPutAwayDetailsAsync(string[] warehouseCodes, string putAwayCode, string textToSearch, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(putAwayCode))
            {
                return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>("Mã lệnh để hàng không được để trống", false);
            }
            if (string.IsNullOrEmpty(textToSearch))
            {
                return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>("Văn bản tìm kiếm không được để trống", false);
            }
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>("Trang và kích thước trang phải lớn hơn 0", false);
            }
            try
            {
                var warehouseCodesQuery = string.Join("&warehouseCodes=", warehouseCodes.Select(Uri.EscapeDataString));
                var response = await _httpClient.GetAsync($"PutAway/{Uri.EscapeDataString(putAwayCode)}/PutAwayDetail/SearchPutAwayDetails?warehouseCodes={warehouseCodesQuery}&textToSearch={Uri.EscapeDataString(textToSearch)}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<PutAwayDetailResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>(result?.Message ?? "Không thể tìm kiếm chi tiết lệnh để hàng", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<PutAwayDetailResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể tìm kiếm chi tiết lệnh để hàng";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<PutAwayDetailResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<bool>> UpdatePutAwayDetail(PutAwayDetailRequestDTO putAwayDetail)
        {
            if (putAwayDetail == null)
            {
                return new ApiResult<bool>("Dữ liệu chi tiết lệnh để hàng không được để trống", false);
            }
            if (string.IsNullOrEmpty(putAwayDetail.PutAwayCode) || string.IsNullOrEmpty(putAwayDetail.ProductCode))
            {
                return new ApiResult<bool>("Mã lệnh để hàng và mã sản phẩm không được để trống", false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(putAwayDetail);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"PutAway/{Uri.EscapeDataString(putAwayDetail.PutAwayCode)}/PutAwayDetail/UpdatePutAwayDetail", content).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể cập nhật chi tiết lệnh để hàng", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể cập nhật chi tiết lệnh để hàng";
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

        public async Task<ApiResult<bool>> DeletePutAwayDetail(string putAwayCode, string productCode)
        {
            if (string.IsNullOrEmpty(putAwayCode) || string.IsNullOrEmpty(productCode))
            {
                return new ApiResult<bool>("Mã lệnh để hàng và mã sản phẩm không được để trống", false);
            }
            try
            {
                var response = await _httpClient.DeleteAsync($"PutAway/{Uri.EscapeDataString(putAwayCode)}/PutAwayDetail/DeletePutAwayDetail?productCode={Uri.EscapeDataString(productCode)}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể xóa chi tiết lệnh để hàng", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể xóa chi tiết lệnh để hàng";
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
    }
}
