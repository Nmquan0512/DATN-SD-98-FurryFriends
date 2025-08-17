using FurryFriends.API.Models.DTO.BanHang;
using FurryFriends.API.Models.DTO.BanHang.Requests;
using FurryFriends.Web.Services;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/BanHang/[action]")] // Định tuyến tập trung, dễ gọi từ JS
    public class BanHangController : Controller
    {
        private readonly IBanHangService _banHangService;
        private readonly ILogger<BanHangController> _logger;

        public BanHangController(IBanHangService banHangService, ILogger<BanHangController> logger)
        {
            _banHangService = banHangService;
            _logger = logger;
        }

        #region Actions trả về View (Giữ nguyên)
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
                TempData["error"] = "Không thể tải lịch sử hóa đơn: " + ex.Message;
                return View(new List<HoaDonBanHangDto>());
            }
        }

        [HttpGet]
        public IActionResult TaoHoaDonMoi()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var hoaDon = await _banHangService.GetHoaDonByIdAsync(id);
                if (hoaDon.TrangThai == "Chua Thanh Toan") return View("Details_Interactive", hoaDon);
                return View("Details_ReadOnly", hoaDon);
            }
            catch (ApiException ex)
            {
                TempData["error"] = "Lỗi khi tải chi tiết hóa đơn: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion

        #region Actions xử lý Form POST (Giữ nguyên)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoHoaDonCho()
        {
            try
            {
                var request = new TaoHoaDonRequest { LaKhachLe = true, GhiChu = "Hóa đơn tại quầy" };
                var result = await _banHangService.TaoHoaDonAsync(request);
                TempData["success"] = "Đã tạo hóa đơn chờ mới.";
                return RedirectToAction(nameof(Details), new { id = result.HoaDonId });
            }
            catch (ApiException ex)
            {
                TempData["error"] = $"Tạo hóa đơn thất bại: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion

        #region === TOÀN BỘ PHẦN AJAX ĐƯỢC LÀM LẠI HOÀN TOÀN ===

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInitialInvoice()
        {
            try
            {
                var request = new TaoHoaDonRequest { LaKhachLe = true };
                var result = await _banHangService.TaoHoaDonAsync(request);
                return Json(new { success = true, data = result });
            }
            catch (ApiException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{hoaDonId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemSanPham([FromBody] ThemSanPhamRequest request, Guid hoaDonId)
        {
            try { var result = await _banHangService.ThemSanPhamVaoHoaDonAsync(hoaDonId, request); return Json(new { success = true, data = result }); }
            catch (ApiException ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpPut("{hoaDonId}/{sanPhamChiTietId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatSoLuong([FromBody] CapNhatSoLuongRequest request, Guid hoaDonId, Guid sanPhamChiTietId)
        {
            try { var result = await _banHangService.CapNhatSoLuongSanPhamAsync(hoaDonId, sanPhamChiTietId, request); return Json(new { success = true, data = result }); }
            catch (ApiException ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpDelete("{hoaDonId}/{sanPhamChiTietId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaSanPham(Guid hoaDonId, Guid sanPhamChiTietId)
        {
            try { var result = await _banHangService.XoaSanPhamKhoiHoaDonAsync(hoaDonId, sanPhamChiTietId); return Json(new { success = true, data = result }); }
            catch (ApiException ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpPost("{hoaDonId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GanKhachHang([FromBody] GanKhachHangRequest request, Guid hoaDonId)
        {
            try { var result = await _banHangService.GanKhachHangAsync(hoaDonId, request); return Json(new { success = true, data = result }); }
            catch (ApiException ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpPost("{hoaDonId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GanKhachLe(Guid hoaDonId)
        {
            try
            {
                var khachLeList = await _banHangService.TimKiemKhachHangAsync("Khách lẻ");
                var khachLe = khachLeList.FirstOrDefault(k => k.TenKhachHang == "Khách lẻ");

                if (khachLe == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy tài khoản 'Khách lẻ' mặc định." });
                }

                var request = new GanKhachHangRequest { KhachHangId = khachLe.KhachHangId };
                var result = await _banHangService.GanKhachHangAsync(hoaDonId, request);
                return Json(new { success = true, data = result });
            }
            catch (ApiException ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpPost("{hoaDonId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApDungVoucher([FromBody] ApDungVoucherRequest request, Guid hoaDonId)
        {
            try { var result = await _banHangService.ApDungVoucherAsync(hoaDonId, request); return Json(new { success = true, data = result }); }
            catch (ApiException ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpDelete("{hoaDonId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GoBoVoucher(Guid hoaDonId)
        {
            try { var result = await _banHangService.GoBoVoucherAsync(hoaDonId); return Json(new { success = true, data = result }); }
            catch (ApiException ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpPost("{hoaDonId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThanhToan([FromBody] ThanhToanRequest request, Guid hoaDonId)
        {
            if (!ModelState.IsValid) return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
            try
            {
                var result = await _banHangService.ThanhToanHoaDonAsync(hoaDonId, request);
                TempData["success"] = "Thanh toán hóa đơn thành công!";
                return Json(new { success = true, data = result, redirectUrl = Url.Action("Index") });
            }
            catch (ApiException ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoKhachHangMoi([FromBody] TaoKhachHangRequest request)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault();
                return Json(new { success = false, message = error?.ErrorMessage ?? "Dữ liệu không hợp lệ." });
            }
            try { var result = await _banHangService.TaoKhachHangMoiAsync(request); return Json(new { success = true, data = result }); }
            catch (ApiException ex) { return Json(new { success = false, message = ex.Message }); }
        }

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
        #endregion
    }
}