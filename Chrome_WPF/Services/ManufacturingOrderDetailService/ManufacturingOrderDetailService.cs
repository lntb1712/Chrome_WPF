using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.ManufacturingOrderDetailDTO;
using Chrome_WPF.Models.PagedResponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.ManufacturingOrderDetailService
{
    public class ManufacturingOrderDetailService : IManufacturingOrderDetailService
    {
        private readonly HttpClient _httpClient;
        private readonly API_Constant _baseUrl;

        public ManufacturingOrderDetailService(API_Constant baseUrl)
        {
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            _httpClient = _baseUrl.GetHttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AccessToken);
        }

        public async Task<ApiResult<ForecastManufacturingOrderDetailDTO>> GetForecastManufacturingOrderDetail(string manufacturingOrderCode, string productCode)
        {
            if (string.IsNullOrEmpty(manufacturingOrderCode) || string.IsNullOrEmpty(productCode))
            {
                return new ApiResult<ForecastManufacturingOrderDetailDTO>("Mã lệnh sản xuất và mã sản phẩm không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"ManufacturingOrderDetail/GetForecastManufacturingOrderDetail?manufacturingOrderCode={Uri.EscapeDataString(manufacturingOrderCode)}&productCode={Uri.EscapeDataString(productCode)}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<ForecastManufacturingOrderDetailDTO>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<ForecastManufacturingOrderDetailDTO>(result?.Message ?? "Không thể lấy chi tiết lệnh sản xuất", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<ForecastManufacturingOrderDetailDTO>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy chi tiết lệnh sản xuất";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<ForecastManufacturingOrderDetailDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<ForecastManufacturingOrderDetailDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<ForecastManufacturingOrderDetailDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<ForecastManufacturingOrderDetailDTO>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<ForecastManufacturingOrderDetailDTO>((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<ForecastManufacturingOrderDetailDTO>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<ForecastManufacturingOrderDetailDTO>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<ForecastManufacturingOrderDetailDTO>($"Lỗi không xác định: {ex.Message}", false);
            }
        }
        

        public async Task<ApiResult<PagedResponse<ManufacturingOrderDetailResponseDTO>>>GetManufacturingOrderDetail(string manufacturingOrderCode)
        {
            if (string.IsNullOrEmpty(manufacturingOrderCode))
            {
                return new ApiResult<PagedResponse<ManufacturingOrderDetailResponseDTO>>("Mã lệnh sản xuất không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"ManufacturingOrderDetail/GetManufacturingOrderDetails?manufacturingOrderCode={Uri.EscapeDataString(manufacturingOrderCode)}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<ManufacturingOrderDetailResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<ManufacturingOrderDetailResponseDTO>>(result?.Message ?? "Không thể lấy danh sách chi tiết lệnh sản xuất", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<ManufacturingOrderDetailResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách chi tiết lệnh sản xuất";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<ManufacturingOrderDetailResponseDTO>> ((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<ManufacturingOrderDetailResponseDTO>> ((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<ManufacturingOrderDetailResponseDTO>> ((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<ManufacturingOrderDetailResponseDTO>> ((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<PagedResponse<ManufacturingOrderDetailResponseDTO>> ((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<ManufacturingOrderDetailResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<ManufacturingOrderDetailResponseDTO>> ($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<ManufacturingOrderDetailResponseDTO>> ($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<ManufacturingOrderDetailResponseDTO>> GetManufacturingOrderDetail(string manufacturingOrderCode, string productCode)
        {
            if (string.IsNullOrEmpty(manufacturingOrderCode) || string.IsNullOrEmpty(productCode))
            {
                return new ApiResult<ManufacturingOrderDetailResponseDTO>("Mã lệnh sản xuất và mã sản phẩm không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"ManufacturingOrderDetail/GetManufacturingOrderDetailByCode?manufacturingOrderCode={Uri.EscapeDataString(manufacturingOrderCode)}&productCode={Uri.EscapeDataString(productCode)}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<ManufacturingOrderDetailResponseDTO>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<ManufacturingOrderDetailResponseDTO>(result?.Message ?? "Không thể lấy chi tiết lệnh sản xuất", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<ManufacturingOrderDetailResponseDTO>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy chi tiết lệnh sản xuất";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<ManufacturingOrderDetailResponseDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<ManufacturingOrderDetailResponseDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<ManufacturingOrderDetailResponseDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<ManufacturingOrderDetailResponseDTO>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<ManufacturingOrderDetailResponseDTO>((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<ManufacturingOrderDetailResponseDTO>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<ManufacturingOrderDetailResponseDTO>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<ManufacturingOrderDetailResponseDTO>($"Lỗi không xác định: {ex.Message}", false);
            }
        }
    }
}
