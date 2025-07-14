using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Models;
using FurryFriends.Web.Services.IService;
using FurryFriends.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Collections.Generic;
using FurryFriends.API.Services.IServices;
using WebThongTinCaNhanService = FurryFriends.Web.Services.IService.IThongTinCaNhanService;

namespace FurryFriends.Web.Controllers
{
    public class ThongTinCaNhanController : Controller
    {
		private readonly WebThongTinCaNhanService _thongTinCaNhanService;
		private readonly IDiaChiKhachHangService _diaChiKhachHangService;
		private readonly ITaiKhoanService _taiKhoanService;

		public ThongTinCaNhanController(WebThongTinCaNhanService thongTinCaNhanService, IDiaChiKhachHangService diaChiKhachHangService, ITaiKhoanService taiKhoanService)
		{
			_thongTinCaNhanService = thongTinCaNhanService;
			_diaChiKhachHangService = diaChiKhachHangService;
			_taiKhoanService = taiKhoanService;
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
				TempData["Error"] = "Không tìm thấy thông tin cá nhân. Vui lòng liên hệ hỗ trợ!";
				return RedirectToAction("Index", "Home");
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

		// CRUD Địa chỉ khách hàng
		[HttpGet]
		public IActionResult AddDiaChi()
		{
			return View(new DiaChiKhachHangViewModel());
		}

		[HttpPost]
		public async Task<IActionResult> AddDiaChi(DiaChiKhachHangViewModel model)
		{
			if (!ModelState.IsValid) return View(model);
			var taiKhoanIdStr = HttpContext.Session.GetString("TaiKhoanId");
			if (string.IsNullOrEmpty(taiKhoanIdStr) || !Guid.TryParse(taiKhoanIdStr, out var taiKhoanId))
				return RedirectToAction("DangNhap", "Auth");

			var taiKhoan = await _taiKhoanService.GetByIdAsync(taiKhoanId);
			if (taiKhoan == null || taiKhoan.KhachHangId == null)
			{
				TempData["Error"] = "Không tìm thấy thông tin khách hàng!";
				return RedirectToAction("DanhSachDiaChi");
			}

			var diaChi = new FurryFriends.API.Models.DiaChiKhachHang
			{
				DiaChiId = Guid.NewGuid(),
				TenDiaChi = model.TenDiaChi,
				MoTa = model.MoTa,
				PhuongXa = model.PhuongXa,
				ThanhPho = model.ThanhPho,
				SoDienThoai = model.SoDienThoai,
				GhiChu = model.GhiChu,
				TrangThai = model.LaMacDinh ? 1 : 0,
				NgayTao = DateTime.Now,
				NgayCapNhat = DateTime.Now,
				KhachHangId = taiKhoan.KhachHangId.Value
			};
			await _diaChiKhachHangService.AddAsync(diaChi);
			TempData["Message"] = "Thêm địa chỉ thành công!";
			return RedirectToAction("DanhSachDiaChi");
		}

		[HttpGet]
		public async Task<IActionResult> EditDiaChi(Guid id)
		{
			var diaChi = await _diaChiKhachHangService.GetByIdAsync(id);
			if (diaChi == null)
			{
				TempData["Error"] = "Không tìm thấy địa chỉ!";
				return RedirectToAction("DanhSachDiaChi");
			}
			var model = new DiaChiKhachHangViewModel
			{
				DiaChiId = diaChi.DiaChiId,
				TenDiaChi = diaChi.TenDiaChi,
				MoTa = diaChi.MoTa,
				PhuongXa = diaChi.PhuongXa,
				ThanhPho = diaChi.ThanhPho,
				SoDienThoai = diaChi.SoDienThoai,
				GhiChu = diaChi.GhiChu,
				LaMacDinh = diaChi.TrangThai == 1
			};
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> EditDiaChi(DiaChiKhachHangViewModel model)
		{
			if (!ModelState.IsValid) return View(model);
			var taiKhoanIdStr = HttpContext.Session.GetString("TaiKhoanId");
			if (string.IsNullOrEmpty(taiKhoanIdStr) || !Guid.TryParse(taiKhoanIdStr, out var taiKhoanId))
				return RedirectToAction("DangNhap", "Auth");
			var taiKhoan = await _taiKhoanService.GetByIdAsync(taiKhoanId);
			if (taiKhoan == null || taiKhoan.KhachHangId == null)
			{
				TempData["Error"] = "Không tìm thấy thông tin khách hàng!";
				return RedirectToAction("DanhSachDiaChi");
			}
			var diaChi = new FurryFriends.API.Models.DiaChiKhachHang
			{
				DiaChiId = model.DiaChiId,
				TenDiaChi = model.TenDiaChi,
				MoTa = model.MoTa,
				PhuongXa = model.PhuongXa,
				ThanhPho = model.ThanhPho,
				SoDienThoai = model.SoDienThoai,
				GhiChu = model.GhiChu,
				TrangThai = model.LaMacDinh ? 1 : 0,
				NgayCapNhat = DateTime.Now,
				KhachHangId = taiKhoan.KhachHangId.Value
			};
			await _diaChiKhachHangService.UpdateAsync(diaChi);
			TempData["Message"] = "Cập nhật địa chỉ thành công!";
			return RedirectToAction("DanhSachDiaChi");
		}

		[HttpPost]
		public async Task<IActionResult> DeleteDiaChi(Guid id)
		{
			await _diaChiKhachHangService.DeleteAsync(id);
			TempData["Message"] = "Xóa địa chỉ thành công!";
			return RedirectToAction("DanhSachDiaChi");
		}

		[HttpPost]
		public async Task<IActionResult> SetDefaultDiaChi(Guid id)
		{
			// Lấy địa chỉ cần set mặc định
			var diaChi = await _diaChiKhachHangService.GetByIdAsync(id);
			if (diaChi == null)
			{
				TempData["Error"] = "Không tìm thấy địa chỉ!";
				return RedirectToAction("DanhSachDiaChi");
			}

			// Lấy tất cả địa chỉ của khách hàng
			var all = await _diaChiKhachHangService.GetByKhachHangIdAsync(diaChi.KhachHangId);
			foreach (var dc in all)
			{
				dc.TrangThai = (dc.DiaChiId == id) ? 1 : 0;
				await _diaChiKhachHangService.UpdateAsync(dc);
			}

			TempData["Message"] = "Đã chọn địa chỉ mặc định!";
			return RedirectToAction("DanhSachDiaChi");
		}

		[HttpGet]
		public async Task<IActionResult> DanhSachDiaChi()
		{
			var taiKhoanIdStr = HttpContext.Session.GetString("TaiKhoanId");
			if (string.IsNullOrEmpty(taiKhoanIdStr) || !Guid.TryParse(taiKhoanIdStr, out var taiKhoanId))
				return RedirectToAction("DangNhap", "Auth");

			var taiKhoan = await _taiKhoanService.GetByIdAsync(taiKhoanId);
			if (taiKhoan == null || taiKhoan.KhachHangId == null)
			{
				TempData["Error"] = "Không tìm thấy thông tin khách hàng!";
				return RedirectToAction("Index");
			}

			var model = await _thongTinCaNhanService.GetDanhSachDiaChiAsync(taiKhoan.KhachHangId.Value);
			return View(model);
		}
	}
}
