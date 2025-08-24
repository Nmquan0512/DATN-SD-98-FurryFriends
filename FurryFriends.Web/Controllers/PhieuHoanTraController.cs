using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;
using FurryFriends.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Controllers
{
    public class PhieuHoanTraController : Controller
    {
		private readonly IPhieuHoanTraService _phieuHoanTraService;

		public PhieuHoanTraController(IPhieuHoanTraService phieuHoanTraService)
		{
			_phieuHoanTraService = phieuHoanTraService;
		}

		// 📌 Danh sách phiếu hoàn trả theo Hóa đơn
		[HttpGet]
		public async Task<IActionResult> Index(Guid hoaDonId)
		{
			var list = await _phieuHoanTraService.GetByHoaDonIdAsync(hoaDonId);
			ViewBag.HoaDonId = hoaDonId;
			return View(list);
		}

		// 📌 Xem chi tiết 1 phiếu hoàn trả
		[HttpGet]
		public async Task<IActionResult> Details(Guid id)
		{
			var phieu = await _phieuHoanTraService.GetByIdAsync(id);
			if (phieu == null) return NotFound();
			return View(phieu);
		}

		// 📌 Hiển thị form tạo phiếu hoàn trả
		[HttpGet]
		public IActionResult Create(Guid hoaDonId)
		{
			var model = new PhieuHoanTraViewModel
			{
				HoaDonChiTietId = hoaDonId
			};
			return View(model);
		}

		// 📌 Xử lý tạo phiếu hoàn trả
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(PhieuHoanTraViewModel model)
		{
			if (!ModelState.IsValid) return View(model);

			var success = await _phieuHoanTraService.CreateAsync(model);
			if (success)
			{
				TempData["Success"] = "Tạo phiếu hoàn trả thành công!";
				return RedirectToAction(nameof(Index), new { hoaDonId = model.HoaDonChiTietId });
			}

			TempData["Error"] = "Tạo phiếu hoàn trả thất bại!";
			return View(model);
		}

		// 📌 Cập nhật trạng thái phiếu hoàn trả
		[HttpPost]
		public async Task<IActionResult> UpdateTrangThai(Guid id, int trangThai, Guid hoaDonId)
		{
			var success = await _phieuHoanTraService.UpdateTrangThaiAsync(id, trangThai);
			if (success)
			{
				TempData["Success"] = "Cập nhật trạng thái thành công!";
			}
			else
			{
				TempData["Error"] = "Cập nhật trạng thái thất bại!";
			}
			return RedirectToAction(nameof(Index), new { hoaDonId });
		}

		// 📌 Xóa phiếu hoàn trả
		[HttpPost]
		public async Task<IActionResult> Delete(Guid id, Guid hoaDonId)
		{
			var success = await _phieuHoanTraService.DeleteAsync(id);
			if (success)
			{
				TempData["Success"] = "Xóa phiếu hoàn trả thành công!";
			}
			else
			{
				TempData["Error"] = "Xóa phiếu hoàn trả thất bại!";
			}
			return RedirectToAction(nameof(Index), new { hoaDonId });
		}
	}
}
