using FurryFriends.API.Models;
using FurryFriends.Web.Filter;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdminOnly]
    public class VouchersController : Controller
    {
        private readonly IVoucherService _voucherService;

        public VouchersController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        // GET: Admin/Voucher
        public async Task<IActionResult> Index()
        {
            var vouchers = await _voucherService.GetAllAsync();
            return View(vouchers);
        }

        // GET: Admin/Voucher/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var voucher = await _voucherService.GetByIdAsync(id);
            if (voucher == null)
                return NotFound();

            return View(voucher);
        }

        // GET: Admin/Voucher/Create
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: Admin/Voucher/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Voucher voucher)
        {
            if (!ModelState.IsValid)
            {
                return View(voucher);
            }

            voucher.NgayTao = DateTime.Now;

            try
            {
                await _voucherService.CreateAsync(voucher);
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                // Phân tích lỗi validation trả về
                var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(ex.Message);

                if (problemDetails?.Errors != null)
                {
                    foreach (var error in problemDetails.Errors)
                    {
                        foreach (var msg in error.Value)
                        {
                            ModelState.AddModelError(error.Key, msg);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Lỗi xác thực.");
                }

                return View(voucher);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Lỗi: {ex.Message}");
                return View(voucher);
            }
        }

        // GET: Admin/Voucher/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var voucher = await _voucherService.GetByIdAsync(id);
            if (voucher == null)
                return NotFound();
            return View(voucher);
        }

        // POST: Admin/Voucher/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Voucher voucher)
        {
            if (!ModelState.IsValid)
            {
                return View(voucher);
            }

            voucher.NgayCapNhat = DateTime.Now;

            try
            {
                await _voucherService.UpdateAsync(id, voucher);
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(ex.Message);

                if (problemDetails?.Errors != null)
                {
                    foreach (var error in problemDetails.Errors)
                    {
                        foreach (var msg in error.Value)
                        {
                            ModelState.AddModelError(error.Key, msg);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Lỗi xác thực.");
                }

                return View(voucher);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Lỗi: {ex.Message}");
                return View(voucher);
            }
        }


        // GET: Admin/Voucher/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var voucher = await _voucherService.GetByIdAsync(id);
            if (voucher == null)
                return NotFound();

            return View(voucher);
        }

        // POST: Admin/Voucher/DeleteConfirmed/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var success = await _voucherService.DeleteAsync(id);
                if (success)
                {
                    TempData["success"] = "Xóa voucher thành công!";
                }
                else
                {
                    TempData["error"] = "Có lỗi xảy ra khi xóa voucher.";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Có lỗi xảy ra: {ex.Message}";
            }
            return RedirectToAction(nameof(Index), new { area = "Admin" });
        }
    }
}