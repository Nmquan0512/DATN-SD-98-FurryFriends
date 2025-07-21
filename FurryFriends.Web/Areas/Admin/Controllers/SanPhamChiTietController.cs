using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using FurryFriends.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Thêm using này

namespace FurryFriends.Web.Areas.Admin.Controllers 
{ 

[Area("Admin")]
    public class SanPhamChiTietController : Controller
    {
        private readonly ISanPhamChiTietService _chiTietService;
        private readonly IAnhService _anhService;
        private readonly ISanPhamService _sanPhamService;
        private readonly IKichCoService _kichCoService; // Thêm
        private readonly IMauSacService _mauSacService; // Thêm

        public SanPhamChiTietController(
            ISanPhamChiTietService chiTietService, 
            IAnhService anhService, 
            ISanPhamService sanPhamService,
            IKichCoService kichCoService, // Thêm
            IMauSacService mauSacService // Thêm
        )
        {
            _chiTietService = chiTietService;
            _anhService = anhService;
            _sanPhamService = sanPhamService;
            _kichCoService = kichCoService; // Thêm
            _mauSacService = mauSacService; // Thêm
        }

        // ------------ GET: Tạo chi tiết sản phẩm cho sản phẩm đã có ------------
        [HttpGet]
        public async Task<IActionResult> Create(Guid sanPhamId)
        {
            var allKichCo = await _kichCoService.GetAllAsync();
            var kichCoList = new List<SelectListItem>();
            foreach (var kc in allKichCo)
            {
                if (kc.TrangThai)
                {
                    kichCoList.Add(new SelectListItem { Value = kc.KichCoId.ToString(), Text = kc.TenKichCo });
                }
            }
            ViewBag.KichCoList = kichCoList;
            var allMauSac = await _mauSacService.GetAllAsync();
            var mauSacList = new List<SelectListItem>();
            foreach (var ms in allMauSac)
            {
                if (ms.TrangThai)
                {
                    mauSacList.Add(new SelectListItem { Value = ms.MauSacId.ToString(), Text = ms.TenMau });
                }
            }
            ViewBag.MauSacList = mauSacList;
            var anhList = await _anhService.GetAllAsync();
            ViewBag.AnhList = new SelectList(anhList, "AnhId", "DuongDan");
            var viewModel = new SanPhamChiTietCreateViewModel
            {
                SanPhamChiTietId = null,
                SanPhamId = sanPhamId
            };
            ViewBag.SanPhamId = sanPhamId;
            return View(viewModel);
        }

        // ------------ POST: Tạo chi tiết sản phẩm ------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid sanPhamId, SanPhamChiTietCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.SanPhamId = sanPhamId;
                return View(model);
            }

            var dto = new SanPhamChiTietDTO
            {
                SanPhamId = sanPhamId,
                MauSacId = model.MauSacId,
                KichCoId = model.KichCoId,
                SoLuong = model.SoLuongTon,
                Gia = model.GiaBan,
                AnhId = model.AnhId,
                MoTa = model.MoTa
            };

            var result = await _chiTietService.CreateAsync(dto);
            if (result.Data == null)
            {
                ModelState.AddModelError("", "Không thể tạo chi tiết sản phẩm.");
                ViewBag.SanPhamId = sanPhamId;
                return View(model);
            }

            return RedirectToAction("Index", new { sanPhamId = sanPhamId });
        }

        // ------------ POST: Cập nhật chi tiết sản phẩm ------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SanPhamChiTietCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var kichCoList = await _kichCoService.GetAllAsync();
                var mauSacList = await _mauSacService.GetAllAsync();
                var anhList = await _anhService.GetAllAsync();
                ViewBag.KichCoList = new SelectList(kichCoList, "KichCoId", "TenKichCo", model.KichCoId);
                ViewBag.MauSacList = new SelectList(mauSacList, "MauSacId", "TenMau", model.MauSacId);
                ViewBag.AnhList = new SelectList(anhList, "AnhId", "DuongDan", model.AnhId);
                ViewBag.SanPhamId = model.SanPhamId;
                return View(model);
            }

            var dto = new SanPhamChiTietDTO
            {
                MauSacId = model.MauSacId,
                KichCoId = model.KichCoId,
                SoLuong = model.SoLuongTon,
                Gia = model.GiaBan,
                MoTa = model.MoTa,
                AnhId = model.AnhId,
                TrangThai = model.TrangThai
            };

            var result = await _chiTietService.UpdateAsync(id, dto);
            if (!result.Data)
            {
                ModelState.AddModelError("", "Không thể cập nhật chi tiết sản phẩm.");
                var kichCoList = await _kichCoService.GetAllAsync();
                var mauSacList = await _mauSacService.GetAllAsync();
                var anhList = await _anhService.GetAllAsync();
                ViewBag.KichCoList = new SelectList(kichCoList, "KichCoId", "TenKichCo", model.KichCoId);
                ViewBag.MauSacList = new SelectList(mauSacList, "MauSacId", "TenMau", model.MauSacId);
                ViewBag.AnhList = new SelectList(anhList, "AnhId", "DuongDan", model.AnhId);
                ViewBag.SanPhamId = model.SanPhamId;
                return View(model);
            }

            return RedirectToAction("Index", new { sanPhamId = model.SanPhamId });
        }

        // ------------ POST: Xóa chi tiết sản phẩm ------------
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id, Guid sanPhamId)
        {
            var success = await _chiTietService.DeleteAsync(id);
            if (!success)
            {
                TempData["Error"] = "Không thể xóa sản phẩm chi tiết.";
            }

            return RedirectToAction("Details", "SanPham", new { id = sanPhamId });
        }

        [HttpPost]
        public async Task<IActionResult> XoaNhanh(Guid id, Guid sanPhamId)
        {
            var chiTiet = await _chiTietService.GetByIdAsync(id);
            if (chiTiet == null)
            {
                TempData["Error"] = "Không tìm thấy biến thể để xoá.";
                return RedirectToAction("Index", new { sanPhamId });
            }
            chiTiet.TrangThai = 0; // Ngưng hoạt động
            await _chiTietService.UpdateAsync(id, chiTiet);
            TempData["Success"] = "Đã chuyển biến thể sang trạng thái Ngưng hoạt động.";
            return RedirectToAction("Index", new { sanPhamId });
        }

        [HttpPost]
        public async Task<IActionResult> DoiTrangThai(Guid id, Guid sanPhamId)
        {
            var chiTiet = await _chiTietService.GetByIdAsync(id);
            if (chiTiet == null)
            {
                TempData["Error"] = "Không tìm thấy biến thể để đổi trạng thái.";
                return RedirectToAction("Index", new { sanPhamId });
            }
            chiTiet.TrangThai = (chiTiet.TrangThai == 1) ? 0 : 1;
            await _chiTietService.UpdateAsync(id, chiTiet);
            TempData["Success"] = "Đã đổi trạng thái biến thể thành công.";
            return RedirectToAction("Index", new { sanPhamId });
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid sanPhamId)
        {
            var list = await _chiTietService.GetAllAsync();
            var filtered = list.Where(x => x.SanPhamId == sanPhamId).ToList();
            ViewBag.SanPhamId = sanPhamId;
            // Lấy tên sản phẩm cha
            var sanPham = await _sanPhamService.GetByIdAsync(sanPhamId);
            ViewBag.TenSanPham = sanPham?.TenSanPham ?? "";
            // Truyền danh sách kích cỡ, màu sắc cho view
            ViewBag.DanhSachKichCo = await _kichCoService.GetAllAsync();
            ViewBag.DanhSachMauSac = await _mauSacService.GetAllAsync();
            return View(filtered);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var chiTiet = await _chiTietService.GetByIdAsync(id);
            if (chiTiet == null) return NotFound();
            // Map DTO sang ViewModel
            var viewModel = new SanPhamChiTietCreateViewModel
            {
                SanPhamChiTietId = chiTiet.SanPhamChiTietId,
                SanPhamId = chiTiet.SanPhamId,
                MauSacId = chiTiet.MauSacId,
                KichCoId = chiTiet.KichCoId,
                SoLuongTon = chiTiet.SoLuong,
                GiaBan = chiTiet.Gia,
                MoTa = chiTiet.MoTa,
                AnhId = chiTiet.AnhId,
                TrangThai = chiTiet.TrangThai ?? 1,
                DuongDan = chiTiet.DuongDan
            };
            // --- SỬA KÍCH CỠ ---
            var allKichCo = await _kichCoService.GetAllAsync();
            var kichCoList = new List<SelectListItem>();
            foreach (var kc in allKichCo)
            {
                if (kc.TrangThai)
                {
                    kichCoList.Add(new SelectListItem { Value = kc.KichCoId.ToString(), Text = kc.TenKichCo });
                }
                else if (kc.KichCoId == viewModel.KichCoId)
                {
                    kichCoList.Add(new SelectListItem { Value = kc.KichCoId.ToString(), Text = kc.TenKichCo + " (Ngưng hoạt động)" });
                }
            }
            ViewBag.KichCoList = kichCoList;
            ViewBag.DanhSachKichCo = allKichCo.ToList();
            // --- END SỬA KÍCH CỠ ---
            // --- SỬA MÀU SẮC ---
            var allMauSac = await _mauSacService.GetAllAsync();
            var mauSacList = new List<SelectListItem>();
            foreach (var ms in allMauSac)
            {
                if (ms.TrangThai)
                {
                    mauSacList.Add(new SelectListItem { Value = ms.MauSacId.ToString(), Text = ms.TenMau });
                }
                else if (ms.MauSacId == viewModel.MauSacId)
                {
                    mauSacList.Add(new SelectListItem { Value = ms.MauSacId.ToString(), Text = ms.TenMau + " (Ngưng hoạt động)" });
                }
            }
            ViewBag.MauSacList = mauSacList;
            ViewBag.DanhSachMauSac = allMauSac.ToList();
            // --- END SỬA MÀU SẮC ---
            var anhList = await _anhService.GetAllAsync();
            ViewBag.AnhList = new SelectList(anhList, "AnhId", "DuongDan", viewModel.AnhId);
            ViewBag.SanPhamId = chiTiet.SanPhamId;
            return View(viewModel);
        }

    }
}
