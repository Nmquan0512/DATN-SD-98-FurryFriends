using FurryFriends.Web.Services.IService;
using FurryFriends.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class PhieuHoanTraController : Controller
	{
		private readonly IPhieuHoanTraService _service;

		public PhieuHoanTraController(IPhieuHoanTraService service)
		{
			_service = service;
		}

		// Danh sách phiếu hoàn
		public async Task<IActionResult> Index()
		{
			var list = await _service.GetAllAsync();
			return View(list);
		}

		// Xem chi tiết
		public async Task<IActionResult> Details(Guid id)
		{
			var phieu = await _service.GetByIdAsync(id);
			if (phieu == null) return NotFound();
			return View(phieu);
		}

		// Duyệt (chỉ đổi trạng thái)
		public async Task<IActionResult> Edit(Guid id)
		{
			var phieu = await _service.GetByIdAsync(id);
			if (phieu == null) return NotFound();

			// Trả về model edit chỉ để Hiển thị readonly + đổi trạng thái
			var vm = new PhieuHoanTraUpdateRequest
			{
				SoLuongHoan = phieu.SoLuongHoan,     // readonly ở view
				LyDoHoanTra = phieu.LyDoHoanTra,     // readonly ở view
				TrangThai = phieu.TrangThai        // only field to change
			};

			// Có thể cần hiển thị thêm thông tin ở ViewBag
			ViewBag.Header = phieu;
			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Guid id, PhieuHoanTraUpdateRequest request)
		{
			// Chỉ cho phép đổi trạng thái → đảm bảo không nhận thay đổi khác:
			var current = await _service.GetByIdAsync(id);
			if (current == null) return NotFound();

			// Khóa cứng các trường khác để tránh bị sửa ngoài ý muốn
			var toUpdate = new PhieuHoanTraUpdateRequest
			{
				SoLuongHoan = current.SoLuongHoan,
				LyDoHoanTra = current.LyDoHoanTra,
				TrangThai = request.TrangThai      // chỉ field cho phép
			};

			var ok = await _service.UpdateAsync(id, toUpdate);
			if (ok)
			{
				TempData["Success"] = "Cập nhật trạng thái phiếu hoàn thành công.";
				return RedirectToAction(nameof(Index));
			}

			ModelState.AddModelError(string.Empty, "Cập nhật thất bại.");
			ViewBag.Header = current;
			return View(request);
		}

		// ❌ Không có Delete. Nếu lỡ có route cũ gọi vào, có thể trả 404:
		// public IActionResult Delete(Guid id) => NotFound();
	}
}
