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
            var list = await _kichCoService.GetAllAsync();
            ViewBag.TotalCount = list.Count();
            ViewBag.ActiveCount = list.Count(x => x.TrangThai);
            ViewBag.InactiveCount = list.Count(x => !x.TrangThai);
            return View(list);
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
            var success = await _kichCoService.DeleteAsync(id);
            if (success)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Xóa thất bại!");
            return RedirectToAction("Delete", new { id });
        }
    }
}
