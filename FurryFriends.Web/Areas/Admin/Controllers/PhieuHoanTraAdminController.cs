using FurryFriends.Web.Services.IService;
using FurryFriends.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using FurryFriends.Web.Filter;
using FurryFriends.API.Models.DTO;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[AuthorizeAdminOnly]
	public class PhieuHoanTraAdminController : Controller
    {
		private readonly IPhieuHoanTraService _phieuHoanTraService;
		private readonly IThongBaoService _thongBaoService;

		public PhieuHoanTraAdminController(IPhieuHoanTraService phieuHoanTraService, IThongBaoService thongBaoService)
		{
			_phieuHoanTraService = phieuHoanTraService;
			_thongBaoService = thongBaoService;
		}

		// GET: Hiển thị danh sách phiếu hoàn trả (Admin xem tất cả)
		public async Task<IActionResult> Index(Guid? hoaDonId)
		{
			IEnumerable<PhieuHoanTraViewModel> danhSach;

			if (hoaDonId.HasValue)
			{
				danhSach = await _phieuHoanTraService.GetByHoaDonIdAsync(hoaDonId.Value);
			}
			else
			{
				// Trường hợp này bạn cần API trả về tất cả phiếu hoàn trả
				// Nếu chưa có API thì tạm để rỗng hoặc mock data
				danhSach = new List<PhieuHoanTraViewModel>();
			}

			return View(danhSach);
		}

		// GET: Xem chi tiết
		public async Task<IActionResult> Details(Guid id)
		{
			var phieu = await _phieuHoanTraService.GetByIdAsync(id);
			if (phieu == null) return NotFound();

			return View(phieu);
		}

		// GET: Cập nhật trạng thái
		public async Task<IActionResult> UpdateTrangThai(Guid id, int trangThai)
		{
			var phieu = await _phieuHoanTraService.GetByIdAsync(id);
			if (phieu == null) return NotFound();

			ViewBag.CurrentTrangThai = phieu.TrangThai;
			return View(phieu);
		}

		// POST: Cập nhật trạng thái
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateTrangThai(Guid id, int trangThai, IFormCollection form)
		{
			var result = await _phieuHoanTraService.UpdateTrangThaiAsync(id, trangThai);
			if (result)
			{
				return RedirectToAction(nameof(Index));
			}

			ModelState.AddModelError("", "Cập nhật trạng thái thất bại");
			return View();
		}

		// POST: /PhieuHoanTraAdmin/ToggleStatus/{id}
		[HttpPost]
		public async Task<IActionResult> ToggleStatus(Guid id)
		{
			try
			{
				var phieuHoanTra = await _phieuHoanTraService.GetByIdAsync(id);
				if (phieuHoanTra == null)
				{
					return Json(new { success = false, message = "Không tìm thấy phiếu hoàn trả." });
				}

				// Toggle trạng thái (chuyển từ int sang int)
				var newTrangThai = phieuHoanTra.TrangThai == 1 ? 0 : 1;
				var updateResult = await _phieuHoanTraService.UpdateTrangThaiAsync(id, newTrangThai);
				
				if (updateResult)
				{
					var action = newTrangThai == 1 ? "kích hoạt" : "vô hiệu hóa";
					var message = $"Phiếu hoàn trả '{phieuHoanTra.PhieuHoanTraId}' đã được {action} thành công.";

					// 🔔 Thêm thông báo
					var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
					await _thongBaoService.CreateAsync(new ThongBaoDTO
					{
						TieuDe = newTrangThai == 1 ? "Kích hoạt phiếu hoàn trả" : "Vô hiệu hóa phiếu hoàn trả",
						NoiDung = $"Phiếu hoàn trả '{phieuHoanTra.PhieuHoanTraId}' đã được {action}",
						Loai = "PhieuHoanTra",
						UserName = userName,
						NgayTao = DateTime.Now,
						DaDoc = false
					});

					return Json(new { 
						success = true, 
						message = message,
						newStatus = newTrangThai == 1,
						statusText = newTrangThai == 1 ? "Đang hoạt động" : "Không hoạt động",
						statusClass = newTrangThai == 1 ? "bg-success" : "bg-secondary"
					});
				}

				return Json(new { success = false, message = "Cập nhật trạng thái thất bại!" });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
			}
		}

		// GET: Xóa
		public async Task<IActionResult> Delete(Guid id)
		{
			var phieu = await _phieuHoanTraService.GetByIdAsync(id);
			if (phieu == null) return NotFound();

			return View(phieu);
		}

		// POST: Xác nhận xóa
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(Guid id)
		{
			var result = await _phieuHoanTraService.DeleteAsync(id);
			if (result)
			{
				return RedirectToAction(nameof(Index));
			}

			ModelState.AddModelError("", "Xóa phiếu hoàn trả thất bại");
			return View();
		}
	}
}
