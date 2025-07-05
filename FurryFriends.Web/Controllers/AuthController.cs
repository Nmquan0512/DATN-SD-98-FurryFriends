using FurryFriends.Web.Models;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Controllers
{
	
	public class AuthController : Controller
    {
		private readonly ITaiKhoanService _taiKhoanService;

		public AuthController(ITaiKhoanService taiKhoanService)
		{
			_taiKhoanService = taiKhoanService;
		}

		// GET: /Auth/DangNhap
		[HttpGet]
		public IActionResult DangNhap()
		{
			return View(); // Views/Auth/DangNhap.cshtml
		}

		// POST: /Auth/DangNhap
		[HttpPost]
		public async Task<IActionResult> DangNhap(LoginRequest model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var result = await _taiKhoanService.DangNhapAdminAsync(model);
			if (result == null)
			{
				ViewBag.Error = "Sai tài khoản hoặc mật khẩu, hoặc không có quyền.";
				return View(model);
			}

			// ✅ Lưu thông tin đăng nhập vào session
			HttpContext.Session.SetString("TaiKhoanId", result.TaiKhoanId.ToString());
			HttpContext.Session.SetString("Role", result.Role);
			HttpContext.Session.SetString("HoTen", result.HoTen ?? "");

			// ✅ Điều hướng theo quyền
			if (result.Role == "Admin")
			{
				return RedirectToAction("Index", "TaiKhoan", new { area = "Admin" });
			}
			else if (result.Role == "NhanVien")
			{
				TempData["Message"] = "Bạn không có quyền truy cập khu vực Admin.";
				return RedirectToAction("Index", "Home"); // hoặc trang khác tùy bạn
			}

			// Nếu không phải Admin/Nhân viên (phòng trường hợp lỗi)
			TempData["Error"] = "Quyền không xác định.";
			return RedirectToAction("DangNhap");
		}

		// GET: /Auth/Logout
		[HttpGet]
		public IActionResult Logout()
		{
			// Xóa toàn bộ session (bao gồm Role, HoTen, TaiKhoanId, ...)
			HttpContext.Session.Clear();

			// Chuyển hướng về trang đăng nhập
			return RedirectToAction("DangNhap", "Auth");
		}
	}
}
