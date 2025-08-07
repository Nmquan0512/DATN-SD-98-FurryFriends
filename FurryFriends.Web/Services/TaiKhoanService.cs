using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.Web.Services.IService;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using LoginRequest = FurryFriends.API.Models.LoginRequest;
using LoginResponse = FurryFriends.API.Models.LoginResponse;

namespace FurryFriends.Web.Services
{
    public class TaiKhoanService : ITaiKhoanService
    {
        private readonly HttpClient _httpClient;

        public TaiKhoanService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IEnumerable<TaiKhoan>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<TaiKhoan>>("TaiKhoanApi")
                ?? throw new HttpRequestException("Không thể lấy danh sách tài khoản.");
        }

        public async Task<TaiKhoan?> GetByIdAsync(Guid taiKhoanId)
        {
            if (taiKhoanId == Guid.Empty)
                throw new ArgumentException("TaiKhoanId không hợp lệ.");

            return await _httpClient.GetFromJsonAsync<TaiKhoan>($"TaiKhoanApi/{taiKhoanId}")
                ?? throw new HttpRequestException($"Không tìm thấy tài khoản với ID {taiKhoanId}.");
        }

        public async Task AddAsync(TaiKhoan taiKhoan)
        {
            if (taiKhoan == null)
                throw new ArgumentNullException(nameof(taiKhoan));

            taiKhoan.NhanVien = null;
            taiKhoan.KhachHang = null;
       

            var response = await _httpClient.PostAsJsonAsync("TaiKhoanApi", taiKhoan);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest &&
                    response.Content?.Headers.ContentType?.MediaType == "application/problem+json")
                {
                    throw new ValidationException(content);
                }

                throw new HttpRequestException(content);
            }
        }

        public async Task UpdateAsync(TaiKhoan taiKhoan)
        {
            var response = await _httpClient.PutAsJsonAsync($"TaiKhoanApi/{taiKhoan.TaiKhoanId}", taiKhoan);
            if (taiKhoan == null)
                throw new ArgumentNullException(nameof(taiKhoan));

            taiKhoan.NhanVien = null;
            taiKhoan.KhachHang = null;
      

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest &&
                    response.Content?.Headers.ContentType?.MediaType == "application/problem+json")
                {
                    throw new ValidationException(content);
                }

                throw new HttpRequestException(content);
            }
        }

        public async Task DeleteAsync(Guid taiKhoanId)
        {
            if (taiKhoanId == Guid.Empty)
                throw new ArgumentException("TaiKhoanId không hợp lệ.");

            var response = await _httpClient.DeleteAsync($"TaiKhoanApi/{taiKhoanId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<TaiKhoan>> FindByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Tên đăng nhập không được để trống.");

            var all = await GetAllAsync();
            return all.Where(tk => tk.UserName == userName);
        }

        public async Task<IEnumerable<TaiKhoan>> GetAllTaiKhoanAsync()
        {
            return await GetAllAsync();
        }

        public async Task<LoginResponse?> DangNhapAdminAsync(LoginRequest model)
        {
            var response = await _httpClient.PostAsJsonAsync("TaiKhoanApi/dang-nhap-admin", model);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }

        public async Task<LoginResponse?> DangNhapKhachHangAsync(LoginRequest model)
        {
            var response = await _httpClient.PostAsJsonAsync("TaiKhoanApi/dang-nhap-khachhang", model);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }
    }
}