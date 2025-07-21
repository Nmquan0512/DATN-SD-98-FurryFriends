using FurryFriends.API.Models;
using FurryFriends.Web.Filter;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdminOnly]
    public class TaiKhoanController : Controller
    {
        public readonly ITaiKhoanService _taiKhoanService;

        public TaiKhoanController(ITaiKhoanService taiKhoanService)
        {
            _taiKhoanService = taiKhoanService;
        }
        public async Task<IActionResult> Index()
        {
            var taiKhoans = await _taiKhoanService.GetAllAsync();
            ViewBag.TotalCount = taiKhoans.Count();
            ViewBag.ActiveCount = taiKhoans.Count(x => x.TrangThai);
            ViewBag.InactiveCount = taiKhoans.Count(x => !x.TrangThai);
            return View(taiKhoans);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TaiKhoan taiKhoan)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    taiKhoan.TaiKhoanId = Guid.NewGuid();
                    taiKhoan.NgayTaoTaiKhoan = DateTime.Now;
                    taiKhoan.TrangThai = taiKhoan.TrangThai;
                    taiKhoan.HoaDons = null;
                    taiKhoan.KhachHang = null;
                    taiKhoan.KhachHangId = null;

                    await _taiKhoanService.AddAsync(taiKhoan);

                    TempData["Success"] = "Tài khoản đã được tạo thành công.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ValidationException ex)
                {
                    // Parse lỗi JSON
                    var problemDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<ValidationProblemDetails>(ex.Message);
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
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                }
            }

            return View(taiKhoan);
        }
        public async Task<IActionResult> Edit(Guid id)
        {
            var taiKhoan = await _taiKhoanService.GetByIdAsync(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }
            return View(taiKhoan);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, TaiKhoan taiKhoan)
        {
            if (id != taiKhoan.TaiKhoanId)
                return BadRequest("ID không khớp.");

            if (ModelState.IsValid)
            {
                try
                {
                    var taiKhoanCu = await _taiKhoanService.GetByIdAsync(id);
                    if (string.IsNullOrWhiteSpace(taiKhoan.Password) && taiKhoanCu != null)
                    {
                        taiKhoan.Password = taiKhoanCu.Password;
                    }

                    await _taiKhoanService.UpdateAsync(taiKhoan);

                    TempData["Success"] = "Tài khoản đã được cập nhật thành công.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ValidationException ex)
                {
                    // Parse ValidationProblemDetails
                    var problemDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<ValidationProblemDetails>(ex.Message);
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
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                }
            }

            return View(taiKhoan);
        }
        public async Task<IActionResult> Delete(Guid id)
        {
            var taiKhoan = await _taiKhoanService.GetByIdAsync(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }
            return View(taiKhoan);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _taiKhoanService.DeleteAsync(id);
                TempData["Success"] = "Tài khoản đã được xóa thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi: {ex.Message}");
            }
            var taiKhoan = await _taiKhoanService.GetByIdAsync(id);
            return View(taiKhoan);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return RedirectToAction(nameof(Index));

            try
            {
                var taiKhoans = await _taiKhoanService.FindByUserNameAsync(userName);

                if (taiKhoans == null || !taiKhoans.Any())
                {
                    TempData["Error"] = "Không tìm thấy tài khoản.";
                    return RedirectToAction(nameof(Index));
                }

                return View("Index", taiKhoans); // ✅ vì taiKhoans là IEnumerable<TaiKhoan>
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Index", new List<TaiKhoan>());
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Index", new List<TaiKhoan>());
            }
        }
    }
}