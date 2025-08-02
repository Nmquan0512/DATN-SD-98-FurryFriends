using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using FurryFriends.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Controllers
{
    public class SanPhamKhachHangController : Controller
    {
        private readonly ISanPhamService _sanPhamService;
        private readonly ISanPhamChiTietService _sanPhamChiTietService;

        public SanPhamKhachHangController(ISanPhamService sanPhamService, ISanPhamChiTietService sanPhamChiTietService)
        {
            _sanPhamService = sanPhamService;
            _sanPhamChiTietService = sanPhamChiTietService;
        }

        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var danhSachSanPhamDTO = await _sanPhamService.GetAllAsync();

            var viewModelList = new List<SanPhamViewModel>();

            // Lấy toàn bộ danh sách chi tiết một lần để tránh gọi API nhiều lần
            var allChiTietListDTO = await _sanPhamChiTietService.GetAllAsync();

            foreach (var sp in danhSachSanPhamDTO)
            {
                var chiTietListDTO = allChiTietListDTO
                                        .Where(ct => ct.SanPhamId == sp.SanPhamId)
                                        .ToList();

                string? anhDaiDien = chiTietListDTO
                                        .FirstOrDefault(ct => !string.IsNullOrEmpty(ct.DuongDan))
                                        ?.DuongDan;

                // 👉 Chuyển sang ViewModel
                var chiTietVMs = chiTietListDTO.Select(ct => new SanPhamChiTietViewModel
                {
                    SanPhamChiTietId = ct.SanPhamChiTietId,
                    MauSac = ct.TenMau ?? "",
                    KichCo = ct.TenKichCo ?? "",
                    SoLuongTon = ct.SoLuong,
                    GiaBan = ct.Gia,
                    DanhSachAnh = ct.DuongDan != null ? new List<string> { ct.DuongDan } : new List<string>()
                }).ToList();

                var sanPhamVM = new SanPhamViewModel
                {
                    SanPhamId = sp.SanPhamId,
                    TenSanPham = sp.TenSanPham,
                    MoTa = sp.TenThuongHieu ?? "",
                    TrangThai = sp.TrangThai,
                    AnhDaiDienUrl = anhDaiDien,
                    SanPhamChiTietId = chiTietListDTO.FirstOrDefault()?.SanPhamChiTietId ?? Guid.Empty,

                    // 🔥 Bổ sung đầy đủ danh sách chi tiết
                    ChiTietList = chiTietVMs
                };

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
                DanhSachAnh = ct.DuongDan != null ? new List<string> { ct.DuongDan } : new List<string>()
            }).ToList();

            var vm = new SanPhamViewModel
            {
                SanPhamId = sp.SanPhamId,
                TenSanPham = sp.TenSanPham,
                MoTa = sp.TenThuongHieu ?? "",
                TrangThai = sp.TrangThai,
                AnhDaiDienUrl = chiTietListDTO.FirstOrDefault()?.DuongDan,
                ChiTietList = chiTietViewModels
            };

            return View(vm);
        }
    }
}
