using FurryFriends.Web.Services.IService;
using FurryFriends.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Controllers
{
    public class DonHangController : Controller
    {
        private readonly IHoaDonService _hoaDonService;

        public DonHangController(IHoaDonService hoaDonService)
        {
            _hoaDonService = hoaDonService;
        }

        public async Task<IActionResult> Index()
        {
            var khachHangId = HttpContext.Session.GetString("KhachHangId");
            if (string.IsNullOrEmpty(khachHangId))
            {
                return RedirectToAction("Login", "Account");
            }

            var customerGuid = Guid.Parse(khachHangId);
            
            // Get all orders and filter by customer
            var allOrders = await _hoaDonService.GetHoaDonListAsync();
            var customerOrders = allOrders
                .Where(h => h.KhachHangId == customerGuid)
                .OrderByDescending(h => h.NgayTao)
                .Select(h => new DonHangKhachHangViewModel
                {
                    HoaDonId = h.HoaDonId,
                    NgayTao = h.NgayTao,
                    NgayNhanHang = h.NgayNhanHang,
                    TongTien = h.TongTien,
                    TongTienSauKhiGiam = h.TongTienSauKhiGiam,
                    TrangThai = h.TrangThai,
                    TrangThaiText = GetTrangThaiText(h.TrangThai),
                    GhiChu = h.GhiChu ?? "",
                    HinhThucThanhToan = h.HinhThucThanhToan?.TenHinhThuc ?? "Không xác định",
                    DiaChiGiaoHang = !string.IsNullOrEmpty(h.DiaChiGiaoHangLucMua) ? 
                        h.DiaChiGiaoHangLucMua : 
                        (h.DiaChiGiaoHang != null ? 
                            $"{h.DiaChiGiaoHang.TenDiaChi}, {h.DiaChiGiaoHang.PhuongXa}, {h.DiaChiGiaoHang.ThanhPho}" : 
                            "Không xác định"),
                    // ✅ Sử dụng snapshot voucher info
                    VoucherCode = h.Voucher?.TenVoucher,
                    // Hiển thị số tiền giảm dương, tính theo tổng cuối có cộng phí ship
                    VoucherDiscount = h.Voucher != null ?
                        Math.Max(0, (h.TongTien + ((h.TongTienSauKhiGiam >= 500000m) ? 0m : 30000m)) - h.TongTienSauKhiGiam)
                        : (decimal?)null,
                    ThongTinVoucherLucMua = h.ThongTinVoucherLucMua,
                    // ✅ Load lịch sử thay đổi trạng thái
                    LichSuTrangThaiHoaDons = h.LichSuTrangThaiHoaDons?.Select(l => new LichSuTrangThaiHoaDonViewModel
                    {
                        Id = l.Id,
                        HoaDonId = l.HoaDonId,
                        TrangThaiCu = l.TrangThaiCu,
                        TrangThaiMoi = l.TrangThaiMoi,
                        ThoiGianThayDoi = l.ThoiGianThayDoi,
                        GhiChu = l.GhiChu,
                        NhanVienId = l.NhanVienId
                    }).ToList() ?? new List<LichSuTrangThaiHoaDonViewModel>(),
                    ChiTiets = h.HoaDonChiTiets?.Select(ct => new DonHangChiTietViewModel
                    {
                        HoaDonChiTietId = ct.HoaDonChiTietId,
                        // ✅ Sử dụng snapshot data thay vì real-time data
                        TenSanPham = ct.TenSanPhamLucMua ?? ct.SanPhamChiTiet?.SanPham?.TenSanPham ?? "Không xác định",
                        AnhSanPham = ct.AnhSanPhamLucMua ?? ct.SanPhamChiTiet?.Anh?.DuongDan ?? "",
                        MauSac = ct.MauSacLucMua ?? ct.SanPhamChiTiet?.MauSac?.TenMau ?? "Không xác định",
                        KichCo = ct.KichCoLucMua ?? ct.SanPhamChiTiet?.KichCo?.TenKichCo ?? "Không xác định",
                        ThuongHieu = ct.ThuongHieuLucMua ?? ct.SanPhamChiTiet?.SanPham?.ThuongHieu?.TenThuongHieu ?? "",
                        MoTa = ct.MoTaSanPhamLucMua ?? ct.SanPhamChiTiet?.MoTa ?? "",
                        // ✅ Thêm các thuộc tính snapshot
                        TenSanPhamLucMua = ct.TenSanPhamLucMua,
                        AnhSanPhamLucMua = ct.AnhSanPhamLucMua,
                        MauSacLucMua = ct.MauSacLucMua,
                        KichCoLucMua = ct.KichCoLucMua,
                        ThuongHieuLucMua = ct.ThuongHieuLucMua,
                        MoTaSanPhamLucMua = ct.MoTaSanPhamLucMua,
                        GiaLucMua = ct.GiaLucMua,
                        SoLuong = ct.SoLuongSanPham,
                        Gia = ct.GiaLucMua ?? ct.Gia,
                        ThanhTien = (ct.GiaLucMua ?? ct.Gia) * ct.SoLuongSanPham
                    }).ToList() ?? new List<DonHangChiTietViewModel>()
                })
                .ToList();

            return View(customerOrders);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var khachHangId = HttpContext.Session.GetString("KhachHangId");
            if (string.IsNullOrEmpty(khachHangId))
            {
                return RedirectToAction("Login", "Account");
            }

            var customerGuid = Guid.Parse(khachHangId);

            try
            {
                var hoaDon = await _hoaDonService.GetHoaDonByIdAsync(id);
                
                // Check if this order belongs to the current customer
                if (hoaDon.KhachHangId != customerGuid)
                {
                    return NotFound();
                }

                var orderViewModel = new DonHangKhachHangViewModel
                {
                    HoaDonId = hoaDon.HoaDonId,
                    NgayTao = hoaDon.NgayTao,
                    NgayNhanHang = hoaDon.NgayNhanHang,
                    TongTien = hoaDon.TongTien,
                    TongTienSauKhiGiam = hoaDon.TongTienSauKhiGiam,
                    TrangThai = hoaDon.TrangThai,
                    TrangThaiText = GetTrangThaiText(hoaDon.TrangThai),
                    GhiChu = hoaDon.GhiChu ?? "",
                    HinhThucThanhToan = hoaDon.HinhThucThanhToan?.TenHinhThuc ?? "Không xác định",
                    DiaChiGiaoHang = !string.IsNullOrEmpty(hoaDon.DiaChiGiaoHangLucMua) ? 
                        hoaDon.DiaChiGiaoHangLucMua : 
                        (hoaDon.DiaChiGiaoHang != null ? 
                            $"{hoaDon.DiaChiGiaoHang.TenDiaChi}, {hoaDon.DiaChiGiaoHang.PhuongXa}, {hoaDon.DiaChiGiaoHang.ThanhPho}" : 
                            "Không xác định"),
                    // ✅ Sử dụng snapshot voucher info
                    VoucherCode = hoaDon.Voucher?.TenVoucher,
                    // Hiển thị số tiền giảm dương, tính theo tổng cuối có cộng phí ship
                    VoucherDiscount = hoaDon.Voucher != null ?
                        Math.Max(0, (hoaDon.TongTien + ((hoaDon.TongTienSauKhiGiam >= 500000m) ? 0m : 30000m)) - hoaDon.TongTienSauKhiGiam)
                        : (decimal?)null,
                    ThongTinVoucherLucMua = hoaDon.ThongTinVoucherLucMua,
                    // ✅ Load lịch sử thay đổi trạng thái
                    LichSuTrangThaiHoaDons = hoaDon.LichSuTrangThaiHoaDons?.Select(l => new LichSuTrangThaiHoaDonViewModel
                    {
                        Id = l.Id,
                        HoaDonId = l.HoaDonId,
                        TrangThaiCu = l.TrangThaiCu,
                        TrangThaiMoi = l.TrangThaiMoi,
                        ThoiGianThayDoi = l.ThoiGianThayDoi,
                        GhiChu = l.GhiChu,
                        NhanVienId = l.NhanVienId
                    }).ToList() ?? new List<LichSuTrangThaiHoaDonViewModel>(),
                    ChiTiets = hoaDon.HoaDonChiTiets?.Select(ct => new DonHangChiTietViewModel
                    {
                        HoaDonChiTietId = ct.HoaDonChiTietId,
                        // ✅ Sử dụng snapshot data thay vì real-time data
                        TenSanPham = ct.TenSanPhamLucMua ?? ct.SanPhamChiTiet?.SanPham?.TenSanPham ?? "Không xác định",
                        AnhSanPham = ct.AnhSanPhamLucMua ?? ct.SanPhamChiTiet?.Anh?.DuongDan ?? "",
                        MauSac = ct.MauSacLucMua ?? ct.SanPhamChiTiet?.MauSac?.TenMau ?? "Không xác định",
                        KichCo = ct.KichCoLucMua ?? ct.SanPhamChiTiet?.KichCo?.TenKichCo ?? "Không xác định",
                        ThuongHieu = ct.ThuongHieuLucMua ?? ct.SanPhamChiTiet?.SanPham?.ThuongHieu?.TenThuongHieu ?? "",
                        MoTa = ct.MoTaSanPhamLucMua ?? ct.SanPhamChiTiet?.MoTa ?? "",
                        // ✅ Thêm các thuộc tính snapshot
                        TenSanPhamLucMua = ct.TenSanPhamLucMua,
                        AnhSanPhamLucMua = ct.AnhSanPhamLucMua,
                        MauSacLucMua = ct.MauSacLucMua,
                        KichCoLucMua = ct.KichCoLucMua,
                        ThuongHieuLucMua = ct.ThuongHieuLucMua,
                        MoTaSanPhamLucMua = ct.MoTaSanPhamLucMua,
                        GiaLucMua = ct.GiaLucMua,
                        SoLuong = ct.SoLuongSanPham,
                        Gia = ct.GiaLucMua ?? ct.Gia,
                        ThanhTien = (ct.GiaLucMua ?? ct.Gia) * ct.SoLuongSanPham
                    }).ToList() ?? new List<DonHangChiTietViewModel>()
                };

                return View(orderViewModel);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

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

        // ✅ Hủy đơn hàng
        [HttpPost]
        public async Task<IActionResult> HuyDonHang(Guid hoaDonId)
        {
            try
            {
                var khachHangId = HttpContext.Session.GetString("KhachHangId");
                if (string.IsNullOrEmpty(khachHangId))
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện thao tác này!" });
                }

                var customerGuid = Guid.Parse(khachHangId);

                // Kiểm tra đơn hàng có thuộc về khách hàng này không
                var hoaDon = await _hoaDonService.GetHoaDonByIdAsync(hoaDonId);
                if (hoaDon.KhachHangId != customerGuid)
                {
                    return Json(new { success = false, message = "Bạn không có quyền hủy đơn hàng này!" });
                }

                // Gọi API hủy đơn hàng
                var result = await _hoaDonService.HuyDonHangAsync(hoaDonId);
                
                if (result.Success)
                {
                    return Json(new { success = true, message = result.Message });
                }
                else
                {
                    return Json(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
    }
}
