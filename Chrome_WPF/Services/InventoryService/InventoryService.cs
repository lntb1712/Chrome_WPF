using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.CategoryDTO;
using Chrome_WPF.Models.InventoryDTO;
using Chrome_WPF.Models.PagedResponse;
using Newtonsoft.Json;
using System.Net.Http;

namespace Chrome_WPF.Services.InventoryService
{
    public class InventoryService : IInventoryService
    {
        private readonly API_Constant _baseUrl;
        private readonly HttpClient _httpClient;
        private List<string> warehousePermissions = new List<string>();
        private void UpdateWarehousePermissions()
        {
            var savedPermissions = Properties.Settings.Default.WarehousePermission;
            if (savedPermissions != null)
            {
                warehousePermissions = savedPermissions.Cast<string>().ToList();
            }
        }
        public InventoryService(API_Constant baseUrl)
        {
            _baseUrl = baseUrl;
            _httpClient = _baseUrl.GetHttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AccessToken);

        }

        public async Task<ApiResult<List<CategoryResponseDTO>>> GetAllCategories()
        {
            try
            {
                var response = await _httpClient.GetAsync("Inventory/GetAllCategories").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<CategoryResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
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

        public async Task<ApiResult<PagedResponse<InventorySummaryDTO>>> GetListProductInventory(int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<InventorySummaryDTO>>("Dữ liệu nhận vào không hợp lệ", false);
            }
            try
            {
                UpdateWarehousePermissions();
                var queryWarehousePermission = string.Join("&", warehousePermissions.Select(id => $"warehouseCodes={Uri.EscapeDataString(id)}"));
                var response = await _httpClient.GetAsync($"Inventory/GetListProductInventory?{queryWarehousePermission}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<InventorySummaryDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<InventorySummaryDTO>>(result?.Message ?? "Không thể phân tích phản hồi", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<InventorySummaryDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách sản phẩm theo loại";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<InventorySummaryDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<InventorySummaryDTO>>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<InventorySummaryDTO>>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<InventorySummaryDTO>>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<PagedResponse<InventorySummaryDTO>>(errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<PagedResponse<InventorySummaryDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<PagedResponse<InventorySummaryDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<PagedResponse<InventorySummaryDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<InventorySummaryDTO>>> GetListProductInventoryByCategoryIds(string[] categoryIds, int page, int pageSize)
        {
            if (categoryIds == null || categoryIds.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<InventorySummaryDTO>>("Dữ liệu nhận vào không hợp lệ", false);
            }
            try
            {
                UpdateWarehousePermissions();
                var queryWarehousePermission = string.Join("&", warehousePermissions.Select(id => $"warehouseCodes={Uri.EscapeDataString(id)}"));
                var queryCategoryIds = string.Join("&", categoryIds.Select(id => $"categoryIds={Uri.EscapeDataString(id)}"));
                var query = $"{queryWarehousePermission}&{queryCategoryIds}&page={page}&pageSize={pageSize}";
                var response = await _httpClient.GetAsync($"Inventory/GetListProductInventoryByCategoryIds?{query}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<InventorySummaryDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<InventorySummaryDTO>>(result?.Message ?? "Không thể phân tích phản hồi", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<InventorySummaryDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách sản phẩm theo danh mục";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<InventorySummaryDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<InventorySummaryDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<InventorySummaryDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<InventorySummaryDTO>>(errorMessage, false);
                }
                else
                {
                    return new ApiResult<PagedResponse<InventorySummaryDTO>>(errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<InventorySummaryDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<InventorySummaryDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<InventorySummaryDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<ProductWithLocationsDTO>>> GetProductWithLocations(string productCode, int page, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(productCode) || page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<ProductWithLocationsDTO>>("Dữ liệu nhận vào không hợp lệ", false);
            }
            try
            {
                UpdateWarehousePermissions();
                var queryWarehousePermission = string.Join("&", warehousePermissions.Select(id => $"warehouseCodes={Uri.EscapeDataString(id)}"));
                var query = $"{queryWarehousePermission}&page={page}&pageSize={pageSize}";
                var response = await _httpClient.GetAsync($"Inventory/GetProductWithLocations?productCode={Uri.EscapeDataString(productCode)}&{query}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<ProductWithLocationsDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<ProductWithLocationsDTO>>(result?.Message ?? "Không thể phân tích phản hồi", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<ProductWithLocationsDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy thông tin sản phẩm với vị trí";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<ProductWithLocationsDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<ProductWithLocationsDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<ProductWithLocationsDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<ProductWithLocationsDTO>>(errorMessage, false);
                }
                else
                {
                    return new ApiResult<PagedResponse<ProductWithLocationsDTO>>(errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<ProductWithLocationsDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<ProductWithLocationsDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<ProductWithLocationsDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<InventorySummaryDTO>>> SearchProductInventory(string textToSearch, int page, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(textToSearch) || page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<InventorySummaryDTO>>("Dữ liệu nhận vào không hợp lệ", false);
            }
            try
            {
                UpdateWarehousePermissions();
                var queryWarehousePermission = string.Join("&", warehousePermissions.Select(id => $"warehouseCodes={Uri.EscapeDataString(id)}"));
                var query = $"{queryWarehousePermission}&textToSearch={Uri.EscapeDataString(textToSearch)}&page={page}&pageSize={pageSize}";
                var response = await _httpClient.GetAsync($"Inventory/SearchProductInventory?{query}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<InventorySummaryDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<InventorySummaryDTO>>(result?.Message ?? "Không thể phân tích phản hồi", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<InventorySummaryDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể tìm kiếm sản phẩm trong kho";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<InventorySummaryDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<InventorySummaryDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<InventorySummaryDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<InventorySummaryDTO>>(errorMessage, false);
                }
                else
                {
                    return new ApiResult<PagedResponse<InventorySummaryDTO>>(errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<InventorySummaryDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<InventorySummaryDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<InventorySummaryDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<List<WarehouseUsageDTO>>> GetInventoryUsedPercent()
        {
            try
            {
                UpdateWarehousePermissions();
                var queryWarehousePermission = string.Join("&", warehousePermissions.Select(id => $"warehouseCodes={Uri.EscapeDataString(id)}"));
                var response = await _httpClient.GetAsync($"Inventory/GetInventoryUsedPercent?{queryWarehousePermission}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<WarehouseUsageDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<List<WarehouseUsageDTO>>(result?.Message ?? "Không thể phân tích phản hồi", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<List<LocationUsageDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy thông tin phần trăm sử dụng kho";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<List<WarehouseUsageDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<List<WarehouseUsageDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<List<WarehouseUsageDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<List<WarehouseUsageDTO>>(errorMessage, false);
                }
                else
                {
                    return new ApiResult<List<WarehouseUsageDTO>>(errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<List<WarehouseUsageDTO>>($"Lỗi mạng: {ex.Message}", false);
            }

            catch (JsonException ex)
            {
                return new ApiResult<List<WarehouseUsageDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<List<WarehouseUsageDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<double>> GetTotalPriceOfWarehouse()
        {
            try
            {
                UpdateWarehousePermissions();
                var queryWarehousePermission = string.Join("&", warehousePermissions.Select(id => $"warehouseCodes={Uri.EscapeDataString(id)}"));
                var response = await _httpClient.GetAsync($"Inventory/GetTotalPriceOfWarehouse?{queryWarehousePermission}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<double>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<double>(result?.Message ?? "Không thể phân tích phản hồi", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject <ApiResult<double>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy thông tin phần trăm sử dụng kho";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<double>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<double>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<double>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<double>(errorMessage, false);
                }
                else
                {
                    return new ApiResult<double>(errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<double>($"Lỗi mạng: {ex.Message}", false);
            }

            catch (JsonException ex)
            {
                return new ApiResult<double>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<double>($"Lỗi không xác định: {ex.Message}", false);
            }
        }
    }

}