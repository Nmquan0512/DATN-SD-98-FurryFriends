using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ThuongHieuController : Controller
    {
        private readonly IThuongHieuService _thuongHieuService;

        public ThuongHieuController(IThuongHieuService thuongHieuService)
        {
            _thuongHieuService = thuongHieuService;
        }

        // GET: /ThuongHieu
        public async Task<IActionResult> Index()
        {
            var list = await _thuongHieuService.GetAllAsync();
            return View(list);
        }

        // GET: /ThuongHieu/Create
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThuongHieuDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result = await _thuongHieuService.CreateAsync(dto);
            if (result != null)
            {
                TempData["success"] = "Thêm thương hiệu thành công!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Thêm thương hiệu thất bại!");
            return View(dto);
        }



        // GET: /ThuongHieu/Edit/{id}
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

            var success = await _thuongHieuService.UpdateAsync(id, dto);
            if (success)
            {
                TempData["success"] = "Cập nhật thương hiệu thành công!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Cập nhật thất bại!");
            return View(dto);
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
            var success = await _thuongHieuService.DeleteAsync(id);
            if (success)
            {
                TempData["success"] = "Xóa thương hiệu thành công!";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Xóa thất bại!";
            return RedirectToAction("Delete", new { id });
        }
    }
}
