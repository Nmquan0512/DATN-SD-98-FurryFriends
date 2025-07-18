using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using FurryFriends.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamController : Controller
    {
        private readonly ISanPhamService _sanPhamService;
        private readonly ISanPhamChiTietService _chiTietService;
        private readonly IAnhService _anhService;

        public SanPhamController(
            ISanPhamService sanPhamService,
            ISanPhamChiTietService chiTietService,
            IAnhService anhService)
        {
            _sanPhamService = sanPhamService;
            _chiTietService = chiTietService;
            _anhService = anhService;
        }

        // ---------------- GET: Hiển thị danh sách sản phẩm ----------------
        public async Task<IActionResult> Index()
        {
            var result = await _sanPhamService.GetAllAsync();
            return View(result);
        }

        // ---------------- GET: Hiển thị form tạo mới ----------------
        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new SanPhamFullCreateViewModel
            {
                SanPham = new SanPhamDTO(),
                ChiTietList = new List<SanPhamChiTietCreateViewModel>()
            };
            return View(viewModel);
        }

        // ---------------- POST: Tạo sản phẩm đầy đủ ----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPhamFullCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 1. Tạo sản phẩm chính
            var sanPhamToCreate = new SanPhamDTO
            {
                TenSanPham = model.SanPham.TenSanPham,
                LoaiSanPham = model.SanPham.LoaiSanPham,
                ThanhPhanIds = model.SanPham.ThanhPhanIds,
                ChatLieuIds = model.SanPham.ChatLieuIds,
                ThuongHieuId = model.SanPham.ThuongHieuId,
                TrangThai = model.SanPham.TrangThai
            };

            var createdSanPham = await _sanPhamService.CreateAsync(sanPhamToCreate);
            if (createdSanPham == null || createdSanPham.SanPhamId == Guid.Empty)
            {
                ModelState.AddModelError("", "Không thể tạo sản phẩm.");
                return View(model);
            }

            // 2. Tạo chi tiết sản phẩm
            foreach (var chiTiet in model.ChiTietList)
            {
                var chiTietToCreate = new SanPhamChiTietDTO
                {
                    SanPhamId = createdSanPham.SanPhamId,
                    MauSacId = chiTiet.MauSacId,
                    KichCoId = chiTiet.KichCoId,
                    SoLuong = chiTiet.SoLuongTon,
                    Gia = chiTiet.GiaBan
                };

                var createdChiTiet = await _chiTietService.CreateAsync(chiTietToCreate);
                if (createdChiTiet?.Data == null)
                {
                    // Ghi log hoặc thông báo nếu cần
                    continue;
                }

                // 3. Upload ảnh nếu có
                if (chiTiet.Files != null && chiTiet.Files.Any())
                {
                    foreach (var file in chiTiet.Files)
                    {
                        await _anhService.UploadAsync(file, createdChiTiet.Data.SanPhamChiTietId);
                    }
                }
            }

            return RedirectToAction("Index");
        }
    }
}
