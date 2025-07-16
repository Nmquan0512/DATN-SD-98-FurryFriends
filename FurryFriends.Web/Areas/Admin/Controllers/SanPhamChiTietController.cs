using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamChiTietController : Controller
    {
        private readonly ISanPhamChiTietService _service;
        private readonly IMauSacService _mauSacService;
        private readonly IKichCoService _kichCoService;

        public SanPhamChiTietController(
            ISanPhamChiTietService service,
            IMauSacService mauSacService,
            IKichCoService kichCoService)
        {
            _service = service;
            _mauSacService = mauSacService;
            _kichCoService = kichCoService;
        }

        // Danh sách chi tiết của 1 sản phẩm cụ thể
        public async Task<IActionResult> Index(Guid sanPhamId)
        {
            ViewBag.SanPhamId = sanPhamId;
            var allChiTiet = await _service.GetBySanPhamIdAsync(sanPhamId);
            return View(allChiTiet);
        }

        public async Task<IActionResult> Create(Guid sanPhamId)
        {
            ViewBag.SanPhamId = sanPhamId;
            await PopulateDropdownsAsync();
            return View(new SanPhamChiTietDTO { Id = sanPhamId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPhamChiTietDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(dto);
            }

            var result = await _service.CreateAsync(dto);
            if (result.Data != null)
                return RedirectToAction("Index", new { sanPhamId = dto.Id });

            if (result.Errors != null)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(error.Key, string.Join(" ", error.Value));
            }

            await PopulateDropdownsAsync();
            return View(dto);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();

            ViewBag.SanPhamId = dto.Id;
            await PopulateDropdownsAsync();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SanPhamChiTietDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(dto);
            }

            var result = await _service.UpdateAsync(id, dto);
            if (result.Data)
                return RedirectToAction("Index", new { sanPhamId = dto.Id });

            if (result.Errors != null)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(error.Key, string.Join(" ", error.Value));
            }

            await PopulateDropdownsAsync();
            return View(dto);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();

            ViewBag.SanPhamId = dto.Id;
            return View(dto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();

            var deleted = await _service.DeleteAsync(id);
            if (deleted)
                return RedirectToAction("Index", new { sanPhamId = dto.Id });

            ModelState.AddModelError("", "Xóa thất bại!");
            return View(dto);
        }

        private async Task PopulateDropdownsAsync()
        {
            var mauSacs = await _mauSacService.GetAllAsync();
            var kichCos = await _kichCoService.GetAllAsync();

            ViewBag.MauSacList = mauSacs.Select(m => new SelectListItem
            {
                Value = m.MauSacId.ToString(),
                Text = m.TenMau
            }).ToList();

            ViewBag.KichCoList = kichCos.Select(k => new SelectListItem
            {
                Value = k.KichCoId.ToString(),
                Text = k.TenKichCo
            }).ToList();
        }
    }
}
