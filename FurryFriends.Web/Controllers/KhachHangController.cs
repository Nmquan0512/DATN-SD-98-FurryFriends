using FurryFriends.API.Models;
using FurryFriends.Web.Service.IService;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace FurryFriends.Web.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly IKhachHangService _khachHangService;

        public KhachHangController(IKhachHangService khachHangService)
        {
            _khachHangService = khachHangService;
        }

        public async Task<IActionResult> Index()
        {
            var khachHangs = await _khachHangService.GetAllAsync();
            return View(khachHangs);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var kh = await _khachHangService.GetByIdAsync(id);
            if (kh == null) return NotFound();
            return View(kh);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(KhachHang khachHang)
        {
            if (!ModelState.IsValid) return View(khachHang);

            var success = await _khachHangService.CreateAsync(khachHang);
            if (success) return RedirectToAction(nameof(Index));
            return View(khachHang);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var kh = await _khachHangService.GetByIdAsync(id);
            if (kh == null) return NotFound();
            return View(kh);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, KhachHang khachHang)
        {
            if (id != khachHang.KhachHangId) return BadRequest();
            if (!ModelState.IsValid) return View(khachHang);

            var success = await _khachHangService.UpdateAsync(id, khachHang);
            if (success) return RedirectToAction(nameof(Index));
            return View(khachHang);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var kh = await _khachHangService.GetByIdAsync(id);
            if (kh == null) return NotFound();
            return View(kh);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var success = await _khachHangService.DeleteAsync(id);
            if (success) return RedirectToAction(nameof(Index));
            return View();
        }
    }
}
