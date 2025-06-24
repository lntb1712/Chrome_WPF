using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.LocationMasterDTO;
using Chrome_WPF.Models.MovementDTO;
using Chrome_WPF.Models.OrderTypeDTO;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.PickListDTO;
using Chrome_WPF.Models.PutAwayDTO;
using Chrome_WPF.Models.StatusMasterDTO;
using Chrome_WPF.Models.WarehouseMasterDTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.MovementService
{

    public class MovementService : IMovementService
    {
        private readonly HttpClient _httpClient;
        private readonly API_Constant _baseUrl;

        public MovementService(API_Constant baseUrl)
        {
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            _httpClient = _baseUrl.GetHttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AccessToken);
        }

        public async Task<ApiResult<PagedResponse<MovementResponseDTO>>> GetAllMovements(string[] warehouseCodes, int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<MovementResponseDTO>>("Trang và kích thước trang phải lớn hơn 0", false);
            }
            try
            {
                var warehouseCodesQuery = string.Join("&warehouseCodes=", warehouseCodes.Select(Uri.EscapeDataString));
                var response = await _httpClient.GetAsync($"Movement/GetAllMovements?warehouseCodes={warehouseCodesQuery}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<MovementResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<MovementResponseDTO>>(result?.Message ?? "Không thể lấy danh sách lệnh di chuyển", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<MovementResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách lệnh di chuyển";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<MovementResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<MovementResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<MovementResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<MovementResponseDTO>>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<PagedResponse<MovementResponseDTO>>((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<MovementResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<MovementResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<MovementResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<MovementResponseDTO>>> GetAllMovementsWithStatus(string[] warehouseCodes, int statusId, int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<MovementResponseDTO>>("Trang và kích thước trang phải lớn hơn 0", false);
            }
            if (statusId < 1)
            {
                return new ApiResult<PagedResponse<MovementResponseDTO>>("Mã trạng thái phải lớn hơn 0", false);
            }
            try
            {
                var warehouseCodesQuery = string.Join("&warehouseCodes=", warehouseCodes.Select(Uri.EscapeDataString));
                var response = await _httpClient.GetAsync($"Movement/GetAllMovementsWithStatus?warehouseCodes={warehouseCodesQuery}&statusId={statusId}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<MovementResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<MovementResponseDTO>>(result?.Message ?? "Không thể lấy danh sách lệnh di chuyển theo trạng thái", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<MovementResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách lệnh di chuyển theo trạng thái";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<MovementResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<MovementResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<MovementResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<MovementResponseDTO>>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<PagedResponse<MovementResponseDTO>>((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<MovementResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<MovementResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<MovementResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<MovementResponseDTO>>> SearchMovementAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(textToSearch) || page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<MovementResponseDTO>>("Văn bản tìm kiếm, trang và kích thước trang phải hợp lệ", false);
            }
            try
            {
                var warehouseCodesQuery = string.Join("&warehouseCodes=", warehouseCodes.Select(Uri.EscapeDataString));
                var response = await _httpClient.GetAsync($"Movement/SearchMovements?warehouseCodes={warehouseCodesQuery}&textToSearch={Uri.EscapeDataString(textToSearch)}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<MovementResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<MovementResponseDTO>>(result?.Message ?? "Không thể tìm kiếm lệnh di chuyển", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<MovementResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể tìm kiếm lệnh di chuyển";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<MovementResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<MovementResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<MovementResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<MovementResponseDTO>>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<PagedResponse<MovementResponseDTO>>((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<MovementResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<MovementResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<MovementResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<bool>> AddMovement(MovementRequestDTO movement)
        {
            if (movement == null)
            {
                return new ApiResult<bool>("Dữ liệu lệnh di chuyển không được để trống", false);
            }
            if (string.IsNullOrEmpty(movement.MovementCode))
            {
                return new ApiResult<bool>("Mã lệnh di chuyển không được để trống", false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(movement);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("Movement/AddMovement", content).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể thêm lệnh di chuyển", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể thêm lệnh di chuyển";
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
                    return new ApiResult<bool>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<bool>((string)errorMessage, false);
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

        public async Task<ApiResult<bool>> DeleteMovementAsync(string movementCode)
        {
            if (string.IsNullOrEmpty(movementCode))
            {
                return new ApiResult<bool>("Mã lệnh di chuyển không được để trống", false);
            }
            try
            {
                var response = await _httpClient.DeleteAsync($"Movement/DeleteMovement?movementCode={Uri.EscapeDataString(movementCode)}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể xóa lệnh di chuyển", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể xóa lệnh di chuyển";
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
                    return new ApiResult<bool>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<bool>((string)errorMessage, false);
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

        public async Task<ApiResult<bool>> UpdateMovement(MovementRequestDTO movement)
        {
            if (movement == null)
            {
                return new ApiResult<bool>("Dữ liệu lệnh di chuyển không được để trống", false);
            }
            if (string.IsNullOrEmpty(movement.MovementCode))
            {
                return new ApiResult<bool>("Mã lệnh di chuyển không được để trống", false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(movement);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("Movement/UpdateMovement", content).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể cập nhật lệnh di chuyển", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể cập nhật lệnh di chuyển";
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
                    return new ApiResult<bool>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<bool>((string)errorMessage, false);
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

        public async Task<ApiResult<List<OrderTypeResponseDTO>>> GetListOrderType(string prefix)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Movement/GetListOrderType?prefix={Uri.EscapeDataString(prefix ?? "")}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<OrderTypeResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<List<OrderTypeResponseDTO>>(result?.Message ?? "Không thể lấy danh sách loại lệnh", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<List<OrderTypeResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách loại lệnh";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<List<OrderTypeResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<List<OrderTypeResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<List<OrderTypeResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<List<OrderTypeResponseDTO>>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<List<OrderTypeResponseDTO>>((string)errorMessage, false);
                }
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

        public async Task<ApiResult<List<AccountManagementResponseDTO>>> GetListResponsibleAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Movement/GetListResponsible").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<AccountManagementResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<List<AccountManagementResponseDTO>>(result?.Message ?? "Không thể lấy danh sách người phụ trách", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<List<AccountManagementResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách người phụ trách";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<List<AccountManagementResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<List<AccountManagementResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<List<AccountManagementResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<List<AccountManagementResponseDTO>>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<List<AccountManagementResponseDTO>>((string)errorMessage, false);
                }
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
                var response = await _httpClient.GetAsync("Movement/GetListStatusMaster").ConfigureAwait(false);
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
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<List<StatusMasterResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<List<StatusMasterResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<List<StatusMasterResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<List<StatusMasterResponseDTO>>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<List<StatusMasterResponseDTO>>((string)errorMessage, false);
                }
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
                var response = await _httpClient.GetAsync($"Movement/GetListWarehousePermission?warehouseCodes={warehouseCodesQuery}").ConfigureAwait(false);
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
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<List<WarehouseMasterResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<List<WarehouseMasterResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<List<WarehouseMasterResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<List<WarehouseMasterResponseDTO>>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<List<WarehouseMasterResponseDTO>>((string)errorMessage, false);
                }
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

        public async Task<ApiResult<List<LocationMasterResponseDTO>>> GetListFromLocation(string warehouseCode)
        {
            if (string.IsNullOrEmpty(warehouseCode))
            {
                return new ApiResult<List<LocationMasterResponseDTO>>("Mã kho không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"Movement/GetListFromLocation?warehouseCode={Uri.EscapeDataString(warehouseCode)}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<LocationMasterResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<List<LocationMasterResponseDTO>>(result?.Message ?? "Không thể lấy danh sách vị trí nguồn", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<List<LocationMasterResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách vị trí nguồn";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<List<LocationMasterResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<List<LocationMasterResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<List<LocationMasterResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<List<LocationMasterResponseDTO>>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<List<LocationMasterResponseDTO>>((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<List<LocationMasterResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<List<LocationMasterResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<List<LocationMasterResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<List<LocationMasterResponseDTO>>> GetListToLocation(string warehouseCode)
        {
            if (string.IsNullOrEmpty(warehouseCode))
            {
                return new ApiResult<List<LocationMasterResponseDTO>>("Mã kho không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"Movement/GetListToLocation?warehouseCode={Uri.EscapeDataString(warehouseCode)}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<LocationMasterResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<List<LocationMasterResponseDTO>>(result?.Message ?? "Không thể lấy danh sách vị trí đích", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<List<LocationMasterResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách vị trí đích";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<List<LocationMasterResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<List<LocationMasterResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<List<LocationMasterResponseDTO>>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<List<LocationMasterResponseDTO>>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<List<LocationMasterResponseDTO>>((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<List<LocationMasterResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<List<LocationMasterResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<List<LocationMasterResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PutAwayResponseDTO>> GetPutAwayContainsMovement(string movementCode)
        {
            if (string.IsNullOrEmpty(movementCode))
            {
                return new ApiResult<PutAwayResponseDTO>("Mã lệnh di chuyển không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"Movement/GetPutAwayContainsMovement?movementCode={Uri.EscapeDataString(movementCode)}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PutAwayResponseDTO>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PutAwayResponseDTO>(result?.Message ?? "Không thể lấy thông tin lệnh để hàng liên quan", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PutAwayResponseDTO>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy thông tin lệnh để hàng liên quan";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PutAwayResponseDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PutAwayResponseDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PutAwayResponseDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PutAwayResponseDTO>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<PutAwayResponseDTO>((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PutAwayResponseDTO>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PutAwayResponseDTO>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PutAwayResponseDTO>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PickListResponseDTO>> GetPickListContainsMovement(string movementCode)
        {
            if (string.IsNullOrEmpty(movementCode))
            {
                return new ApiResult<PickListResponseDTO>("Mã lệnh di chuyển không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"Movement/GetPickListContainsMovement?movementCode={Uri.EscapeDataString(movementCode)}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PickListResponseDTO>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PickListResponseDTO>(result?.Message ?? "Không thể lấy thông tin phiếu lấy hàng liên quan", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PickListResponseDTO>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy thông tin phiếu lấy hàng liên quan";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PickListResponseDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PickListResponseDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PickListResponseDTO>((string)errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PickListResponseDTO>((string)errorMessage, false);
                }
                else
                {
                    return new ApiResult<PickListResponseDTO>((string)errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<PickListResponseDTO>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PickListResponseDTO>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PickListResponseDTO>($"Lỗi không xác định: {ex.Message}", false);
            }
        }
    }
}
