using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.BOMComponentDTO;
using Chrome_WPF.Models.BOMMasterDTO;
using Chrome_WPF.Models.PagedResponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.BOMComponentService
{
    public class BOMComponentService:IBOMComponentService
    {
        private readonly HttpClient _httpClient;
        private readonly API_Constant _baseUrl;

        public BOMComponentService(API_Constant baseUrl)
        {
            _baseUrl = baseUrl;
            _httpClient = _baseUrl.GetHttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AccessToken);
        }

        public async Task<ApiResult<bool>> AddBomComponent(BOMComponentRequestDTO bOMComponentRequestDTO)
        {
            if (bOMComponentRequestDTO == null)
            {
                return new ApiResult<bool>("Dữ liệu nhận vào không hợp lệ", false);
            }
            if (string.IsNullOrEmpty(bOMComponentRequestDTO.BOMCode) || string.IsNullOrEmpty(bOMComponentRequestDTO.BOMVersion) || string.IsNullOrEmpty(bOMComponentRequestDTO.ComponentCode))
            {
                return new ApiResult<bool>("Dữ liệu nhận vào không được để trống",false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(bOMComponentRequestDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"BOMMaster/{bOMComponentRequestDTO.BOMCode}/{bOMComponentRequestDTO.BOMVersion}/BOMComponent/AddBomComponent", content).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if(response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể thêm chi tiết định mức nguyên vật liệu", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể thêm chi tiết định mức";
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

        public async Task<ApiResult<bool>> DeleteBomComponent(string bomCode, string bomVersion, string componentCode)
        {
            if (string.IsNullOrEmpty(bomCode) || string.IsNullOrEmpty(bomVersion)|| string.IsNullOrEmpty(componentCode))
            {
                return new ApiResult<bool>("Dữ liệu nhận vào không được để trống", false);
            }
            try
            {
                var response = await _httpClient.DeleteAsync($"BOMMaster/{bomCode}/{bomVersion}/BOMComponent/DeleteBomComponent?componentCode={componentCode}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể xóa", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể xóa ";
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

        public async Task<ApiResult<List<BOMComponentResponseDTO>>> GetAllBOMComponent(string bomCode, string bomVersion)
        {
            if(string.IsNullOrEmpty(bomCode)||string.IsNullOrEmpty(bomVersion))
            {
                return new ApiResult<List<BOMComponentResponseDTO>>("Dữ liệu nhận vào không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"BOMMaster/{bomCode}/{bomVersion}/BOMComponent/GetAllBOMComponent").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if(response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<BOMComponentResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<List<BOMComponentResponseDTO>>(result?.Message ?? "Không thể phân tích phản hồi");
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<List<BOMComponentResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách định mức";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<List<BOMComponentResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<List<BOMComponentResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<List<BOMComponentResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<List<BOMComponentResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<List<BOMComponentResponseDTO>>((string)errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<List<BOMComponentResponseDTO>>($"Lỗi mạng: {ex.Message}", false);

            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<List<BOMComponentResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<List<BOMComponentResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<BOMComponentResponseDTO>> GetBOMComponent(string bomCode, string bomVersion, string componentCode)
        {
            if(string.IsNullOrEmpty(bomCode)||string.IsNullOrEmpty(bomVersion) || string.IsNullOrEmpty(componentCode))
            {
                return new ApiResult<BOMComponentResponseDTO>("Dữ liệu nhận vào không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"BOMMaster/{bomCode}/{bomVersion}/BOMComponent/GetBOMComponent?componentCode={componentCode}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if(response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<BOMComponentResponseDTO>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<BOMComponentResponseDTO>(result?.Message ?? "Không thể phân tích phản hồi");
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<BOMComponentResponseDTO>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách quy tắc để hàng";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<BOMComponentResponseDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<BOMComponentResponseDTO>((string)errorMessage, false); 
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<BOMComponentResponseDTO>((string)errorMessage, false); 
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<BOMComponentResponseDTO>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<BOMComponentResponseDTO>((string)errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<BOMComponentResponseDTO>($"Lỗi mạng: {ex.Message}", false);

            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<BOMComponentResponseDTO>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<BOMComponentResponseDTO>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<List<BOMNodeDTO>>> GetRecursiveBOMAsync(string bomCode, string bomVersion)
        {
            if (string.IsNullOrEmpty(bomCode) || string.IsNullOrEmpty(bomVersion))
            {
                return new ApiResult<List<BOMNodeDTO>>("Dữ liệu nhận vào không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"BOMMaster/{bomCode}/{bomVersion}/BOMComponent/GetRecursiveBOMAsync").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if(response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<BOMNodeDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<List<BOMNodeDTO>>(result?.Message ?? "Không thể phân tích phản hồi");
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<List<BOMNodeDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách định mức";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<List<BOMNodeDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<List<BOMNodeDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<List<BOMNodeDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<List<BOMNodeDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<List<BOMNodeDTO>>((string)errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<List<BOMNodeDTO>>($"Lỗi mạng: {ex.Message}", false);

            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<List<BOMNodeDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<List<BOMNodeDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<bool>> UpdateBomComponent(BOMComponentRequestDTO bOMComponentRequestDTO)
        {
            if (bOMComponentRequestDTO == null)
            {
                return new ApiResult<bool>("Dữ liệu nhận vào không hợp lệ", false);
            }
            if (string.IsNullOrEmpty(bOMComponentRequestDTO.BOMCode) || string.IsNullOrEmpty(bOMComponentRequestDTO.BOMVersion) || string.IsNullOrEmpty(bOMComponentRequestDTO.ComponentCode))
            {
                return new ApiResult<bool>("Dữ liệu nhận vào không được để trống", false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(bOMComponentRequestDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"BOMMaster/{bOMComponentRequestDTO.BOMCode}/{bOMComponentRequestDTO.BOMVersion}/BOMComponent/UpdateBomComponent", content).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể cập nhật chi tiết định mức nguyên vật liệu", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể cập nhật chi tiết định mức";
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
