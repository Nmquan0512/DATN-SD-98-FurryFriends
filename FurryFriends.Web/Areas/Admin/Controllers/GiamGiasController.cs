using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GiamGiasController : Controller
    {
        private readonly IGiamGiaService _giamGiaService;
        private readonly IHttpClientFactory _clientFactory;

        public GiamGiasController(IGiamGiaService giamGiaService, IHttpClientFactory clientFactory)
        {
            _giamGiaService = giamGiaService;
            _clientFactory = clientFactory;
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
                // Gửi thông báo
                var thongBao = new {
                    TieuDe = "Thêm giảm giá mới",
                    NoiDung = $"Giảm giá {giamGia.TenGiamGia} vừa được thêm.",
                    Loai = "GiamGia",
                    UserName = User.Identity?.Name ?? "admin"
                };
                var client = _clientFactory.CreateClient();
                var json = JsonConvert.SerializeObject(thongBao);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PostAsync("https://localhost:7289/api/ThongBao", content);

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
            if (success) return RedirectToAction(nameof(Index));

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
            var success = await _giamGiaService.DeleteAsync(id);
            if (success) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Xóa thất bại.");
            var giamGia = await _giamGiaService.GetByIdAsync(id);
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