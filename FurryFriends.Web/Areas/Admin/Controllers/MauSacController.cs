using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MauSacController : Controller
    {
        private readonly IMauSacService _mauSacService;

        public MauSacController(IMauSacService mauSacService)
        {
            _mauSacService = mauSacService;
        }

        // GET: /Admin/MauSac
        public async Task<IActionResult> Index()
        {
            var list = await _mauSacService.GetAllAsync();
            return View(list);
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
            var success = await _mauSacService.DeleteAsync(id);
            if (success)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Xóa thất bại!");
            return RedirectToAction("Delete", new { id });
        }
    }
}
