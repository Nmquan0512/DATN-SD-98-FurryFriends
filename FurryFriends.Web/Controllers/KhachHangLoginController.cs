using FurryFriends.Web.Models;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Controllers
{
    public class KhachHangLoginController : Controller
    {
		private readonly ITaiKhoanService _taiKhoanService;

		public KhachHangLoginController(ITaiKhoanService taiKhoanService)
		{
			_taiKhoanService = taiKhoanService;
		}

		[HttpGet]
		public IActionResult DangNhap()
		{
			return View(); // Mặc định sẽ tìm Views/KhachHangLogin/DangNhap.cshtml
		}

		[HttpPost]
		public async Task<IActionResult> DangNhap(LoginRequest model)
		{
			if (!ModelState.IsValid) return View(model);

			var result = await _taiKhoanService.DangNhapKhachHangAsync(model);
			if (result == null)
			{
				ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu.";
				return View(model);
			}

			// Lưu session
			HttpContext.Session.SetString("TaiKhoanId", result.TaiKhoanId.ToString());
			HttpContext.Session.SetString("Role", result.Role);
			HttpContext.Session.SetString("HoTen", result.HoTen ?? "");

			return RedirectToAction("Index", "Home"); // Sau khi đăng nhập, chuyển đến trang chính của khách hàng
		}

		[HttpGet]
		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("DangNhap");
		}
	}
}
