using FurryFriends.API.Models;
using FurryFriends.Web.Services;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DotGiamGiasController : Controller
    {
        private readonly IDotGiamGiaService _dotGiamGiaService;
        private readonly ILichSuThaoTacService _lichSuService;

        public DotGiamGiasController(
            IDotGiamGiaService dotGiamGiaService,
            ILichSuThaoTacService lichSuService)
        {
            _dotGiamGiaService = dotGiamGiaService;
            _lichSuService = lichSuService;
        }



        // GET: DotGiamGia
        public async Task<IActionResult> Index()
        {
            var danhSachGiamGia = await GetDanhSachGiamGiaAsync();
            var danhSachSanPham = await GetDanhSachSanPhamAsync();

            ViewBag.GiamGiaList = new SelectList(danhSachGiamGia, "GiamGiaId", "TenGiamGia");
            ViewBag.SanPhamList = new SelectList(danhSachSanPham, "SanPhamId", "TenSanPham");

            var list = await _dotGiamGiaService.GetAllAsync();
            return View(list);
        }

        // GET: DotGiamGia/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var dot = await _dotGiamGiaService.GetByIdAsync(id);
            if (dot == null) return NotFound();
            return View(dot);
        }

        // GET: DotGiamGia/Create
        public async Task<IActionResult> Create()
        {
            var danhSachGiamGia = await GetDanhSachGiamGiaAsync();
            var danhSachSanPham = await GetDanhSachSanPhamAsync();

            ViewBag.GiamGiaList = new SelectList(danhSachGiamGia, "GiamGiaId", "TenGiamGia");
            ViewBag.SanPhamList = new SelectList(danhSachSanPham, "SanPhamId", "TenSanPham");
            return View();
        }

        // POST: DotGiamGia/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DotGiamGiaSanPham dot)
        {
            if (!ModelState.IsValid)
            {
                var danhSachGiamGia = await GetDanhSachGiamGiaAsync();
                var danhSachSanPham = await GetDanhSachSanPhamAsync();

                ViewBag.GiamGiaList = new SelectList(danhSachGiamGia, "GiamGiaId", "TenGiamGia");
                ViewBag.SanPhamList = new SelectList(danhSachSanPham, "SanPhamId", "TenSanPham");

                return View(dot);
            }

            var success = await _dotGiamGiaService.AddAsync(dot);
            if (success)
            {
                var log = new LichSuThaoTac
                {
                    TaiKhoan = User.Identity?.Name,
                    HanhDong = "Thêm đợt giảm giá",
                    NoiDung = $"Thêm đợt giảm giá cho sản phẩm ID: {dot.SanPhamId}, Giảm giá ID: {dot.GiamGiaId}",
                    ThoiGian = DateTime.Now
                };
                await _lichSuService.AddLogAsync(log);

                return RedirectToAction(nameof(Index));
            }


            ModelState.AddModelError("", "Tạo đợt giảm giá thất bại.");

            // ✅ Load lại đúng dữ liệu nếu thất bại
            var danhSachGiamGia2 = await GetDanhSachGiamGiaAsync();
            var danhSachSanPham2 = await GetDanhSachSanPhamAsync();

            ViewBag.GiamGiaList = new SelectList(danhSachGiamGia2, "GiamGiaId", "TenGiamGia");
            ViewBag.SanPhamList = new SelectList(danhSachSanPham2, "SanPhamId", "TenSanPham");

            return View(dot);
        }

        // GET: DotGiamGia/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {

            var danhSachGiamGia = await GetDanhSachGiamGiaAsync();
            var danhSachSanPham = await GetDanhSachSanPhamAsync();

            ViewBag.GiamGiaList = new SelectList(danhSachGiamGia, "GiamGiaId", "TenGiamGia");
            ViewBag.SanPhamList = new SelectList(danhSachSanPham, "SanPhamId", "TenSanPham");
            var dot = await _dotGiamGiaService.GetByIdAsync(id);
            if (dot == null) return NotFound();
            return View(dot);
        }

        // POST: DotGiamGia/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, DotGiamGiaSanPham dot)
        {
            if (id != dot.DotGiamGiaSanPhamId) return BadRequest();

            if (!ModelState.IsValid)
            {
                // Load lại danh sách cho dropdown khi ModelState không hợp lệ
                var danhSachGiamGia = await GetDanhSachGiamGiaAsync();
                var danhSachSanPham = await GetDanhSachSanPhamAsync();

                ViewBag.GiamGiaList = new SelectList(danhSachGiamGia, "GiamGiaId", "TenGiamGia", dot.GiamGiaId);
                ViewBag.SanPhamList = new SelectList(danhSachSanPham, "SanPhamId", "TenSanPham", dot.SanPhamId);

                return View(dot);
            }

            var success = await _dotGiamGiaService.UpdateAsync(id, dot);
            if (success)
            {
                var log = new LichSuThaoTac
                {
                    TaiKhoan = User.Identity?.Name,
                    HanhDong = "Cập nhật đợt giảm giá",
                    NoiDung = $"Cập nhật đợt giảm giá ID: {dot.DotGiamGiaSanPhamId}, Sản phẩm ID: {dot.SanPhamId}, Giảm giá ID: {dot.GiamGiaId}",
                    ThoiGian = DateTime.Now
                };
                await _lichSuService.AddLogAsync(log);

                return RedirectToAction(nameof(Index));
            }


            // Nếu thất bại, cũng load lại danh sách
            ModelState.AddModelError("", "Cập nhật thất bại.");
            var danhSachGiamGia2 = await GetDanhSachGiamGiaAsync();
            var danhSachSanPham2 = await GetDanhSachSanPhamAsync();

            ViewBag.GiamGiaList = new SelectList(danhSachGiamGia2, "GiamGiaId", "TenGiamGia", dot.GiamGiaId);
            ViewBag.SanPhamList = new SelectList(danhSachSanPham2, "SanPhamId", "TenSanPham", dot.SanPhamId);

            return View(dot);
        }

        // GET: DotGiamGia/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var dot = await _dotGiamGiaService.GetByIdAsync(id);
            if (dot == null) return NotFound();
            return View(dot);
        }

        // POST: DotGiamGia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var dot = await _dotGiamGiaService.GetByIdAsync(id);
            var success = await _dotGiamGiaService.DeleteAsync(id);
            if (success)
            {
                var log = new LichSuThaoTac
                {
                    TaiKhoan = User.Identity?.Name,
                    HanhDong = "Xóa đợt giảm giá",
                    NoiDung = $"Đã xóa đợt giảm giá ID: {dot?.DotGiamGiaSanPhamId}, Sản phẩm ID: {dot?.SanPhamId}, Giảm giá ID: {dot?.GiamGiaId}",
                    ThoiGian = DateTime.Now
                };
                await _lichSuService.AddLogAsync(log);

                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Xóa thất bại.");
            return View("Delete", dot);
        }

        private async Task<List<SanPham>> GetDanhSachSanPhamAsync()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7289/api/"); // sửa URL nếu cần

            var response = await client.GetAsync("SanPhams"); // endpoint API lấy danh sách sản phẩm
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<List<SanPham>>();
                return data ?? new List<SanPham>();
            }
            return new List<SanPham>();
        }
        private async Task<List<GiamGia>> GetDanhSachGiamGiaAsync()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7289/api/"); // sửa URL nếu cần
            var response = await client.GetAsync("GiamGia"); // endpoint API lấy danh sách giảm giá
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<List<GiamGia>>();
                return data ?? new List<GiamGia>();
            }
            return new List<GiamGia>();
        }
    }
}