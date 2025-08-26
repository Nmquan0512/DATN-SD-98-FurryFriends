using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using FurryFriends.Web.Filter;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdminOnly]

    public class ChatLieuController : Controller
    {
        private readonly IChatLieuService _chatLieuService;
        private readonly IThongBaoService _thongBaoService; // 👈 thêm

        public ChatLieuController(IChatLieuService chatLieuService, IThongBaoService thongBaoService)
        {
            _chatLieuService = chatLieuService;
            _thongBaoService = thongBaoService; // 👈 gán
        }


        // GET: /ChatLieu
        public async Task<IActionResult> Index()
        {
            var allChatLieus = await _chatLieuService.GetAllAsync();
            ViewBag.TotalCount = allChatLieus.Count();
            ViewBag.ActiveCount = allChatLieus.Count(x => x.TrangThai);
            ViewBag.InactiveCount = allChatLieus.Count(x => !x.TrangThai);
            return View(allChatLieus);
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

            if (result.Success)
            {
                TempData["success"] = "Thêm chất liệu thành công!";
                var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
                {
                    TieuDe = "Thêm chất liệu",
                    NoiDung = $"Chất liệu '{dto.TenChatLieu}' đã được thêm vào hệ thống.",
                    Loai = "ChatLieu",
                    UserName = tenNhanVien,
                    NgayTao = DateTime.Now,
                    DaDoc = false
                });
                return RedirectToAction("Index");
            }

            // Đẩy lỗi từ API vào ModelState
            if (result.Errors != null)
            {
                foreach (var field in result.Errors)
                {
                    foreach (var error in field.Value)
                    {
                        ModelState.AddModelError(field.Key, error);
                    }
                }
            }

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

            var result = await _chatLieuService.UpdateAsync(id, dto);
            if (result.Data)
            {
                TempData["success"] = "Cập nhật chất liệu thành công!";
                var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
                {
                    TieuDe = "Cập nhật chất liệu",
                    NoiDung = $"Chất liệu '{dto.TenChatLieu}' đã được chỉnh sửa.",
                    Loai = "ChatLieu",
                    UserName = tenNhanVien,
                    NgayTao = DateTime.Now,
                    DaDoc = false
                });
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

        // POST: /ChatLieu/ToggleStatus/{id}
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var chatLieu = await _chatLieuService.GetByIdAsync(id);
                if (chatLieu == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy chất liệu." });
                }

                // Toggle trạng thái
                chatLieu.TrangThai = !chatLieu.TrangThai;
                var updateResult = await _chatLieuService.UpdateAsync(id, chatLieu);
                
                if (updateResult.Data)
                {
                    var action = chatLieu.TrangThai ? "kích hoạt" : "vô hiệu hóa";
                    var message = $"Chất liệu '{chatLieu.TenChatLieu}' đã được {action} thành công.";

                    // 🔔 Thêm thông báo
                    var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                    await _thongBaoService.CreateAsync(new ThongBaoDTO
                    {
                        TieuDe = chatLieu.TrangThai ? "Kích hoạt chất liệu" : "Vô hiệu hóa chất liệu",
                        NoiDung = $"Chất liệu '{chatLieu.TenChatLieu}' đã được {action}",
                        Loai = "ChatLieu",
                        UserName = userName,
                        NgayTao = DateTime.Now,
                        DaDoc = false
                    });

                    return Json(new { 
                        success = true, 
                        message = message,
                        newStatus = chatLieu.TrangThai,
                        statusText = chatLieu.TrangThai ? "Đang hoạt động" : "Không hoạt động",
                        statusClass = chatLieu.TrangThai ? "bg-success" : "bg-secondary"
                    });
                }

                return Json(new { success = false, message = "Cập nhật trạng thái thất bại!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
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
            try
            {
                var chatLieu = await _chatLieuService.GetByIdAsync(id);
                if (chatLieu == null)
                {
                    TempData["Error"] = "Không tìm thấy chất liệu.";
                    return RedirectToAction(nameof(Index));
                }

                // Xóa mềm - đổi trạng thái thành không hoạt động
                chatLieu.TrangThai = false;
                var updateResult = await _chatLieuService.UpdateAsync(id, chatLieu);
                
                if (updateResult.Data)
                {
                    TempData["Success"] = "Chất liệu đã được vô hiệu hóa thành công.";

                    // 🔔 Thêm thông báo
                    var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                    await _thongBaoService.CreateAsync(new ThongBaoDTO
                    {
                        TieuDe = "Vô hiệu hóa chất liệu",
                        NoiDung = $"Chất liệu '{chatLieu.TenChatLieu}' đã được vô hiệu hóa",
                        Loai = "ChatLieu",
                        UserName = userName,
                        NgayTao = DateTime.Now,
                        DaDoc = false
                    });

                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "Vô hiệu hóa thất bại!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}