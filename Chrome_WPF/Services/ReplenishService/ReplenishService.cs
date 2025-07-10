using Chrome.DTO.ReplenishDTO;
using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Models.ReplenishDTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.ReplenishService
{
    public class ReplenishService: IReplenishService
    {
        private readonly HttpClient _httpClient;
        private readonly API_Constant _baseUrl;

        public ReplenishService(API_Constant baseUrl)
        {
            _baseUrl = baseUrl;
            _httpClient= _baseUrl.GetHttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AccessToken);
        }

        public async Task<ApiResult<bool>> AddReplenishAsync(ReplenishRequestDTO replenishRequest)
        {
            if (replenishRequest == null)
            {
                return new ApiResult<bool>("Dữ liệu nhận vào không hợp lệ", false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(replenishRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"Replenish/AddReplenishAsync", content).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    return result ?? new ApiResult<bool>("Không thể chuyển đổi dữ liệu trả về", false);
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định từ máy chủ";
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

        public async Task<ApiResult<List<string>>> CheckReplenishWarningsAsync(string warehouseCode)
        {
            if (string.IsNullOrEmpty(warehouseCode))
            {
                return new ApiResult<List<string>>("Mã kho không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"Replenish/CheckReplenishWarningsAsync?warehouseCode={warehouseCode}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<string>>>(jsonResponse);
                    return result ?? new ApiResult<List<string>>("Không thể chuyển đổi dữ liệu trả về", false);
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<List<string>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định từ máy chủ";
                if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<List<string>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<List<string>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<List<string>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<List<string>>(errorMessage, false);
                }
                else
                {
                    return new ApiResult<List<string>>(errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<List<string>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<List<string>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<List<string>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<bool>> DeleteReplenishAsync(string warehouseCode, string productCode)
        {
            if (string.IsNullOrEmpty(warehouseCode) || string.IsNullOrEmpty(productCode))
            {
                return new ApiResult<bool>("Mã kho và mã sản phẩm không được để trống", false);
            }
            try
            {
                var response = await _httpClient.DeleteAsync($"Replenish/DeleteReplenishAsync?warehouseCode={warehouseCode}&productCode={productCode}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    return result ?? new ApiResult<bool>("Không thể chuyển đổi dữ liệu trả về", false);
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định từ máy chủ";
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

        public async Task<ApiResult<List<ProductMasterResponseDTO>>> GetListProductForReplenish()
        {
            try
            {
                var response = await _httpClient.GetAsync($"Replenish/GetListProductToReplenish").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<ProductMasterResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<List<ProductMasterResponseDTO>>(result?.Message ?? "Không thể phân tích phản hồi", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<List<ProductMasterResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Không thể lấy danh sách sản phẩm xuất kho";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<List<ProductMasterResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<List<ProductMasterResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<List<ProductMasterResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<List<ProductMasterResponseDTO>>(errorMessage, false);
                }
                else
                {
                    return new ApiResult<List<ProductMasterResponseDTO>>(errorMessage, false);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<List<ProductMasterResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<List<ProductMasterResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<List<ProductMasterResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<ReplenishResponseDTO>>> GetReplenishListAsync(string warehouseCode, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(warehouseCode) || page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<ReplenishResponseDTO>>("Mã kho không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"Replenish/GetAllReplenishAsync?warehouseCode={warehouseCode}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<ReplenishResponseDTO>>>(jsonResponse);
                    return result ?? new ApiResult<PagedResponse<ReplenishResponseDTO>>("Không thể chuyển đổi dữ liệu trả về", false);
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<ReplenishResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định từ máy chủ";
                if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<ReplenishResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<ReplenishResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<ReplenishResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<ReplenishResponseDTO>>(errorMessage, false);
                }
                else
                {
                    return new ApiResult<PagedResponse<ReplenishResponseDTO>>(errorMessage, false);
                }
            }catch( HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<ReplenishResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<ReplenishResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<ReplenishResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }

        }

        public async Task<ApiResult<int>> GetTotalReplenishCountAsync(string warehouseCode)
        {
            if(string.IsNullOrEmpty(warehouseCode))
            {
                return new ApiResult<int>("Mã kho không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"Replenish/GetTotalReplenishCountAsync?warehouseCode={warehouseCode}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<int>>(jsonResponse);
                    return result ?? new ApiResult<int>("Không thể chuyển đổi dữ liệu trả về", false);
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<int>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định từ máy chủ";
                if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<int>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<int>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<int>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<int>(errorMessage, false);
                }
                else
                {
                    return new ApiResult<int>(errorMessage, false);
                }
            }catch(HttpRequestException ex)
            {
                return new ApiResult<int>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<int>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<int>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<ReplenishResponseDTO>>> SearchReplenishAsync(string warehouseCode, string textToSearch, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(warehouseCode) || string.IsNullOrEmpty(textToSearch) || page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<ReplenishResponseDTO>>("Mã kho, từ khóa tìm kiếm, trang và kích thước trang không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"Replenish/SearchReplenishAsync?warehouseCode={warehouseCode}&textToSearch={textToSearch}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<ReplenishResponseDTO>>>(jsonResponse);
                    return result ?? new ApiResult<PagedResponse<ReplenishResponseDTO>>("Không thể chuyển đổi dữ liệu trả về", false);
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<ReplenishResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định từ máy chủ";
                if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<ReplenishResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<ReplenishResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<ReplenishResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<ReplenishResponseDTO>>(errorMessage, false);
                }
                else
                {
                    return new ApiResult<PagedResponse<ReplenishResponseDTO>>(errorMessage, false);
                }
            }catch (HttpRequestException ex)
            {
                return new ApiResult<PagedResponse<ReplenishResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<PagedResponse<ReplenishResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<PagedResponse<ReplenishResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<bool>> UpdateReplenishAsync(ReplenishRequestDTO replenishRequest)
        {
            if (replenishRequest == null)
            {
                return new ApiResult<bool>("Dữ liệu nhận vào không hợp lệ", false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(replenishRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"Replenish/UpdateReplenishAsync", content).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    return result ?? new ApiResult<bool>("Không thể chuyển đổi dữ liệu trả về", false);
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định từ máy chủ";
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
            }catch(HttpRequestException ex)
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
