using Microsoft.AspNetCore.Mvc;
using FurryFriends.Web.Services.IService;
using FurryFriends.API.Models;
using FurryFriends.Web.ViewModels;
using System.Security.Claims;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HoaDonController : Controller
    {
        private readonly IHoaDonService _hoaDonService;
        private readonly ILogger<HoaDonController> _logger;

        public HoaDonController(IHoaDonService hoaDonService, ILogger<HoaDonController> logger)
        {
            _hoaDonService = hoaDonService;
            _logger = logger;
        }

        // GET: Admin/HoaDon
        public async Task<IActionResult> Index()
        {
            try
            {
                var hoaDons = await _hoaDonService.GetAllAsync();
                
                // Thống kê tổng quan
                ViewBag.TotalCount = hoaDons?.Count() ?? 0;
                ViewBag.BanTaiQuayCount = hoaDons?.Count(h => h.LoaiHoaDon == "BanTaiQuay") ?? 0;
                ViewBag.OnlineCount = hoaDons?.Count(h => h.LoaiHoaDon == "BanTrucTiep") ?? 0;
                
                // Chỉ lấy những đơn đã hoàn thành (trạng thái = 3)
                var hoaDonsHoanThanh = hoaDons?.Where(h => h.TrangThai == 3).ToList() ?? new List<HoaDon>();
                
                return View(hoaDonsHoanThanh);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading orders");
                TempData["Error"] = "Có lỗi xảy ra khi tải danh sách đơn hàng";
                return View(new List<HoaDon>());
            }
        }

        // GET: Admin/HoaDon/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var hoaDon = await _hoaDonService.GetByIdAsync(id);
                if (hoaDon == null)
                {
                    return NotFound();
                }

                return View(hoaDon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading order details");
                TempData["Error"] = "Có lỗi xảy ra khi tải chi tiết đơn hàng";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/HoaDon/DuyetDon
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DuyetDon(Guid id)
        {
            try
            {
                var hoaDon = await _hoaDonService.GetByIdAsync(id);
                if (hoaDon == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                // Kiểm tra trạng thái hiện tại
                if (hoaDon.TrangThai != 0) // 0 = Chờ duyệt
                {
                    return Json(new { success = false, message = "Chỉ có thể duyệt đơn hàng đang chờ duyệt" });
                }

                // Cập nhật trạng thái thành "Đã duyệt" (1)
                var result = await _hoaDonService.CapNhatTrangThaiAsync(id, 1);
                
                if (result.Success)
                {
                    return Json(new { success = true, message = "Duyệt đơn hàng thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving order {OrderId}", id);
                return Json(new { success = false, message = "Có lỗi xảy ra khi duyệt đơn hàng" });
            }
        }

        // POST: Admin/HoaDon/DoiTrangThai
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiTrangThai(Guid id, int trangThaiMoi)
        {
            try
            {
                var hoaDon = await _hoaDonService.GetByIdAsync(id);
                if (hoaDon == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                // Validate trạng thái chuyển đổi
                if (!IsValidStatusTransition(hoaDon.TrangThai, trangThaiMoi))
                {
                    return Json(new { success = false, message = GetInvalidTransitionMessage(hoaDon.TrangThai, trangThaiMoi) });
                }

                // Cập nhật trạng thái
                var result = await _hoaDonService.CapNhatTrangThaiAsync(id, trangThaiMoi);
                
                if (result.Success)
                {
                    var trangThaiText = GetTrangThaiText(trangThaiMoi);
                    return Json(new { success = true, message = $"Cập nhật trạng thái thành công! Trạng thái mới: {trangThaiText}" });
                }
                else
                {
                    return Json(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing order status {OrderId} to {NewStatus}", id, trangThaiMoi);
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật trạng thái" });
            }
        }

        // Kiểm tra tính hợp lệ của việc chuyển đổi trạng thái
        private bool IsValidStatusTransition(int trangThaiHienTai, int trangThaiMoi)
        {
            // Quy tắc chuyển đổi trạng thái:
            // 0 (Chờ duyệt) → 1 (Đã duyệt) ✓
            // 1 (Đã duyệt) → 2 (Đang giao) ✓
            // 2 (Đang giao) → 3 (Đã giao) ✓
            // 4 (Đã hủy) → Không thể chuyển sang trạng thái khác

            if (trangThaiHienTai == 4) // Đã hủy
            {
                return false; // Không thể chuyển từ trạng thái đã hủy
            }

            switch (trangThaiHienTai)
            {
                case 0: // Chờ duyệt
                    return trangThaiMoi == 1; // Chỉ có thể chuyển thành "Đã duyệt"
                
                case 1: // Đã duyệt
                    return trangThaiMoi == 2; // Chỉ có thể chuyển thành "Đang giao"
                
                case 2: // Đang giao
                    return trangThaiMoi == 3; // Chỉ có thể chuyển thành "Đã giao"
                
                case 3: // Đã giao
                    return false; // Không thể chuyển từ trạng thái đã giao
                
                default:
                    return false;
            }
        }

        // Lấy thông báo lỗi khi chuyển đổi trạng thái không hợp lệ
        private string GetInvalidTransitionMessage(int trangThaiHienTai, int trangThaiMoi)
        {
            var trangThaiHienTaiText = GetTrangThaiText(trangThaiHienTai);
            var trangThaiMoiText = GetTrangThaiText(trangThaiMoi);

            if (trangThaiHienTai == 4)
            {
                return "Không thể thay đổi trạng thái của đơn hàng đã hủy";
            }

            if (trangThaiHienTai == 3)
            {
                return "Không thể thay đổi trạng thái của đơn hàng đã giao thành công";
            }

            if (trangThaiHienTai == 1 && trangThaiMoi == 0)
            {
                return "Không thể chuyển đơn hàng từ 'Đã duyệt' về 'Chờ duyệt'";
            }

            if (trangThaiHienTai == 2 && (trangThaiMoi == 0 || trangThaiMoi == 1))
            {
                return "Không thể chuyển đơn hàng từ 'Đang giao' về 'Chờ duyệt' hoặc 'Đã duyệt'";
            }

            if (trangThaiHienTai == 3 && (trangThaiMoi == 0 || trangThaiMoi == 1 || trangThaiMoi == 2))
            {
                return "Không thể chuyển đơn hàng từ 'Đã giao' về trạng thái trước đó";
            }

            return $"Không thể chuyển đơn hàng từ '{trangThaiHienTaiText}' sang '{trangThaiMoiText}'";
        }

        // Lấy text hiển thị cho trạng thái
        private string GetTrangThaiText(int trangThai)
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


    }
}
