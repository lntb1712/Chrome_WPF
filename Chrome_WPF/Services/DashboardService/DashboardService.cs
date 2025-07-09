using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.DashboardDTO;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.PutAwayDTO;
using DocumentFormat.OpenXml.VariantTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.DashboardService
{
    public class DashboardService:IDashboardService
    {
        private readonly API_Constant _baseUrl;
        private readonly HttpClient _httpClient;
        public DashboardService(API_Constant baseUrl)
        {
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            _httpClient = _baseUrl.GetHttpClient();
        }

        public async Task<ApiResult<DashboardResponseDTO>> GetDashboardInformation(string[] warehouseCodes)
        {
            try
            {
                var warehouseCodesQuery = string.Join("&warehouseCodes=", warehouseCodes.Select(Uri.EscapeDataString));
                var response = await _httpClient.GetAsync($"Dashboard/GetDashboardInformation?warehouseCodes={warehouseCodesQuery}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<DashboardResponseDTO>>(jsonResponse);
                    return result ?? new ApiResult<DashboardResponseDTO>("Không có dữ liệu", false);
                }
                var errorResult = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<DashboardResponseDTO>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định khi lấy thông tin Dashboard";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<DashboardResponseDTO> ((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<DashboardResponseDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<DashboardResponseDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<DashboardResponseDTO>  ((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<DashboardResponseDTO>((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<DashboardResponseDTO>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<DashboardResponseDTO>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<DashboardResponseDTO>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<DashboardStockInOutSummaryDTO>> GetStockInOutSummaryAsync(string[] warehouseCodes)
        {
            try
            {
                var warehouseCodesQuery = string.Join("&warehouseCodes=", warehouseCodes.Select(Uri.EscapeDataString));
                var response = await _httpClient.GetAsync($"Dashboard/GetStockInOutSummaryAsync?warehouseCodes={warehouseCodesQuery}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<DashboardStockInOutSummaryDTO>>(jsonResponse);
                    return result ?? new ApiResult<DashboardStockInOutSummaryDTO>("Không có dữ liệu", false);
                }
                var errorResult = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<DashboardStockInOutSummaryDTO>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định khi lấy thông tin Dashboard";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<DashboardStockInOutSummaryDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<DashboardStockInOutSummaryDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<DashboardStockInOutSummaryDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<DashboardStockInOutSummaryDTO>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<DashboardStockInOutSummaryDTO>((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<DashboardStockInOutSummaryDTO>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<DashboardStockInOutSummaryDTO>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<DashboardStockInOutSummaryDTO>($"Lỗi không xác định: {ex.Message}", false);
            }
        }
    }
}
