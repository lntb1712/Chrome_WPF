using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.CategoryDTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.CodeGeneratorService
{
    public class CodeGeneratorService : ICodeGenerateService
    {
        private readonly HttpClient _httpClient;
        private readonly API_Constant _baseUrl;
        public CodeGeneratorService (API_Constant baseUrl)
        {
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            _httpClient = _baseUrl.GetHttpClient();
        }

        public async Task<ApiResult<string>> CodeGeneratorAsync(string warehouseCode,string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return new ApiResult<string>("Mã loại không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"CodeGenerator/GenerateCodeAsync?warehouseCode={warehouseCode}&type={type}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<string>>(jsonResponse);
                    return result ?? new ApiResult<string>("Không thể giải mã kết quả", false);
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<string>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy mã, vui lòng thử lại sau.";
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<string>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Không tìm thấy loại mã"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<string>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<string>(errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Xử lý lỗi kết nối hoặc lỗi mạng
                return new ApiResult<string>($"Lỗi kết nối: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<string>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi khác
                return new ApiResult<string>($"Lỗi không xác định: {ex.Message}", false);
            }
        }
    }
}
