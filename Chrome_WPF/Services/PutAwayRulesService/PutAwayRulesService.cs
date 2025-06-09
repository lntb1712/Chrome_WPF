using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.PutAwayRulesDTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.PutAwayRulesService
{
    public class PutAwayRulesService:IPutAwayRulesService
    {
        private readonly HttpClient _httpClient;
        private readonly API_Constant _baseUrl;

        public PutAwayRulesService(API_Constant baseUrl)
        {
            _baseUrl = baseUrl;
            _httpClient = _baseUrl.GetHttpClient();
            _httpClient.DefaultRequestHeaders.Authorization= new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AccessToken);

        }

        public async Task<ApiResult<bool>> AddPutAwayRule(PutAwayRulesRequestDTO putAwayRule)
        {
            if(putAwayRule == null)
            {
                return new ApiResult<bool>("Dữ liệu quy tắc để hàng không được để trống", false);
            }
            if (string.IsNullOrEmpty(putAwayRule.PutAwayRuleCode) || string.IsNullOrEmpty(putAwayRule.WarehouseToApply) || string.IsNullOrEmpty(putAwayRule.ProductCode) || string.IsNullOrEmpty(putAwayRule.LocationCode))
            {
                return new ApiResult<bool>("Các trường mã quy tắc, kho áp dụng, mã sản phẩm và mã vị trí không được để trống", false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(putAwayRule);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"PutAwayRules/AddPutAwayRule", content).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể thêm quy tắc để hàng", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể thêm quy tắc";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<bool>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Trả về thông điệp lỗi chung
                }

            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<bool>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<bool>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<bool>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<bool>> DeletePutAwayRule(string putAwayRuleCode)
        {
            if (string.IsNullOrEmpty(putAwayRuleCode))
            {
                return new ApiResult<bool>("Mã quy tắc để hàng không được để trống", false);
            }
            try
            {
                var response = await _httpClient.DeleteAsync($"PutAwayRules/DeletePutAwayRule?putAwayRuleCode={putAwayRuleCode}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể xóa quy tắc để hàng", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể xóa quy tắc để hàng";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<bool>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<bool>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<bool>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<bool>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<PutAwayRulesResponseDTO>>> GetAllPutAwayRules(int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<PutAwayRulesResponseDTO>>("Trang và kích thước trang phải lớn hơn 0", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"PutAwayRules/GetAllPutAwayRules?page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<PutAwayRulesResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<PutAwayRulesResponseDTO>>(result?.Message ?? "Không thể lấy danh sách quy tắc để hàng", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<PutAwayRulesResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách quy tắc để hàng";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<PutAwayRulesResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<PutAwayRulesResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<PutAwayRulesResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<PutAwayRulesResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<PagedResponse<PutAwayRulesResponseDTO>>((string)errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<PagedResponse<PutAwayRulesResponseDTO>>($"Lỗi mạng: {ex.Message}", false);

            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<PagedResponse<PutAwayRulesResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<PagedResponse<PutAwayRulesResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PutAwayRulesResponseDTO>> GetPutAwayRuleByCode(string putAwayRuleCode)
        {
            if (string.IsNullOrEmpty(putAwayRuleCode))
            {
                return new ApiResult<PutAwayRulesResponseDTO>("Mã quy tắc để hàng không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"PutAwayRules/GetPutAwayRulesByCode?putAwayRuleCode={putAwayRuleCode}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PutAwayRulesResponseDTO>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PutAwayRulesResponseDTO>(result?.Message ?? "Không thể lấy quy tắc để hàng", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PutAwayRulesResponseDTO>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy quy tắc để hàng";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PutAwayRulesResponseDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PutAwayRulesResponseDTO>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PutAwayRulesResponseDTO>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PutAwayRulesResponseDTO>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<PutAwayRulesResponseDTO>((string)errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<PutAwayRulesResponseDTO>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<PutAwayRulesResponseDTO>($"Lỗi phân tích phản hồi: {ex.Message}", false);

            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<PutAwayRulesResponseDTO>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<int>> GetTotalPutAwayRuleCount()
        {
            try
            {
                var response = await _httpClient.GetAsync("PutAwayRules/GetTotalPutAwayRuleCount").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<int>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<int>(result?.Message ?? "Không thể lấy tổng số quy tắc để hàng", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<int>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy tổng số quy tắc để hàng";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<int>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<int>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<int>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<int>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<int>((string)errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<int>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<int>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<int>($"Lỗi không xác định: {ex.Message}", false);
            }

        }

        public async Task<ApiResult<PagedResponse<PutAwayRulesResponseDTO>>> SearchPutAwayRules(string textToSearch, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(textToSearch) || page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<PutAwayRulesResponseDTO>>("Văn bản tìm kiếm, trang và kích thước trang phải lớn hơn 0", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"PutAwayRules/SearchPutAwayRules?textToSearch={textToSearch}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<PutAwayRulesResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<PutAwayRulesResponseDTO>>(result?.Message ?? "Không thể tìm kiếm quy tắc để hàng", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<PutAwayRulesResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể tìm kiếm quy tắc để hàng";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<PutAwayRulesResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<PutAwayRulesResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<PutAwayRulesResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<PutAwayRulesResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<PagedResponse<PutAwayRulesResponseDTO>>((string)errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<PagedResponse<PutAwayRulesResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<PagedResponse<PutAwayRulesResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<PagedResponse<PutAwayRulesResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<bool>> UpdatePutAwayRule(PutAwayRulesRequestDTO putAwayRule)
        {
            if (putAwayRule == null)
            {
                return new ApiResult<bool>("Dữ liệu quy tắc để hàng không được để trống", false);
            }
            if (string.IsNullOrEmpty(putAwayRule.PutAwayRuleCode) || string.IsNullOrEmpty(putAwayRule.WarehouseToApply) || string.IsNullOrEmpty(putAwayRule.ProductCode) || string.IsNullOrEmpty(putAwayRule.LocationCode))
            {
                return new ApiResult<bool>("Các trường mã quy tắc, kho áp dụng, mã sản phẩm và mã vị trí không được để trống", false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(putAwayRule);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"PutAwayRules/UpdatePutAwayRule", content).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể cập nhật quy tắc để hàng", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể cập nhật quy tắc để hàng";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<bool>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<bool>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<bool>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch ( Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<bool>($"Lỗi không xác định: {ex.Message}", false);
            }
        }
    }
}
