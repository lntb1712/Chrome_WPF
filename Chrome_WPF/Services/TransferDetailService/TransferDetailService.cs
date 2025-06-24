using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.InventoryDTO;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.TransferDetailDTO;
using Chrome_WPF.Models.TransferDTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.TransferDetailService
{
    public class TransferDetailService : ITransferDetailService
    {
        private readonly HttpClient _httpClient;
        private readonly API_Constant _baseUrl;

        public TransferDetailService(API_Constant baseUrl)
        {
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            _httpClient = _baseUrl.GetHttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AccessToken);
        }

        public async Task<ApiResult<PagedResponse<TransferDetailResponseDTO>>> GetAllTransferDetailsAsync(string[] warehouseCodes, int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<TransferDetailResponseDTO>>("Trang và kích thước trang phải lớn hơn 0", false);
            }
            try
            {
                var warehouseCodesQuery = string.Join("&warehouseCodes=", warehouseCodes.Select(Uri.EscapeDataString));
                var response = await _httpClient.GetAsync($"TransferDetail/GetAllTransferDetails?warehouseCodes={warehouseCodesQuery}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<TransferDetailResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<TransferDetailResponseDTO>>(result?.Message ?? "Không thể lấy danh sách chi tiết phiếu chuyển kho", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<TransferDetailResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách chi tiết phiếu chuyển kho";
                return HandleErrorResponse<PagedResponse<TransferDetailResponseDTO>>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<TransferDetailResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<TransferDetailResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<TransferDetailResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<TransferDetailResponseDTO>>> GetTransferDetailsByTransferCodeAsync(string transferCode, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(transferCode) || page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<TransferDetailResponseDTO>>("Mã phiếu chuyển kho, trang và kích thước trang phải hợp lệ", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"TransferDetail/GetTransferDetailsByTransferCode?transferCode={Uri.EscapeDataString(transferCode)}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<TransferDetailResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<TransferDetailResponseDTO>>(result?.Message ?? "Không thể lấy chi tiết phiếu chuyển kho theo mã", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<TransferDetailResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy chi tiết phiếu chuyển kho theo mã";
                return HandleErrorResponse<PagedResponse<TransferDetailResponseDTO>>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<TransferDetailResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<TransferDetailResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<TransferDetailResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<TransferDetailResponseDTO>>> SearchTransferDetailsAsync(string[] warehouseCodes, string transferCode, string textToSearch, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(transferCode) || string.IsNullOrEmpty(textToSearch) || page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<TransferDetailResponseDTO>>("Mã phiếu chuyển kho, văn bản tìm kiếm, trang và kích thước trang phải hợp lệ", false);
            }
            try
            {
                var warehouseCodesQuery = string.Join("&warehouseCodes=", warehouseCodes.Select(Uri.EscapeDataString));
                var response = await _httpClient.GetAsync($"TransferDetail/SearchTransferDetails?warehouseCodes={warehouseCodesQuery}&transferCode={Uri.EscapeDataString(transferCode)}&textToSearch={Uri.EscapeDataString(textToSearch)}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<TransferDetailResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<TransferDetailResponseDTO>>(result?.Message ?? "Không thể tìm kiếm chi tiết phiếu chuyển kho", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<TransferDetailResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể tìm kiếm chi tiết phiếu chuyển kho";
                return HandleErrorResponse<PagedResponse<TransferDetailResponseDTO>>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<TransferDetailResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<TransferDetailResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<TransferDetailResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<bool>> AddTransferDetail(TransferDetailRequestDTO transferDetail)
        {
            if (transferDetail == null)
            {
                return new ApiResult<bool>("Dữ liệu chi tiết phiếu chuyển kho không được để trống", false);
            }
            if (string.IsNullOrEmpty(transferDetail.TransferCode) || string.IsNullOrEmpty(transferDetail.ProductCode))
            {
                return new ApiResult<bool>("Mã phiếu chuyển kho và mã sản phẩm không được để trống", false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(transferDetail);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("TransferDetail/AddTransferDetail", content).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể thêm chi tiết phiếu chuyển kho", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể thêm chi tiết phiếu chuyển kho";
                return HandleErrorResponse<bool>(response.StatusCode, errorMessage);
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

        public async Task<ApiResult<bool>> UpdateTransferDetail(TransferDetailRequestDTO transferDetail)
        {
            if (transferDetail == null)
            {
                return new ApiResult<bool>("Dữ liệu chi tiết phiếu chuyển kho không được để trống", false);
            }
            if (string.IsNullOrEmpty(transferDetail.TransferCode) || string.IsNullOrEmpty(transferDetail.ProductCode))
            {
                return new ApiResult<bool>("Mã phiếu chuyển kho và mã sản phẩm không được để trống", false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(transferDetail);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("TransferDetail/UpdateTransferDetail", content).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể cập nhật chi tiết phiếu chuyển kho", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể cập nhật chi tiết phiếu chuyển kho";
                return HandleErrorResponse<bool>(response.StatusCode, errorMessage);
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

        public async Task<ApiResult<bool>> DeleteTransferDetail(string transferCode, string productCode)
        {
            if (string.IsNullOrEmpty(transferCode) || string.IsNullOrEmpty(productCode))
            {
                return new ApiResult<bool>("Mã phiếu chuyển kho và mã sản phẩm không được để trống", false);
            }
            try
            {
                var response = await _httpClient.DeleteAsync($"TransferDetail/DeleteTransferDetail?transferCode={Uri.EscapeDataString(transferCode)}&productCode={Uri.EscapeDataString(productCode)}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể xóa chi tiết phiếu chuyển kho", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể xóa chi tiết phiếu chuyển kho";
                return HandleErrorResponse<bool>(response.StatusCode, errorMessage);
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

        public async Task<ApiResult<List<InventorySummaryDTO>>> GetProductByWarehouseCode(string warehouseCode)
        {
            if (string.IsNullOrEmpty(warehouseCode))
            {
                return new ApiResult<List<InventorySummaryDTO>>("Mã kho không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"TransferDetail/GetProductByWarehouseCode?warehouseCode={Uri.EscapeDataString(warehouseCode)}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<InventorySummaryDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<List<InventorySummaryDTO>>(result?.Message ?? "Không thể lấy danh sách sản phẩm theo mã kho", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<List<InventorySummaryDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách sản phẩm theo mã kho";
                return HandleErrorResponse<List<InventorySummaryDTO>>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<List<InventorySummaryDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<List<InventorySummaryDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<List<InventorySummaryDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        private ApiResult<T> HandleErrorResponse<T>(System.Net.HttpStatusCode statusCode, string errorMessage)
        {
            if (statusCode == System.Net.HttpStatusCode.Unauthorized ||
                statusCode == System.Net.HttpStatusCode.Forbidden ||
                statusCode == System.Net.HttpStatusCode.NotFound ||
                statusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return new ApiResult<T>(errorMessage, false);
            }
            return new ApiResult<T>(errorMessage, false);
        }
    }
}
