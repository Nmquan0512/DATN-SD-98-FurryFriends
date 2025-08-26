using FurryFriends.Web.Filter;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdminOnly]
    public class ThongBaoController : Controller
    {
        private readonly IThongBaoService _thongBaoService;

        public ThongBaoController(IThongBaoService thongBaoService)
        {
            _thongBaoService = thongBaoService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
        {
            var thongBaos = await _thongBaoService.GetAllAsync();
            
            // Phân trang
            var totalCount = thongBaos.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var pagedThongBaos = thongBaos
                .OrderByDescending(tb => tb.NgayTao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;
            ViewBag.PageSize = pageSize;

            return View(pagedThongBaos);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            try
            {
                await _thongBaoService.MarkAsReadAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                await _thongBaoService.MarkAllAsReadAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetNotificationCount()
        {
            try
            {
                var thongBaos = await _thongBaoService.GetAllAsync();
                var unreadCount = thongBaos.Count(tb => !tb.DaDoc);
                
                // Format count: if > 100, show "99+"
                var displayCount = unreadCount > 100 ? "99+" : unreadCount.ToString();
                
                return Json(new { 
                    success = true, 
                    count = unreadCount,
                    displayCount = displayCount
                });
            }
            catch (Exception ex)
            {
                // Log lỗi để debug
                System.Diagnostics.Debug.WriteLine($"GetNotificationCount error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRecentNotifications()
        {
            try
            {
                var thongBaos = await _thongBaoService.GetAllAsync();
                var recentNotifications = thongBaos
                    .OrderByDescending(tb => tb.NgayTao)
                    .Take(5)
                    .Select(tb => new
                    {
                        tb.ThongBaoId,
                        TieuDe = tb.NoiDung, // Sử dụng NoiDung làm TieuDe
                        tb.NoiDung,
                        tb.Loai,
                        tb.UserName,
                        tb.NgayTao,
                        tb.DaDoc,
                        FormattedDate = tb.NgayTao.ToString("dd/MM/yyyy HH:mm")
                    })
                    .ToList();

                return Json(new { success = true, notifications = recentNotifications });
            }
            catch (Exception ex)
            {
                // Log lỗi để debug
                System.Diagnostics.Debug.WriteLine($"GetRecentNotifications error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
} 