using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.OrderTypeDTO;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.StatusMasterDTO;
using Chrome_WPF.Models.TransferDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.TransferService
{
    public class TransferService : ITransferService
    {
        private readonly HttpClient _httpClient;
        private readonly API_Constant _baseUrl;

        public TransferService(API_Constant baseUrl)
        {
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            _httpClient = _baseUrl.GetHttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AccessToken);
        }

        public async Task<ApiResult<PagedResponse<TransferResponseDTO>>> GetAllTransfers(string[] warehouseCodes, int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<TransferResponseDTO>>("Trang và kích thước trang phải lớn hơn 0", false);
            }
            try
            {
                var warehouseCodesQuery = string.Join("&warehouseCodes=", warehouseCodes.Select(Uri.EscapeDataString));
                var response = await _httpClient.GetAsync($"Transfer/GetAllTransfers?warehouseCodes={warehouseCodesQuery}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<TransferResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<TransferResponseDTO>>(result?.Message ?? "Không thể lấy danh sách phiếu chuyển kho", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<TransferResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách phiếu chuyển kho";
                return HandleErrorResponse<PagedResponse<TransferResponseDTO>>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<TransferResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<TransferResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<TransferResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<TransferResponseDTO>>> GetAllTransfersWithStatus(string[] warehouseCodes, int statusId, int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<TransferResponseDTO>>("Trang và kích thước trang phải lớn hơn 0", false);
            }
            if (statusId < 1)
            {
                return new ApiResult<PagedResponse<TransferResponseDTO>>("Mã trạng thái phải lớn hơn 0", false);
            }
            try
            {
                var warehouseCodesQuery = string.Join("&warehouseCodes=", warehouseCodes.Select(Uri.EscapeDataString));
                var response = await _httpClient.GetAsync($"Transfer/GetAllTransfersWithStatus?warehouseCodes={warehouseCodesQuery}&statusId={statusId}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<TransferResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<TransferResponseDTO>>(result?.Message ?? "Không thể lấy danh sách phiếu chuyển kho theo trạng thái", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<TransferResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách phiếu chuyển kho theo trạng thái";
                return HandleErrorResponse<PagedResponse<TransferResponseDTO>>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<TransferResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<TransferResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<TransferResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<TransferResponseDTO>>> SearchTransfersAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(textToSearch) || page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<TransferResponseDTO>>("Văn bản tìm kiếm, trang và kích thước trang phải hợp lệ", false);
            }
            try
            {
                var warehouseCodesQuery = string.Join("&warehouseCodes=", warehouseCodes.Select(Uri.EscapeDataString));
                var response = await _httpClient.GetAsync($"Transfer/SearchTransfers?warehouseCodes={warehouseCodesQuery}&textToSearch={Uri.EscapeDataString(textToSearch)}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<TransferResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<TransferResponseDTO>>(result?.Message ?? "Không thể tìm kiếm phiếu chuyển kho", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<TransferResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể tìm kiếm phiếu chuyển kho";
                return HandleErrorResponse<PagedResponse<TransferResponseDTO>>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<TransferResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<TransferResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<TransferResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<bool>> AddTransfer(TransferRequestDTO transfer)
        {
            if (transfer == null)
            {
                return new ApiResult<bool>("Dữ liệu phiếu chuyển kho không được để trống", false);
            }
            if (string.IsNullOrEmpty(transfer.TransferCode))
            {
                return new ApiResult<bool>("Mã phiếu chuyển kho không được để trống", false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(transfer);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("Transfer/AddTransfer", content).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể thêm phiếu chuyển kho", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể thêm phiếu chuyển kho";
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

        public async Task<ApiResult<bool>> DeleteTransferAsync(string transferCode)
        {
            if (string.IsNullOrEmpty(transferCode))
            {
                return new ApiResult<bool>("Mã phiếu chuyển kho không được để trống", false);
            }
            try
            {
                var response = await _httpClient.DeleteAsync($"Transfer/DeleteTransfer?transferCode={Uri.EscapeDataString(transferCode)}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể xóa phiếu chuyển kho", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể xóa phiếu chuyển kho";
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

        public async Task<ApiResult<bool>> UpdateTransfer(TransferRequestDTO transfer)
        {
            if (transfer == null)
            {
                return new ApiResult<bool>("Dữ liệu phiếu chuyển kho không được để trống", false);
            }
            if (string.IsNullOrEmpty(transfer.TransferCode))
            {
                return new ApiResult<bool>("Mã phiếu chuyển kho không được để trống", false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(transfer);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("Transfer/UpdateTransfer", content).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể cập nhật phiếu chuyển kho", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể cập nhật phiếu chuyển kho";
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

        public async Task<ApiResult<List<OrderTypeResponseDTO>>> GetListOrderType(string prefix)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Transfer/GetListOrderType?prefix={Uri.EscapeDataString(prefix ?? "")}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<OrderTypeResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<List<OrderTypeResponseDTO>>(result?.Message ?? "Không thể lấy danh sách loại phiếu", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<List<OrderTypeResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách loại phiếu";
                return HandleErrorResponse<List<OrderTypeResponseDTO>>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<List<OrderTypeResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<List<OrderTypeResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<List<OrderTypeResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<List<AccountManagementResponseDTO>>> GetListFromResponsibleAsync(string warehouseCode)
        {
            if (string.IsNullOrEmpty(warehouseCode))
            {
                return new ApiResult<List<AccountManagementResponseDTO>>("Mã kho không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"Transfer/GetListFromResponsible?warehouseCode={Uri.EscapeDataString(warehouseCode)}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<AccountManagementResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<List<AccountManagementResponseDTO>>(result?.Message ?? "Không thể lấy danh sách người phụ trách xuất kho", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<List<AccountManagementResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách người phụ trách xuất kho";
                return HandleErrorResponse<List<AccountManagementResponseDTO>>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<List<AccountManagementResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<List<AccountManagementResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<List<AccountManagementResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<List<AccountManagementResponseDTO>>> GetListToResponsibleAsync(string warehouseCode)
        {
            if (string.IsNullOrEmpty(warehouseCode))
            {
                return new ApiResult<List<AccountManagementResponseDTO>>("Mã kho không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"Transfer/GetListToResponsible?warehouseCode={Uri.EscapeDataString(warehouseCode)}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<AccountManagementResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<List<AccountManagementResponseDTO>>(result?.Message ?? "Không thể lấy danh sách người phụ trách nhận kho", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<List<AccountManagementResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách người phụ trách nhận kho";
                return HandleErrorResponse<List<AccountManagementResponseDTO>>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<List<AccountManagementResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<List<AccountManagementResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<List<AccountManagementResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<List<StatusMasterResponseDTO>>> GetListStatusMaster()
        {
            try
            {
                var response = await _httpClient.GetAsync("Transfer/GetListStatusMaster").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<StatusMasterResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<List<StatusMasterResponseDTO>>(result?.Message ?? "Không thể lấy danh sách trạng thái", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<List<StatusMasterResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách trạng thái";
                return HandleErrorResponse<List<StatusMasterResponseDTO>>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<List<StatusMasterResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<List<StatusMasterResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<List<StatusMasterResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission(string[] warehouseCodes)
        {
            try
            {
                var warehouseCodesQuery = string.Join("&warehouseCodes=", warehouseCodes.Select(Uri.EscapeDataString));
                var response = await _httpClient.GetAsync($"Transfer/GetListWarehousePermission?warehouseCodes={warehouseCodesQuery}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<WarehouseMasterResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<List<WarehouseMasterResponseDTO>>(result?.Message ?? "Không thể lấy danh sách kho được phép", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<List<WarehouseMasterResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách kho được phép";
                return HandleErrorResponse<List<WarehouseMasterResponseDTO>>(response.StatusCode, errorMessage);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<List<WarehouseMasterResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<List<WarehouseMasterResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<List<WarehouseMasterResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
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
