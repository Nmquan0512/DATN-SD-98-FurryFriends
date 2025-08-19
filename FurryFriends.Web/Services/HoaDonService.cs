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

        // ✅ Method mới cho quản lý đơn hàng - chỉ lấy hóa đơn trạng thái 0-5
        public async Task<IEnumerable<HoaDon>> GetDonHangListAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/don-hang");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<IEnumerable<HoaDon>>();
                    return result ?? new List<HoaDon>();
                }
                
                // Log error details
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Error: {response.StatusCode} - {errorContent}");
                
                throw new Exception($"Lỗi khi lấy danh sách đơn hàng: {response.StatusCode}");
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
                throw new Exception($"Lỗi khi lấy danh sách đơn hàng: {ex.Message}");
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
                // ✅ Chỉ tính tổng đơn hàng có trạng thái từ 0-5
                return allOrders.Where(h => h.TrangThai >= 0 && h.TrangThai <= 5).Count();
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
                
                // ✅ Chỉ tính doanh thu từ đơn hàng có trạng thái 3 (Đã giao) và 7 (Đã thanh toán)
                // ✅ Doanh thu = TongTienSauKhiGiam - PhiShip (nếu có)
                var monthlyRevenue = allOrders
                    .Where(h => h.NgayTao.Month == currentMonth && h.NgayTao.Year == currentYear && (h.TrangThai == 3 || h.TrangThai == 7))
                    .Sum(h => {
                        // Tính phí ship nếu có
                        decimal phiShip = 0;
                        if (h.LoaiHoaDon == "GiaoHang" || !string.IsNullOrEmpty(h.DiaChiGiaoHangLucMua))
                        {
                            // Logic freeship: Đơn hàng trên 500k được freeship
                            var tongTienHang = h.TongTien - (h.TongTien - h.TongTienSauKhiGiam); // Tổng tiền sau khi giảm voucher
                            phiShip = tongTienHang >= 500000m ? 0m : 30000m;
                        }
                        return h.TongTienSauKhiGiam - phiShip;
                    });
                
                return monthlyRevenue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMonthlyRevenueAsync: {ex.Message}");
                return 0;
            }
        }

        // ✅ Lấy doanh thu theo tháng
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
                
                // ✅ Tính doanh thu theo từng tháng - chỉ tính từ đơn hàng có trạng thái 3 (Đã giao) và 7 (Đã thanh toán)
                // ✅ Doanh thu = TongTienSauKhiGiam - PhiShip (nếu có)
                foreach (var order in allOrders.Where(h => h.NgayTao.Year == currentYear && (h.TrangThai == 3 || h.TrangThai == 7)))
                {
                    var monthIndex = order.NgayTao.Month - 1; // Month bắt đầu từ 1, index bắt đầu từ 0
                    if (monthIndex >= 0 && monthIndex < 12)
                    {
                        // Tính phí ship nếu có
                        decimal phiShip = 0;
                        if (order.LoaiHoaDon == "GiaoHang" || !string.IsNullOrEmpty(order.DiaChiGiaoHangLucMua))
                        {
                            // Logic freeship: Đơn hàng trên 500k được freeship
                            var tongTienHang = order.TongTien - (order.TongTien - order.TongTienSauKhiGiam); // Tổng tiền sau khi giảm voucher
                            phiShip = tongTienHang >= 500000m ? 0m : 30000m;
                        }
                        values[monthIndex] += order.TongTienSauKhiGiam - phiShip;
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

        // ✅ Lấy doanh thu theo ngày (24 giờ)
        public async Task<List<object>> GetRevenueByDayAsync()
        {
            try
            {
                var allOrders = await GetHoaDonListAsync();
                var currentDate = DateTime.Now.Date;
                
                var dailyData = new List<object>();
                var labels = new string[24];
                var values = new decimal[24];
                
                // Khởi tạo dữ liệu cho 24 giờ
                for (int i = 0; i < 24; i++)
                {
                    labels[i] = $"{i:D2}:00";
                    values[i] = 0;
                }
                
                // ✅ Tính doanh thu theo từng giờ - chỉ tính từ đơn hàng có trạng thái 3 (Đã giao) và 7 (Đã thanh toán)
                foreach (var order in allOrders.Where(h => h.NgayTao.Date == currentDate && (h.TrangThai == 3 || h.TrangThai == 7)))
                {
                    var hour = order.NgayTao.Hour;
                    if (hour >= 0 && hour < 24)
                    {
                        values[hour] += order.TongTienSauKhiGiam;
                    }
                }
                
                dailyData.Add(new { labels = labels });
                dailyData.Add(new { values = values });
                
                return dailyData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetRevenueByDayAsync: {ex.Message}");
                return new List<object>
                {
                    new { labels = new[] { "00:00", "01:00", "02:00", "03:00", "04:00", "05:00", "06:00", "07:00", "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00" } },
                    new { values = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } }
                };
            }
        }

        // ✅ Lấy doanh thu theo tuần (7 ngày)
        public async Task<List<object>> GetRevenueByWeekAsync()
        {
            try
            {
                var allOrders = await GetHoaDonListAsync();
                var currentDate = DateTime.Now.Date;
                var startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek); // Bắt đầu tuần
                
                var weeklyData = new List<object>();
                var labels = new string[7];
                var values = new decimal[7];
                
                // Khởi tạo dữ liệu cho 7 ngày trong tuần
                for (int i = 0; i < 7; i++)
                {
                    labels[i] = startOfWeek.AddDays(i).ToString("dd/MM");
                    values[i] = 0;
                }
                
                // ✅ Tính doanh thu theo từng ngày trong tuần - chỉ tính từ đơn hàng có trạng thái 3 (Đã giao) và 7 (Đã thanh toán)
                foreach (var order in allOrders.Where(h => h.NgayTao.Date >= startOfWeek && h.NgayTao.Date < startOfWeek.AddDays(7) && (h.TrangThai == 3 || h.TrangThai == 7)))
                {
                    var dayOfWeek = (int)order.NgayTao.DayOfWeek;
                    if (dayOfWeek >= 0 && dayOfWeek < 7)
                    {
                        values[dayOfWeek] += order.TongTienSauKhiGiam;
                    }
                }
                
                weeklyData.Add(new { labels = labels });
                weeklyData.Add(new { values = values });
                
                return weeklyData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetRevenueByWeekAsync: {ex.Message}");
                return new List<object>
                {
                    new { labels = new[] { "CN", "T2", "T3", "T4", "T5", "T6", "T7" } },
                    new { values = new[] { 0, 0, 0, 0, 0, 0, 0 } }
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
                    { 4, 0 }, // Đã hủy
                    { 5, 0 }  // Đã thanh toán
                };
                
                // ✅ Đếm số lượng đơn hàng theo từng trạng thái - chỉ đếm trạng thái từ 0-5
                foreach (var order in allOrders.Where(h => h.TrangThai >= 0 && h.TrangThai <= 5))
                {
                    if (statusCounts.ContainsKey(order.TrangThai))
                    {
                        statusCounts[order.TrangThai]++;
                    }
                }
                
                var labels = new[] { "Chờ duyệt", "Đã duyệt", "Đang giao", "Đã giao", "Đã hủy", "Đã thanh toán" };
                var values = new[] { 
                    statusCounts[0], 
                    statusCounts[1], 
                    statusCounts[2], 
                    statusCounts[3], 
                    statusCounts[4],
                    statusCounts[5]
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
                    new { labels = new[] { "Chờ duyệt", "Đã duyệt", "Đang giao", "Đã giao", "Đã hủy", "Đã thanh toán" } },
                    new { values = new[] { 0, 0, 0, 0, 0, 0 } }
                };
            }
        }

        public async Task<List<object>> GetRecentOrdersAsync(int count)
        {
            try
            {
                var allOrders = await GetHoaDonListAsync();
                // ✅ Chỉ lấy đơn hàng có trạng thái từ 0-5
                var recentOrders = allOrders
                    .Where(h => h.TrangThai >= 0 && h.TrangThai <= 5)
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
                5 => "Đã thanh toán",
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

                // ✅ Debug logging
                Console.WriteLine($"🔍 Debug - CapNhatTrangThaiAsync Response Status: {response.StatusCode}");
                Console.WriteLine($"🔍 Debug - CapNhatTrangThaiAsync Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        // ✅ Thử parse theo format API trả về: { success = true, message = "..." }
                        var jsonResult = await response.Content.ReadFromJsonAsync<object>();
                        Console.WriteLine($"🔍 Debug - Raw JSON result: {jsonResult}");
                        
                        // Nếu response có success = true, coi như thành công
                        if (jsonResult != null)
                        {
                            var jsonString = jsonResult.ToString();
                            if (jsonString.Contains("\"success\":true") || jsonString.Contains("success") && jsonString.Contains("true"))
                            {
                                Console.WriteLine($"🔍 Debug - Success detected from JSON");
                                return new ApiResult { Success = true, Message = "Cập nhật trạng thái thành công!" };
                            }
                        }
                        
                        // Thử parse ApiResult
                        var result = await response.Content.ReadFromJsonAsync<ApiResult>();
                        Console.WriteLine($"🔍 Debug - Parsed ApiResult: Success={result?.Success}, Message={result?.Message}");
                        return result ?? new ApiResult { Success = true, Message = "Cập nhật trạng thái thành công!" };
                    }
                    catch (Exception parseEx)
                    {
                        Console.WriteLine($"🔍 Debug - Parse error: {parseEx.Message}");
                        // Nếu không parse được, coi như thành công vì status code là 200
                        return new ApiResult { Success = true, Message = "Cập nhật trạng thái thành công!" };
                    }
                }
                else
                {
                    try
                    {
                        var errorResult = await response.Content.ReadFromJsonAsync<ApiResult>();
                        Console.WriteLine($"🔍 Debug - Error ApiResult: Success={errorResult?.Success}, Message={errorResult?.Message}");
                        return errorResult ?? new ApiResult { Success = false, Message = $"Lỗi: {response.StatusCode}" };
                    }
                    catch
                    {
                        Console.WriteLine($"🔍 Debug - Error parse failed, returning generic error");
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

        // ✅ Lấy chi tiết hóa đơn
        public async Task<IEnumerable<HoaDonChiTiet>> GetChiTietHoaDonAsync(Guid hoaDonId)
        {
            try
            {
                if (hoaDonId == Guid.Empty)
                {
                    throw new ArgumentException("ID hóa đơn không hợp lệ");
                }

                var response = await _httpClient.GetAsync($"{BaseUrl}/{hoaDonId}/chi-tiet");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<IEnumerable<HoaDonChiTiet>>();
                    return result ?? new List<HoaDonChiTiet>();
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new List<HoaDonChiTiet>();
                }
                
                throw new Exception($"Lỗi khi lấy chi tiết hóa đơn: {response.StatusCode}");
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
                throw new Exception($"Lỗi khi lấy chi tiết hóa đơn: {ex.Message}");
            }
        }
    }
} 