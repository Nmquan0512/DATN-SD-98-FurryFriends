using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using FurryFriends.Web.Services;
using FurryFriends.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using FurryFriends.Web.Filter;

namespace FurryFriends.Web.Controllers
{
    public class SanPhamKhachHangController : Controller
    {
        private readonly ISanPhamService _sanPhamService;
        private readonly ISanPhamChiTietService _sanPhamChiTietService;
        private readonly DiscountCalculationService _discountCalculationService;

        public SanPhamKhachHangController(
            ISanPhamService sanPhamService, 
            ISanPhamChiTietService sanPhamChiTietService,
            DiscountCalculationService discountCalculationService)
        {
            _sanPhamService = sanPhamService;
            _sanPhamChiTietService = sanPhamChiTietService;
            _discountCalculationService = discountCalculationService;
        }

        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var danhSachSanPhamDTO = await _sanPhamService.GetAllAsync();
            var viewModelList = new List<SanPhamViewModel>();

            // Lấy toàn bộ chi tiết một lần
            var allChiTietListDTO = await _sanPhamChiTietService.GetAllAsync();

            foreach (var sp in danhSachSanPhamDTO)
            {
                // Bỏ qua sản phẩm không hoạt động
                if (!sp.TrangThai)
                    continue;

                // Lọc chi tiết theo sản phẩm
                var chiTietListDTO = allChiTietListDTO
                    .Where(ct => ct.SanPhamId == sp.SanPhamId
                              && ct.SoLuong > 0
                              && ct.TrangThai == 1) // ✅ chỉ lấy chi tiết còn hàng & hoạt động
                    .ToList();

                if (!chiTietListDTO.Any())
                    continue; // ✅ nếu không có chi tiết hợp lệ thì bỏ luôn sp

                string? anhDaiDien = chiTietListDTO
                    .FirstOrDefault(ct => !string.IsNullOrEmpty(ct.DuongDan))
                    ?.DuongDan;

                // Chuyển sang ViewModel chi tiết
                var chiTietVMs = chiTietListDTO.Select(ct => new SanPhamChiTietViewModel
                {
                    SanPhamChiTietId = ct.SanPhamChiTietId,
                    MauSac = ct.TenMau ?? "",
                    KichCo = ct.TenKichCo ?? "",
                    SoLuongTon = ct.SoLuong,
                    GiaBan = ct.Gia,
                    DanhSachAnh = ct.DuongDan != null ? new List<string> { ct.DuongDan } : new List<string>(),

                    // Thông tin giảm giá sẽ cập nhật sau
                    CoGiamGia = false,
                    PhanTramGiamGia = null,
                    GiaSauGiam = null
                }).ToList();

                var sanPhamVM = new SanPhamViewModel
                {
                    SanPhamId = sp.SanPhamId,
                    TenSanPham = sp.TenSanPham,
                    MoTa = sp.TenThuongHieu, // ✅ sửa lại đúng mô tả
                    TrangThai = sp.TrangThai,
                    AnhDaiDienUrl = anhDaiDien,
                    SanPhamChiTietId = chiTietListDTO.First().SanPhamChiTietId, // luôn có vì đã check Any()
                    GiaBan = chiTietListDTO.Min(ct => ct.Gia), // lấy min hoặc max thay vì FirstOrDefault
                    SoLuongTon = chiTietListDTO.Sum(ct => ct.SoLuong), // tổng số lượng tồn

                    TenThuongHieu = sp.TenThuongHieu,
                    ThuongHieuId = sp.ThuongHieuId,

                    ChiTietList = chiTietVMs
                };

                // Áp dụng logic giảm giá
                sanPhamVM = await _discountCalculationService.UpdateProductDiscount(sanPhamVM);

                viewModelList.Add(sanPhamVM);
            }

            var khachHangId = HttpContext.Session.GetString("KhachHangId");
            ViewBag.KhachHangId = khachHangId;

            return View(viewModelList);
        }


        // Hiển thị chi tiết sản phẩm
        public async Task<IActionResult> ChiTiet(Guid id)
        {
            var sp = await _sanPhamService.GetByIdAsync(id);
            if (sp == null) return NotFound();

            var chiTietListDTO = (await _sanPhamChiTietService.GetAllAsync())
                                    .Where(ct => ct.SanPhamId == id)
                                    .ToList();

            var chiTietViewModels = chiTietListDTO.Select(ct => new SanPhamChiTietViewModel
            {
                SanPhamChiTietId = ct.SanPhamChiTietId,
                MauSac = ct.TenMau ?? "",
                KichCo = ct.TenKichCo ?? "",
                SoLuongTon = ct.SoLuong,
                GiaBan = ct.Gia,
                DanhSachAnh = ct.DuongDan != null ? new List<string> { ct.DuongDan } : new List<string>(),
                
                // Thông tin giảm giá sẽ được tính toán sau
                CoGiamGia = false,
                PhanTramGiamGia = null,
                GiaSauGiam = null
            }).ToList();

            var vm = new SanPhamViewModel
            {
                SanPhamId = sp.SanPhamId,
                TenSanPham = sp.TenSanPham,
                MoTa = sp.TenThuongHieu ?? "", // Đây là mô tả, không phải tên thương hiệu
                TrangThai = sp.TrangThai,
                AnhDaiDienUrl = chiTietListDTO.FirstOrDefault()?.DuongDan,
                GiaBan = chiTietListDTO.FirstOrDefault()?.Gia ?? 0,
                SoLuongTon = chiTietListDTO.FirstOrDefault()?.SoLuong ?? 0,
                
                // Thông tin thương hiệu
                TenThuongHieu = sp.TenThuongHieu, // Tên thương hiệu
                ThuongHieuId = sp.ThuongHieuId,
                
                ChiTietList = chiTietViewModels
            };

            // Áp dụng logic giảm giá với % cao nhất
            vm = await _discountCalculationService.UpdateProductDiscount(vm);

            return View("Details", vm);
        }
    }
}
