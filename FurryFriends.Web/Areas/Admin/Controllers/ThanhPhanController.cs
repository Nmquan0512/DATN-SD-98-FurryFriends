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
        public async Task<IActionResult> Create(ThanhPhanDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _thanhPhanService.CreateAsync(dto);
            if (result.Success)
            {
                TempData["success"] = "Thêm thành phần thành công!";
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
