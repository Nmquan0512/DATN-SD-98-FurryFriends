using FurryFriends.Web.Filter;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdminOnly]
    public class DashboardController : Controller
    {
        private readonly IHoaDonService _hoaDonService;
        private readonly IKhachHangService _khachHangService;
        private readonly ISanPhamService _sanPhamService;
        private readonly INhanVienService _nhanVienService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IHoaDonService hoaDonService,
            IKhachHangService khachHangService,
            ISanPhamService sanPhamService,
            INhanVienService nhanVienService,
            ILogger<DashboardController> logger)
        {
            _hoaDonService = hoaDonService;
            _khachHangService = khachHangService;
            _sanPhamService = sanPhamService;
            _nhanVienService = nhanVienService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Lấy dữ liệu thật từ tất cả services
                var totalOrders = await _hoaDonService.GetTotalOrdersAsync();
                var monthlyRevenue = await _hoaDonService.GetMonthlyRevenueAsync();
                var revenueByMonth = await _hoaDonService.GetRevenueByMonthAsync();
                var ordersByStatus = await _hoaDonService.GetOrdersByStatusAsync();
                var recentOrders = await _hoaDonService.GetRecentOrdersAsync(10);
                
                // Lấy dữ liệu thật cho khách hàng, sản phẩm, nhân viên
                var totalCustomers = await _khachHangService.GetTotalCustomersAsync();
                var activeCustomers = await _khachHangService.GetActiveCustomersAsync();
                var inactiveCustomers = await _khachHangService.GetInactiveCustomersAsync();
                var totalProducts = await _sanPhamService.GetTotalProductsAsync();
                var totalEmployees = await _nhanVienService.GetTotalEmployeesAsync();
                
                // Cập nhật tất cả dữ liệu thành thật
                ViewBag.TotalOrders = totalOrders;
                ViewBag.MonthlyRevenue = monthlyRevenue;
                ViewBag.RevenueByMonth = revenueByMonth;
                ViewBag.OrdersByStatus = ordersByStatus;
                ViewBag.RecentOrders = recentOrders;
                ViewBag.TotalCustomers = totalCustomers;
                ViewBag.ActiveCustomers = activeCustomers;
                ViewBag.InactiveCustomers = inactiveCustomers;
                ViewBag.TotalProducts = totalProducts;
                ViewBag.TotalEmployees = totalEmployees;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
                
                // Fallback data nếu có lỗi
                ViewBag.TotalCustomers = 0;
                ViewBag.ActiveCustomers = 0;
                ViewBag.InactiveCustomers = 0;
                ViewBag.TotalProducts = 0;
                ViewBag.TotalEmployees = 0;
                ViewBag.TotalOrders = 0;
                ViewBag.MonthlyRevenue = 0;
                ViewBag.RevenueByMonth = new List<object>();
                ViewBag.OrdersByStatus = new List<object>();
                ViewBag.RecentOrders = new List<object>();
                
                return View();
            }
        }

        // API endpoint để lấy dữ liệu dashboard (cho AJAX)
        [HttpGet]
        public async Task<IActionResult> GetChartData(string period = "month")
        {
            try
            {
                List<object> revenueData;
                
                // Lấy dữ liệu doanh thu theo khoảng thời gian được chọn
                switch (period.ToLower())
                {
                    case "day":
                        revenueData = await GetRevenueByDayAsync();
                        break;
                    case "week":
                        revenueData = await GetRevenueByWeekAsync();
                        break;
                    case "quarter":
                        revenueData = await GetRevenueByQuarterAsync();
                        break;
                    case "year":
                        revenueData = await GetRevenueByYearAsync();
                        break;
                    default: // month
                        revenueData = await _hoaDonService.GetRevenueByMonthAsync();
                        break;
                }
                
                // Dữ liệu cho biểu đồ đơn hàng theo trạng thái
                var orderStatusData = await _hoaDonService.GetOrdersByStatusAsync();
                
                // Dữ liệu cho biểu đồ sản phẩm bán chạy (giữ nguyên mock data)
                var topSellingData = new List<object>
                {
                    new { name = "Po", sales = 150, revenue = 15000000 },
                    new { name = "Bóng tennis", sales = 120, revenue = 12000000 },
                    new { name = "Dây xích", sales = 100, revenue = 10000000 },
                    new { name = "Vòng cổ", sales = 80, revenue = 8000000 },
                    new { name = "Đồ chơi gặm", sales = 60, revenue = 6000000 }
                };

                return Json(new
                {
                    success = true,
                    revenueData,
                    orderStatusData,
                    topSellingData,
                    period = period
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chart data for period: {Period}", period);
                return Json(new { success = false, message = "Có lỗi xảy ra khi tải dữ liệu biểu đồ" });
            }
        }

        // Lấy doanh thu theo ngày
        private async Task<List<object>> GetRevenueByDayAsync()
        {
            try
            {
                var allOrders = await _hoaDonService.GetHoaDonListAsync();
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
                // ✅ Doanh thu = TongTienSauKhiGiam - PhiShip (nếu có)
                foreach (var order in allOrders.Where(h => h.NgayTao.Date == currentDate && (h.TrangThai == 3 || h.TrangThai == 7)))
                {
                    var hour = order.NgayTao.Hour;
                    if (hour >= 0 && hour < 24)
                    {
                        // Tính phí ship nếu có
                        decimal phiShip = 0;
                        if (order.LoaiHoaDon == "GiaoHang" || !string.IsNullOrEmpty(order.DiaChiGiaoHangLucMua))
                        {
                            // Logic freeship: Đơn hàng trên 500k được freeship
                            var tongTienHang = order.TongTien - (order.TongTien - order.TongTienSauKhiGiam); // Tổng tiền sau khi giảm voucher
                            phiShip = tongTienHang >= 500000m ? 0m : 30000m;
                        }
                        values[hour] += order.TongTienSauKhiGiam - phiShip;
                    }
                }
                
                dailyData.Add(new { labels = labels });
                dailyData.Add(new { values = values });
                
                return dailyData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily revenue data");
                return new List<object>
                {
                    new { labels = new[] { "00:00", "01:00", "02:00", "03:00", "04:00", "05:00", "06:00", "07:00", "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00" } },
                    new { values = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } }
                };
            }
        }

        // Lấy doanh thu theo tuần
        private async Task<List<object>> GetRevenueByWeekAsync()
        {
            try
            {
                var allOrders = await _hoaDonService.GetHoaDonListAsync();
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
                // ✅ Doanh thu = TongTienSauKhiGiam - PhiShip (nếu có)
                foreach (var order in allOrders.Where(h => h.NgayTao.Date >= startOfWeek && h.NgayTao.Date < startOfWeek.AddDays(7) && (h.TrangThai == 3 || h.TrangThai == 7)))
                {
                    var dayOfWeek = (int)order.NgayTao.DayOfWeek;
                    if (dayOfWeek >= 0 && dayOfWeek < 7)
                    {
                        // Tính phí ship nếu có
                        decimal phiShip = 0;
                        if (order.LoaiHoaDon == "GiaoHang" || !string.IsNullOrEmpty(order.DiaChiGiaoHangLucMua))
                        {
                            // Logic freeship: Đơn hàng trên 500k được freeship
                            var tongTienHang = order.TongTien - (order.TongTien - order.TongTienSauKhiGiam); // Tổng tiền sau khi giảm voucher
                            phiShip = tongTienHang >= 500000m ? 0m : 30000m;
                        }
                        values[dayOfWeek] += order.TongTienSauKhiGiam - phiShip;
                    }
                }
                
                weeklyData.Add(new { labels = labels });
                weeklyData.Add(new { values = values });
                
                return weeklyData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting weekly revenue data");
                return new List<object>
                {
                    new { labels = new[] { "CN", "T2", "T3", "T4", "T5", "T6", "T7" } },
                    new { values = new[] { 0, 0, 0, 0, 0, 0, 0 } }
                };
            }
        }

        // Lấy doanh thu theo quý
        private async Task<List<object>> GetRevenueByQuarterAsync()
        {
            try
            {
                var allOrders = await _hoaDonService.GetHoaDonListAsync();
                var currentYear = DateTime.Now.Year;
                
                var quarterlyData = new List<object>();
                var labels = new string[4];
                var values = new decimal[4];
                
                // Khởi tạo dữ liệu cho 4 quý
                for (int i = 0; i < 4; i++)
                {
                    labels[i] = $"Q{i + 1}";
                    values[i] = 0;
                }
                
                // ✅ Tính doanh thu theo từng quý - chỉ tính từ đơn hàng có trạng thái 3 (Đã giao) và 7 (Đã thanh toán)
                // ✅ Doanh thu = TongTienSauKhiGiam - PhiShip (nếu có)
                foreach (var order in allOrders.Where(h => h.NgayTao.Year == currentYear && (h.TrangThai == 3 || h.TrangThai == 7)))
                {
                    var quarter = (order.NgayTao.Month - 1) / 3; // Tính quý (0-3)
                    if (quarter >= 0 && quarter < 4)
                    {
                        // Tính phí ship nếu có
                        decimal phiShip = 0;
                        if (order.LoaiHoaDon == "GiaoHang" || !string.IsNullOrEmpty(order.DiaChiGiaoHangLucMua))
                        {
                            // Logic freeship: Đơn hàng trên 500k được freeship
                            var tongTienHang = order.TongTien - (order.TongTien - order.TongTienSauKhiGiam); // Tổng tiền sau khi giảm voucher
                            phiShip = tongTienHang >= 500000m ? 0m : 30000m;
                        }
                        values[quarter] += order.TongTienSauKhiGiam - phiShip;
                    }
                }
                
                quarterlyData.Add(new { labels = labels });
                quarterlyData.Add(new { values = values });
                
                return quarterlyData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quarterly revenue data");
                return new List<object>
                {
                    new { labels = new[] { "Q1", "Q2", "Q3", "Q4" } },
                    new { values = new[] { 0, 0, 0, 0 } }
                };
            }
        }

        // Lấy doanh thu theo năm (5 năm gần nhất)
        private async Task<List<object>> GetRevenueByYearAsync()
        {
            var currentYear = DateTime.Now.Year; // Khai báo ở đầu method
            
            try
            {
                var allOrders = await _hoaDonService.GetHoaDonListAsync();
                
                var yearlyData = new List<object>();
                var labels = new string[5];
                var values = new decimal[5];
                
                // Khởi tạo dữ liệu cho 5 năm gần nhất
                for (int i = 0; i < 5; i++)
                {
                    var year = currentYear - 4 + i;
                    labels[i] = year.ToString();
                    values[i] = 0;
                }
                
                // ✅ Tính doanh thu theo từng năm - chỉ tính từ đơn hàng có trạng thái 3 (Đã giao) và 7 (Đã thanh toán)
                // ✅ Doanh thu = TongTienSauKhiGiam - PhiShip (nếu có)
                foreach (var order in allOrders.Where(h => h.TrangThai == 3 || h.TrangThai == 7))
                {
                    var yearIndex = order.NgayTao.Year - (currentYear - 4);
                    if (yearIndex >= 0 && yearIndex < 5)
                    {
                        // Tính phí ship nếu có
                        decimal phiShip = 0;
                        if (order.LoaiHoaDon == "GiaoHang" || !string.IsNullOrEmpty(order.DiaChiGiaoHangLucMua))
                        {
                            // Logic freeship: Đơn hàng trên 500k được freeship
                            var tongTienHang = order.TongTien - (order.TongTien - order.TongTienSauKhiGiam); // Tổng tiền sau khi giảm voucher
                            phiShip = tongTienHang >= 500000m ? 0m : 30000m;
                        }
                        values[yearIndex] += order.TongTienSauKhiGiam - phiShip;
                    }
                }
                
                yearlyData.Add(new { labels = labels });
                yearlyData.Add(new { values = values });
                
                return yearlyData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting yearly revenue data");
                return new List<object>
                {
                    new { labels = new[] { (currentYear - 4).ToString(), (currentYear - 3).ToString(), (currentYear - 2).ToString(), (currentYear - 1).ToString(), currentYear.ToString() } },
                    new { values = new[] { 0, 0, 0, 0, 0 } }
                };
            }
        }
    }
}