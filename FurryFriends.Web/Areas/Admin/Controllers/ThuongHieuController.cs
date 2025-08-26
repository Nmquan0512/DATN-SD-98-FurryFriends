using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Filter;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
   [AuthorizeAdminOnly]

    public class ThuongHieuController : Controller
    {
        private readonly IThuongHieuService _thuongHieuService;
        private readonly IThongBaoService _thongBaoService;

        public ThuongHieuController(IThuongHieuService thuongHieuService, IThongBaoService thongBaoService)
        {
            _thuongHieuService = thuongHieuService;
            _thongBaoService = thongBaoService;
        }

        // GET: /ThuongHieu
        public async Task<IActionResult> Index()
        {
            var allThuongHieus = await _thuongHieuService.GetAllAsync();
            ViewBag.TotalCount = allThuongHieus.Count();
            ViewBag.ActiveCount = allThuongHieus.Count(x => x.TrangThai);
            ViewBag.InactiveCount = allThuongHieus.Count(x => !x.TrangThai);
            return View(allThuongHieus);
        }

        // GET: /ThuongHieu/Create
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThuongHieuDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _thuongHieuService.CreateAsync(dto);
            if (result.Success)
            {
                TempData["success"] = "Thêm thương hiệu thành công!";
                var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
                {
                    TieuDe = "Thêm thương hiệu",
                    NoiDung = $"Thương hiệu '{dto.TenThuongHieu}' đã được thêm thành công.",
                    Loai = "ThuongHieu",
                    UserName = userName,
                    NgayTao = DateTime.Now,
                    DaDoc = false
                });
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
            var item = await _thuongHieuService.GetByIdAsync(id);
            if (item == null)
                return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ThuongHieuDTO dto)
        {
            if (id != dto.ThuongHieuId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(dto);

            var result = await _thuongHieuService.UpdateAsync(id, dto);
            if (result.Data)
            {
                TempData["success"] = "Cập nhật thương hiệu thành công!";
                var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
                {
                    TieuDe = "Cập nhật thương hiệu",
                    NoiDung = $"Thương hiệu '{dto.TenThuongHieu}' đã được cập nhật",
                    Loai = "ThuongHieu",
                    UserName = userName,
                    NgayTao = DateTime.Now,
                    DaDoc = false
                });
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

        // POST: /ThuongHieu/ToggleStatus/{id}
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var thuongHieu = await _thuongHieuService.GetByIdAsync(id);
                if (thuongHieu == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thương hiệu." });
                }

                // Toggle trạng thái
                thuongHieu.TrangThai = !thuongHieu.TrangThai;
                var updateResult = await _thuongHieuService.UpdateAsync(id, thuongHieu);
                
                if (updateResult.Data)
                {
                    var action = thuongHieu.TrangThai ? "kích hoạt" : "vô hiệu hóa";
                    var message = $"Thương hiệu '{thuongHieu.TenThuongHieu}' đã được {action} thành công.";

                    // 🔔 Thêm thông báo
                    var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                    await _thongBaoService.CreateAsync(new ThongBaoDTO
                    {
                        TieuDe = thuongHieu.TrangThai ? "Kích hoạt thương hiệu" : "Vô hiệu hóa thương hiệu",
                        NoiDung = $"Thương hiệu '{thuongHieu.TenThuongHieu}' đã được {action}",
                        Loai = "ThuongHieu",
                        UserName = userName,
                        NgayTao = DateTime.Now,
                        DaDoc = false
                    });

                    return Json(new { 
                        success = true, 
                        message = message,
                        newStatus = thuongHieu.TrangThai,
                        statusText = thuongHieu.TrangThai ? "Đang hoạt động" : "Không hoạt động",
                        statusClass = thuongHieu.TrangThai ? "bg-success" : "bg-secondary"
                    });
                }

                return Json(new { success = false, message = "Cập nhật trạng thái thất bại!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // GET: /ThuongHieu/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _thuongHieuService.GetByIdAsync(id);
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
                var thuongHieu = await _thuongHieuService.GetByIdAsync(id);
                if (thuongHieu == null)
                {
                    TempData["Error"] = "Không tìm thấy thương hiệu.";
                    return RedirectToAction(nameof(Index));
                }

                // Xóa mềm - đổi trạng thái thành không hoạt động
                thuongHieu.TrangThai = false;
                var updateResult = await _thuongHieuService.UpdateAsync(id, thuongHieu);
                
                if (updateResult.Data)
                {
                    TempData["Success"] = "Thương hiệu đã được vô hiệu hóa thành công.";

                    // 🔔 Thêm thông báo
                    var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                    await _thongBaoService.CreateAsync(new ThongBaoDTO
                    {
                        TieuDe = "Vô hiệu hóa thương hiệu",
                        NoiDung = $"Thương hiệu '{thuongHieu.TenThuongHieu}' đã được vô hiệu hóa",
                        Loai = "ThuongHieu",
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
