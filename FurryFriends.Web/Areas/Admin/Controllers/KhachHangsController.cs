using System.Net.Http;
using System.Text;
using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KhachHangsController : Controller
    {
        private readonly IKhachHangService _khachHangService;
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly IHttpClientFactory _clientFactory;

        public KhachHangsController(IKhachHangService khachHangService, ITaiKhoanService taiKhoanService, IHttpClientFactory clientFactory)
        {
            _khachHangService = khachHangService;
            _taiKhoanService = taiKhoanService;
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var khachHangs = await _khachHangService.GetAllAsync();
            return View(khachHangs);
        }

        // GET: Admin/KhachHangs/Create
        public async Task<IActionResult> Create()
        {
            var taiKhoans = await _taiKhoanService.GetAllTaiKhoanAsync();
            ViewBag.TaiKhoanList = new SelectList(taiKhoans, "TaiKhoanId", "TenDangNhap");
            return View();
        }

        // POST: Admin/KhachHangs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhachHang khachHang)
        {
            var taiKhoans = await _taiKhoanService.GetAllTaiKhoanAsync();
            ViewBag.TaiKhoanList = new SelectList(taiKhoans, "TaiKhoanId", "TenDangNhap", khachHang.TaiKhoanId);
            if (!ModelState.IsValid) return View(khachHang);

            var success = await _khachHangService.CreateAsync(khachHang);
            if (success) return RedirectToAction(nameof(Index));
            return View(khachHang);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var khachHang = await _khachHangService.GetByIdAsync(id);
            if (khachHang == null) return NotFound();
            return View(khachHang);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var khachHang = await _khachHangService.GetByIdAsync(id);
            if (khachHang == null) return NotFound();
            var taiKhoans = await _taiKhoanService.GetAllTaiKhoanAsync();
            ViewBag.TaiKhoanList = new SelectList(taiKhoans, "TaiKhoanId", "TenDangNhap", khachHang.TaiKhoanId);
            return View(khachHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, KhachHang model)
        {
            var taiKhoans = await _taiKhoanService.GetAllTaiKhoanAsync();
            ViewBag.TaiKhoanList = new SelectList(taiKhoans, "TaiKhoanId", "TenDangNhap", model.TaiKhoanId);
            if (id != model.KhachHangId) return BadRequest();
            if (!ModelState.IsValid) return View(model);
            await _khachHangService.UpdateAsync(model.KhachHangId, model);
            TempData["success"] = "Cập nhật khách hàng thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var khachHang = await _khachHangService.GetByIdAsync(id);
            if (khachHang == null) return NotFound();
            return View(khachHang);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _khachHangService.DeleteAsync(id);
            TempData["success"] = "Xóa khách hàng thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
} 