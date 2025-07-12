using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.ProductMasterDTO;
using Chrome_WPF.Models.PurchaseOrderDetailDTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.PurchaseOrderDetailService
{
    public class PurchaseOrderDetailService : IPurchaseOrderDetailService
    {
        private readonly HttpClient _httpClient;
        private readonly API_Constant _baseUrl;
        public PurchaseOrderDetailService(API_Constant baseUrl)
        {
            _baseUrl = baseUrl;
            _httpClient = _baseUrl.GetHttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AccessToken);
        }

        public async Task<ApiResult<bool>> AddPurchaseOrderDetail(PurchaseOrderDetailRequestDTO purchaseOrderDetailRequestDTO)
        {
            if (purchaseOrderDetailRequestDTO == null)
            {
                return new ApiResult<bool>("Dữ liệu nhận vào không hợp lệ", false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(purchaseOrderDetailRequestDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"PurchaseOrder/{Uri.EscapeDataString(purchaseOrderDetailRequestDTO.PurchaseOrderCode)}/PurchaseOrderDetail/AddPurchaseOrderDetail", content).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    return result ?? new ApiResult<bool>("Không thể chuyển đổi dữ liệu trả về", false);
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định khi thêm chi tiết đơn hàng mua";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<bool>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<bool>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<bool>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<bool>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<bool>(errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<bool>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<bool>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<bool>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<bool>> CheckAndUpdateStockBackOrderStatus(string purchaseOrderCode)
        {
            if (string.IsNullOrEmpty(purchaseOrderCode))
            {
                return new ApiResult<bool>("Mã phiếu đặt hàng không được để trống", false);
            }
            try
            {
                var response = await _httpClient.PostAsync($"PurchaseOrder/{Uri.UnescapeDataString(purchaseOrderCode)}/PurchaseOrderDetail/CheckAndUpdateStockBackOrderStatus",null).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    return result ?? new ApiResult<bool>("Không thể chuyển đổi dữ liệu trả về", false);
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định khi kiểm tra và cập nhật trạng thái tồn kho đơn hàng mua";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<bool>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<bool>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<bool>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<bool>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<bool>(errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<bool>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<bool>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<bool>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<bool>> ConfirmPurchaseOrderDetail(string purchaseOrderCode)
        {
            if(string.IsNullOrEmpty(purchaseOrderCode))
            {
                return new ApiResult<bool>("Mã phiếu đặt hàng không được để trống", false);
            }
            try
            {
                var response = await _httpClient.PostAsync($"PurchaseOrder/{Uri.EscapeDataString(purchaseOrderCode)}/PurchaseOrderDetail/ConfirmPurchaseOrderDetail", null).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if(response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    return result ?? new ApiResult<bool>("Không thể chuyển đổi dữ liệu trả về", false);
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định khi xác nhận chi tiết đơn hàng mua";
                if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<bool>(errorMessage, false);
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<bool>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<bool>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<bool>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<bool>(errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }catch(HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<bool>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<bool>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<bool>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<bool>> CreateBackOrder(string purchaseOrderCode, string backOrderDescription, string dateBackOrder)
        {
            if(string.IsNullOrEmpty(purchaseOrderCode) || string.IsNullOrEmpty(backOrderDescription) || string.IsNullOrEmpty(dateBackOrder))
            {
                return new ApiResult<bool>("Mã phiếu đặt hàng, mô tả đơn hàng và ngày đặt hàng không được để trống", false);
            }
            try
            {
                var response = await _httpClient.PostAsync($"PurchaseOrder/{Uri.EscapeDataString(purchaseOrderCode)}/PurchaseOrderDetail/CreateBackOrder?backOrderDescription={Uri.EscapeDataString(backOrderDescription)}&dateBackOrder={Uri.EscapeDataString(dateBackOrder)}", null).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if(response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    return result ?? new ApiResult<bool>("Không thể chuyển đổi dữ liệu trả về", false);
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định khi tạo đơn hàng mua lại";
                if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<bool>(errorMessage, false);
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<bool>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<bool>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<bool>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<bool>(errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<bool>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<bool>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<bool>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<bool>> DeletePurchaseOrderDetail(string purchaseOrderCode, string productCode)
        {
            if (string.IsNullOrEmpty(purchaseOrderCode) || string.IsNullOrEmpty(productCode))
            {
                return new ApiResult<bool>("Mã phiếu đặt hàng hoặc mã sản phẩm không được để trống", false);
            }
            try
            {
                var response = await _httpClient.DeleteAsync($"PurchaseOrder/{Uri.EscapeDataString(purchaseOrderCode)}/PurchaseOrderDetail/DeletePurchaseOrderDetail?productCode={productCode}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    return result ?? new ApiResult<bool>("Không thể chuyển đổi dữ liệu trả về", false);
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định khi xóa chi tiết đơn hàng mua";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<bool>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<bool>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<bool>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<bool>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<bool>(errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<bool>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<bool>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<bool>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<PurchaseOrderDetailResponseDTO>>> GetAllPurchaseOrderDetails(string purchaseOrderCode, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(purchaseOrderCode) || page < 1 || pageSize < 1)
            {
                return new ApiResult<PagedResponse<PurchaseOrderDetailResponseDTO>>("Mã phiếu đặt hàng không được để trống và trang phải lớn hơn 0", false);
            }

            try
            {
                var response = await _httpClient.GetAsync($"PurchaseOrder/{Uri.EscapeDataString(purchaseOrderCode)}/PurchaseOrderDetail/GetAllPurchaseOrderDetails?page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<PurchaseOrderDetailResponseDTO>>>(jsonResponse);
                    return result ?? new ApiResult<PagedResponse<PurchaseOrderDetailResponseDTO>>("Không thể chuyển đổi dữ liệu trả về", false);
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PagedResponse<PurchaseOrderDetailResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định khi lấy danh sách chi tiết đơn hàng mua";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<PurchaseOrderDetailResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<PurchaseOrderDetailResponseDTO>>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<PurchaseOrderDetailResponseDTO>>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<PurchaseOrderDetailResponseDTO>>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<PagedResponse<PurchaseOrderDetailResponseDTO>>(errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<PagedResponse<PurchaseOrderDetailResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<PagedResponse<PurchaseOrderDetailResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<PagedResponse<PurchaseOrderDetailResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<List<ProductMasterResponseDTO>>> GetListProductToPO(string purchaseOrderCode)
        {
            try
            {
                var response = await _httpClient.GetAsync($"PurchaseOrder/{Uri.EscapeDataString(purchaseOrderCode)}/PurchaseOrderDetail/GetListProductToPO").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<List<ProductMasterResponseDTO>>>(jsonResponse);
                    return result ?? new ApiResult<List<ProductMasterResponseDTO>>("Không thể chuyển đổi dữ liệu trả về", false);
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<List<ProductMasterResponseDTO>>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định khi lấy danh sách sản phẩm cho đơn hàng mua";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<List<ProductMasterResponseDTO>>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<List<ProductMasterResponseDTO>>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<List<ProductMasterResponseDTO>>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<List<ProductMasterResponseDTO>>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<List<ProductMasterResponseDTO>>(errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }catch(HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<List<ProductMasterResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<List<ProductMasterResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<List<ProductMasterResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PurchaseOrderDetailResponseDTO>> GetPurchaseOrderDetailByCode(string purchaseOrderCode, string productCode)
        {
            if(string.IsNullOrEmpty(purchaseOrderCode) || string.IsNullOrEmpty(productCode))
            {
                return new ApiResult<PurchaseOrderDetailResponseDTO>("Mã phiếu đặt hàng hoặc mã sản phẩm không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"PurchaseOrder/{Uri.EscapeDataString(purchaseOrderCode)}/PurchaseOrderDetail/GetPurchaseOrderDetailByCode?productCode={productCode}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if(response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PurchaseOrderDetailResponseDTO>>(jsonResponse);
                    return result ?? new ApiResult<PurchaseOrderDetailResponseDTO>("Không thể chuyển đổi dữ liệu trả về", false);
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<PurchaseOrderDetailResponseDTO>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định khi lấy chi tiết đơn hàng mua";
                if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PurchaseOrderDetailResponseDTO>(errorMessage, false);
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PurchaseOrderDetailResponseDTO>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PurchaseOrderDetailResponseDTO>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PurchaseOrderDetailResponseDTO>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<PurchaseOrderDetailResponseDTO>(errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch(HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<PurchaseOrderDetailResponseDTO>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<PurchaseOrderDetailResponseDTO>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<PurchaseOrderDetailResponseDTO>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<bool>> UpdatePurchaseOrderDetail(PurchaseOrderDetailRequestDTO purchaseOrderDetailRequestDTO)
        {
            if (purchaseOrderDetailRequestDTO == null)
            {
                return new ApiResult<bool>("Dữ liệu nhận vào không hợp lệ", false);
            }
            try
            {
                var json = JsonConvert.SerializeObject(purchaseOrderDetailRequestDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"PurchaseOrder/{Uri.EscapeDataString(purchaseOrderDetailRequestDTO.PurchaseOrderCode)}/PurchaseOrderDetail/UpdatePurchaseOrderDetail", content).ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    return result ?? new ApiResult<bool>("Không thể chuyển đổi dữ liệu trả về", false);
                }
                var errorResult = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định khi cập nhật chi tiết đơn hàng mua";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<bool>(errorMessage, false);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<bool>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<bool>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<bool>(errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<bool>(errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<bool>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<bool>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<bool>($"Lỗi không xác định: {ex.Message}", false);
            }
        }
    }
}
