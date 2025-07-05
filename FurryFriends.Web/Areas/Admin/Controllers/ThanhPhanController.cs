using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ThanhPhanController : Controller
    {
        private readonly IThanhPhanService _thanhPhanService;

        public ThanhPhanController(IThanhPhanService thanhPhanService)
        {
            _thanhPhanService = thanhPhanService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _thanhPhanService.GetAllAsync();
            return View(list);
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
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return BadRequest(ModelState);
                return View(dto);
            }

            var result = await _thanhPhanService.CreateAsync(dto);
            if (result != null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(result);
                return RedirectToAction("Index");
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return BadRequest("Thêm thành phần thất bại");

            ModelState.AddModelError("", "Thêm thành phần thất bại!");
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
            if (!ModelState.IsValid)
                return View(dto);

            var success = await _thanhPhanService.UpdateAsync(id, dto);
            if (success)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Cập nhật thất bại!");
            return View(dto);
        }

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
            var success = await _thanhPhanService.DeleteAsync(id);
            if (success)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Xóa thất bại!");
            return RedirectToAction("Delete", new { id });
        }
    }
}
