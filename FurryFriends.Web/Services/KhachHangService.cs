using FurryFriends.API.Models;
using FurryFriends.Web.Services;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using FurryFriends.Web.Services.IService;

namespace FurryFriends.Web.Services
{
    public class KhachHangService : IKhachHangService
    {
        private readonly HttpClient _httpClient;

        public KhachHangService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<KhachHang>> GetAllAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<KhachHang>>("api/KhachHang")
                    ?? throw new HttpRequestException("Không thể lấy danh sách khách hàng.");
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Lỗi khi lấy danh sách khách hàng: {ex.Message}");
            }
        }

        public async Task<KhachHang> GetByIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new ArgumentException("ID khách hàng không hợp lệ.");

                return await _httpClient.GetFromJsonAsync<KhachHang>($"api/KhachHang/{id}")
                    ?? throw new HttpRequestException($"Không tìm thấy khách hàng với ID {id}.");
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Lỗi khi lấy thông tin khách hàng: {ex.Message}");
            }
        }

        public async Task<bool> CreateAsync(KhachHang khachHang)
        {
            var response = await _httpClient.PostAsJsonAsync("api/KhachHang", khachHang);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(Guid id, KhachHang khachHang)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/KhachHang/{id}", khachHang);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/KhachHang/{id}");
            return response.IsSuccessStatusCode;
        }

        public Task<string?> GetAllKhachHangAsync()
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetKhachHangByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteKhachHangAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task AddKhachHangAsync(KhachHang model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/KhachHang", model);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Đăng ký khách hàng thất bại!");
            }
        }

        public Task UpdateKhachHangAsync(KhachHang model)
        {
            throw new NotImplementedException();
        }

        public async Task<KhachHang?> FindByEmailAsync(string email)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<KhachHang>($"api/KhachHang/email/{email}");
            }
            catch
            {
                return null; // Return null if not found or error
            }
        }

        public async Task<KhachHang?> FindByPhoneAsync(string phone)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<KhachHang>($"api/KhachHang/phone/{phone}");
            }
            catch
            {
                return null; // Return null if not found or error
            }
        }

        public async Task<int> GetTotalCustomersAsync()
        {
            try
            {
                var all = await GetAllAsync();
                // Nếu có thuộc tính TrangThai, chỉ đếm khách hàng đang hoạt động:
                // return all.Count(kh => kh.TrangThai == true);
                return all.Count();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error getting total customers: {ex.Message}");
                return 0; // Trả về 0 nếu có lỗi HTTP
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error getting total customers: {ex.Message}");
                return 0; // Trả về 0 nếu có lỗi khác
            }
        }

        // ✅ Thêm phương thức thống kê cho dashboard
        public async Task<int> GetActiveCustomersAsync()
        {
            try
            {
                var all = await GetAllAsync();
                return all.Count(kh => kh.TrangThai == 1); // 1 = Đang hoạt động
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error getting active customers: {ex.Message}");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error getting active customers: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> GetInactiveCustomersAsync()
        {
            try
            {
                var all = await GetAllAsync();
                return all.Count(kh => kh.TrangThai != 1); // Khác 1 = Không hoạt động
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error getting inactive customers: {ex.Message}");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error getting inactive customers: {ex.Message}");
                return 0;
            }
        }
    }
}