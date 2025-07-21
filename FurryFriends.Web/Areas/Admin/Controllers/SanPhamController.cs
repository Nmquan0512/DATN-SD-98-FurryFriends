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
        private readonly IThuongHieuService _thuongHieuService;
        private readonly IThanhPhanService _thanhPhanService;
        private readonly IChatLieuService _chatLieuService;
        private readonly IMauSacService _mauSacService;
        private readonly IKichCoService _kichCoService;

        public SanPhamController(
            ISanPhamService sanPhamService,
            ISanPhamChiTietService chiTietService,
            IAnhService anhService,
            IThuongHieuService thuongHieuService,
            IThanhPhanService thanhPhanService,
            IChatLieuService chatLieuService,
            IMauSacService mauSacService,
            IKichCoService kichCoService)
        {
            _sanPhamService = sanPhamService;
            _chiTietService = chiTietService;
            _anhService = anhService;
            _thuongHieuService = thuongHieuService;
            _thanhPhanService = thanhPhanService;
            _chatLieuService = chatLieuService;
            _mauSacService = mauSacService;
            _kichCoService = kichCoService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _sanPhamService.GetAllAsync();
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdownData();
            var model = new SanPhamFullCreateViewModel
            {
                SanPham = new(),
                ChiTietList = new List<SanPhamChiTietCreateViewModel> { new() }
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPhamFullCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownData();
                return View(model);
            }

            var sanPham = await _sanPhamService.CreateAsync(model.SanPham);
            if (sanPham == null)
            {
                ModelState.AddModelError("", "❌ Không thể tạo sản phẩm chính.");
                await LoadDropdownData();
                return View(model);
            }

            foreach (var chiTiet in model.ChiTietList)
            {
                var chiTietDTO = new SanPhamChiTietDTO
                {
                    SanPhamId = sanPham.SanPhamId,
                    MauSacId = chiTiet.MauSacId,
                    KichCoId = chiTiet.KichCoId,
                    SoLuong = chiTiet.SoLuongTon,
                    Gia = chiTiet.GiaBan,
                    MoTa = chiTiet.MoTa
                };

                var chiTietResult = await _chiTietService.CreateAsync(chiTietDTO);
                if (!chiTietResult.Success || chiTietResult.Data == null)
                {
                    ModelState.AddModelError("", "❌ Không thể tạo biến thể.");
                    continue;
                }

                foreach (var file in chiTiet.Files ?? new())
                {
                    await _anhService.UploadAsync(file, chiTietResult.Data.SanPhamChiTietId);
                }
            }

            TempData["Success"] = "✅ Tạo sản phẩm và biến thể thành công!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var sanPham = await _sanPhamService.GetByIdAsync(id);
            var chiTietList = (await _chiTietService.GetAllAsync()).Where(x => x.SanPhamId == id).ToList();
            ViewBag.SanPham = sanPham;
            return View(chiTietList);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _sanPhamService.GetByIdAsync(id);
            if (model == null) return NotFound();
            await LoadDropdownData();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SanPhamDTO model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownData();
                return View(model);
            }

            try
            {
                await _sanPhamService.UpdateAsync(id, model);
                TempData["Success"] = "✅ Cập nhật thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                await LoadDropdownData();
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleTrangThaiSanPham(Guid id)
        {
            var sp = await _sanPhamService.GetByIdAsync(id);
            if (sp == null) return NotFound();

            sp.TrangThai = !sp.TrangThai;
            await _sanPhamService.UpdateAsync(id, sp);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> AddChiTiet(Guid sanPhamId)
        {
            var model = new SanPhamChiTietCreateViewModel
            {
                KichCoList = (await _kichCoService.GetAllAsync()).ToList(),
                MauSacList = (await _mauSacService.GetAllAsync()).ToList()
            };
            ViewBag.SanPhamId = sanPhamId;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddChiTiet(Guid sanPhamId, SanPhamChiTietCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.KichCoList = (await _kichCoService.GetAllAsync()).ToList();
                model.MauSacList = (await _mauSacService.GetAllAsync()).ToList();
                ViewBag.SanPhamId = sanPhamId;
                return View(model);
            }

            var chiTietDTO = new SanPhamChiTietDTO
            {
                SanPhamId = sanPhamId,
                MauSacId = model.MauSacId,
                KichCoId = model.KichCoId,
                SoLuong = model.SoLuongTon,
                Gia = model.GiaBan,
                MoTa = model.MoTa
            };

            var result = await _chiTietService.CreateAsync(chiTietDTO);
            if (result.Success && result.Data != null)
            {
                foreach (var file in model.Files ?? new())
                {
                    await _anhService.UploadAsync(file, result.Data.SanPhamChiTietId);
                }
                TempData["Success"] = "✅ Thêm biến thể thành công!";
            }

            return RedirectToAction("Details", new { id = sanPhamId });
        }

        [HttpGet]
        public async Task<IActionResult> EditChiTiet(Guid id)
        {
            var chiTiet = await _chiTietService.GetByIdAsync(id);
            if (chiTiet == null) return NotFound();

            var model = new SanPhamChiTietCreateViewModel
            {
                SanPhamChiTietId = chiTiet.SanPhamChiTietId,
                MauSacId = chiTiet.MauSacId,
                KichCoId = chiTiet.KichCoId,
                SoLuongTon = chiTiet.SoLuong,
                GiaBan = chiTiet.Gia,
                MoTa = chiTiet.MoTa,
                KichCoList = (await _kichCoService.GetAllAsync()).ToList(),
                MauSacList = (await _mauSacService.GetAllAsync()).ToList()
            };

            ViewBag.SanPhamId = chiTiet.SanPhamId;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditChiTiet(Guid id, SanPhamChiTietCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.KichCoList = (await _kichCoService.GetAllAsync()).ToList();
                model.MauSacList = (await _mauSacService.GetAllAsync()).ToList();
                return View(model);
            }

            var dto = new SanPhamChiTietDTO
            {
                SanPhamChiTietId = model.SanPhamChiTietId,
                MauSacId = model.MauSacId,
                KichCoId = model.KichCoId,
                SoLuong = model.SoLuongTon,
                Gia = model.GiaBan,
                MoTa = model.MoTa
            };

            var result = await _chiTietService.UpdateAsync(id, dto);
            if (result.Success)
                TempData["Success"] = "✅ Cập nhật biến thể thành công!";
            else
                TempData["Error"] = "❌ Cập nhật thất bại.";

            return RedirectToAction("Details", new { id = dto.SanPhamId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteChiTiet(Guid id, Guid sanPhamId)
        {
            await _chiTietService.DeleteAsync(id);
            TempData["Success"] = "✅ Xóa biến thể thành công!";
            return RedirectToAction("Details", new { id = sanPhamId });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleTrangThaiChiTiet(Guid id)
        {
            var chiTiet = await _chiTietService.GetByIdAsync(id);
            if (chiTiet == null) return NotFound();

            chiTiet.TrangThai = !chiTiet.TrangThai;
            await _chiTietService.UpdateAsync(id, chiTiet);
            return RedirectToAction("Details", new { id = chiTiet.SanPhamId });
        }

        private async Task LoadDropdownData()
        {
            ViewBag.ThuongHieuList = await _thuongHieuService.GetAllAsync();
            ViewBag.ThanhPhanList = await _thanhPhanService.GetAllAsync();
            ViewBag.ChatLieuList = await _chatLieuService.GetAllAsync();
            ViewBag.MauSacList = await _mauSacService.GetAllAsync();
            ViewBag.KichCoList = await _kichCoService.GetAllAsync();
        }
    }
}
