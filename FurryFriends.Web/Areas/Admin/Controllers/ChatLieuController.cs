using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ChatLieuController : Controller
    {
        private readonly IChatLieuService _chatLieuService;

        public ChatLieuController(IChatLieuService chatLieuService)
        {
            _chatLieuService = chatLieuService;
        }

        // GET: /ChatLieu
        public async Task<IActionResult> Index()
        {
            var list = await _chatLieuService.GetAllAsync();
            return View(list);
        }

        // GET: /ChatLieu/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChatLieuDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result = await _chatLieuService.CreateAsync(dto);
            if (result != null)
            {
                TempData["success"] = "Thêm chất liệu thành công!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Thêm chất liệu thất bại!");
            return View(dto);
        }

  


        // GET: /ChatLieu/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var item = await _chatLieuService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // POST: /ChatLieu/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ChatLieuDTO dto)
        {
            if (id != dto.ChatLieuId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(dto);

            var success = await _chatLieuService.UpdateAsync(id, dto);
            if (success)
            {
                TempData["success"] = "Cập nhật chất liệu thành công!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Cập nhật thất bại!");
            return View(dto);
        }

        // GET: /ChatLieu/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _chatLieuService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // POST: /ChatLieu/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var success = await _chatLieuService.DeleteAsync(id);
            if (success)
            {
                TempData["success"] = "Xóa chất liệu thành công!";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Xóa thất bại!";
            return RedirectToAction("Delete", new { id });
        }
    }
}