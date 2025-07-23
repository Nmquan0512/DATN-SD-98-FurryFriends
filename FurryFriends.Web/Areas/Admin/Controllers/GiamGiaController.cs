using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using FurryFriends.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GiamGiaController : Controller
    {
        private readonly IGiamGiaService _giamGiaService;
        private readonly ISanPhamChiTietService _sanPhamChiTietService;

        public GiamGiaController(IGiamGiaService giamGiaService, ISanPhamChiTietService sanPhamChiTietService)
        {
            _giamGiaService = giamGiaService;
            _sanPhamChiTietService = sanPhamChiTietService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _giamGiaService.GetAllAsync();
            ViewBag.TotalCount = list.Count();
            ViewBag.ActiveCount = list.Count(x => x.TrangThai);
            ViewBag.InactiveCount = list.Count(x => !x.TrangThai);
            return View(list);
        }

        // ✅ GET: Admin/GiamGia/Create
        public async Task<IActionResult> Create()
        {
            var sanPhams = await _sanPhamChiTietService.GetAllAsync();

            var viewModel = new GiamGiaCreateViewModel
            {
                SanPhamChiTietList = sanPhams.Select(sp => new SanPhamChiTietGiamGiaItemViewModel
                {
                    SanPhamChiTietId = sp.SanPhamChiTietId,
                    TenSanPham = sp.TenSanPham ?? "",
                    TenMau = sp.TenMau ?? "",
                    TenKichCo = sp.TenKichCo ?? "",
                    Gia = sp.Gia,
                    DuongDan = sp.DuongDan,
                    DuocChon = false
                }).ToList()
            };

            return View(viewModel);
        }

        // ✅ POST: Admin/GiamGia/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GiamGiaCreateViewModel model)
        {
            model.GiamGia.SanPhamChiTietIds = model.SanPhamChiTietList
                .Where(x => x.DuocChon)
                .Select(x => x.SanPhamChiTietId)
                .ToList();

            if (!ModelState.IsValid)
            {
                var sanPhams = await _sanPhamChiTietService.GetAllAsync();
                model.SanPhamChiTietList = sanPhams.Select(sp => new SanPhamChiTietGiamGiaItemViewModel
                {
                    SanPhamChiTietId = sp.SanPhamChiTietId,
                    TenSanPham = sp.TenSanPham ?? "",
                    TenMau = sp.TenMau ?? "",
                    TenKichCo = sp.TenKichCo ?? "",
                    Gia = sp.Gia,
                    DuongDan = sp.DuongDan,
                    DuocChon = model.GiamGia.SanPhamChiTietIds.Contains(sp.SanPhamChiTietId)
                }).ToList();

                return View(model);
            }

            var result = await _giamGiaService.CreateAsync(model.GiamGia);
            if (result.Success)
            {
                TempData["success"] = "Tạo chương trình giảm giá thành công!";
                return RedirectToAction("Index");
            }

            if (result.Errors != null)
            {
                foreach (var err in result.Errors)
                {
                    foreach (var msg in err.Value)
                    {
                        ModelState.AddModelError(err.Key, msg);
                    }
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var item = await _giamGiaService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, GiamGiaDTO dto)
        {
            if (id != dto.GiamGiaId) return BadRequest();
            if (!ModelState.IsValid) return View(dto);

            var result = await _giamGiaService.UpdateAsync(id, dto);
            if (result.Data)
            {
                TempData["success"] = "Cập nhật chương trình giảm giá thành công!";
                return RedirectToAction("Index");
            }

            if (result.Errors != null)
                foreach (var err in result.Errors)
                    foreach (var msg in err.Value)
                        ModelState.AddModelError(err.Key, msg);
            else
                ModelState.AddModelError("", "Lỗi khi cập nhật!");

            return View(dto);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var item = await _giamGiaService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // ✅ GET: Admin/GiamGia/AddSanPham/{id}
        public async Task<IActionResult> AddSanPham(Guid id)
        {
            var giamGia = await _giamGiaService.GetByIdAsync(id);
            if (giamGia == null) return NotFound();

            var sanPhams = await _sanPhamChiTietService.GetAllAsync();

            var viewModel = new GiamGiaCreateViewModel
            {
                GiamGia = giamGia,
                SanPhamChiTietList = sanPhams.Select(sp => new SanPhamChiTietGiamGiaItemViewModel
                {
                    SanPhamChiTietId = sp.SanPhamChiTietId,
                    TenSanPham = sp.TenSanPham ?? "",
                    TenMau = sp.TenMau ?? "",
                    TenKichCo = sp.TenKichCo ?? "",
                    Gia = sp.Gia,
                    DuongDan = sp.DuongDan,
                    DuocChon = giamGia.SanPhamChiTietIds?.Contains(sp.SanPhamChiTietId) ?? false
                }).ToList()
            };

            return View(viewModel);
        }

        // ✅ POST: Admin/GiamGia/AddSanPham/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSanPham(GiamGiaCreateViewModel model)
        {
            var selectedIds = model.SanPhamChiTietList
                                   .Where(x => x.DuocChon)
                                   .Select(x => x.SanPhamChiTietId)
                                   .ToList();

            if (selectedIds.Count == 0)
            {
                TempData["error"] = "Vui lòng chọn ít nhất một sản phẩm.";
                return RedirectToAction("AddSanPham", new { id = model.GiamGia.GiamGiaId });
            }

            var success = await _giamGiaService.AddSanPhamChiTietToGiamGiaAsync(model.GiamGia.GiamGiaId, selectedIds);
            if (success)
            {
                TempData["success"] = "Gán sản phẩm chi tiết vào giảm giá thành công!";
                return RedirectToAction("Details", new { id = model.GiamGia.GiamGiaId });
            }

            TempData["error"] = "Gán sản phẩm thất bại.";
            return RedirectToAction("AddSanPham", new { id = model.GiamGia.GiamGiaId });
        }
    }
}
