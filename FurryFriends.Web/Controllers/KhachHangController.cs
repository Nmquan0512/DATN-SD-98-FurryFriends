using FurryFriends.API.Models;
using FurryFriends.Web.Service.IService;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace FurryFriends.Web.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly IKhachHangService _khachHangService;
        private readonly ITaiKhoanService _taiKhoanService; // Phục vụ dropdown tài khoản
        private readonly IDiaChiKhachHangService _diaChiService;
        private readonly IHoaDonService _hoaDonService;
        private readonly IGioHangService _gioHangService;

        public KhachHangController(
            IKhachHangService khachHangService,
            ITaiKhoanService taiKhoanService,
            IDiaChiKhachHangService diaChiService,
            IHoaDonService hoaDonService,
            IGioHangService gioHangService)
        {
            _khachHangService = khachHangService;
            _taiKhoanService = taiKhoanService;
            _diaChiService = diaChiService;
            _hoaDonService = hoaDonService;
            _gioHangService = gioHangService;
        }

        public async Task<IActionResult> Index()
        {
            var jsonResult = await _khachHangService.GetAllKhachHangAsync();
            var list = JsonSerializer.Deserialize<List<KhachHang>>(jsonResult ?? "[]", new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            await LoadTaiKhoanDropDown();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhachHang model)
        {
            if (ModelState.IsValid)
            {
                model.KhachHangId = Guid.NewGuid();
                model.NgayTaoTaiKhoan = DateTime.Now;
                var success = await _khachHangService.CreateKhachHangAsync(model);
                if (success) return RedirectToAction(nameof(Index));
            }
            await LoadTaiKhoanDropDown();
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var json = await _khachHangService.GetKhachHangByIdAsync(id);
            var model = JsonSerializer.Deserialize<KhachHang>(json ?? "{}", new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (model == null) return NotFound();

            await LoadTaiKhoanDropDown();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, KhachHang model)
        {
            if (id != model.KhachHangId) return BadRequest();

            if (ModelState.IsValid)
            {
                model.NgayCapNhatCuoiCung = DateTime.Now;
                await _khachHangService.UpdateKhachHangAsync(model);
                return RedirectToAction(nameof(Index));
            }

            await LoadTaiKhoanDropDown();
            return View(model);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var json = await _khachHangService.GetKhachHangByIdAsync(id);
            var model = JsonSerializer.Deserialize<KhachHang>(json ?? "{}", new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _khachHangService.DeleteKhachHangAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var json = await _khachHangService.GetKhachHangByIdAsync(id);
            var khachHang = JsonSerializer.Deserialize<KhachHang>(json ?? "{}", new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (khachHang == null) return NotFound();

            var diaChiJson = await _diaChiService.GetDiaChiByKhachHangIdAsync(id);
            var hoaDonJson = await _hoaDonService.GetHoaDonByKhachHangIdAsync(id);
            var gioHangJson = await _gioHangService.GetGioHangByKhachHangIdAsync(id);

            ViewBag.DiaChiList = JsonSerializer.Deserialize<List<DiaChiKhachHang>>(diaChiJson ?? "[]", new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            ViewBag.HoaDonList = JsonSerializer.Deserialize<List<HoaDon>>(hoaDonJson ?? "[]", new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            ViewBag.GioHangList = JsonSerializer.Deserialize<List<GioHang>>(gioHangJson ?? "[]", new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(khachHang);
        }

        private async Task LoadTaiKhoanDropDown()
        {
            var json = await _taiKhoanService.GetAllTaiKhoanAsync();
            var danhSach = JsonSerializer.Deserialize<List<TaiKhoan>>(json ?? "[]", new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            ViewBag.TaiKhoanList = new SelectList(danhSach, "TaiKhoanId", "UserName");
        }
    }
}
