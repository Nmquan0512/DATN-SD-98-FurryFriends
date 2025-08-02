using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    public class HinhThucThanhToansController : Controller
    {
        private readonly IHinhThucThanhToanService _hinhThucThanhToanService;

        public HinhThucThanhToansController(IHinhThucThanhToanService hinhThucThanhToanService)
        {
            _hinhThucThanhToanService = hinhThucThanhToanService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.HinhThucThanhToanList = await _hinhThucThanhToanService.GetAllAsync();
            // Các phần giỏ hàng khác
            return View();
        }

    }
}
