using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.DashboardDTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.DashboardService
{
    public class DashboardService : IDashboardService
    {
        private readonly API_Constant _baseUrl;
        private readonly HttpClient _httpClient;

        public DashboardService(API_Constant baseUrl)
        {
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            _httpClient = _baseUrl.GetHttpClient();
        }

        public async Task<ApiResult<DashboardResponseDTO>> GetDashboardInformation(DashboardRequestDTO dashboardRequest)
        {
            try
            {
                if (dashboardRequest == null || (dashboardRequest.warehouseCodes == null && !dashboardRequest.Quarter.HasValue && !dashboardRequest.Month.HasValue && !dashboardRequest.Year.HasValue))
                    return new ApiResult<DashboardResponseDTO>("Dữ liệu nhận vào không hợp lệ", false);

                // Tạo query string từ warehouseCodes và các bộ lọc ngày, tháng, năm
                var queryParams = new List<string>();
                if (dashboardRequest.warehouseCodes != null && dashboardRequest.warehouseCodes.Any())
                {
                    var warehouseCodesQuery = string.Join("&warehouseCodes=", dashboardRequest.warehouseCodes.Select(Uri.EscapeDataString));
                    queryParams.Add($"warehouseCodes={warehouseCodesQuery}");
                }
                if (dashboardRequest.Quarter.HasValue)
                    queryParams.Add($"day={dashboardRequest.Quarter.Value}");
                if (dashboardRequest.Month.HasValue)
                    queryParams.Add($"month={dashboardRequest.Month.Value}");
                if (dashboardRequest.Year.HasValue)
                    queryParams.Add($"year={dashboardRequest.Year.Value}");

                var queryString = string.Join("&", queryParams);
                var response = await _httpClient.PostAsJsonAsync($"Dashboard/GetDashboardInformation", dashboardRequest).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<DashboardResponseDTO>>(jsonResponse);
                    return result ?? new ApiResult<DashboardResponseDTO>("Không có dữ liệu", false);
                }

                var errorResult = JsonConvert.DeserializeObject<ApiResult<DashboardResponseDTO>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định khi lấy thông tin Dashboard";

                return response.StatusCode switch
                {
                    System.Net.HttpStatusCode.Unauthorized => new ApiResult<DashboardResponseDTO>(errorMessage, false),
                    System.Net.HttpStatusCode.Forbidden => new ApiResult<DashboardResponseDTO>(errorMessage, false),
                    System.Net.HttpStatusCode.NotFound => new ApiResult<DashboardResponseDTO>(errorMessage, false),
                    System.Net.HttpStatusCode.InternalServerError => new ApiResult<DashboardResponseDTO>(errorMessage, false),
                    _ => new ApiResult<DashboardResponseDTO>(errorMessage, false)
                };
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

        public async Task<ApiResult<DashboardStockInOutSummaryDTO>> GetStockInOutSummaryAsync(DashboardRequestDTO dashboardRequest)
        {
            try
            {
                if (dashboardRequest == null || (dashboardRequest.warehouseCodes == null && !dashboardRequest.Quarter.HasValue && !dashboardRequest.Month.HasValue && !dashboardRequest.Year.HasValue))
                    return new ApiResult<DashboardStockInOutSummaryDTO>("Dữ liệu nhận vào không hợp lệ", false);

                // Tạo query string từ warehouseCodes và các bộ lọc ngày, tháng, năm
                var queryParams = new List<string>();
                if (dashboardRequest.warehouseCodes != null && dashboardRequest.warehouseCodes.Any())
                {
                    var warehouseCodesQuery = string.Join("&warehouseCodes=", dashboardRequest.warehouseCodes.Select(Uri.EscapeDataString));
                    queryParams.Add($"warehouseCodes={warehouseCodesQuery}");
                }
                if (dashboardRequest.Quarter.HasValue)
                    queryParams.Add($"day={dashboardRequest.Quarter.Value}");
                if (dashboardRequest.Month.HasValue)
                    queryParams.Add($"month={dashboardRequest.Month.Value}");
                if (dashboardRequest.Year.HasValue)
                    queryParams.Add($"year={dashboardRequest.Year.Value}");

                var queryString = string.Join("&", queryParams);
                var response = await _httpClient.PostAsJsonAsync($"Dashboard/GetStockInOutSummaryAsync", dashboardRequest).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<DashboardStockInOutSummaryDTO>>(jsonResponse);
                    return result ?? new ApiResult<DashboardStockInOutSummaryDTO>("Không có dữ liệu", false);
                }

                var errorResult = JsonConvert.DeserializeObject<ApiResult<DashboardStockInOutSummaryDTO>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định khi lấy thông tin Stock In/Out";

                return response.StatusCode switch
                {
                    System.Net.HttpStatusCode.Unauthorized => new ApiResult<DashboardStockInOutSummaryDTO>(errorMessage, false),
                    System.Net.HttpStatusCode.Forbidden => new ApiResult<DashboardStockInOutSummaryDTO>(errorMessage, false),
                    System.Net.HttpStatusCode.NotFound => new ApiResult<DashboardStockInOutSummaryDTO>(errorMessage, false),
                    System.Net.HttpStatusCode.InternalServerError => new ApiResult<DashboardStockInOutSummaryDTO>(errorMessage, false),
                    _ => new ApiResult<DashboardStockInOutSummaryDTO>(errorMessage, false)
                };
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
