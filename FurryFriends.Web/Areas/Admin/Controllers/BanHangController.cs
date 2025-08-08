using FurryFriends.API.Models.DTO.BanHang;
using FurryFriends.API.Models.DTO.BanHang.Requests;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize] // Yêu cầu đăng nhập cho toàn bộ chức năng bán hàng
    public class BanHangController : Controller
    {
        private readonly IBanHangService _banHangService;
        private readonly ILogger<BanHangController> _logger;

        public BanHangController(IBanHangService banHangService, ILogger<BanHangController> logger)
        {
            _banHangService = banHangService;
            _logger = logger;
        }

        /// <summary>
        /// Hiển thị danh sách lịch sử hóa đơn.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                var model = await _banHangService.GetAllHoaDonsAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách hóa đơn.");
                TempData["error"] = "Không thể tải lịch sử hóa đơn.";
                return View(new List<HoaDonBanHangDto>());
            }
        }

        /// <summary>
        /// Bắt đầu quá trình tạo hóa đơn mới.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create()
        {
            try
            {
                // Yêu cầu tạo một hóa đơn trống mặc định là khách lẻ
                var request = new TaoHoaDonRequest { LaKhachLe = true };
                var result = await _banHangService.TaoHoaDonAsync(request);

                TempData["success"] = "Đã tạo hóa đơn mới (hóa đơn chờ).";
                return RedirectToAction("Details", new { id = result.HoaDonId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo hóa đơn mới.");
                TempData["error"] = $"Tạo hóa đơn thất bại: {ex.Message}";
                return RedirectToAction("Index");
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
                    return RedirectToAction("Index");
                }
                return View(hoaDon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi xem chi tiết hóa đơn {id}.");
                TempData["error"] = "Lỗi khi tải dữ liệu chi tiết hóa đơn.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemSanPham(Guid hoaDonId, Guid sanPhamChiTietId, int soLuong)
        {
            try
            {
                if (soLuong <= 0)
                {
                    TempData["error"] = "Số lượng phải lớn hơn 0.";
                    return RedirectToAction("Details", new { id = hoaDonId });
                }
                var request = new ThemSanPhamRequest { SanPhamChiTietId = sanPhamChiTietId, SoLuong = soLuong };
                await _banHangService.ThemSanPhamVaoHoaDonAsync(hoaDonId, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi thêm sản phẩm vào hóa đơn {hoaDonId}.");
                TempData["error"] = ex.Message;
            }
            return RedirectToAction("Details", new { id = hoaDonId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatSoLuong(Guid hoaDonId, Guid sanPhamChiTietId, int soLuongMoi)
        {
            try
            {
                var request = new CapNhatSoLuongRequest { SoLuongMoi = soLuongMoi };
                await _banHangService.CapNhatSoLuongSanPhamAsync(hoaDonId, sanPhamChiTietId, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi cập nhật số lượng sản phẩm {sanPhamChiTietId}.");
                TempData["error"] = ex.Message;
            }
            return RedirectToAction("Details", new { id = hoaDonId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaSanPham(Guid hoaDonId, Guid sanPhamChiTietId)
        {
            try
            {
                await _banHangService.XoaSanPhamKhoiHoaDonAsync(hoaDonId, sanPhamChiTietId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi xóa sản phẩm khỏi hóa đơn {hoaDonId}.");
                TempData["error"] = ex.Message;
            }
            return RedirectToAction("Details", new { id = hoaDonId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApDungVoucher(Guid hoaDonId, string maVoucher)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maVoucher))
                {
                    TempData["error"] = "Mã voucher không được để trống.";
                    return RedirectToAction("Details", new { id = hoaDonId });
                }
                var request = new ApDungVoucherRequest { MaVoucher = maVoucher };
                await _banHangService.ApDungVoucherAsync(hoaDonId, request);
                TempData["success"] = "Áp dụng voucher thành công!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi áp dụng voucher cho hóa đơn {hoaDonId}.");
                TempData["error"] = ex.Message;
            }
            return RedirectToAction("Details", new { id = hoaDonId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GoBoVoucher(Guid hoaDonId)
        {
            try
            {
                await _banHangService.GoBoVoucherAsync(hoaDonId);
                TempData["success"] = "Đã gỡ bỏ voucher.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi gỡ voucher khỏi hóa đơn {hoaDonId}.");
                TempData["error"] = ex.Message;
            }
            return RedirectToAction("Details", new { id = hoaDonId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThanhToan(Guid hoaDonId, ThanhToanRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["error"] = "Thông tin thanh toán không hợp lệ.";
                    return RedirectToAction("Details", new { id = hoaDonId });
                }

                var result = await _banHangService.ThanhToanHoaDonAsync(hoaDonId, request);
                TempData["success"] = "Thanh toán hóa đơn thành công!";
                // Chuyển hướng đến trang chi tiết đã thanh toán (read-only) hoặc trang in hóa đơn
                return RedirectToAction("Details", new { id = result.HoaDonId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi thanh toán hóa đơn {hoaDonId}.");
                TempData["error"] = ex.Message;
                return RedirectToAction("Details", new { id = hoaDonId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyHoaDon(Guid hoaDonId)
        {
            try
            {
                await _banHangService.HuyHoaDonAsync(hoaDonId);
                TempData["success"] = "Đã hủy hóa đơn thành công.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi hủy hóa đơn {hoaDonId}.");
                TempData["error"] = ex.Message;
                return RedirectToAction("Details", new { id = hoaDonId });
            }
        }

        #region AJAX Actions

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GanKhachHang(Guid hoaDonId, Guid khachHangId)
        {
            try
            {
                var request = new GanKhachHangRequest { KhachHangId = khachHangId };
                await _banHangService.GanKhachHangAsync(hoaDonId, request);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> TaoKhachHangMoi([FromBody] TaoKhachHangRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Lấy lỗi validation đầu tiên
                    var error = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault();
                    return Json(new { success = false, message = error?.ErrorMessage ?? "Dữ liệu không hợp lệ." });
                }

                var result = await _banHangService.TaoKhachHangMoiAsync(request);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo khách hàng mới qua AJAX.");
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion
    }
}