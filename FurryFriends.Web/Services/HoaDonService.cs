using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;
using System.Net.Http.Json;

namespace FurryFriends.Web.Services
{
    public class HoaDonService : IHoaDonService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/HoaDon";

        public HoaDonService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<HoaDon>> GetHoaDonListAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(BaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<IEnumerable<HoaDon>>();
                    return result ?? new List<HoaDon>();
                }
                
                // Log error details
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Error: {response.StatusCode} - {errorContent}");
                
                throw new Exception($"Lỗi khi lấy danh sách hóa đơn: {response.StatusCode}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"Timeout error: {ex.Message}");
                throw new Exception("Yêu cầu bị timeout. Vui lòng thử lại sau.");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request error: {ex.Message}");
                throw new Exception("Không thể kết nối đến server. Vui lòng kiểm tra kết nối mạng.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                throw new Exception($"Lỗi khi lấy danh sách hóa đơn: {ex.Message}");
            }
        }

        public async Task<HoaDon> GetHoaDonByIdAsync(Guid hoaDonId)
        {
            try
            {
                if (hoaDonId == Guid.Empty)
                {
                    throw new ArgumentException("ID hóa đơn không hợp lệ");
                }

                var response = await _httpClient.GetAsync($"{BaseUrl}/{hoaDonId}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<HoaDon>();
                    if (result == null)
                    {
                        throw new KeyNotFoundException($"Không tìm thấy hóa đơn với ID: {hoaDonId}");
                    }
                    return result;
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new KeyNotFoundException($"Không tìm thấy hóa đơn với ID: {hoaDonId}");
                }
                
                throw new Exception($"Lỗi khi lấy thông tin hóa đơn: {response.StatusCode}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"Timeout error: {ex.Message}");
                throw new Exception("Yêu cầu bị timeout. Vui lòng thử lại sau.");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request error: {ex.Message}");
                throw new Exception("Không thể kết nối đến server. Vui lòng kiểm tra kết nối mạng.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                throw new Exception($"Lỗi khi lấy thông tin hóa đơn: {ex.Message}");
            }
        }

        public async Task<IEnumerable<HoaDon>> SearchHoaDonAsync(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    throw new ArgumentException("Từ khóa tìm kiếm không được để trống");
                }

                var response = await _httpClient.GetAsync($"{BaseUrl}/search?keyword={Uri.EscapeDataString(keyword)}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<IEnumerable<HoaDon>>();
                    return result ?? new List<HoaDon>();
                }
                throw new Exception($"Lỗi khi tìm kiếm hóa đơn: {response.StatusCode}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"Timeout error: {ex.Message}");
                throw new Exception("Yêu cầu bị timeout. Vui lòng thử lại sau.");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request error: {ex.Message}");
                throw new Exception("Không thể kết nối đến server. Vui lòng kiểm tra kết nối mạng.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                throw new Exception($"Lỗi khi tìm kiếm hóa đơn: {ex.Message}");
            }
        }

        public async Task<byte[]> ExportHoaDonToPdfAsync(Guid hoaDonId)
        {
            try
            {
                if (hoaDonId == Guid.Empty)
                {
                    throw new ArgumentException("ID hóa đơn không hợp lệ");
                }

                var response = await _httpClient.GetAsync($"{BaseUrl}/{hoaDonId}/pdf");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                throw new Exception($"Lỗi khi xuất hóa đơn PDF: {response.StatusCode}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"Timeout error: {ex.Message}");
                throw new Exception("Yêu cầu bị timeout. Vui lòng thử lại sau.");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request error: {ex.Message}");
                throw new Exception("Không thể kết nối đến server. Vui lòng kiểm tra kết nối mạng.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                throw new Exception($"Lỗi khi xuất hóa đơn PDF: {ex.Message}");
            }
        }

        // Dashboard methods
        public async Task<int> GetTotalOrdersAsync()
        {
            // Simulate database call
            await Task.Delay(100);
            return 1250; // Mock data
        }

        public async Task<decimal> GetMonthlyRevenueAsync()
        {
            // Simulate database call
            await Task.Delay(100);
            return 35000000; // Mock data
        }

        public async Task<List<object>> GetRevenueByMonthAsync()
        {
            // Simulate database call
            await Task.Delay(100);
            
            return new List<object>
            {
                new { labels = new[] { "T1", "T2", "T3", "T4", "T5", "T6", "T7", "T8", "T9", "T10", "T11", "T12" } },
                new { values = new[] { 12000000, 15000000, 18000000, 14000000, 20000000, 22000000, 25000000, 23000000, 28000000, 30000000, 32000000, 35000000 } }
            };
        }

        public async Task<List<object>> GetOrdersByStatusAsync()
        {
            // Simulate database call
            await Task.Delay(100);
            
            return new List<object>
            {
                new { labels = new[] { "Đã thanh toán", "Chờ xử lý", "Đang giao", "Hoàn thành", "Hủy" } },
                new { values = new[] { 45, 25, 15, 10, 5 } }
            };
        }

        public async Task<List<object>> GetRecentOrdersAsync(int count)
        {
            // Simulate database call
            await Task.Delay(100);
            
            var orders = new List<object>
            {
                new { MaHoaDon = "HD001", TenKhachHang = "Nguyễn Văn A", SoLuongSanPham = 3, TongTien = 1500000, TrangThai = "Đã thanh toán", NgayTao = DateTime.Now.AddDays(-1) },
                new { MaHoaDon = "HD002", TenKhachHang = "Trần Thị B", SoLuongSanPham = 2, TongTien = 800000, TrangThai = "Chờ xử lý", NgayTao = DateTime.Now.AddDays(-2) },
                new { MaHoaDon = "HD003", TenKhachHang = "Lê Văn C", SoLuongSanPham = 5, TongTien = 2500000, TrangThai = "Đang giao", NgayTao = DateTime.Now.AddDays(-3) },
                new { MaHoaDon = "HD004", TenKhachHang = "Phạm Thị D", SoLuongSanPham = 1, TongTien = 500000, TrangThai = "Hoàn thành", NgayTao = DateTime.Now.AddDays(-4) },
                new { MaHoaDon = "HD005", TenKhachHang = "Hoàng Văn E", SoLuongSanPham = 4, TongTien = 1800000, TrangThai = "Đã thanh toán", NgayTao = DateTime.Now.AddDays(-5) }
            };

            return orders.GetRange(0, Math.Min(count, orders.Count));
        }
    }
} 