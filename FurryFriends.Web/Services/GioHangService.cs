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

                try
                {
                    // Bước 1: parse trực tiếp
                    var error = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    string msg = error?.message ?? "Có lỗi xảy ra.";
                    throw new Exception((string)msg);
                }
                catch (JsonReaderException)
                {
                    // Bước 2: Nếu không phải JSON, giữ nguyên
                    throw new Exception($"API lỗi: {(int)response.StatusCode} - {responseContent}");
                }
            }

            Console.WriteLine("✅ Thêm giỏ hàng thành công.");
            Console.WriteLine("📥 Kết quả từ API: " + responseContent); // 👈 In ra xem có TenSanPham không
        }



        public async Task<(bool Success, string Message)> UpdateSoLuongAsync(Guid chiTietId, int soLuong)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/GioHang/update/{chiTietId}", soLuong);
            if (response.IsSuccessStatusCode)
            {
                return (true, "Cập nhật số lượng thành công");
            }

            // Đọc lỗi từ API
            var errorJson = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            var errorMessage = errorJson?.Message ?? "Có lỗi xảy ra khi cập nhật số lượng";
            return (false, errorMessage);
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
                // Nếu voucher không đạt điều kiện tối thiểu, xem như không áp dụng và trả về tổng cũ
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("⚠️ Không áp dụng voucher: " + error);
                return 0; // Signal để controller không hiển thị dòng Giảm giá
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"➡️ Response từ API ap-dung-voucher: {responseBody}");

            dynamic result = JsonConvert.DeserializeObject(responseBody);
            decimal tienSauGiam = result.tienSauGiam ?? 0;

            return tienSauGiam;
        }

        public async Task<VoucherPreviewResult?> PreviewVoucherAsync(Guid khachHangId, Guid voucherId)
        {
            var dto = new GioHangVoucherDTO { KhachHangId = khachHangId, VoucherId = voucherId };
            var response = await _httpClient.PostAsJsonAsync("/api/GioHang/ap-dung-voucher", dto);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"➡️ Response từ API ap-dung-voucher: {responseBody}");

            dynamic result = JsonConvert.DeserializeObject(responseBody);

            return new VoucherPreviewResult
            {
                TongTienHang = result.tongTienHang ?? 0,
                PhiVanChuyen = result.phiVanChuyen ?? 0,
                TongDonHang = result.tongDonHang ?? 0,
                GiamGia = result.giamGia ?? 0,
                TienSauGiam = result.tienSauGiam ?? 0,
                PhanTramGiam = result.phanTramGiam ?? 0,
                TenVoucher = result.tenVoucher ?? "",
                MaVoucher = result.maVoucher ?? ""
            };
        }

        public async Task<ThanhToanResultViewModel> ThanhToanAsync(ThanhToanDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/giohang/thanh-toan", dto);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("❌ Lỗi từ server:");
                Console.WriteLine(responseContent);
                
                // ✅ Cải thiện xử lý lỗi để hiển thị thông báo thân thiện từ API
                try
                {
                    // Thử parse JSON error response từ API
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    if (errorResponse?.message != null)
                    {
                        // Sử dụng thông báo lỗi thân thiện từ API
                        throw new Exception((string)errorResponse.message);
                    }
                }
                catch (JsonReaderException)
                {
                    // Nếu không phải JSON, sử dụng response content trực tiếp
                    if (responseContent.Contains("Rất tiếc!"))
                    {
                        // Nếu có thông báo thân thiện trong response, sử dụng nó
                        throw new Exception(responseContent);
                    }
                }
                
                // Fallback: thông báo lỗi chung
                throw new Exception("😔 Có lỗi xảy ra khi xử lý thanh toán. Vui lòng thử lại hoặc liên hệ hỗ trợ.");
            }

            return await response.Content.ReadFromJsonAsync<ThanhToanResultViewModel>();
        }

        public async Task<int> GetDonChoDuyetCountAsync(Guid khachHangId)
        {
            var resp = await _httpClient.GetAsync($"/api/GioHang/cho-duyet-count/{khachHangId}");
            if (!resp.IsSuccessStatusCode) return 0;

            var body = await resp.Content.ReadAsStringAsync();
            dynamic obj = JsonConvert.DeserializeObject(body);
            int count = obj?.count ?? 0;
            return count;
        }

        public async Task<(bool CoThayDoi, List<string> ThongBao)> KiemTraThayDoiGiaAsync(Guid khachHangId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/GioHang/kiem-tra-thay-doi-gia/{khachHangId}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<dynamic>();
                    var coThayDoi = result?.coThayDoi ?? false;
                    var thongBao = result?.thongBao?.ToObject<List<string>>() ?? new List<string>();
                    return (coThayDoi, thongBao);
                }
                return (false, new List<string>());
            }
            catch
            {
                return (false, new List<string>());
            }
        }
        public class ApiErrorResponse
        {
            public string Message { get; set; }
        }
    }
}
