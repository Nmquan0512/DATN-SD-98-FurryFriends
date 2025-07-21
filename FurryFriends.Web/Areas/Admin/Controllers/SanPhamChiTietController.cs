using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamChiTietController : Controller
    {
        private readonly ISanPhamChiTietService _chiTietService;
        private readonly ISanPhamService _sanPhamService;
        private readonly IKichCoService _kichCoService;
        private readonly IMauSacService _mauSacService;

        public SanPhamChiTietController(
            ISanPhamChiTietService chiTietService,
            ISanPhamService sanPhamService,
            IKichCoService kichCoService,
            IMauSacService mauSacService)
        {
            _chiTietService = chiTietService;
            _sanPhamService = sanPhamService;
            _kichCoService = kichCoService;
            _mauSacService = mauSacService;
        }

        public async Task<IActionResult> Index(Guid sanPhamId)
        {
            ViewBag.SanPhamId = sanPhamId;
            var chiTietList = await _chiTietService.GetBySanPhamIdAsync(sanPhamId);
            return View(chiTietList);
        }

        public async Task<IActionResult> Create(Guid sanPhamId)
        {
            ViewBag.SanPhamId = sanPhamId;
            ViewBag.MauSacList = await _mauSacService.GetAllAsync();
            ViewBag.KichCoList = await _kichCoService.GetAllAsync();
            return View(new SanPhamChiTietDTO { SanPhamId = sanPhamId });
        }

        [HttpPost]
        public async Task<IActionResult> Create(SanPhamChiTietDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MauSacList = await _mauSacService.GetAllAsync();
                ViewBag.KichCoList = await _kichCoService.GetAllAsync();
                return View(dto);
            }

            await _chiTietService.CreateAsync(dto);
            return RedirectToAction("Index", new { sanPhamId = dto.SanPhamId });
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var chiTiet = await _chiTietService.GetByIdAsync(id);
            if (chiTiet == null) return NotFound();

            ViewBag.MauSacList = await _mauSacService.GetAllAsync();
            ViewBag.KichCoList = await _kichCoService.GetAllAsync();
            return View(chiTiet);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SanPhamChiTietDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MauSacList = await _mauSacService.GetAllAsync();
                ViewBag.KichCoList = await _kichCoService.GetAllAsync();
                return View(dto);
            }

            await _chiTietService.UpdateAsync(dto);
            return RedirectToAction("Index", new { sanPhamId = dto.SanPhamId });
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var chiTiet = await _chiTietService.GetByIdAsync(id);
            if (chiTiet == null) return NotFound();

            return View(chiTiet);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id, Guid sanPhamId)
        {
            await _chiTietService.DeleteAsync(id);
            return RedirectToAction("Index", new { sanPhamId });
        }
    }

}
