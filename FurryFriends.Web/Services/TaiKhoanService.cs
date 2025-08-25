using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;
using System.Net.Http.Json;

namespace FurryFriends.Web.Services
{
    public class TaiKhoanService : ITaiKhoanService
    {
        private readonly HttpClient _httpClient;

        public TaiKhoanService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<TaiKhoan>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<TaiKhoan>>("api/taiKhoans")
                   ?? Enumerable.Empty<TaiKhoan>();
        }

        public async Task<TaiKhoan?> GetByIdAsync(Guid taiKhoanId)
        {
            return await _httpClient.GetFromJsonAsync<TaiKhoan>($"api/taiKhoans/{taiKhoanId}");
        }

        public async Task AddAsync(TaiKhoan taiKhoan)
        {
            var response = await _httpClient.PostAsJsonAsync("api/taiKhoans", taiKhoan);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(TaiKhoan taiKhoan)
        {
            if (taiKhoan == null)
                throw new ArgumentNullException(nameof(taiKhoan));

            // tránh vòng lặp khi serialize
            taiKhoan.NhanVien = null;
            taiKhoan.KhachHang = null;

            var response = await _httpClient.PutAsJsonAsync($"api/taiKhoans/{taiKhoan.TaiKhoanId}", taiKhoan);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(Guid taiKhoanId)
        {
            var response = await _httpClient.DeleteAsync($"api/taiKhoans/{taiKhoanId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<TaiKhoan>> FindByUserNameAsync(string userName)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<TaiKhoan>>($"api/taiKhoans/search/{userName}")
                   ?? Enumerable.Empty<TaiKhoan>();
        }

        public async Task<IEnumerable<TaiKhoan>> GetAllTaiKhoanAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<TaiKhoan>>("api/taiKhoans")
                   ?? Enumerable.Empty<TaiKhoan>();
        }

        public async Task<(LoginResponse? Response, string? ErrorMessage)> DangNhapAdminAsync(LoginRequest model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/taiKhoans/dangnhap-admin", model);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    return (loginResponse, null);
                }

                var error = await response.Content.ReadAsStringAsync();
                return (null, error);
            }
            catch (Exception ex)
            {
                return (null, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<(LoginResponse? Response, string? ErrorMessage)> DangNhapKhachHangAsync(LoginRequest model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
                {
                    return (null, "Tên đăng nhập và mật khẩu không được để trống.");
                }

                var response = await _httpClient.PostAsJsonAsync("api/taiKhoans/dangnhap-khachhang", model);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                    {
                        return (loginResponse, null);
                    }

                    return (null, "Tài khoản không hợp lệ hoặc bị khóa.");
                }

                var error = await response.Content.ReadAsStringAsync();
                return (null, error);
            }
            catch (Exception ex)
            {
                return (null, $"Lỗi hệ thống: {ex.Message}");
            }
        }
    }
}
