using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LichSuThaoTacsController : Controller
    {
        private readonly ILichSuThaoTacService _lichSuService;

        public LichSuThaoTacsController(ILichSuThaoTacService lichSuService)
        {
            _lichSuService = lichSuService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRecent()
        {
            var logs = await _lichSuService.GetRecentLogsAsync(5); // lấy 5 log mới nhất
            return PartialView("_ThongBaoPartial", logs);
        }
    }
}
