using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Models.DTO.BanHang;
using FurryFriends.API.Models.DTO.BanHang.Requests;
using FurryFriends.Web.Services;
using FurryFriends.Web.Services.IService;
using FurryFriends.Web.Filter;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/BanHang/[action]")] // Định tuyến tập trung, dễ gọi từ JS
    [AuthorizeEmployee]
    public class BanHangController : Controller
    {
        private readonly IBanHangService _banHangService;
        private readonly IHinhThucThanhToanService _hinhThucThanhToanService;
        private readonly ILogger<BanHangController> _logger;
        private readonly IThongBaoService _thongBaoService;

        public BanHangController(IBanHangService banHangService, IHinhThucThanhToanService hinhThucThanhToanService, ILogger<BanHangController> logger, IThongBaoService thongBaoService)
        {
            _banHangService = banHangService;
            _hinhThucThanhToanService = hinhThucThanhToanService;
            _logger = logger;
            _thongBaoService = thongBaoService;
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
            try
            {
                // Tạo danh sách hình thức thanh toán mặc định cho bán hàng offline
                var hinhThucThanhToanList = new List<HinhThucThanhToanDto>
                {
                    new HinhThucThanhToanDto
                    {
                        HinhThucThanhToanId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                        TenHinhThuc = "Tiền mặt",
                        MoTa = "Thanh toán tiền mặt tại quầy",
                        LoaiHinhThuc = 1, // Tiền mặt
                        TrangThai = true
                    },
                    new HinhThucThanhToanDto
                    {
                        HinhThucThanhToanId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                        TenHinhThuc = "Chuyển khoản",
                        MoTa = "Thanh toán chuyển khoản qua VietQR",
                        LoaiHinhThuc = 2, // Chuyển khoản
                        TrangThai = true
                    }
                };
                
                ViewBag.HinhThucThanhToanList = hinhThucThanhToanList;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo danh sách hình thức thanh toán");
                ViewBag.HinhThucThanhToanList = new List<HinhThucThanhToanDto>();
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var hoaDon = await _banHangService.GetHoaDonByIdAsync(id);
                
                // ✅ Sử dụng cùng danh sách hình thức thanh toán như TaoHoaDonMoi cho bán hàng offline
                var hinhThucThanhToanList = new List<HinhThucThanhToanDto>
                {
                    new HinhThucThanhToanDto
                    {
                        HinhThucThanhToanId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                        TenHinhThuc = "Tiền mặt",
                        MoTa = "Thanh toán tiền mặt tại quầy",
                        LoaiHinhThuc = 1, // Tiền mặt
                        TrangThai = true
                    },
                    new HinhThucThanhToanDto
                    {
                        HinhThucThanhToanId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                        TenHinhThuc = "Chuyển khoản",
                        MoTa = "Thanh toán chuyển khoản qua VietQR",
                        LoaiHinhThuc = 2, // Chuyển khoản
                        TrangThai = true
                    }
                };
                
                ViewBag.HinhThucThanhToanList = hinhThucThanhToanList;
                
                // ✅ Kiểm tra các trạng thái hóa đơn chờ (có thể sửa)
                var trangThaiCho = new[] { "Chua Thanh Toan", "Offline ChuaThanhToan" };
                if (trangThaiCho.Contains(hoaDon.TrangThai)) 
                {
                    return View("Details_Interactive", hoaDon);
                }
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
                var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
                {
                    TieuDe = "Tạo hóa đơn mới",
                    NoiDung = $"Hóa đơn chờ #{result.MaHoaDon} đã được tạo.",
                    Loai = "HoaDon",
                    UserName = tenNhanVien,
                    NgayTao = DateTime.Now,
                    DaDoc = false
                });
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
            try { var result = await _banHangService.ThemSanPhamVaoHoaDonAsync(hoaDonId, request);
                var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
            {
                TieuDe = "Thêm sản phẩm vào hóa đơn",
                NoiDung = $"Sản phẩm {request.SanPhamChiTietId} đã được thêm vào hóa đơn #{hoaDonId}.",
                Loai = "HoaDon",
                UserName = tenNhanVien, // bạn có thể lấy từ session / User.Identity.Name nếu cần
                NgayTao = DateTime.Now,
                DaDoc = false
            }); return Json(new { success = true, data = result } ); }
            catch (ApiException ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpPut("{hoaDonId}/{sanPhamChiTietId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatSoLuong([FromBody] CapNhatSoLuongRequest request, Guid hoaDonId, Guid sanPhamChiTietId)
        {
            try { var result = await _banHangService.CapNhatSoLuongSanPhamAsync(hoaDonId, sanPhamChiTietId, request);
                var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
            {
                TieuDe = "Cập nhật số lượng sản phẩm",
                NoiDung = $"Sản phẩm trong hóa đơn #{hoaDonId} đã được cập nhật số lượng thành {request.SoLuongMoi}.",
                Loai = "HoaDon",
                UserName = tenNhanVien, // hoặc lấy từ User.Identity.Name nếu có đăng nhập
                NgayTao = DateTime.Now,
                DaDoc = false
            }); return Json(new { success = true, data = result }); }
            catch (ApiException ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpDelete("{hoaDonId}/{sanPhamChiTietId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaSanPham(Guid hoaDonId, Guid sanPhamChiTietId)
        {
            try { var result = await _banHangService.XoaSanPhamKhoiHoaDonAsync(hoaDonId, sanPhamChiTietId);
                var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
            {
                TieuDe = "Cập nhật hóa đơn",
                NoiDung = $"Đã xóa sản phẩm khỏi hóa đơn #{hoaDonId}",
                Loai = "HoaDon",
                UserName = tenNhanVien,
                NgayTao = DateTime.Now,
                DaDoc = false
            }); return Json(new { success = true, data = result }); }
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
            try { var result = await _banHangService.ApDungVoucherAsync(hoaDonId, request);
                var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
            {
                TieuDe = "Áp dụng voucher",
                NoiDung = $"Đã áp dụng voucher {request.MaVoucher} cho hóa đơn #{hoaDonId}",
                Loai = "HoaDon",
                UserName = tenNhanVien,
                NgayTao = DateTime.Now,
                DaDoc = false
            });
                return Json(new { success = true, data = result }); }
            catch (ApiException ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpPost("{hoaDonId}/ap-dung-voucher-giohang")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApDungVoucherGioHang([FromBody] ApDungVoucherGioHangRequest request, Guid hoaDonId)
        {
            try 
            { 
                var result = await _banHangService.ApDungVoucherGioHangAsync(request.KhachHangId, request.VoucherId); 
                return Json(new { success = true, data = result }); 
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpPost("{hoaDonId}/ap-dung-voucher-by-code")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApDungVoucherByCode([FromBody] ApDungVoucherByCodeRequest request, Guid hoaDonId)
        {
            try 
            { 
                // Tìm voucher theo mã
                var vouchers = await _banHangService.TimKiemVoucherHopLeAsync(hoaDonId);
                var voucher = vouchers.FirstOrDefault(v => v.MaVoucher == request.MaVoucher);
                
                if (voucher == null)
                {
                    return Json(new { success = false, message = "Mã voucher không tồn tại hoặc không hợp lệ" });
                }

                // Lấy thông tin hóa đơn để có KhachHangId
                var hoaDon = await _banHangService.GetHoaDonByIdAsync(hoaDonId);
                if (hoaDon?.KhachHang?.KhachHangId == null)
                {
                    return Json(new { success = false, message = "Hóa đơn chưa có khách hàng" });
                }

                var result = await _banHangService.ApDungVoucherGioHangAsync(hoaDon.KhachHang.KhachHangId, voucher.VoucherId); 
                return Json(new { success = true, data = result }); 
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpDelete("{hoaDonId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GoBoVoucher(Guid hoaDonId)
        {
            try { var result = await _banHangService.GoBoVoucherAsync(hoaDonId);
                var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
            {
                TieuDe = "Hủy voucher",
                NoiDung = $"Đã gỡ bỏ voucher khỏi hóa đơn #{hoaDonId}",
                Loai = "HoaDon",
                UserName = tenNhanVien,
                NgayTao = DateTime.Now,
                DaDoc = false
            });
                return Json(new { success = true, data = result }); }
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
                var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
                {
                    TieuDe = "Thanh toán",
                    NoiDung = $"Hóa đơn #{result.MaHoaDon} đã được thanh toán thành công.",
                    Loai = "HoaDon",
                    UserName = tenNhanVien,
                    NgayTao = DateTime.Now,
                    DaDoc = false
                });
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
            try { var result = await _banHangService.TaoKhachHangMoiAsync(request);
                var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
            {
                TieuDe = "Khách hàng mới",
                NoiDung = $"Khách hàng {request.TenKhachHang} đã được tạo và gán vào hóa đơn",
                Loai = "KhachHang",
                UserName = tenNhanVien,
                NgayTao = DateTime.Now,
                DaDoc = false
            });
                return Json(new { success = true, data = result }); }
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