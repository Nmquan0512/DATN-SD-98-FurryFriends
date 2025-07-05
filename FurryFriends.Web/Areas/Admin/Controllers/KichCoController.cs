using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KichCoController : Controller
    {
        private readonly IKichCoService _kichCoService;

        public KichCoController(IKichCoService kichCoService)
        {
            _kichCoService = kichCoService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _kichCoService.GetAllAsync();
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
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return BadRequest(ModelState);
                return View(dto);
            }

            var result = await _kichCoService.CreateAsync(dto);
            if (result != null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(result);
                return RedirectToAction("Index");
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return BadRequest("Thêm kích cỡ thất bại");

            ModelState.AddModelError("", "Thêm kích cỡ thất bại!");
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
            if (!ModelState.IsValid)
                return View(dto);

            var success = await _kichCoService.UpdateAsync(id, dto);
            if (success)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Cập nhật thất bại!");
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
