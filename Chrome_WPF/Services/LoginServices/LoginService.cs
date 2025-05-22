using Chrome_WPF.Constants.API_Constant;
using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.LoginDTO;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.LoginServices
{
    public class LoginService : ILoginService
    {
        private readonly HttpClient _httpClient;
        private readonly API_Constant _baseUrl;

        public LoginService(API_Constant baseUrl)
        {
            _baseUrl = baseUrl;
            // Khởi tạo HttpClient một lần duy nhất
            _httpClient = _baseUrl.GetHttpClient();
        }

        public async Task<ApiResult<LoginResponseDTO>> AuthenticateAsync(LoginRequestDTO loginRequest)
        {
            // Kiểm tra đầu vào
            if (loginRequest == null)
            {
                return new ApiResult<LoginResponseDTO>("Yêu cầu đăng nhập không được null", false);
            }
            // Validate Username
            if (string.IsNullOrWhiteSpace(loginRequest.Username))
            {
                return new ApiResult<LoginResponseDTO>("Tên đăng nhập không được để trống", false);
            }

            // Validate Password
            if (string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return new ApiResult<LoginResponseDTO>("Mật khẩu không được để trống", false);
            }

            // Optional: Trim whitespace to clean input
            loginRequest.Username = loginRequest.Username.Trim();
            loginRequest.Password = loginRequest.Password.Trim();
            try
            {
                // Chuyển đổi yêu cầu đăng nhập thành JSON
                var json = JsonConvert.SerializeObject(loginRequest);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gửi yêu cầu POST đến API
                var response = await _httpClient.PostAsync("Login/Login", content).ConfigureAwait(false);

                // Đọc nội dung phản hồi
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                // Kiểm tra phản hồi thành công
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<LoginResponseDTO>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<LoginResponseDTO>(result?.Message ?? "Không thể phân tích phản hồi đăng nhập", false);
                    }

                    // Kiểm tra dữ liệu lồng trong trường hợp thành công
                    if (result!.Data == null || !result.Success)
                    {
                        return new ApiResult<LoginResponseDTO>(result.Message ?? "Dữ liệu đăng nhập không hợp lệ", false);
                    }

                    return result;
                }

                // Xử lý các lỗi HTTP cụ thể
                var errorResult = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định từ server";

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<LoginResponseDTO>((string)errorMessage, false); // Giữ nguyên thông điệp từ server, ví dụ: "Tài khoản không tồn tại"
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return new ApiResult<LoginResponseDTO>($"Yêu cầu không hợp lệ: {errorMessage}", false);
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return new ApiResult<LoginResponseDTO>("API đăng nhập không tồn tại", false);
                }

                // Xử lý các lỗi HTTP khác
                return new ApiResult<LoginResponseDTO>($"Đăng nhập thất bại: {response.StatusCode}. {errorMessage}", false);
            }
            catch (HttpRequestException ex)
            {
                // Lỗi mạng
                return new ApiResult<LoginResponseDTO>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                // Lỗi phân tích JSON
                return new ApiResult<LoginResponseDTO>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return new ApiResult<LoginResponseDTO>($"Lỗi không xác định: {ex.Message}", false);
            }
        }

        // Đảm bảo giải phóng HttpClient khi dịch vụ bị hủy
        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public async Task<ApiResult<UserInformationDTO>> GetUserInformation(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return new ApiResult<UserInformationDTO>("Tên người dùng không được để trống", false);
            }
            try
            {
                var response = await _httpClient.GetAsync($"Login/GetUserInformation?userName={userName}").ConfigureAwait(false);
                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<UserInformationDTO>>(jsonResponse);
                    if (result == null || !result.Success)
                    {
                        return new ApiResult<UserInformationDTO>(result?.Message ?? "Không thể phân tích phản hồi thông tin người dùng", false);
                    }
                    return result;
                }
                var errorResult = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                var errorMessage = errorResult?.Message ?? "Lỗi không xác định từ server";
                return new ApiResult<UserInformationDTO>($"Lỗi khi lấy thông tin người dùng: {errorMessage}", false);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<UserInformationDTO>($"Lỗi mạng: {ex.Message}", false);
            }
            catch (JsonException ex)
            {
                return new ApiResult<UserInformationDTO>($"Lỗi phân tích phản hồi: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return new ApiResult<UserInformationDTO>($"Lỗi không xác định: {ex.Message}", false);
            }
        }
    }
}