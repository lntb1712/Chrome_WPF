using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.WarehouseMasterDTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.WarehouseMasterService
{
    public class WarehouseMasterService : IWarehouseMasterService
    {
        private readonly HttpClient _httpClient;
        private readonly API_Constant _baseUrl;

        public WarehouseMasterService(API_Constant baseUrl)
        {
            _baseUrl = baseUrl;
            _httpClient = _baseUrl.GetHttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AccessToken); // Replace with actual token
        }

        public async Task<ApiResult<bool>> AddWarehouseMaster(WarehouseMasterRequestDTO warehouseMaster)
        {
            if (warehouseMaster == null)
            {
                return new ApiResult<bool>("Dữ liệu nhận vào không hợp lệ", false);
            }

            if (string.IsNullOrEmpty(warehouseMaster.WarehouseCode) || string.IsNullOrEmpty(warehouseMaster.WarehouseName))
            {
                return new ApiResult<bool>("Mã kho và tên kho không được để trống", false);
            }

            try
            {
                var json = JsonConvert.SerializeObject(warehouseMaster);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"WarehouseMaster/AddWarehouseMaster", content).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể phân tích phan hồi", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể thêm kho";
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

        public async Task<ApiResult<bool>> DeleteWarehouseMaster(string warehouseCode)
        {
            if (string.IsNullOrEmpty(warehouseCode))
            {
                return new ApiResult<bool>("Mã kho không được để trống", false);
            }
            warehouseCode = warehouseCode.Trim(); // Loại bỏ khoảng trắng thừa nếu có
            try
            {
                var response = await _httpClient.DeleteAsync($"WarehouseMaster/DeleteWarehouseMaster?warehouseCode={warehouseCode}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể phân tích phản hồi", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể xóa kho";
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

        public async Task<ApiResult<PagedResponse<WarehouseMasterResponseDTO>>> GetAllWarehouseMaster(int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<WarehouseMasterResponseDTO>>("Trang và kích thước trang phải lớn hơn 0", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"WarehouseMaster/GetAllWarehouseMaster?page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<WarehouseMasterResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<WarehouseMasterResponseDTO>>(result?.Message ?? "Không thể phân tích phản hồi", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<WarehouseMasterResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách kho";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<WarehouseMasterResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<WarehouseMasterResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<WarehouseMasterResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<WarehouseMasterResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<PagedResponse<WarehouseMasterResponseDTO>>((string)errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<PagedResponse<WarehouseMasterResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<PagedResponse<WarehouseMasterResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<PagedResponse<WarehouseMasterResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<int>> GetTotalWarehouseCount()
        {
            try
            {
                var response = await _httpClient.GetAsync("WarehouseMaster/GetTotalWarehouseCount").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<int>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<int>(result?.Message ?? "Không thể phân tích phản hồi", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<int>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy tổng số kho";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<int>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
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

        public async Task<ApiResult<WarehouseMasterResponseDTO>> GetWarehouseWithWarehouseCode(string warehouseCode)
        {
            if (string.IsNullOrEmpty(warehouseCode))
            {
                return new ApiResult<WarehouseMasterResponseDTO>("Mã kho không được để trống", false);
            }
            warehouseCode = warehouseCode.Trim(); // Loại bỏ khoảng trắng thừa nếu có
            try
            {
                var response = await _httpClient.GetAsync($"WarehouseMaster/GetWarehouseWithCode?warehouseCode={warehouseCode}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<WarehouseMasterResponseDTO>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<WarehouseMasterResponseDTO>(result?.Message ?? "Không thể phân tích phản hồi", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<WarehouseMasterResponseDTO>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy thông tin kho";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<WarehouseMasterResponseDTO>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<WarehouseMasterResponseDTO>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<WarehouseMasterResponseDTO>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<WarehouseMasterResponseDTO>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<WarehouseMasterResponseDTO>((string)errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<WarehouseMasterResponseDTO>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<WarehouseMasterResponseDTO>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<WarehouseMasterResponseDTO>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<WarehouseMasterResponseDTO>>> SearchWarehouseMaster(string textToSearch, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(textToSearch))
            {
                return new ApiResult<PagedResponse<WarehouseMasterResponseDTO>>("Chuỗi tìm kiếm không được để trống", false);
            }
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<WarehouseMasterResponseDTO>>("Trang và kích thước trang phải lớn hơn 0", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"WarehouseMaster/SearchWarehouse?textToSearch={textToSearch}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<WarehouseMasterResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<WarehouseMasterResponseDTO>>(result?.Message ?? "Không thể phân tích phản hồi", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<WarehouseMasterResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể tìm kiếm kho";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<WarehouseMasterResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<WarehouseMasterResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<WarehouseMasterResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<WarehouseMasterResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<PagedResponse<WarehouseMasterResponseDTO>>((string)errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<PagedResponse<WarehouseMasterResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<PagedResponse<WarehouseMasterResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<PagedResponse<WarehouseMasterResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<bool>> UpdateWarehouseMaster(WarehouseMasterRequestDTO warehouseMaster)
        {
            if (warehouseMaster == null)
            {
                return new ApiResult<bool>("Dữ liệu nhận vào không hợp lệ", false);
            }
            if (string.IsNullOrEmpty(warehouseMaster.WarehouseCode) || string.IsNullOrEmpty(warehouseMaster.WarehouseName))
            {
                return new ApiResult<bool>("Mã kho và tên kho không được để trống", false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(warehouseMaster);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"WarehouseMaster/UpdateWarehouseMaster", content).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể phân tích phản hồi", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể cập nhật kho";
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
    }
}
