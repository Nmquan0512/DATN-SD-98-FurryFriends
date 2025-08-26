using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using FurryFriends.Web.Filter;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
        [AuthorizeAdminOnly]

    public class MauSacController : Controller
    {
        private readonly IMauSacService _mauSacService;
        private readonly IThongBaoService _thongBaoService;
        public MauSacController(IMauSacService mauSacService, IThongBaoService thongBaoService)
        {
            _mauSacService = mauSacService;
            _thongBaoService = thongBaoService;
        }

        // GET: /Admin/MauSac
        public async Task<IActionResult> Index()
        {
            var allMauSacs = await _mauSacService.GetAllAsync();
            ViewBag.TotalCount = allMauSacs.Count();
            ViewBag.ActiveCount = allMauSacs.Count(x => x.TrangThai);
            ViewBag.InactiveCount = allMauSacs.Count(x => !x.TrangThai);
            return View(allMauSacs);
        }

        // GET: /Admin/MauSac/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MauSacDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _mauSacService.CreateAsync(dto);
            if (result.Success)
            {
                var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
                {
                    TieuDe = "Màu sắc mới",
                    NoiDung = $"Đã thêm màu sắc \"{dto.TenMau}\".",
                    Loai = "MauSac",
                    UserName = tenNhanVien,
                    NgayTao = DateTime.Now,
                    DaDoc = false
                });
                TempData["success"] = "Thêm màu sắc thành công!";
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


        // GET: /Admin/MauSac/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var item = await _mauSacService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // POST: /Admin/MauSac/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, MauSacDTO dto)
        {
            if (id != dto.MauSacId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(dto);

            var result = await _mauSacService.UpdateAsync(id, dto);
            if (result.Data)
            {
                var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
                {
                    TieuDe = "Cập nhật màu sắc",
                    NoiDung = $"Đã cập nhật màu sắc \"{dto.TenMau}\" (ID: {dto.MauSacId}).",
                    Loai = "MauSac",
                    UserName = tenNhanVien,
                    NgayTao = DateTime.Now,
                    DaDoc = false
                });
                TempData["success"] = "Cập nhật màu sắc thành công!";
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

        // POST: /Admin/MauSac/ToggleStatus/{id}
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var mauSac = await _mauSacService.GetByIdAsync(id);
                if (mauSac == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy màu sắc." });
                }

                // Toggle trạng thái
                mauSac.TrangThai = !mauSac.TrangThai;
                var updateResult = await _mauSacService.UpdateAsync(id, mauSac);
                
                if (updateResult.Data)
                {
                    var action = mauSac.TrangThai ? "kích hoạt" : "vô hiệu hóa";
                    var message = $"Màu sắc '{mauSac.TenMau}' đã được {action} thành công.";

                    // 🔔 Thêm thông báo
                    var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                    await _thongBaoService.CreateAsync(new ThongBaoDTO
                    {
                        TieuDe = mauSac.TrangThai ? "Kích hoạt màu sắc" : "Vô hiệu hóa màu sắc",
                        NoiDung = $"Màu sắc '{mauSac.TenMau}' đã được {action}",
                        Loai = "MauSac",
                        UserName = userName,
                        NgayTao = DateTime.Now,
                        DaDoc = false
                    });

                    return Json(new { 
                        success = true, 
                        message = message,
                        newStatus = mauSac.TrangThai,
                        statusText = mauSac.TrangThai ? "Đang hoạt động" : "Không hoạt động",
                        statusClass = mauSac.TrangThai ? "bg-success" : "bg-secondary"
                    });
                }

                return Json(new { success = false, message = "Cập nhật trạng thái thất bại!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // GET: /Admin/MauSac/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _mauSacService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // POST: /Admin/MauSac/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var mauSac = await _mauSacService.GetByIdAsync(id);
                if (mauSac == null)
                {
                    TempData["Error"] = "Không tìm thấy màu sắc.";
                    return RedirectToAction(nameof(Index));
                }

                // Xóa mềm - đổi trạng thái thành không hoạt động
                mauSac.TrangThai = false;
                var updateResult = await _mauSacService.UpdateAsync(id, mauSac);
                
                if (updateResult.Data)
                {
                    TempData["Success"] = "Màu sắc đã được vô hiệu hóa thành công.";

                    // 🔔 Thêm thông báo
                    var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                    await _thongBaoService.CreateAsync(new ThongBaoDTO
                    {
                        TieuDe = "Vô hiệu hóa màu sắc",
                        NoiDung = $"Màu sắc '{mauSac.TenMau}' đã được vô hiệu hóa",
                        Loai = "MauSac",
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
