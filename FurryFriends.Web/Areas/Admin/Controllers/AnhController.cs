using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.IO;
using FurryFriends.Web.Filter;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdminOnly]
    public class AnhController : Controller
    {
        private readonly IAnhService _anhService;
        private readonly IThongBaoService _thongBaoService;

        public AnhController(IAnhService anhService, IThongBaoService thongBaoService)
        {
            _anhService = anhService;
            _thongBaoService = thongBaoService;
        }

        // GET: /Admin/Anh
        public async Task<IActionResult> Index()
        {
            Console.WriteLine("📄 [Anh/Index] Load danh sách ảnh...");
            var list = await _anhService.GetAllAsync();
            return View(list);
        }

        // GET: /Admin/Anh/GetAll (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _anhService.GetAllAsync();
                return Json(list);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // POST: /Admin/Anh/Upload (AJAX)
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
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

            var result = await _anhService.UploadAsync(file, null);

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
            var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
            await _thongBaoService.CreateAsync(new ThongBaoDTO
            {
                TieuDe = "Thêm ảnh mới",
                NoiDung = $"Ảnh '{result.TenAnh}' đã được tải lên hệ thống.",
                Loai = "Anh",
                UserName = tenNhanVien,
                NgayTao = DateTime.Now,
                DaDoc = false
            });
            return Ok(new
            {
                success = true,
                message = "✅ Ảnh đã được tải lên thành công!",
                data = result
            });
        }

        // POST: /Anh/ToggleStatus/{id}
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var anh = await _anhService.GetByIdAsync(id);
                if (anh == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy ảnh." });
                }

                // Toggle trạng thái
                anh.TrangThai = !anh.TrangThai;
                var updateResult = await _anhService.UpdateAsync(id, anh);
                
                if (updateResult)
                {
                    var action = anh.TrangThai ? "kích hoạt" : "vô hiệu hóa";
                    var message = $"Ảnh '{anh.TenAnh}' đã được {action} thành công.";

                    // 🔔 Thêm thông báo
                    var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                    await _thongBaoService.CreateAsync(new ThongBaoDTO
                    {
                        TieuDe = anh.TrangThai ? "Kích hoạt ảnh" : "Vô hiệu hóa ảnh",
                        NoiDung = $"Ảnh '{anh.TenAnh}' đã được {action}",
                        Loai = "Anh",
                        UserName = userName,
                        NgayTao = DateTime.Now,
                        DaDoc = false
                    });

                    return Json(new { 
                        success = true, 
                        message = message,
                        newStatus = anh.TrangThai,
                        statusText = anh.TrangThai ? "Đang hoạt động" : "Không hoạt động",
                        statusClass = anh.TrangThai ? "bg-success" : "bg-secondary"
                    });
                }

                return Json(new { success = false, message = "Cập nhật trạng thái thất bại!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // GET: /Anh/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _anhService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }
    }
}
