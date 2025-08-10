using FurryFriends.API.Models.DTO.BanHang;
using FurryFriends.API.Models.DTO.BanHang.Requests;
using FurryFriends.Web.Filter;
using FurryFriends.Web.Services; // Nơi chứa ApiException
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdminOnly]
    public class BanHangController : Controller
    {
        private readonly IBanHangService _banHangService;
        private readonly ILogger<BanHangController> _logger;

        public BanHangController(IBanHangService banHangService, ILogger<BanHangController> logger)
        {
            _banHangService = banHangService;
            _logger = logger;
        }

        #region Page-Level Actions (Tải trang chính)

        /// <summary>
        /// Hiển thị danh sách lịch sử hóa đơn và các hóa đơn đang chờ.
        /// </summary>
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

        /// <summary>
        /// Hiển thị giao diện bán hàng chi tiết cho một hóa đơn.
        /// </summary>
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
                return View(hoaDon);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, $"Lỗi API khi xem chi tiết hóa đơn {id}.");
                TempData["error"] = "Lỗi khi tải dữ liệu chi tiết hóa đơn.";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Form Post Actions (Các hành động lớn, cuối cùng)

        /// <summary>
        /// Tạo một hóa đơn mới và chuyển hướng đến trang chi tiết.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoHoaDonMoi()
        {
            try
            {
                var request = new TaoHoaDonRequest { LaKhachLe = true };
                var result = await _banHangService.TaoHoaDonAsync(request);
                TempData["success"] = "Đã tạo hóa đơn chờ mới.";
                return RedirectToAction(nameof(Details), new { id = result.HoaDonId });
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Lỗi API khi tạo hóa đơn mới.");
                TempData["error"] = $"Tạo hóa đơn thất bại: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Hủy một hóa đơn đang chờ.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyHoaDon(Guid hoaDonId)
        {
            try
            {
                await _banHangService.HuyHoaDonAsync(hoaDonId);
                TempData["success"] = "Đã hủy hóa đơn thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, $"Lỗi API khi hủy hóa đơn {hoaDonId}.");
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id = hoaDonId });
            }
        }

        /// <summary>
        /// Xử lý thanh toán cuối cùng cho hóa đơn.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThanhToan(Guid hoaDonId, ThanhToanRequest request)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Thông tin thanh toán không hợp lệ.";
                return RedirectToAction(nameof(Details), new { id = hoaDonId });
            }
            try
            {
                var result = await _banHangService.ThanhToanHoaDonAsync(hoaDonId, request);
                TempData["success"] = "Thanh toán hóa đơn thành công!";
                return RedirectToAction(nameof(Details), new { id = result.HoaDonId });
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, $"Lỗi API khi thanh toán hóa đơn {hoaDonId}.");
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id = hoaDonId });
            }
        }

        #endregion

        #region AJAX Actions (Các hành động tương tác, trả về JSON)

        [HttpPost]
        public async Task<IActionResult> ThemSanPhamVaoHoaDon([FromBody] ThemSanPhamRequest request, [FromRoute] Guid hoaDonId)
        {
            try
            {
                var result = await _banHangService.ThemSanPhamVaoHoaDonAsync(hoaDonId, request);
                return Json(new { success = true, data = result });
            }
            catch (ApiException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> CapNhatSoLuong([FromBody] CapNhatSoLuongRequest request, [FromRoute] Guid hoaDonId, [FromRoute] Guid sanPhamChiTietId)
        {
            try
            {
                var result = await _banHangService.CapNhatSoLuongSanPhamAsync(hoaDonId, sanPhamChiTietId, request);
                return Json(new { success = true, data = result });
            }
            catch (ApiException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> XoaSanPhamKhoiHoaDon([FromRoute] Guid hoaDonId, [FromRoute] Guid sanPhamChiTietId)
        {
            try
            {
                var result = await _banHangService.XoaSanPhamKhoiHoaDonAsync(hoaDonId, sanPhamChiTietId);
                return Json(new { success = true, data = result });
            }
            catch (ApiException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApDungVoucher([FromBody] ApDungVoucherRequest request, [FromRoute] Guid hoaDonId)
        {
            try
            {
                var result = await _banHangService.ApDungVoucherAsync(hoaDonId, request);
                return Json(new { success = true, data = result });
            }
            catch (ApiException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> GoBoVoucher([FromRoute] Guid hoaDonId)
        {
            try
            {
                var result = await _banHangService.GoBoVoucherAsync(hoaDonId);
                return Json(new { success = true, data = result });
            }
            catch (ApiException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GanKhachHang([FromBody] GanKhachHangRequest request, [FromRoute] Guid hoaDonId)
        {
            try
            {
                var result = await _banHangService.GanKhachHangAsync(hoaDonId, request);
                return Json(new { success = true, data = result });
            }
            catch (ApiException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> TaoKhachHangMoi([FromBody] TaoKhachHangRequest request)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault();
                return Json(new { success = false, message = error?.ErrorMessage ?? "Dữ liệu không hợp lệ." });
            }
            try
            {
                var result = await _banHangService.TaoKhachHangMoiAsync(request);
                return Json(new { success = true, data = result });
            }
            catch (ApiException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> TimKiemSanPham(string keyword)
        {
            var results = await _banHangService.TimKiemSanPhamAsync(keyword);
            return PartialView("_SanPhamSearchResults", results);
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