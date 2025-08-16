using FurryFriends.API.Models.DTO.BanHang;
using FurryFriends.API.Models.DTO.BanHang.Requests;
using FurryFriends.Web.Services; // Nơi chứa ApiException
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BanHangController : Controller
    {
        private readonly IBanHangService _banHangService;
        private readonly ILogger<BanHangController> _logger;

        public BanHangController(IBanHangService banHangService, ILogger<BanHangController> logger)
        {
            _banHangService = banHangService;
            _logger = logger;
        }

        #region Actions trả về View (Tải trang)

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var model = await _banHangService.GetAllHoaDonsAsync();
                return View(model);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Lỗi API khi lấy danh sách hóa đơn.");
                TempData["error"] = "Không thể tải lịch sử hóa đơn. Vui lòng thử lại.";
                return View(new List<HoaDonBanHangDto>());
            }
        }

        [HttpGet]
        public IActionResult TaoHoaDonMoi()
        {
            // Có thể truyền ViewBag.HinhThucThanhToanList ở đây nếu cần
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var hoaDon = await _banHangService.GetHoaDonByIdAsync(id);
                if (hoaDon == null)
                {
                    TempData["error"] = "Không tìm thấy hóa đơn.";
                    return RedirectToAction(nameof(Index));
                }

                // ViewBag.HinhThucThanhToanList = await _htttService.GetAllAsync();

                if (hoaDon.TrangThai == "Chua Thanh Toan")
                {
                    return View("Details_Interactive", hoaDon);
                }

                return View("Details_ReadOnly", hoaDon);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, $"Lỗi API khi xem chi tiết hóa đơn {id}.");
                TempData["error"] = "Lỗi khi tải dữ liệu chi tiết hóa đơn.";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Actions xử lý AJAX (Trả về JSON hoặc Partial View)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInitialInvoice()
        {
            try
            {
                var request = new TaoHoaDonRequest { LaKhachLe = true, GhiChu = "Hóa đơn tại quầy" };
                var result = await _banHangService.TaoHoaDonAsync(request);
                return Json(new { success = true, data = result });
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Lỗi API khi tạo hóa đơn chờ ban đầu.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("Admin/BanHang/HoaDon/{hoaDonId}/ThemSanPham")]
        public async Task<IActionResult> ThemSanPhamVaoHoaDon(Guid hoaDonId, [FromBody] ThemSanPhamRequest request)
        {
            try { var result = await _banHangService.ThemSanPhamVaoHoaDonAsync(hoaDonId, request); return Json(new { success = true, data = result }); }
            catch (ApiException ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpPut("Admin/BanHang/HoaDon/{hoaDonId}/SanPham/{sanPhamChiTietId}")]
        public async Task<IActionResult> CapNhatSoLuong(Guid hoaDonId, Guid sanPhamChiTietId, [FromBody] CapNhatSoLuongRequest request)
        {
            try { var result = await _banHangService.CapNhatSoLuongSanPhamAsync(hoaDonId, sanPhamChiTietId, request); return Json(new { success = true, data = result }); }
            catch (ApiException ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpDelete("Admin/BanHang/HoaDon/{hoaDonId}/SanPham/{sanPhamChiTietId}")]
        public async Task<IActionResult> XoaSanPhamKhoiHoaDon(Guid hoaDonId, Guid sanPhamChiTietId)
        {
            try { var result = await _banHangService.XoaSanPhamKhoiHoaDonAsync(hoaDonId, sanPhamChiTietId); return Json(new { success = true, data = result }); }
            catch (ApiException ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpPut("Admin/BanHang/HoaDon/{hoaDonId}/GanKhachHang")]
        public async Task<IActionResult> GanKhachHang(Guid hoaDonId, [FromBody] GanKhachHangRequest request)
        {
            try { var result = await _banHangService.GanKhachHangAsync(hoaDonId, request); return Json(new { success = true, data = result }); }
            catch (ApiException ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpPut("Admin/BanHang/HoaDon/{hoaDonId}/AssignKhachLe")]
        public async Task<IActionResult> AssignKhachLe(Guid hoaDonId)
        {
            try
            {
                // Giả sử service của bạn có một phương thức chuyên dụng để gán khách lẻ
                var result = await _banHangService.GanKhachLeAsync(hoaDonId);
                return Json(new { success = true, data = result });
            }
            catch (ApiException ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpPost("Admin/BanHang/HoaDon/{hoaDonId}/ApDungVoucher")]
        public async Task<IActionResult> ApDungVoucher(Guid hoaDonId, [FromBody] ApDungVoucherRequest request)
        {
            try { var result = await _banHangService.ApDungVoucherAsync(hoaDonId, request); return Json(new { success = true, data = result }); }
            catch (ApiException ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpDelete("Admin/BanHang/HoaDon/{hoaDonId}/GoBoVoucher")]
        public async Task<IActionResult> GoBoVoucher(Guid hoaDonId)
        {
            try { var result = await _banHangService.GoBoVoucherAsync(hoaDonId); return Json(new { success = true, data = result }); }
            catch (ApiException ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpPost("Admin/BanHang/HoaDon/{hoaDonId}/ThanhToan")]
        public async Task<IActionResult> ThanhToan(Guid hoaDonId, [FromBody] ThanhToanRequest request)
        {
            try
            {
                var result = await _banHangService.ThanhToanHoaDonAsync(hoaDonId, request);
                TempData["success"] = "Thanh toán hóa đơn thành công!";
                return Json(new { success = true, data = result, redirectUrl = Url.Action("Index") });
            }
            catch (ApiException ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpPost("Admin/BanHang/HoaDon/{hoaDonId}/Huy")]
        public async Task<IActionResult> HuyHoaDonAjax(Guid hoaDonId)
        {
            try
            {
                await _banHangService.HuyHoaDonAsync(hoaDonId);
                TempData["success"] = "Đã hủy hóa đơn thành công.";
                return Json(new { success = true, redirectUrl = Url.Action("Index") });
            }
            catch (ApiException ex) { return Json(new { success = false, message = ex.Message }); }
        }

        // --- Các Action trả về Partial View ---

        [HttpGet]
        public async Task<IActionResult> TimKiemSanPham(string keyword)
        {
            var results = await _banHangService.TimKiemSanPhamAsync(keyword);
            return PartialView("_TimKiemSanPhamKetQua", results);
        }

        [HttpGet]
        public async Task<IActionResult> LaySanPhamGoiY()
        {
            var suggestedProducts = await _banHangService.GetSuggestedProductsAsync();
            return PartialView("_TimKiemSanPhamKetQua", suggestedProducts);
        }

        [HttpGet]
        public async Task<IActionResult> TimKiemKhachHang(string keyword)
        {
            var results = await _banHangService.TimKiemKhachHangAsync(keyword);
            return PartialView("_KhachHangSearchResults", results);
        }

        // --- Action tạo mới ---

        [HttpPost]
        public async Task<IActionResult> TaoKhachHangMoi([FromBody] TaoKhachHangRequest request)
        {
            try
            {
                var result = await _banHangService.TaoKhachHangMoiAsync(request);
                return Json(new { success = true, data = result });
            }
            catch (ApiException ex) { return Json(new { success = false, message = ex.Message }); }
        }
        #endregion
    }
}