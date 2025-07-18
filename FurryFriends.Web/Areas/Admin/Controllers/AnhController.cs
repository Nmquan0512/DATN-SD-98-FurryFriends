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
            Console.WriteLine("📄 [Anh/Index] Load danh sách ảnh...");
            var list = await _anhService.GetAllAsync();
            return View(list);
        }

        // POST: /Admin/Anh/Upload (AJAX)
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, Guid sanPhamChiTietId)
        {
            Console.WriteLine("📤 [Anh/Upload] Bắt đầu upload ảnh...");
            if (file == null || file.Length == 0)
            {
                Console.WriteLine("❌ File null hoặc rỗng!");
                return BadRequest(new
                {
                    success = false,
                    message = "❌ File không hợp lệ!"
                });
            }

            if (sanPhamChiTietId == Guid.Empty)
            {
                Console.WriteLine("❌ sanPhamChiTietId không hợp lệ!");
                return BadRequest(new
                {
                    success = false,
                    message = "❌ ID sản phẩm chi tiết không hợp lệ!"
                });
            }

            Console.WriteLine($"📝 Tên file: {file.FileName}, Kích thước: {file.Length} bytes, SanPhamChiTietId: {sanPhamChiTietId}");

            var result = await _anhService.UploadAsync(file, sanPhamChiTietId);

            if (result == null)
            {
                Console.WriteLine("❌ Upload thất bại hoặc định dạng không hỗ trợ.");
                return BadRequest(new
                {
                    success = false,
                    message = "❌ Tải ảnh thất bại hoặc định dạng không hỗ trợ!"
                });
            }

            Console.WriteLine("✅ Upload ảnh thành công!");
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
            Console.WriteLine($"🗑️ [Anh/Delete] Yêu cầu xóa ảnh ID: {id}");

            if (id == Guid.Empty)
            {
                Console.WriteLine("❌ ID ảnh không hợp lệ!");
                TempData["error"] = "❌ ID ảnh không hợp lệ!";
                return RedirectToAction("Index");
            }

            var success = await _anhService.DeleteAsync(id);
            if (success)
            {
                Console.WriteLine("✅ Ảnh đã được xóa!");
                TempData["success"] = "🗑️ Ảnh đã được xóa!";
            }
            else
            {
                Console.WriteLine("❌ Không tìm thấy ảnh để xóa hoặc xóa thất bại!");
                TempData["error"] = "❌ Không tìm thấy ảnh để xóa hoặc xóa thất bại!";
            }

            return RedirectToAction("Index");
        }
    }
}
