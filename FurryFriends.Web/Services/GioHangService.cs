using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using FurryFriends.Web.ViewModels;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace FurryFriends.Web.Services
{
    public class GioHangService : IGioHangService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GioHangService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GioHangDTO> GetGioHangAsync(Guid khachHangId)
        {
            var response = await _httpClient.GetAsync($"/api/GioHang/{khachHangId}");
            response.EnsureSuccessStatusCode();
            if (response.StatusCode == HttpStatusCode.NoContent)
                return null; // hoặc trả về DTO mặc định

            return await response.Content.ReadFromJsonAsync<GioHangDTO>();

        }

        public async Task AddToCartAsync(AddToCartDTO dto)
        {
            var json = JsonConvert.SerializeObject(dto);
            Console.WriteLine("📤 JSON gửi lên API: " + json);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/GioHang/add", content);

            var responseContent = await response.Content.ReadAsStringAsync(); // 👈 Đọc dữ liệu trả về

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("❌ API trả về lỗi: " + responseContent);
                throw new Exception($"API lỗi: {(int)response.StatusCode} - {responseContent}");
            }

            Console.WriteLine("✅ Thêm giỏ hàng thành công.");
            Console.WriteLine("📥 Kết quả từ API: " + responseContent); // 👈 In ra xem có TenSanPham không
        }



        public async Task UpdateSoLuongAsync(Guid chiTietId, int soLuong)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/GioHang/update/{chiTietId}", soLuong);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveAsync(Guid chiTietId)
        {
            var response = await _httpClient.DeleteAsync($"/api/GioHang/delete/{chiTietId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<decimal> TinhTongTienSauVoucher(Guid khachHangId, Guid voucherId)
        {
            var dto = new GioHangVoucherDTO
            {
                KhachHangId = khachHangId,
                VoucherId = voucherId
            };

            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/GioHang/ap-dung-voucher", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception("API trả về lỗi khi áp dụng voucher: " + error);
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"➡️ Response từ API ap-dung-voucher: {responseBody}");

            dynamic result = JsonConvert.DeserializeObject(responseBody);
            decimal tienSauGiam = result.tienSauGiam ?? 0; //đoạn này lỗi tien sau giam = 0

            return tienSauGiam;
        }

        public async Task<ThanhToanResultViewModel> ThanhToanAsync(ThanhToanDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/giohang/thanh-toan", dto);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("❌ Lỗi từ server:");
                Console.WriteLine(responseContent);
                throw new Exception($"Thanh toán thất bại ({response.StatusCode}): {responseContent}");
            }

            return await response.Content.ReadFromJsonAsync<ThanhToanResultViewModel>();
        }

    }
}
