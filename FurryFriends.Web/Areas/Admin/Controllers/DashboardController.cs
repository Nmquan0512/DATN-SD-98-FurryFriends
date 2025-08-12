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
                var totalProducts = await _sanPhamService.GetTotalProductsAsync();
                var totalEmployees = await _nhanVienService.GetTotalEmployeesAsync();
                
                // Cập nhật tất cả dữ liệu thành thật
                ViewBag.TotalOrders = totalOrders;
                ViewBag.MonthlyRevenue = monthlyRevenue;
                ViewBag.RevenueByMonth = revenueByMonth;
                ViewBag.OrdersByStatus = ordersByStatus;
                ViewBag.RecentOrders = recentOrders;
                ViewBag.TotalCustomers = totalCustomers;
                ViewBag.TotalProducts = totalProducts;
                ViewBag.TotalEmployees = totalEmployees;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
                
                // Fallback data nếu có lỗi
                ViewBag.TotalCustomers = 0;
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
                
                // Tính doanh thu theo từng quý
                foreach (var order in allOrders.Where(h => h.NgayTao.Year == currentYear))
                {
                    var quarter = (order.NgayTao.Month - 1) / 3; // Tính quý (0-3)
                    if (quarter >= 0 && quarter < 4)
                    {
                        values[quarter] += order.TongTienSauKhiGiam;
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
                
                // Tính doanh thu theo từng năm
                foreach (var order in allOrders)
                {
                    var yearIndex = order.NgayTao.Year - (currentYear - 4);
                    if (yearIndex >= 0 && yearIndex < 5)
                    {
                        values[yearIndex] += order.TongTienSauKhiGiam;
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