using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KhachHangController : Controller
    {
        private readonly IKhachHangService _khachHangService;

        public KhachHangController(IKhachHangService khachHangService)
        {
            _khachHangService = khachHangService;
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
            if (id == Guid.Empty)
            {
                TempData["error"] = "ID khách hàng không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            try
            {
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhachHang model)
        {
            if (!ModelState.IsValid)
                return View(model);

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

        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                TempData["error"] = "ID khách hàng không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            var khachHang = await _khachHangService.GetKhachHangByIdAsync(id);
            if (khachHang == null)
            {
                TempData["error"] = "Không tìm thấy khách hàng";
                return RedirectToAction(nameof(Index));
            }

            return View(khachHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(KhachHang model)
        {
            if (!ModelState.IsValid)
                return View(model);

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

        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                TempData["error"] = "ID khách hàng không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _khachHangService.DeleteKhachHangAsync(id);
                TempData["success"] = "Xóa khách hàng thành công!";
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
