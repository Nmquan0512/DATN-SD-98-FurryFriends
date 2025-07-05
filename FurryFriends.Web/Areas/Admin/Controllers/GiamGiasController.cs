using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GiamGiasController : Controller
    {
        private readonly IGiamGiaService _giamGiaService;
        private readonly ILichSuThaoTacService _lichSuService;

        public GiamGiasController(IGiamGiaService giamGiaService, ILichSuThaoTacService lichSuService)
        {
            _giamGiaService = giamGiaService;
            _lichSuService = lichSuService;
        }

        // GET: GiamGia
        public async Task<IActionResult> Index()
        {
            var list = await _giamGiaService.GetAllAsync();
            return View(list);
        }

        // GET: GiamGia/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var giamGia = await _giamGiaService.GetByIdAsync(id);
            if (giamGia == null) return NotFound();
            return View(giamGia);
        }

        // GET: GiamGia/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GiamGia/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GiamGia giamGia)
        {
            if (!ModelState.IsValid) return View(giamGia);

            var success = await _giamGiaService.CreateAsync(giamGia);
            if (success)
            {
                // ✅ Ghi vào lịch sử thao tác
                var log = new LichSuThaoTac
                {
                    TaiKhoan = User.Identity?.Name, // hoặc truyền user từ session nếu bạn lưu khác
                    HanhDong = "Thêm giảm giá",
                    NoiDung = $"Đã thêm giảm giá: {giamGia.TenGiamGia} ({giamGia.PhanTramKhuyenMai}% từ {giamGia.NgayBatDau:dd/MM/yyyy} đến {giamGia.NgayKetThuc:dd/MM/yyyy})",
                    ThoiGian = DateTime.Now
                };
                await _lichSuService.AddLogAsync(log);

                TempData["success"] = "Thêm giảm giá thành công!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Tạo giảm giá thất bại.");
            return RedirectToAction("Index");
        }

        // GET: GiamGia/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var giamGia = await _giamGiaService.GetByIdAsync(id);
            if (giamGia == null) return NotFound();
            return View(giamGia);
        }

        // POST: GiamGia/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, GiamGia giamGia)
        {
            if (id != giamGia.GiamGiaId) return BadRequest();
            if (!ModelState.IsValid) return View(giamGia);

            var success = await _giamGiaService.UpdateAsync(id, giamGia);
            if (success)
            {
                // ✅ Ghi log thao tác cập nhật
                var log = new LichSuThaoTac
                {
                    TaiKhoan = User.Identity?.Name,
                    HanhDong = "Cập nhật giảm giá",
                    NoiDung = $"Đã cập nhật giảm giá: {giamGia.TenGiamGia} ({giamGia.PhanTramKhuyenMai}% từ {giamGia.NgayBatDau:dd/MM/yyyy} đến {giamGia.NgayKetThuc:dd/MM/yyyy})",
                    ThoiGian = DateTime.Now
                };
                await _lichSuService.AddLogAsync(log);

                TempData["success"] = "Cập nhật giảm giá thành công!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Cập nhật thất bại.");
            return View(giamGia);
        }

        // GET: GiamGia/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var giamGia = await _giamGiaService.GetByIdAsync(id);
            if (giamGia == null) return NotFound();
            return View(giamGia);
        }

        // POST: GiamGia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var giamGia = await _giamGiaService.GetByIdAsync(id);
            var success = await _giamGiaService.DeleteAsync(id);
            if (success)
            {
                // ✅ Ghi log thao tác xóa
                var log = new LichSuThaoTac
                {
                    TaiKhoan = User.Identity?.Name,
                    HanhDong = "Xóa giảm giá",
                    NoiDung = $"Đã xóa giảm giá: {giamGia?.TenGiamGia} ({giamGia?.PhanTramKhuyenMai}% từ {giamGia?.NgayBatDau:dd/MM/yyyy} đến {giamGia?.NgayKetThuc:dd/MM/yyyy})",
                    ThoiGian = DateTime.Now
                };
                await _lichSuService.AddLogAsync(log);

                TempData["success"] = "Xóa giảm giá thành công!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Xóa thất bại.");
            return View("Delete", giamGia);
        }

        [HttpGet("api/giamgia/{id}/phantram")]
        public async Task<IActionResult> GetPhanTramKhuyenMaiAsync(Guid id)
        {
            var giamGia = await _giamGiaService.GetByIdAsync(id); // hoặc _giamGiaRepo nếu bạn gọi repo trực tiếp
            if (giamGia == null)
                return NotFound();

            return Ok(giamGia.PhanTramKhuyenMai);
        }
    }
}