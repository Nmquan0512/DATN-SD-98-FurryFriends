using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Models;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Controllers
{
    public class ThongTinCaNhanController : Controller
    {
		private readonly IThongTinCaNhanService _thongTinCaNhanService;

		public ThongTinCaNhanController(IThongTinCaNhanService thongTinCaNhanService)
		{
			_thongTinCaNhanService = thongTinCaNhanService;
		}

		// Hiển thị thông tin cá nhân
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var taiKhoanIdStr = HttpContext.Session.GetString("TaiKhoanId");
			if (string.IsNullOrEmpty(taiKhoanIdStr) || !Guid.TryParse(taiKhoanIdStr, out var taiKhoanId))
				return RedirectToAction("DangNhap", "Auth");

			var model = await _thongTinCaNhanService.GetThongTinCaNhanAsync(taiKhoanId);
			if (model == null)
			{
				TempData["Error"] = "Không tìm thấy thông tin cá nhân.";
				return RedirectToAction("DangNhap", "Auth");
			}

			return View(model);
		}

		// Cập nhật thông tin cá nhân
		[HttpPost]
		public async Task<IActionResult> Index(ThongTinCaNhanViewModel model)
		{
			var taiKhoanIdStr = HttpContext.Session.GetString("TaiKhoanId");
			if (string.IsNullOrEmpty(taiKhoanIdStr) || !Guid.TryParse(taiKhoanIdStr, out var taiKhoanId))
				return RedirectToAction("DangNhap", "Auth");

			model.TaiKhoanId = taiKhoanId;

			var capNhat = await _thongTinCaNhanService.UpdateThongTinCaNhanAsync(taiKhoanId, model);
			if (capNhat)
			{
				TempData["Message"] = "Cập nhật thông tin thành công.";
				return RedirectToAction("Index");
			}

			TempData["Error"] = "Cập nhật thông tin thất bại.";
			return View(model);
		}

		// Hiển thị form đổi mật khẩu riêng
		[HttpGet]
		public IActionResult DoiMatKhau()
		{
			return View();
		}

		// Xử lý đổi mật khẩu
		[HttpPost]
		public async Task<IActionResult> DoiMatKhau(DoiMatKhauViewModel model)
		{
			var taiKhoanIdStr = HttpContext.Session.GetString("TaiKhoanId");
			if (string.IsNullOrEmpty(taiKhoanIdStr) || !Guid.TryParse(taiKhoanIdStr, out var taiKhoanId))
				return RedirectToAction("DangNhap", "Auth");

			if (model.MatKhauMoi != model.XacNhanMatKhauMoi)
			{
				ModelState.AddModelError("XacNhanMatKhauMoi", "Mật khẩu xác nhận không khớp.");
				return View(model);
			}

			var doiMK = await _thongTinCaNhanService.DoiMatKhauAsync(taiKhoanId, model.MatKhauCu, model.MatKhauMoi);
			if (!doiMK)
			{
				ModelState.AddModelError("MatKhauCu", "Mật khẩu cũ không chính xác.");
				return View(model);
			}

			TempData["Message"] = "Đổi mật khẩu thành công.";
			return RedirectToAction("Index");
		}
	}
}
