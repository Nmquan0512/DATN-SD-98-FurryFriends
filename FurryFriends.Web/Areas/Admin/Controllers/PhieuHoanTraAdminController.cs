using FurryFriends.Web.Services.IService;
using FurryFriends.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using FurryFriends.Web.Filter;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[AuthorizeAdminOnly]
	public class PhieuHoanTraAdminController : Controller
    {
		private readonly IPhieuHoanTraService _phieuHoanTraService;

		public PhieuHoanTraAdminController(IPhieuHoanTraService phieuHoanTraService)
		{
			_phieuHoanTraService = phieuHoanTraService;
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
