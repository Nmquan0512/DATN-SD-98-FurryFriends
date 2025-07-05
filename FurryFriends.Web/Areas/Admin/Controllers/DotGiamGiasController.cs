using FurryFriends.API.Models;
using FurryFriends.Web.Services;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DotGiamGiasController : Controller
    {
        private readonly IDotGiamGiaService _dotGiamGiaService;
        private readonly IHttpClientFactory _clientFactory;
        //private readonly IGiamGiaService _giamGiaService;
        //private readonly ISanPhamService _sanPhamService;

        public DotGiamGiasController(
            IDotGiamGiaService dotGiamGiaService,
            IHttpClientFactory clientFactory
            /*IGiamGiaService giamGiaService*/)
        //ISanPhamService sanPhamService)
        {
            _dotGiamGiaService = dotGiamGiaService;
            _clientFactory = clientFactory;
            //_giamGiaService = giamGiaService;
            //_sanPhamService = sanPhamService;
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
                // Gửi thông báo
                var thongBao = new {
                    TieuDe = "Thêm đợt giảm giá mới",
                    NoiDung = $"Đợt giảm giá cho sản phẩm ID {dot.SanPhamId} vừa được thêm.",
                    Loai = "DotGiamGia",
                    UserName = User.Identity?.Name ?? "admin"
                };
                var client = _clientFactory.CreateClient();
                var json = JsonConvert.SerializeObject(thongBao);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PostAsync("https://localhost:7289/api/ThongBao", content);

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
            if (success) return RedirectToAction(nameof(Index));

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
            var success = await _dotGiamGiaService.DeleteAsync(id);
            if (success) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Xóa thất bại.");
            var dot = await _dotGiamGiaService.GetByIdAsync(id);
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

        [HttpGet("api/giamgia/{id}/phantram")]
        public async Task<IActionResult> GetPhanTramKhuyenMaiAsync(Guid id)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7289/api/");
            var response = await client.GetAsync($"GiamGia/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var giamGia = await response.Content.ReadFromJsonAsync<GiamGia>();
                if (giamGia != null)
                {
                    return Ok(giamGia.PhanTramKhuyenMai);
                }
            }
            
            return NotFound();
        }
    }
}