using Microsoft.AspNetCore.Mvc;
using FurryFriends.Web.Services.IService;
using System.Threading.Tasks;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly IKhachHangService _khachHangService;
        private readonly IHoaDonService _hoaDonService;
        private readonly ISanPhamService _sanPhamService;
        private readonly INhanVienService _nhanVienService;

        public DashboardController(
            IKhachHangService khachHangService,
            IHoaDonService hoaDonService,
            ISanPhamService sanPhamService,
            INhanVienService nhanVienService)
        {
            _khachHangService = khachHangService;
            _hoaDonService = hoaDonService;
            _sanPhamService = sanPhamService;
            _nhanVienService = nhanVienService;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy dữ liệu thống kê
            var totalCustomers = await _khachHangService.GetTotalCustomersAsync();
            var totalOrders = await _hoaDonService.GetTotalOrdersAsync();
            var totalProducts = await _sanPhamService.GetTotalProductsAsync();
            var totalEmployees = await _nhanVienService.GetTotalEmployeesAsync();
            var monthlyRevenue = await _hoaDonService.GetMonthlyRevenueAsync();
            var topProducts = await _sanPhamService.GetTopSellingProductsAsync(5);
            var recentOrders = await _hoaDonService.GetRecentOrdersAsync(10);

            ViewBag.TotalCustomers = totalCustomers;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalEmployees = totalEmployees;
            ViewBag.MonthlyRevenue = monthlyRevenue;
            ViewBag.TopProducts = topProducts;
            ViewBag.RecentOrders = recentOrders;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetChartData()
        {
            // Dữ liệu cho biểu đồ doanh thu theo tháng
            var revenueData = await _hoaDonService.GetRevenueByMonthAsync();
            
            // Dữ liệu cho biểu đồ đơn hàng theo trạng thái
            var orderStatusData = await _hoaDonService.GetOrdersByStatusAsync();
            
            // Dữ liệu cho biểu đồ sản phẩm bán chạy
            var topSellingData = await _sanPhamService.GetTopSellingProductsAsync(10);

            return Json(new
            {
                revenueData,
                orderStatusData,
                topSellingData
            });
        }
    }
} 