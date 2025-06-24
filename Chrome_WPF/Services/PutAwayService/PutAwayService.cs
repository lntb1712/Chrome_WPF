using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.PutAwayDTO;
using Chrome_WPF.Models.StatusMasterDTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.PutAwayService
{
    public class PutAwayService : IPutAwayService
    {
        private readonly HttpClient _httpClient;
        private readonly API_Constant _baseUrl;

        public PutAwayService(API_Constant baseUrl)
        {
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            _httpClient = _baseUrl.GetHttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AccessToken);
        }
        public async Task<ApiResult<List<StatusMasterResponseDTO>>> GetListStatusMaster()
        {
            try
            {
                var response = await _httpClient.GetAsync("PutAway/GetListStatusMaster").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<StatusMasterResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<List<StatusMasterResponseDTO>>(result?.Message ?? "Không thể phân tích phản hồi", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<List<StatusMasterResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách trạng thái";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<List<StatusMasterResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<List<StatusMasterResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<List<StatusMasterResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<List<StatusMasterResponseDTO>>(errorMessage, false);
                }
                else
                {
                    return new ApiResult<List<StatusMasterResponseDTO>>(errorMessage, false);
                }
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
        public async Task<ApiResult<PagedResponse<PutAwayResponseDTO>>> GetAllPutAwaysAsync(string[] warehouseCodes, int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<PutAwayResponseDTO>>("Trang và kích thước trang phải lớn hơn 0", false);
            }
            try
            {
                var warehouseCodesQuery = string.Join("&warehouseCodes=", warehouseCodes.Select(Uri.EscapeDataString));
                var response = await _httpClient.GetAsync($"PutAway/GetAllPutAways?warehouseCodes={warehouseCodesQuery}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<PutAwayResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<PutAwayResponseDTO>>(result?.Message ?? "Không thể lấy danh sách lệnh để hàng", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<PutAwayResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách lệnh để hàng";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<PutAwayResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<PutAwayResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<PutAwayResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<PutAwayResponseDTO>>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<PagedResponse<PutAwayResponseDTO>>((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<PutAwayResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<PutAwayResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<PutAwayResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<PutAwayResponseDTO>>> GetAllPutAwaysWithStatusAsync(string[] warehouseCodes, int statusId, int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<PutAwayResponseDTO>>("Trang và kích thước trang phải lớn hơn 0", false);
            }
            if (statusId < 1)
            {
                return new ApiResult<PagedResponse<PutAwayResponseDTO>>("Mã trạng thái phải lớn hơn 0", false);
            }
            try
            {
                var warehouseCodesQuery = string.Join("&warehouseCodes=", warehouseCodes.Select(Uri.EscapeDataString));
                var response = await _httpClient.GetAsync($"PutAway/GetAllPutAwaysWithStatus?warehouseCodes={warehouseCodesQuery}&statusId={statusId}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<PutAwayResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<PutAwayResponseDTO>>(result?.Message ?? "Không thể lấy danh sách lệnh để hàng theo trạng thái", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<PutAwayResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách lệnh để hàng theo trạng thái";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<PutAwayResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<PutAwayResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<PutAwayResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<PutAwayResponseDTO>>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<PagedResponse<PutAwayResponseDTO>>((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<PutAwayResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<PutAwayResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<PutAwayResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<PutAwayResponseDTO>>> SearchPutAwaysAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(textToSearch) || page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<PutAwayResponseDTO>>("Văn bản tìm kiếm, trang và kích thước trang phải hợp lệ", false);
            }
            try
            {
                var warehouseCodesQuery = string.Join("&warehouseCodes=", warehouseCodes.Select(Uri.EscapeDataString));
                var response = await _httpClient.GetAsync($"PutAway/SearchPutAways?warehouseCodes={warehouseCodesQuery}&textToSearch={Uri.EscapeDataString(textToSearch)}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<PutAwayResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<PutAwayResponseDTO>>(result?.Message ?? "Không thể tìm kiếm lệnh để hàng", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<PutAwayResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể tìm kiếm lệnh để hàng";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<PutAwayResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<PutAwayResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<PutAwayResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<PutAwayResponseDTO>>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<PagedResponse<PutAwayResponseDTO>>((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<PutAwayResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<PutAwayResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<PutAwayResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PutAwayResponseDTO>> GetPutAwayByCodeAsync(string putAwayCode)
        {
            if (string.IsNullOrEmpty(putAwayCode))
            {
                return new ApiResult<PutAwayResponseDTO>("Mã lệnh để hàng không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"PutAway/GetPutAwayByCode?putAwayCode={Uri.EscapeDataString(putAwayCode)}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PutAwayResponseDTO>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PutAwayResponseDTO>(result?.Message ?? "Không thể lấy lệnh để hàng", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PutAwayResponseDTO>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy lệnh để hàng";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PutAwayResponseDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PutAwayResponseDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PutAwayResponseDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PutAwayResponseDTO>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<PutAwayResponseDTO>((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PutAwayResponseDTO>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PutAwayResponseDTO>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PutAwayResponseDTO>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<bool>> AddPutAway(PutAwayRequestDTO putAway)
        {
            if (putAway == null)
            {
                return new ApiResult<bool>("Dữ liệu lệnh để hàng không được để trống", false);
            }
            if (string.IsNullOrEmpty(putAway.PutAwayCode))
            {
                return new ApiResult<bool>("Mã lệnh để hàng không được để trống", false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(putAway);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("PutAway/AddPutAway", content).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể thêm lệnh để hàng", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể thêm lệnh để hàng";
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

        public async Task<ApiResult<bool>> DeletePutAway(string putAwayCode)
        {
            if (string.IsNullOrEmpty(putAwayCode))
            {
                return new ApiResult<bool>("Mã lệnh để hàng không được để trống", false);
            }
            try
            {
                var response = await _httpClient.DeleteAsync($"PutAway/DeletePutAway?putAwayCode={Uri.EscapeDataString(putAwayCode)}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể xóa lệnh để hàng", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể xóa lệnh để hàng";
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

        public async Task<ApiResult<bool>> UpdatePutAway(PutAwayRequestDTO putAway)
        {
            if (putAway == null)
            {
                return new ApiResult<bool>("Dữ liệu lệnh để hàng không được để trống", false);
            }
            if (string.IsNullOrEmpty(putAway.PutAwayCode))
            {
                return new ApiResult<bool>("Mã lệnh để hàng không được để trống", false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(putAway);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("PutAway/UpdatePutAway", content).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể cập nhật lệnh để hàng", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể cập nhật lệnh để hàng";
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
