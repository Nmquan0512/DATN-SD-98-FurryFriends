using FurryFriends.API.Models;
using FurryFriends.Web.Controllers;
using FurryFriends.Web.Services;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KhachHangsController : Controller
    {
        private readonly IKhachHangService _khachHangService;
        private readonly ITaiKhoanService _taiKhoanService;

        public KhachHangsController(IKhachHangService khachHangService, ITaiKhoanService taiKhoanService)
        {
            _khachHangService = khachHangService;
            _taiKhoanService = taiKhoanService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var khachHangs = await _khachHangService.GetAllKhachHangAsync();
                return View(khachHangs);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(new List<KhachHang>());
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    TempData["error"] = "ID khách hàng không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                var khachHang = await _khachHangService.GetKhachHangByIdAsync(id);
                if (khachHang == null)
                {
                    TempData["error"] = $"Không tìm thấy khách hàng với ID: {id}";
                    return RedirectToAction(nameof(Index));
                }

                return View(khachHang);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    TempData["error"] = "ID khách hàng không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                await _khachHangService.DeleteKhachHangAsync(id);
                TempData["success"] = "Xóa khách hàng thành công!";
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/KhachHang/Create
        public async Task<IActionResult> Create()
        {
            var taiKhoans = await _taiKhoanService.GetAllTaiKhoanAsync();
            ViewBag.TaiKhoanList = new SelectList(taiKhoans, "TaiKhoanId", "TenDangNhap");
            return View();
        }

        // POST: Admin/KhachHang/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhachHang model)
        {
            var taiKhoans = await _taiKhoanService.GetAllTaiKhoanAsync();
            ViewBag.TaiKhoanList = new SelectList(taiKhoans, "TaiKhoanId", "TenDangNhap");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _khachHangService.AddKhachHangAsync(model);
                TempData["success"] = "Thêm khách hàng thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(model);
            }
        }

        // GET: Admin/KhachHang/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var khachHang = await _khachHangService.GetKhachHangByIdAsync(id);
            if (khachHang == null)
            {
                TempData["error"] = "Không tìm thấy khách hàng";
                return RedirectToAction(nameof(Index));
            }

            var taiKhoans = await _taiKhoanService.GetAllTaiKhoanAsync();
            ViewBag.TaiKhoanList = new SelectList(taiKhoans, "TaiKhoanId", "TenDangNhap");

            return View(khachHang);
        }

        // POST: Admin/KhachHang/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(KhachHang model)
        {
            var taiKhoans = await _taiKhoanService.GetAllTaiKhoanAsync();
            ViewBag.TaiKhoanList = new SelectList(taiKhoans, "TaiKhoanId", "TenDangNhap", model.TaiKhoanId);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _khachHangService.UpdateKhachHangAsync(model);
                TempData["success"] = "Cập nhật khách hàng thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(model);
            }
        }
    }
}

