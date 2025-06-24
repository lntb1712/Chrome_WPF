using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.StockTakeDetailDTO;
using Chrome_WPF.Models.StocktakeDTO;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Chrome_WPF.Services.StockTakeDetailService
{
    public class StockTakeDetailService : IStockTakeDetailService
    {
        private readonly HttpClient _httpClient;
        private readonly API_Constant _baseUrl;

        public StockTakeDetailService(API_Constant baseUrl)
        {
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            _httpClient = _baseUrl.GetHttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AccessToken);
        }

        public async Task<ApiResult<PagedResponse<StockTakeDetailResponseDTO>>> GetAllStockTakeDetailsAsync(string[] warehouseCodes, int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>("Trang và kích thước trang phải lớn hơn 0", false);
            }
            try
            {
                var warehouseCodesQuery = string.Join("&warehouseCodes=", warehouseCodes.Select(Uri.EscapeDataString));
                var response = await _httpClient.GetAsync($"StockTake/{Uri.EscapeDataString("details")}/StockTakeDetail/GetAllStockTakeDetails?warehouseCodes={warehouseCodesQuery}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<StockTakeDetailResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>(result?.Message ?? "Không thể lấy danh sách chi tiết kiểm kê", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<StockTakeDetailResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách chi tiết kiểm kê";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>(errorMessage, false);
                }
                else
                {
                    return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>(errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<StockTakeDetailResponseDTO>>> GetStockTakeDetailsByStockTakeCodeAsync(string stockTakeCode, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(stockTakeCode))
            {
                return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>("Mã lệnh kiểm kê không được để trống", false);
            }
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>("Trang và kích thước trang phải lớn hơn 0", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"StockTake/{Uri.EscapeDataString(stockTakeCode)}/StockTakeDetail/GetStockTakeDetailsByStockTakeCode?page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<StockTakeDetailResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>(result?.Message ?? "Không thể lấy chi tiết kiểm kê theo mã", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<StockTakeDetailResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy chi tiết kiểm kê theo mã";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>(errorMessage, false);
                }
                else
                {
                    return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>(errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<StockTakeDetailResponseDTO>>> SearchStockTakeDetailsAsync(string[] warehouseCodes, string stockTakeCode, string textToSearch, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(stockTakeCode) || string.IsNullOrEmpty(textToSearch) || page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>("Mã lệnh kiểm kê, văn bản tìm kiếm, trang và kích thước trang phải hợp lệ", false);
            }
            try
            {
                var warehouseCodesQuery = string.Join("&warehouseCodes=", warehouseCodes.Select(Uri.EscapeDataString));
                var response = await _httpClient.GetAsync($"StockTake/{Uri.EscapeDataString(stockTakeCode)}/StockTakeDetail/SearchStockTakeDetails?warehouseCodes={warehouseCodesQuery}&textToSearch={Uri.EscapeDataString(textToSearch)}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<StockTakeDetailResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>(result?.Message ?? "Không thể tìm kiếm chi tiết kiểm kê", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<StockTakeDetailResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể tìm kiếm chi tiết kiểm kê";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>(errorMessage, false);
                }
                else
                {
                    return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>(errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<StockTakeDetailResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<bool>> UpdateStockTakeDetail(StockTakeDetailRequestDTO stockTakeDetail)
        {
            if (stockTakeDetail == null)
            {
                return new ApiResult<bool>("Dữ liệu chi tiết kiểm kê không được để trống", false);
            }
            if (string.IsNullOrEmpty(stockTakeDetail.StockTakeCode))
            {
                return new ApiResult<bool>("Mã lệnh kiểm kê không được để trống", false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(stockTakeDetail);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"StockTake/{Uri.EscapeDataString(stockTakeDetail.StockTakeCode)}/StockTakeDetail/UpdateStockTakeDetail", content).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể cập nhật chi tiết kiểm kê", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể cập nhật chi tiết kiểm kê";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<bool>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<bool>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<bool>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<bool>(errorMessage, false);
                }
                else
                {
                    return new ApiResult<bool>(errorMessage, false);
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

        public async Task<ApiResult<bool>> DeleteStockTakeDetail(string stockTakeCode, string productCode, string lotNo, string locationCode)
        {
            if (string.IsNullOrEmpty(stockTakeCode) || string.IsNullOrEmpty(productCode) || string.IsNullOrEmpty(lotNo) || string.IsNullOrEmpty(locationCode))
            {
                return new ApiResult<bool>("Mã lệnh kiểm kê, mã sản phẩm, số lô và mã vị trí không được để trống", false);
            }
            try
            {
                var query = $"productCode={Uri.EscapeDataString(productCode)}&lotNo={Uri.EscapeDataString(lotNo)}&locationCode={Uri.EscapeDataString(locationCode)}";
                var response = await _httpClient.DeleteAsync($"StockTake/{Uri.EscapeDataString(stockTakeCode)}/StockTakeDetail/DeleteStockTakeDetail?{query}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể xóa chi tiết kiểm kê", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể xóa chi tiết kiểm kê";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<bool>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<bool>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<bool>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<bool>(errorMessage, false);
                }
                else
                {
                    return new ApiResult<bool>(errorMessage, false);
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