using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using FurryFriends.Web.Filter;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdminOnly]

    public class KichCoController : Controller
    {
        private readonly IKichCoService _kichCoService;
        private readonly IThongBaoService _thongBaoService;

        public KichCoController(IKichCoService kichCoService, IThongBaoService thongBaoService)
        {
            _kichCoService = kichCoService;
            _thongBaoService = thongBaoService;
        }

        public async Task<IActionResult> Index()
        {
            var allKichCos = await _kichCoService.GetAllAsync();
            ViewBag.TotalCount = allKichCos.Count();
            ViewBag.ActiveCount = allKichCos.Count(x => x.TrangThai);
            ViewBag.InactiveCount = allKichCos.Count(x => !x.TrangThai);
            return View(allKichCos);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KichCoDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _kichCoService.CreateAsync(dto);

            if (result.Success)
            {
                var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
                {
                    TieuDe = "Kích cỡ mới",
                    NoiDung = $"Đã thêm kích cỡ \"{dto.TenKichCo}\".",
                    Loai = "KichCo",
                    UserName = tenNhanVien,
                    NgayTao = DateTime.Now,
                    DaDoc = false
                });
                TempData["success"] = "Thêm kích cỡ thành công!";
                return RedirectToAction("Index");
            }

            if (result.Errors != null)
            {
                foreach (var error in result.Errors)
                {
                    foreach (var msg in error.Value)
                        ModelState.AddModelError(error.Key, msg);
                }
            }

            return View(dto);
        }


        public async Task<IActionResult> Edit(Guid id)
        {
            var item = await _kichCoService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, KichCoDTO dto)
        {
            if (id != dto.KichCoId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(dto);

            var result = await _kichCoService.UpdateAsync(id, dto);
            if (result.Data)
            {
                var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
                {
                    TieuDe = "Cập nhật kích cỡ",
                    NoiDung = $"Đã cập nhật kích cỡ \"{dto.TenKichCo}\" (ID: {dto.KichCoId}).",
                    Loai = "KichCo",
                    UserName = tenNhanVien,
                    NgayTao = DateTime.Now,
                    DaDoc = false
                });
                TempData["success"] = "Cập nhật kích cỡ thành công!";
                return RedirectToAction("Index");
            }

            if (result.Errors != null)
            {
                foreach (var error in result.Errors)
                {
                    foreach (var msg in error.Value)
                        ModelState.AddModelError(error.Key, msg);
                }
            }
            else
            {
                ModelState.AddModelError("", "Cập nhật thất bại!");
            }

            return View(dto);
        }

        // POST: /KichCo/ToggleStatus/{id}
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var kichCo = await _kichCoService.GetByIdAsync(id);
                if (kichCo == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy kích cỡ." });
                }

                // Toggle trạng thái
                kichCo.TrangThai = !kichCo.TrangThai;
                var updateResult = await _kichCoService.UpdateAsync(id, kichCo);
                
                if (updateResult.Data)
                {
                    var action = kichCo.TrangThai ? "kích hoạt" : "vô hiệu hóa";
                    var message = $"Kích cỡ '{kichCo.TenKichCo}' đã được {action} thành công.";

                    // 🔔 Thêm thông báo
                    var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                    await _thongBaoService.CreateAsync(new ThongBaoDTO
                    {
                        TieuDe = kichCo.TrangThai ? "Kích hoạt kích cỡ" : "Vô hiệu hóa kích cỡ",
                        NoiDung = $"Kích cỡ '{kichCo.TenKichCo}' đã được {action}",
                        Loai = "KichCo",
                        UserName = userName,
                        NgayTao = DateTime.Now,
                        DaDoc = false
                    });

                    return Json(new { 
                        success = true, 
                        message = message,
                        newStatus = kichCo.TrangThai,
                        statusText = kichCo.TrangThai ? "Đang hoạt động" : "Không hoạt động",
                        statusClass = kichCo.TrangThai ? "bg-success" : "bg-secondary"
                    });
                }

                return Json(new { success = false, message = "Cập nhật trạng thái thất bại!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // GET: /KichCo/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _kichCoService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var kichCo = await _kichCoService.GetByIdAsync(id);
                if (kichCo == null)
                {
                    TempData["Error"] = "Không tìm thấy kích cỡ.";
                    return RedirectToAction(nameof(Index));
                }

                // Xóa mềm - đổi trạng thái thành không hoạt động
                kichCo.TrangThai = false;
                var updateResult = await _kichCoService.UpdateAsync(id, kichCo);
                
                if (updateResult.Data)
                {
                    TempData["Success"] = "Kích cỡ đã được vô hiệu hóa thành công.";

                    // 🔔 Thêm thông báo
                    var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                    await _thongBaoService.CreateAsync(new ThongBaoDTO
                    {
                        TieuDe = "Vô hiệu hóa kích cỡ",
                        NoiDung = $"Kích cỡ '{kichCo.TenKichCo}' đã được vô hiệu hóa",
                        Loai = "KichCo",
                        UserName = userName,
                        NgayTao = DateTime.Now,
                        DaDoc = false
                    });

                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "Vô hiệu hóa thất bại!";
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
