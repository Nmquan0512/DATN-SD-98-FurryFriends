using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Filter;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdminOnly]

    public class ThanhPhanController : Controller
    {
        private readonly IThanhPhanService _thanhPhanService;
        private readonly IThongBaoService _thongBaoService;

        public ThanhPhanController(IThanhPhanService thanhPhanService, IThongBaoService thongBaoService)
        {
            _thanhPhanService = thanhPhanService;
            _thongBaoService = thongBaoService;
        }

        public async Task<IActionResult> Index()
        {
            var allThanhPhans = await _thanhPhanService.GetAllAsync();
            ViewBag.TotalCount = allThanhPhans.Count();
            ViewBag.ActiveCount = allThanhPhans.Count(x => x.TrangThai);
            ViewBag.InactiveCount = allThanhPhans.Count(x => !x.TrangThai);
            return View(allThanhPhans);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThanhPhanDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _thanhPhanService.CreateAsync(dto);
            if (result.Success)
            {
                TempData["success"] = "Thêm thành phần thành công!";
                var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
                {
                    TieuDe = "Thêm thành phần",
                    NoiDung = $"Thành phần '{dto.TenThanhPhan}' đã được thêm thành công.",
                    Loai = "ThanhPhan",
                    UserName = userName,
                    NgayTao = DateTime.Now,
                    DaDoc = false
                });
                return RedirectToAction("Index");
            }

            if (result.Errors != null)
            {
                foreach (var field in result.Errors)
                {
                    foreach (var error in field.Value)
                        ModelState.AddModelError(field.Key, error);
                }
            }

            return View(dto);
        }


        public async Task<IActionResult> Edit(Guid id)
        {
            var item = await _thanhPhanService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ThanhPhanDTO dto)
        {
            if (id != dto.ThanhPhanId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(dto);

            var result = await _thanhPhanService.UpdateAsync(id, dto);
            if (result.Data)
            {
                TempData["success"] = "Cập nhật thành phần thành công!";
                var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
                {
                    TieuDe = "Cập nhật thành phần",
                    NoiDung = $"Thành phần '{dto.TenThanhPhan}' đã được cập nhật",
                    Loai = "ThanhPhan",
                    UserName = userName,
                    NgayTao = DateTime.Now,
                    DaDoc = false
                });
                return RedirectToAction("Index");
            }

            if (result.Errors != null)
            {
                foreach (var field in result.Errors)
                {
                    foreach (var error in field.Value)
                        ModelState.AddModelError(field.Key, error);
                }
            }
            else
            {
                ModelState.AddModelError("", "Cập nhật thất bại!");
            }

            return View(dto);
        }

        // POST: /ThanhPhan/ToggleStatus/{id}
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var thanhPhan = await _thanhPhanService.GetByIdAsync(id);
                if (thanhPhan == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thành phần." });
                }

                // Toggle trạng thái
                thanhPhan.TrangThai = !thanhPhan.TrangThai;
                var updateResult = await _thanhPhanService.UpdateAsync(id, thanhPhan);
                
                if (updateResult.Data)
                {
                    var action = thanhPhan.TrangThai ? "kích hoạt" : "vô hiệu hóa";
                    var message = $"Thành phần '{thanhPhan.TenThanhPhan}' đã được {action} thành công.";

                    // 🔔 Thêm thông báo
                    var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                    await _thongBaoService.CreateAsync(new ThongBaoDTO
                    {
                        TieuDe = thanhPhan.TrangThai ? "Kích hoạt thành phần" : "Vô hiệu hóa thành phần",
                        NoiDung = $"Thành phần '{thanhPhan.TenThanhPhan}' đã được {action}",
                        Loai = "ThanhPhan",
                        UserName = userName,
                        NgayTao = DateTime.Now,
                        DaDoc = false
                    });

                    return Json(new { 
                        success = true, 
                        message = message,
                        newStatus = thanhPhan.TrangThai,
                        statusText = thanhPhan.TrangThai ? "Đang hoạt động" : "Không hoạt động",
                        statusClass = thanhPhan.TrangThai ? "bg-success" : "bg-secondary"
                    });
                }

                return Json(new { success = false, message = "Cập nhật trạng thái thất bại!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // GET: /ThanhPhan/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _thanhPhanService.GetByIdAsync(id);
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
                var thanhPhan = await _thanhPhanService.GetByIdAsync(id);
                if (thanhPhan == null)
                {
                    TempData["Error"] = "Không tìm thấy thành phần.";
                    return RedirectToAction(nameof(Index));
                }

                // Xóa mềm - đổi trạng thái thành không hoạt động
                thanhPhan.TrangThai = false;
                var updateResult = await _thanhPhanService.UpdateAsync(id, thanhPhan);
                
                if (updateResult.Data)
                {
                    TempData["Success"] = "Thành phần đã được vô hiệu hóa thành công.";

                    // 🔔 Thêm thông báo
                    var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                    await _thongBaoService.CreateAsync(new ThongBaoDTO
                    {
                        TieuDe = "Vô hiệu hóa thành phần",
                        NoiDung = $"Thành phần '{thanhPhan.TenThanhPhan}' đã được vô hiệu hóa",
                        Loai = "ThanhPhan",
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
