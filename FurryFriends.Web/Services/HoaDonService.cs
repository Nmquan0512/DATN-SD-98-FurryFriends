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
            try
            {
                var allOrders = await GetHoaDonListAsync();
                return allOrders.Count();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTotalOrdersAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<decimal> GetMonthlyRevenueAsync()
        {
            try
            {
                var allOrders = await GetHoaDonListAsync();
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;
                
                var monthlyRevenue = allOrders
                    .Where(h => h.NgayTao.Month == currentMonth && h.NgayTao.Year == currentYear)
                    .Sum(h => h.TongTienSauKhiGiam);
                
                return monthlyRevenue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMonthlyRevenueAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<List<object>> GetRevenueByMonthAsync()
        {
            try
            {
                var allOrders = await GetHoaDonListAsync();
                var currentYear = DateTime.Now.Year;
                
                var monthlyData = new List<object>();
                var labels = new string[12];
                var values = new decimal[12];
                
                // Khởi tạo dữ liệu cho 12 tháng
                for (int i = 0; i < 12; i++)
                {
                    labels[i] = $"T{i + 1}";
                    values[i] = 0;
                }
                
                // Tính doanh thu theo từng tháng
                foreach (var order in allOrders.Where(h => h.NgayTao.Year == currentYear))
                {
                    var monthIndex = order.NgayTao.Month - 1; // Month bắt đầu từ 1, index bắt đầu từ 0
                    if (monthIndex >= 0 && monthIndex < 12)
                    {
                        values[monthIndex] += order.TongTienSauKhiGiam;
                    }
                }
                
                monthlyData.Add(new { labels = labels });
                monthlyData.Add(new { values = values });
                
                return monthlyData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetRevenueByMonthAsync: {ex.Message}");
                return new List<object>
                {
                    new { labels = new[] { "T1", "T2", "T3", "T4", "T5", "T6", "T7", "T8", "T9", "T10", "T11", "T12" } },
                    new { values = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } }
                };
            }
        }

        public async Task<List<object>> GetOrdersByStatusAsync()
        {
            try
            {
                var allOrders = await GetHoaDonListAsync();
                
                var statusCounts = new Dictionary<int, int>
                {
                    { 0, 0 }, // Chờ duyệt
                    { 1, 0 }, // Đã duyệt
                    { 2, 0 }, // Đang giao
                    { 3, 0 }, // Đã giao
                    { 4, 0 }  // Đã hủy
                };
                
                // Đếm số lượng đơn hàng theo từng trạng thái
                foreach (var order in allOrders)
                {
                    if (statusCounts.ContainsKey(order.TrangThai))
                    {
                        statusCounts[order.TrangThai]++;
                    }
                }
                
                var labels = new[] { "Chờ duyệt", "Đã duyệt", "Đang giao", "Đã giao", "Đã hủy" };
                var values = new[] { 
                    statusCounts[0], 
                    statusCounts[1], 
                    statusCounts[2], 
                    statusCounts[3], 
                    statusCounts[4] 
                };
                
                return new List<object>
                {
                    new { labels = labels },
                    new { values = values }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetOrdersByStatusAsync: {ex.Message}");
                return new List<object>
                {
                    new { labels = new[] { "Chờ duyệt", "Đã duyệt", "Đang giao", "Đã giao", "Đã hủy" } },
                    new { values = new[] { 0, 0, 0, 0, 0 } }
                };
            }
        }

        public async Task<List<object>> GetRecentOrdersAsync(int count)
        {
            try
            {
                var allOrders = await GetHoaDonListAsync();
                var recentOrders = allOrders
                    .OrderByDescending(h => h.NgayTao)
                    .Take(count)
                    .Select(h => new
                    {
                        h.HoaDonId,
                        h.NgayTao,
                        h.TongTien,
                        h.TongTienSauKhiGiam,
                        h.TrangThai,
                        CustomerName = h.TenCuaKhachHang ?? "Không xác định",
                        StatusText = GetStatusText(h.TrangThai)
                    })
                    .ToList<object>();

                return recentOrders;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetRecentOrdersAsync: {ex.Message}");
                return new List<object>();
            }
        }

        // Helper method để lấy text trạng thái
        private string GetStatusText(int trangThai)
        {
            return trangThai switch
            {
                0 => "Chờ duyệt",
                1 => "Đã duyệt",
                2 => "Đang giao",
                3 => "Đã giao",
                4 => "Đã hủy",
                _ => "Không xác định"
            };
        }

        // ✅ Hủy đơn hàng
        public async Task<ApiResult> HuyDonHangAsync(Guid hoaDonId)
        {
            try
            {
                if (hoaDonId == Guid.Empty)
                {
                    return new ApiResult { Success = false, Message = "ID hóa đơn không hợp lệ" };
                }

                var response = await _httpClient.PostAsync($"{BaseUrl}/{hoaDonId}/huy-don", null);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResult>();
                    return result ?? new ApiResult { Success = true, Message = "Hủy đơn hàng thành công!" };
                }
                else
                {
                    try
                    {
                        var errorResult = await response.Content.ReadFromJsonAsync<ApiResult>();
                        return errorResult ?? new ApiResult { Success = false, Message = $"Lỗi: {response.StatusCode}" };
                    }
                    catch
                    {
                        return new ApiResult { Success = false, Message = $"Lỗi khi hủy đơn hàng: {response.StatusCode}" };
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"Timeout error: {ex.Message}");
                return new ApiResult { Success = false, Message = "Yêu cầu bị timeout. Vui lòng thử lại sau." };
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request error: {ex.Message}");
                return new ApiResult { Success = false, Message = "Không thể kết nối đến server. Vui lòng kiểm tra kết nối mạng." };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                return new ApiResult { Success = false, Message = $"Lỗi khi hủy đơn hàng: {ex.Message}" };
            }
        }

        // ✅ Cập nhật trạng thái đơn hàng
        public async Task<ApiResult> CapNhatTrangThaiAsync(Guid hoaDonId, int trangThaiMoi)
        {
            try
            {
                if (hoaDonId == Guid.Empty)
                {
                    return new ApiResult { Success = false, Message = "ID hóa đơn không hợp lệ" };
                }

                var response = await _httpClient.PutAsync($"{BaseUrl}/{hoaDonId}/trang-thai/{trangThaiMoi}", null);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResult>();
                    return result ?? new ApiResult { Success = true, Message = "Cập nhật trạng thái thành công!" };
                }
                else
                {
                    try
                    {
                        var errorResult = await response.Content.ReadFromJsonAsync<ApiResult>();
                        return errorResult ?? new ApiResult { Success = false, Message = $"Lỗi: {response.StatusCode}" };
                    }
                    catch
                    {
                        return new ApiResult { Success = false, Message = $"Lỗi khi cập nhật trạng thái: {response.StatusCode}" };
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"Timeout error: {ex.Message}");
                return new ApiResult { Success = false, Message = "Yêu cầu bị timeout. Vui lòng thử lại sau." };
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request error: {ex.Message}");
                return new ApiResult { Success = false, Message = "Không thể kết nối đến server. Vui lòng kiểm tra kết nối mạng." };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                return new ApiResult { Success = false, Message = $"Lỗi khi cập nhật trạng thái: {ex.Message}" };
            }
        }

        // ✅ Lấy tất cả đơn hàng
        public async Task<IEnumerable<HoaDon>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(BaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<IEnumerable<HoaDon>>();
                    return result ?? new List<HoaDon>();
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Error: {response.StatusCode} - {errorContent}");
                
                throw new Exception($"Lỗi khi lấy danh sách hóa đơn: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                throw new Exception($"Lỗi khi lấy danh sách hóa đơn: {ex.Message}");
            }
        }

        // ✅ Lấy đơn hàng theo ID
        public async Task<HoaDon> GetByIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("ID hóa đơn không hợp lệ");
                }

                var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<HoaDon>();
                    if (result == null)
                    {
                        throw new KeyNotFoundException($"Không tìm thấy hóa đơn với ID: {id}");
                    }
                    return result;
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new KeyNotFoundException($"Không tìm thấy hóa đơn với ID: {id}");
                }
                
                throw new Exception($"Lỗi khi lấy thông tin hóa đơn: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                throw new Exception($"Lỗi khi lấy thông tin hóa đơn: {ex.Message}");
            }
        }
    }
} 