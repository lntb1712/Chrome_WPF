using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.LocationMasterDTO;
using Chrome_WPF.Models.PagedResponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.LocationMasterService
{
    public class LocationMasterService : ILocationMasterService
    {
        private readonly HttpClient _httpClient;
        private readonly API_Constant _baseUrl;

        public LocationMasterService(API_Constant baseUrl)
        {
            _baseUrl = baseUrl;
            _httpClient = _baseUrl.GetHttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AccessToken);
        }

        public async Task<ApiResult<bool>> AddLocationMaster(LocationMasterRequestDTO locationMaster)
        {
            if (locationMaster == null)
            {
                return new ApiResult<bool>("Dữ liệu không hợp lệ", false);
            }
            if (string.IsNullOrEmpty(locationMaster.LocationCode) || string.IsNullOrEmpty(locationMaster.LocationName))
            {
                return new ApiResult<bool>("Mã và tên vị trí không được để trống", false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(locationMaster);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"WarehouseMaster/{locationMaster.WarehouseCode}/LocationMaster/AddLocationMaster", content).ConfigureAwait(false);
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
                var errorMessage = errorResult?.Message ?? "Không thể thêm vị trí";
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
        public async Task<ApiResult<bool>> DeleteLocationMaster(string warehouseCode, string locationCode)
        {
            if (string.IsNullOrEmpty(locationCode))
            {
                return new ApiResult<bool>("Mã vị trí không được để trống", false);
            }
            locationCode = locationCode.Trim();
            try
            {
                var response = await _httpClient.DeleteAsync($"WarehouseMaster/{warehouseCode}/LocationMaster/DeleteLocationMaster?locationCode={locationCode}").ConfigureAwait(false);
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
                var errorMessage = errorResult?.Message ?? "Không thể xóa vị trí"; if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
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

        public async Task<ApiResult<PagedResponse<LocationMasterResponseDTO>>> GetAllLocationMaster(string warehouseCode, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(warehouseCode) || page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<LocationMasterResponseDTO>>("Dữ liệu không hợp lệ", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"WarehouseMaster/{warehouseCode}/LocationMaster/GetAllLocationMaster?page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<LocationMasterResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<LocationMasterResponseDTO>>(result?.Message ?? "Không thể phân tích phản hồi", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<LocationMasterResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách vị trí";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<LocationMasterResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<LocationMasterResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<LocationMasterResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<LocationMasterResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<PagedResponse<LocationMasterResponseDTO>>((string)errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<PagedResponse<LocationMasterResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            { 
                return new ApiResult<PagedResponse<LocationMasterResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<PagedResponse<LocationMasterResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<LocationMasterResponseDTO>> GetLocationWithLocationCode(string warehouseCode, string locationCode)
        {
            if (string.IsNullOrEmpty(warehouseCode) || string.IsNullOrEmpty(locationCode))
            {
                return new ApiResult<LocationMasterResponseDTO>("Dữ liệu không hợp lệ", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"WarehouseMaster/{warehouseCode}/LocationMaster/GetLocationWithLocationCode?locationCode={locationCode}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<LocationMasterResponseDTO>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<LocationMasterResponseDTO>(result?.Message ?? "Không thể phân tích phản hồi", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<LocationMasterResponseDTO>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy thông tin vị trí";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<LocationMasterResponseDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<LocationMasterResponseDTO>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<LocationMasterResponseDTO>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<LocationMasterResponseDTO>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<LocationMasterResponseDTO>((string)errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<LocationMasterResponseDTO>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<LocationMasterResponseDTO>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<LocationMasterResponseDTO>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<int>> GetTotalLocationCount(string warehouseCode)
        {
            try
            {
                var response = await _httpClient.GetAsync($"WarehouseMaster/{warehouseCode}/LocationMaster/GetTotalLocationCount").ConfigureAwait(false);
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
                var errorMessage = errorResult?.Message ?? "Không thể lấy tổng số vị trí";
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

        public async Task<ApiResult<bool>> UpdateLocationMaster(LocationMasterRequestDTO locationMaster)
        {
            if (locationMaster == null)
            {
                return new ApiResult<bool>("Dữ liệu không hợp lệ", false);
            }
            if (string.IsNullOrEmpty(locationMaster.LocationCode) || string.IsNullOrEmpty(locationMaster.LocationName))
            {
                return new ApiResult<bool>("Mã và tên vị trí không được để trống", false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(locationMaster);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"WarehouseMaster/{locationMaster.WarehouseCode}/LocationMaster/UpdateLocationMaster", content).ConfigureAwait(false);
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
                var errorMessage = errorResult?.Message ?? "Không thể cập nhật vị trí";
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
