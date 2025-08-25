using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Filter;
using FurryFriends.Web.Services;
using FurryFriends.Web.Services.IService;
using FurryFriends.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeEmployee]
    public class DonHangController : Controller
    {
        private readonly IHoaDonService _hoaDonService;
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly ILogger<DonHangController> _logger;
        private readonly IThongBaoService _thongBaoService;

        public DonHangController(IHoaDonService hoaDonService, IEmailNotificationService emailNotificationService, ILogger<DonHangController> logger, IThongBaoService thongBaoService)
        {
            _hoaDonService = hoaDonService;
            _emailNotificationService = emailNotificationService;
            _logger = logger;
            _thongBaoService = thongBaoService;
        }

        // GET: Admin/DonHang
        // ✅ Chỉ hiển thị hóa đơn online (trạng thái 0-5) và sắp xếp theo thời gian gần nhất
        public async Task<IActionResult> Index()
        {
            try
            {
                var hoaDons = await _hoaDonService.GetDonHangListAsync(); // ✅ Sử dụng method mới
                
                // ✅ Lọc bổ sung để đảm bảo chỉ hiển thị trạng thái 0-5
                var hoaDonsFiltered = hoaDons?.Where(h => h.TrangThai >= 0 && h.TrangThai <= 5).ToList() ?? new List<HoaDon>();
                
                return View(hoaDonsFiltered);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading orders");
                TempData["Error"] = "Có lỗi xảy ra khi tải danh sách đơn hàng";
                return View(new List<HoaDon>());
            }
        }

        // GET: Admin/DonHang/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var hoaDon = await _hoaDonService.GetHoaDonByIdAsync(id);
                if (hoaDon == null)
                {
                    return NotFound();
                }

                // ✅ Kiểm tra trạng thái để đảm bảo chỉ xem chi tiết hóa đơn trạng thái 0-5
                if (hoaDon.TrangThai < 0 || hoaDon.TrangThai > 5)
                {
                    return NotFound("Không tìm thấy đơn hàng hoặc đơn hàng không thuộc quản lý đơn hàng");
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

        // POST: Admin/DonHang/DuyetDon
        // ✅ Chỉ xử lý hóa đơn online (trạng thái 0-5)
        [HttpPost]
        public async Task<IActionResult> DuyetDon([FromBody] DuyetDonRequest request)
        {
            try
            {
                if (request?.Id == Guid.Empty)
                {
                    return Json(new { success = false, message = "ID đơn hàng không hợp lệ" });
                }

                var hoaDon = await _hoaDonService.GetHoaDonByIdAsync(request.Id);
                if (hoaDon == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                // ✅ Kiểm tra trạng thái để đảm bảo chỉ xử lý hóa đơn trạng thái 0-5
                if (hoaDon.TrangThai < 0 || hoaDon.TrangThai > 5)
                {
                    return Json(new { success = false, message = "Đơn hàng không thuộc quản lý đơn hàng" });
                }

                // Kiểm tra trạng thái hiện tại
                if (hoaDon.TrangThai != 0) // 0 = Chờ duyệt
                {
                    return Json(new { success = false, message = "Chỉ có thể duyệt đơn hàng đang chờ duyệt" });
                }

                // Cập nhật trạng thái thành "Đã duyệt" (1)
                var result = await _hoaDonService.CapNhatTrangThaiAsync(request.Id, 1);
                
                if (result.Success)
                {
                    var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
                    await _thongBaoService.CreateAsync(new ThongBaoDTO
                    {
                        TieuDe = "Duyệt đơn hàng",
                        NoiDung = $"Đơn hàng #{hoaDon.HoaDonId} đã được duyệt.",
                        Loai = "HoaDon",
                        UserName = tenNhanVien,
                        NgayTao = DateTime.Now,
                        DaDoc = false
                    });
                    return Json(new { success = true, message = "Duyệt đơn hàng thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving order {OrderId}", request?.Id);
                return Json(new { success = false, message = "Có lỗi xảy ra khi duyệt đơn hàng" });
            }
        }

        // POST: Admin/DonHang/DoiTrangThai
        // ✅ Chỉ xử lý hóa đơn online (trạng thái 0-5)
        [HttpPost]
        public async Task<IActionResult> DoiTrangThai([FromBody] DoiTrangThaiRequest request)
        {
            try
            {
                if (request?.Id == Guid.Empty)
                {
                    return Json(new { success = false, message = "ID đơn hàng không hợp lệ" });
                }

                var hoaDon = await _hoaDonService.GetHoaDonByIdAsync(request.Id);
                if (hoaDon == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                // ✅ Kiểm tra trạng thái để đảm bảo chỉ xử lý hóa đơn trạng thái 0-5
                if (hoaDon.TrangThai < 0 || hoaDon.TrangThai > 5)
                {
                    return Json(new { success = false, message = "Đơn hàng không thuộc quản lý đơn hàng" });
                }

                // Kiểm tra tính hợp lệ của việc chuyển trạng thái
                if (!IsValidStatusTransition(hoaDon.TrangThai, request.TrangThaiMoi))
                {
                    var message = GetInvalidTransitionMessage(hoaDon.TrangThai, request.TrangThaiMoi);
                    return Json(new { success = false, message = message });
                }

                // Lưu trạng thái cũ để gửi thông báo
                var trangThaiCu = hoaDon.TrangThai;
                var trangThaiCuText = GetTrangThaiText(trangThaiCu);

                // Cập nhật trạng thái
                var result = await _hoaDonService.CapNhatTrangThaiAsync(request.Id, request.TrangThaiMoi);
                
                if (result.Success)
                {
                    var trangThaiMoiText = GetTrangThaiText(request.TrangThaiMoi);
                    
                    // ✅ Gửi thông báo email cho khách hàng khi đổi trạng thái
                    try
                    {
                        await _emailNotificationService.SendStatusChangeNotificationToCustomerAsync(hoaDon, trangThaiCuText, trangThaiMoiText);
                        _logger.LogInformation($"✅ Customer notification sent for order {request.Id} - Status: {trangThaiCuText} → {trangThaiMoiText}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"❌ Error sending customer notification: {ex.Message}");
                        _logger.LogError($"❌ Error details: {ex}");
                        // Không throw exception để không ảnh hưởng đến luồng cập nhật trạng thái
                    }
                    var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
                    await _thongBaoService.CreateAsync(new ThongBaoDTO
                    {
                        TieuDe = "Cập nhật trạng thái hóa đơn",
                        NoiDung = $"Đơn hàng #{hoaDon.HoaDonId} đã đổi trạng thái: {trangThaiCuText} → {trangThaiMoiText}",
                        Loai = "HoaDon",
                        UserName = tenNhanVien,
                        NgayTao = DateTime.Now,
                        DaDoc = false
                    });

                    return Json(new { success = true, message = $"Cập nhật trạng thái thành công: {trangThaiMoiText}" });
                }
                else
                {
                    return Json(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status {OrderId} to {NewStatus}", request?.Id, request?.TrangThaiMoi);
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật trạng thái" });
            }
        }

        // POST: Admin/DonHang/HuyDon
        // ✅ Chỉ xử lý hóa đơn online (trạng thái 0-5)
        [HttpPost]
        public async Task<IActionResult> HuyDon([FromBody] HuyDonRequest request)
        {
            try
            {
                if (request?.Id == Guid.Empty)
                {
                    return Json(new { success = false, message = "ID đơn hàng không hợp lệ" });
                }

                var hoaDon = await _hoaDonService.GetHoaDonByIdAsync(request.Id);
                if (hoaDon == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                // ✅ Kiểm tra trạng thái để đảm bảo chỉ xử lý hóa đơn trạng thái 0-5
                if (hoaDon.TrangThai < 0 || hoaDon.TrangThai > 5)
                {
                    return Json(new { success = false, message = "Đơn hàng không thuộc quản lý đơn hàng" });
                }

                // Kiểm tra trạng thái hiện tại - chỉ có thể hủy đơn "Chờ duyệt" và "Đã duyệt"
                if (hoaDon.TrangThai != 0 && hoaDon.TrangThai != 1) // Chỉ cho phép hủy khi Chờ duyệt (0) hoặc Đã duyệt (1)
                {
                    return Json(new { success = false, message = "Chỉ có thể hủy đơn hàng khi trạng thái 'Chờ duyệt' hoặc 'Đã duyệt'" });
                }

                // Lưu trạng thái cũ để gửi thông báo
                var trangThaiCu = hoaDon.TrangThai;
                var trangThaiCuText = GetTrangThaiText(trangThaiCu);

                // Cập nhật trạng thái thành "Đã hủy" (4)
                var result = await _hoaDonService.CapNhatTrangThaiAsync(request.Id, 4);
                
                if (result.Success)
                {
                    var trangThaiMoiText = GetTrangThaiText(4);
                    
                    // ✅ Gửi thông báo email cho khách hàng khi hủy đơn
                    try
                    {
                        await _emailNotificationService.SendStatusChangeNotificationToCustomerAsync(hoaDon, trangThaiCuText, trangThaiMoiText);
                        _logger.LogInformation($"✅ Customer notification sent for order {request.Id} - Status: {trangThaiCuText} → {trangThaiMoiText}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"❌ Error sending customer notification: {ex.Message}");
                        _logger.LogError($"❌ Error details: {ex}");
                        // Không throw exception để không ảnh hưởng đến luồng hủy đơn
                    }
                    var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
                    await _thongBaoService.CreateAsync(new ThongBaoDTO
                    {
                        TieuDe = "Hủy đơn hàng",
                        NoiDung = $"Đơn hàng #{hoaDon.HoaDonId} đã bị hủy.",
                        Loai = "HoaDon",
                        UserName = tenNhanVien,
                        NgayTao = DateTime.Now,
                        DaDoc = false
                    });

                    return Json(new { success = true, message = "Hủy đơn hàng thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order {OrderId}", request?.Id);
                return Json(new { success = false, message = "Có lỗi xảy ra khi hủy đơn hàng" });
            }
        }

        // POST: Admin/DonHang/TangTrangThai
        // ✅ Chỉ xử lý hóa đơn online (trạng thái 0-5)
        [HttpPost]
        public async Task<IActionResult> TangTrangThai([FromBody] TangTrangThaiRequest request)
        {
            try
            {
                if (request?.Id == Guid.Empty)
                {
                    return Json(new { success = false, message = "ID đơn hàng không hợp lệ" });
                }

                var hoaDon = await _hoaDonService.GetHoaDonByIdAsync(request.Id);
                if (hoaDon == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                // ✅ Kiểm tra trạng thái để đảm bảo chỉ xử lý hóa đơn trạng thái 0-5
                if (hoaDon.TrangThai < 0 || hoaDon.TrangThai > 5)
                {
                    return Json(new { success = false, message = "Đơn hàng không thuộc quản lý đơn hàng" });
                }

                // Lưu trạng thái cũ để gửi thông báo
                var trangThaiCu = hoaDon.TrangThai;
                var trangThaiCuText = GetTrangThaiText(trangThaiCu);

                // Tăng trạng thái lên 1 đơn vị
                int trangThaiMoi = hoaDon.TrangThai + 1;
                
                // Kiểm tra giới hạn trạng thái (0-3, không bao gồm 4 - đã hủy)
                if (trangThaiMoi > 3)
                {
                    return Json(new { success = false, message = "Đơn hàng đã ở trạng thái cuối cùng" });
                }

                // Cập nhật trạng thái
                var result = await _hoaDonService.CapNhatTrangThaiAsync(request.Id, trangThaiMoi);
                
                if (result.Success)
                {
                    var trangThaiMoiText = GetTrangThaiText(trangThaiMoi);
                    
                    // ✅ Gửi thông báo email cho khách hàng khi tăng trạng thái
                    try
                    {
                        await _emailNotificationService.SendStatusChangeNotificationToCustomerAsync(hoaDon, trangThaiCuText, trangThaiMoiText);
                        _logger.LogInformation($"✅ Customer notification sent for order {request.Id} - Status: {trangThaiCuText} → {trangThaiMoiText}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"❌ Error sending customer notification: {ex.Message}");
                        _logger.LogError($"❌ Error details: {ex}");
                        // Không throw exception để không ảnh hưởng đến luồng cập nhật trạng thái
                    }
                    var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
                    await _thongBaoService.CreateAsync(new ThongBaoDTO
                    {
                        TieuDe = "Cập nhật trạng thái hóa đơn",
                        NoiDung = $"Đơn hàng #{hoaDon.HoaDonId} đã chuyển trạng thái: {trangThaiCuText} → {trangThaiMoiText}",
                        Loai = "HoaDon",
                        UserName = tenNhanVien,
                        NgayTao = DateTime.Now,
                        DaDoc = false
                    });

                    return Json(new { success = true, message = $"Tăng trạng thái thành công: {trangThaiMoiText}" });
                }
                else
                {
                    return Json(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing order status {OrderId}", request?.Id);
                return Json(new { success = false, message = "Có lỗi xảy ra khi tăng trạng thái" });
            }
        }

        // Kiểm tra tính hợp lệ của việc chuyển trạng thái
        private bool IsValidStatusTransition(int trangThaiHienTai, int trangThaiMoi)
        {
            // Quy tắc chuyển trạng thái:
            // 0 (Chờ duyệt) -> 1 (Đã duyệt)
            // 1 (Đã duyệt) -> 2 (Đang giao)
            // 2 (Đang giao) -> 3 (Đã giao)
            // 4 (Đã hủy) - không thể chuyển từ trạng thái này

            if (trangThaiHienTai == 4) // Đã hủy
                return false;

            if (trangThaiHienTai == 3) // Đã giao
                return false;

            // Chỉ cho phép tăng trạng thái theo thứ tự
            return trangThaiMoi == trangThaiHienTai + 1;
        }

        // Lấy thông báo lỗi khi chuyển trạng thái không hợp lệ
        private string GetInvalidTransitionMessage(int trangThaiHienTai, int trangThaiMoi)
        {
            if (trangThaiHienTai == 4)
                return "Không thể thay đổi trạng thái đơn hàng đã hủy";

            if (trangThaiHienTai == 3)
                return "Không thể thay đổi trạng thái đơn hàng đã giao";

            if (trangThaiMoi != trangThaiHienTai + 1)
                return "Chỉ có thể tăng trạng thái lên 1 đơn vị";

            return "Không thể thay đổi trạng thái đơn hàng";
        }

        // Lấy text mô tả trạng thái
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

    // Request models
    public class DuyetDonRequest
    {
        public Guid Id { get; set; }
    }

    public class DoiTrangThaiRequest
    {
        public Guid Id { get; set; }
        public int TrangThaiMoi { get; set; }
    }

    public class HuyDonRequest
    {
        public Guid Id { get; set; }
    }

    public class TangTrangThaiRequest
    {
        public Guid Id { get; set; }
    }
}
