using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.LoginDTO;
using Chrome_WPF.Models.PagedResponse;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace Chrome_WPF.Services.AccountManagementService
{
    public class AccountManagementService:IAccountManagementService
    {
        private readonly HttpClient _httpClient;
        private readonly API_Constant _baseUrl;

        public AccountManagementService(API_Constant baseUrl)
        {
            _baseUrl = baseUrl;
            // Khởi tạo HttpClient một lần duy nhất
            _httpClient = _baseUrl.GetHttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AccessToken);
        }

        public async Task<ApiResult<bool>> AddAccountManagement(AccountManagementRequestDTO accountManagementRequestDTO)
        {
            if (accountManagementRequestDTO == null)
            {
                return new ApiResult<bool>("Dữ liệu nhận vào không hợp lệ", false);
            }

            if (string.IsNullOrEmpty(accountManagementRequestDTO.UserName))
            {
                return new ApiResult<bool>("Tên tài khoản không được để trống", false);
            }
            if (string.IsNullOrEmpty(accountManagementRequestDTO.Password))
            {
                return new ApiResult<bool>("Mật khẩu không được để trống", false);
            }
            accountManagementRequestDTO.UserName = accountManagementRequestDTO.UserName.Trim();
            try
            {
                var json = JsonConvert.SerializeObject(accountManagementRequestDTO);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                // Gửi yêu cầu POST đến API
                var response = await _httpClient.PostAsync("AccountManagement/AddAccountManagement", content).ConfigureAwait(false);
                // Đọc nội dung phản hồi
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                // Kiểm tra phản hồi thành công
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể phân tích phản hồi thêm người dùng", false);
                    }
                    return result;
                }
                
                var errorResult = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định từ server";
                if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Trả về thông điệp lỗi chung
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

        public async Task<ApiResult<bool>> DeleteAccountManagement(string userName)
        {
            if(string.IsNullOrEmpty(userName))
            {
                return new ApiResult<bool>("Tên tài khoản không được để trống", false);
            }
            userName = userName.Trim();
            try
            {
                var response = await _httpClient.DeleteAsync($"AccountManagement/DeleteAccountManagement?UserName={userName}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể phân tích phản hồi đăng nhập", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định từ server";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Trả về thông điệp lỗi chung
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

        public async Task<ApiResult<PagedResponse<AccountManagementResponseDTO>>> GetAllAccount(int page, int pageSize)
        {
            try
            {
                var response = await _httpClient.GetAsync($"AccountManagement/GetAllAccount?page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<AccountManagementResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<AccountManagementResponseDTO>>(result?.Message ?? "Không thể phân tích phản hồi đăng nhập", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định từ server";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<AccountManagementResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<AccountManagementResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<AccountManagementResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<AccountManagementResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<PagedResponse<AccountManagementResponseDTO>>((string)errorMessage, false); // Trả về thông điệp lỗi chung
                }

            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<PagedResponse<AccountManagementResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<PagedResponse<AccountManagementResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<PagedResponse<AccountManagementResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<AccountManagementResponseDTO>>> GetAllAccountWithRole(string groupID, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(groupID))
            {
                return new ApiResult<PagedResponse<AccountManagementResponseDTO>>("ID nhóm không được để trống", false);
            }
            groupID = groupID.Trim();
            try
            {
                var response = await _httpClient.GetAsync($"AccountManagement/GetAllAccountWithRole?GroupID={groupID}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<AccountManagementResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<AccountManagementResponseDTO>>(result?.Message ?? "Không thể phân tích phản hồi đăng nhập", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định từ server";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<AccountManagementResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<AccountManagementResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<AccountManagementResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<AccountManagementResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<PagedResponse<AccountManagementResponseDTO>>((string)errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<PagedResponse<AccountManagementResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<PagedResponse<AccountManagementResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<PagedResponse<AccountManagementResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<PagedResponse<AccountManagementResponseDTO>>> SearchAccountInList(string textToSearch, int page, int pageSize)
        {
            if(string.IsNullOrEmpty(textToSearch))
            {
                return new ApiResult<PagedResponse<AccountManagementResponseDTO>>("Từ khóa tìm kiếm không được để trống", false);
            }
            textToSearch = textToSearch.Trim();
            try
            {
                var response = await _httpClient.GetAsync($"AccountManagement/SearchAccountInList?textToSearch={textToSearch}&page={page}&pageSize={pageSize}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<PagedResponse<AccountManagementResponseDTO>>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<PagedResponse<AccountManagementResponseDTO>>(result?.Message ?? "Không thể phân tích phản hồi đăng nhập", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định từ server";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<PagedResponse<AccountManagementResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<PagedResponse<AccountManagementResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<PagedResponse<AccountManagementResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<PagedResponse<AccountManagementResponseDTO>>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<PagedResponse<AccountManagementResponseDTO>>((string)errorMessage, false); // Trả về thông điệp lỗi chung
                }
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<PagedResponse<AccountManagementResponseDTO>>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<PagedResponse<AccountManagementResponseDTO>>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<PagedResponse<AccountManagementResponseDTO>>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        public async Task<ApiResult<bool>> UpdateAccountManagement(AccountManagementRequestDTO accountManagementRequestDTO)
        {
            if (accountManagementRequestDTO == null)
            {
                return new ApiResult<bool>("Dữ liệu nhận vào không hợp lệ", false);
            }

            if (string.IsNullOrEmpty(accountManagementRequestDTO.UserName))
            {
                return new ApiResult<bool>("Tên tài khoản không được để trống", false);
            }
            if (string.IsNullOrEmpty(accountManagementRequestDTO.Password))
            {
                return new ApiResult<bool>("Mật khẩu không được để trống", false);
            }
            accountManagementRequestDTO.UserName = accountManagementRequestDTO.UserName.Trim();
            try
            {
                var json = JsonConvert.SerializeObject(accountManagementRequestDTO);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                // Gửi yêu cầu POST đến API
                var response = await _httpClient.PutAsync("AccountManagement/UpdateAccountManagement", content).ConfigureAwait(false);
                // Đọc nội dung phản hồi
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                // Kiểm tra phản hồi thành công
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<bool>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<bool>(result?.Message ?? "Không thể phân tích phản hồi đăng nhập", false);
                    }
                    return result;
                }

                var errorResult = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định từ server";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không có quyền truy cập"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Lỗi máy chủ nội bộ"
                }
                else
                {
                    return new ApiResult<bool>((string)errorMessage, false); // Trả về thông điệp lỗi chung
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
        public void Dispose()
        {
            // Giải phóng tài nguyên HttpClient khi không còn cần thiết
            _httpClient?.Dispose();
        }
    }
}
