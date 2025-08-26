using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Filter;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdminOnly]
    public class ChucVuController : Controller
    {
        private readonly IChucVuService _chucVuService;
        private readonly IThongBaoService _thongBaoService;

        public ChucVuController(IChucVuService chucVuService, IThongBaoService thongBaoService)
        {
            _chucVuService = chucVuService;
            _thongBaoService = thongBaoService;
        }

        // GET: Admin/ChucVu
        public async Task<IActionResult> Index()
        {
            var allChucVus = await _chucVuService.GetAllAsync();
            ViewBag.TotalCount = allChucVus.Count();
            ViewBag.ActiveCount = allChucVus.Count(x => x.TrangThai);
            ViewBag.InactiveCount = allChucVus.Count(x => !x.TrangThai);
            return View(allChucVus);
        }

        // GET: Admin/ChucVu/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/ChucVu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChucVu model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _chucVuService.AddAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                // Parse JSON lỗi (ValidationProblemDetails) từ API
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

                return View(model);
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, "Lỗi gửi dữ liệu: " + ex.Message);
                return View(model);
            }
        }

        // GET: Admin/ChucVu/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var chucVu = await _chucVuService.GetByIdAsync(id);
            if (chucVu == null) return NotFound();
            return View(chucVu);
        }

        // POST: Admin/ChucVu/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ChucVu model)
        {
            if (id != model.ChucVuId) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            try
            {
                await _chucVuService.UpdateAsync(model);
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

                return View(model);
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, "Lỗi gửi dữ liệu: " + ex.Message);
                return View(model);
            }
        }

        // POST: /ChucVu/ToggleStatus/{id}
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var chucVu = await _chucVuService.GetByIdAsync(id);
                if (chucVu == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy chức vụ." });
                }

                // Toggle trạng thái
                chucVu.TrangThai = !chucVu.TrangThai;
                await _chucVuService.UpdateAsync(chucVu);
                
                var action = chucVu.TrangThai ? "kích hoạt" : "vô hiệu hóa";
                var message = $"Chức vụ '{chucVu.TenChucVu}' đã được {action} thành công.";

                // 🔔 Thêm thông báo
                var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
                {
                    TieuDe = chucVu.TrangThai ? "Kích hoạt chức vụ" : "Vô hiệu hóa chức vụ",
                    NoiDung = $"Chức vụ '{chucVu.TenChucVu}' đã được {action}",
                    Loai = "ChucVu",
                    UserName = userName,
                    NgayTao = DateTime.Now,
                    DaDoc = false
                });

                return Json(new { 
                    success = true, 
                    message = message,
                    newStatus = chucVu.TrangThai,
                    statusText = chucVu.TrangThai ? "Đang hoạt động" : "Không hoạt động",
                    statusClass = chucVu.TrangThai ? "bg-success" : "bg-secondary"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // GET: Admin/ChucVu/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var chucVu = await _chucVuService.GetByIdAsync(id);
            if (chucVu == null) return NotFound();
            return View(chucVu);
        }

        // POST: Admin/ChucVu/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var chucVu = await _chucVuService.GetByIdAsync(id);
                if (chucVu == null)
                {
                    TempData["Error"] = "Không tìm thấy chức vụ.";
                    return RedirectToAction(nameof(Index));
                }

                // Xóa mềm - đổi trạng thái thành không hoạt động
                chucVu.TrangThai = false;
                await _chucVuService.UpdateAsync(chucVu);

                TempData["Success"] = "Chức vụ đã được vô hiệu hóa thành công.";

                // 🔔 Thêm thông báo
                var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
                {
                    TieuDe = "Vô hiệu hóa chức vụ",
                    NoiDung = $"Chức vụ '{chucVu.TenChucVu}' đã được vô hiệu hóa",
                    Loai = "ChucVu",
                    UserName = userName,
                    NgayTao = DateTime.Now,
                    DaDoc = false
                });

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}