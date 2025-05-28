using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.CategoryDTO;
using Chrome_WPF.Models.GroupManagementDTO;
using Chrome_WPF.Models.PagedResponse;
using Newtonsoft.Json;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly API_Constant _baseUrl;

        public CategoryService(API_Constant baseUrl)
        {
            _baseUrl = baseUrl;
            _httpClient = _baseUrl.GetHttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AccessToken);
        }

        public async Task<ApiResult<List<CategoryResponseDTO>>> GetAllCategories()
        {
            try
            {
                var response = await _httpClient.GetAsync("ProductMaster/GetAllCategories").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if(response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<CategoryResponseDTO>>>(jsonResponse);
                    if(result == null || !result.Success)
                    {
                        return new ApiResult<List<CategoryResponseDTO>>(result?.Message ?? "Không thể phân tích phản hồi", false);
                    }
                    return result;
                }    
                var errorResult = JsonConvert.DeserializeObject<ApiResult<List<CategoryResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định khi lấy danh sách danh mục";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<List<CategoryResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<List<CategoryResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<List<CategoryResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<List<CategoryResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<List<CategoryResponseDTO>>((string)errorMessage, false); // Trả về thông điệp lỗi chung    
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<List<CategoryResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<List<CategoryResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<List<CategoryResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<List<CategorySummaryDTO>>> GetCategorySummary()
        {
            try
            {
                var response = await _httpClient.GetAsync("ProductMaster/GetCategorySummary").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<CategorySummaryDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<List<CategorySummaryDTO>>(result?.Message ?? "Không thể phân tích phản hồi", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<List<CategorySummaryDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định khi lấy tóm tắt danh mục";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<List<CategorySummaryDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<List<CategorySummaryDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<List<CategorySummaryDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<List<CategorySummaryDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<List<CategorySummaryDTO>>((string)errorMessage, false); // Trả về thông điệp lỗi chung    
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<List<CategorySummaryDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<List<CategorySummaryDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<List<CategorySummaryDTO>>($"Lỗi không xác định: {ex.Message}", false);

            }
        }
    }
}
