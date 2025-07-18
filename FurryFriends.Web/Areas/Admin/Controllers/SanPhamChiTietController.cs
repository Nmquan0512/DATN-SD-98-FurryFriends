using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using FurryFriends.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Areas.Admin.Controllers 
{ 

[Area("Admin")]
    public class SanPhamChiTietController : Controller
    {
        private readonly ISanPhamChiTietService _chiTietService;
        private readonly IAnhService _anhService;

        public SanPhamChiTietController(ISanPhamChiTietService chiTietService, IAnhService anhService)
        {
            _chiTietService = chiTietService;
            _anhService = anhService;
        }

        // ------------ GET: Tạo chi tiết sản phẩm cho sản phẩm đã có ------------
        [HttpGet]
        public IActionResult Create(Guid sanPhamId)
        {
            var viewModel = new SanPhamChiTietCreateViewModel
            {
                SanPhamChiTietId = null
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
                Gia = model.GiaBan
            };

            var result = await _chiTietService.CreateAsync(dto);
            if (result.Data == null)
            {
                ModelState.AddModelError("", "Không thể tạo chi tiết sản phẩm.");
                ViewBag.SanPhamId = sanPhamId;
                return View(model);
            }

            // Upload ảnh nếu có
            if (model.Files != null && model.Files.Any())
            {
                foreach (var file in model.Files)
                {
                    await _anhService.UploadAsync(file, result.Data.SanPhamChiTietId);
                }
            }

            return RedirectToAction("Details", "SanPham", new { id = sanPhamId });
        }

        // ------------ POST: Cập nhật chi tiết sản phẩm ------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SanPhamChiTietCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.SanPhamId = id;
                return View(model);
            }

            var dto = new SanPhamChiTietDTO
            {
                MauSacId = model.MauSacId,
                KichCoId = model.KichCoId,
                SoLuong = model.SoLuongTon,
                Gia = model.GiaBan
            };

            var result = await _chiTietService.UpdateAsync(id, dto);
            if (!result.Data)
            {
                ModelState.AddModelError("", "Không thể cập nhật chi tiết sản phẩm.");
                ViewBag.SanPhamId = id;
                return View(model);
            }

            return RedirectToAction("Index", "SanPham");
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

    }
}
