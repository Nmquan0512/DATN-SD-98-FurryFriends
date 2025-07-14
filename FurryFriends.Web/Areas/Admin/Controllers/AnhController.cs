using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AnhController : Controller
    {
        private readonly IAnhService _anhService;

        public AnhController(IAnhService anhService)
        {
            _anhService = anhService;
        }

        // GET: /Admin/Anh
        public async Task<IActionResult> Index()
        {
            var list = await _anhService.GetAllAsync();
            return View(list);
        }

        // POST: /Admin/Anh/Upload (AJAX)
        [HttpPost]
        // Không dùng [ValidateAntiForgeryToken] nếu upload qua FormData & JS
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "❌ File ảnh không hợp lệ!"
                });
            }

            var result = await _anhService.UploadAsync(file);
            if (result == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "❌ Tải ảnh thất bại hoặc định dạng không hỗ trợ!"
                });
            }

            return Ok(new
            {
                success = true,
                message = "✅ Ảnh đã được tải lên thành công!",
                data = result
            });
        }

        // POST: /Admin/Anh/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                TempData["error"] = "❌ ID ảnh không hợp lệ!";
                return RedirectToAction("Index");
            }

            var success = await _anhService.DeleteAsync(id);
            if (success)
            {
                TempData["success"] = "🗑️ Ảnh đã được xóa!";
            }
            else
            {
                TempData["error"] = "❌ Không tìm thấy ảnh để xóa hoặc xóa thất bại!";
            }

            return RedirectToAction("Index");
        }
    }
}
