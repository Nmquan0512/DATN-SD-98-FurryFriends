using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ChucVuController : Controller
    {
		public ChucVuController(IChucVuService chucVuService)
		{
			_chucVuService = chucVuService;
		}

		private readonly IChucVuService _chucVuService;
		// GET: ChucVu
		public async Task<IActionResult> Index()
		{
			var chucVus = await _chucVuService.GetAllAsync();
			return View(chucVus);
		}

		// GET: ChucVu/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: ChucVu/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ChucVu chucVu)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _chucVuService.AddAsync(chucVu);
					TempData["Success"] = "Chức vụ đã được tạo thành công.";
					return RedirectToAction(nameof(Index));
				}
				catch (ArgumentException ex)
				{
					ModelState.AddModelError("", ex.Message);
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", $"Lỗi: {ex.Message}");
				}
			}
			return View(chucVu);
		}

		// GET: ChucVu/Edit/{id}
		public async Task<IActionResult> Edit(Guid id)
		{
			var chucVu = await _chucVuService.GetByIdAsync(id);
			if (chucVu == null)
				return NotFound();
			return View(chucVu);
		}

		// POST: ChucVu/Edit/{id}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Guid id, ChucVu chucVu)
		{
			if (id != chucVu.ChucVuId)
				return BadRequest("ID không khớp.");

			if (ModelState.IsValid)
			{
				try
				{
					await _chucVuService.UpdateAsync(chucVu);
					TempData["Success"] = "Chức vụ đã được cập nhật thành công.";
					return RedirectToAction(nameof(Index));
				}
				catch (ArgumentException ex)
				{
					ModelState.AddModelError("", ex.Message);
				}
				catch (KeyNotFoundException ex)
				{
					ModelState.AddModelError("", ex.Message);
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", $"Lỗi: {ex.Message}");
				}
			}
			return View(chucVu);
		}

		// GET: ChucVu/Delete/{id}
		public async Task<IActionResult> Delete(Guid id)
		{
			var chucVu = await _chucVuService.GetByIdAsync(id);
			if (chucVu == null)
				return NotFound();
			return View(chucVu);
		}

		// POST: ChucVu/Delete/{id}
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(Guid id)
		{
			try
			{
				await _chucVuService.DeleteAsync(id);
				TempData["Success"] = "Chức vụ đã được xóa thành công.";
				return RedirectToAction(nameof(Index));
			}
			catch (KeyNotFoundException ex)
			{
				ModelState.AddModelError("", ex.Message);
			}
			catch (InvalidOperationException ex)
			{
				ModelState.AddModelError("", ex.Message);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"Lỗi: {ex.Message}");
			}
			var chucVu = await _chucVuService.GetByIdAsync(id);
			return View(chucVu);
		}

		// POST: ChucVu/Search
		[HttpPost]
		public async Task<IActionResult> Search(string tenChucVu)
		{
			if (string.IsNullOrWhiteSpace(tenChucVu))
				return RedirectToAction(nameof(Index));

			try
			{
				var chucVus = await _chucVuService.FindByTenChucVuAsync(tenChucVu);
				return View("Index", chucVus);
			}
			catch (ArgumentException ex)
			{
				ModelState.AddModelError("", ex.Message);
				return View("Index", new List<ChucVu>());
			}
		}
	}
}
