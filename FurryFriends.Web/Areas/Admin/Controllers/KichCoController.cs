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
