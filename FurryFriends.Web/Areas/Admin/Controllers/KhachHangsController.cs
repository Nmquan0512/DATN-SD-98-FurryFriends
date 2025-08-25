using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Filter;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdminOnly]
    public class KhachHangsController : Controller
    {
        private readonly IKhachHangService _khachHangService;
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IThongBaoService _thongBaoService;

        public KhachHangsController(IKhachHangService khachHangService, ITaiKhoanService taiKhoanService, IHttpClientFactory clientFactory, IThongBaoService thongBaoService)
        {
            _khachHangService = khachHangService;
            _taiKhoanService = taiKhoanService;
            _clientFactory = clientFactory;
            _thongBaoService = thongBaoService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
            var khachHangs = await _khachHangService.GetAllAsync();
                
                // Tính toán thống kê - chỉ đếm những khách hàng chưa bị xóa
                var totalCount = khachHangs.Count();
                var activeCount = khachHangs.Count(kh => kh.TrangThai == 1); // 1 = Đang hoạt động
                var inactiveCount = khachHangs.Count(kh => kh.TrangThai != 1 && kh.TrangThai != 0); // Khác 1 và khác 0 = Không hoạt động (không tính đã xóa)
                
                ViewBag.TotalCount = totalCount;
                ViewBag.ActiveCount = activeCount;
                ViewBag.InactiveCount = inactiveCount;
                
            return View(khachHangs);
            }
            catch (Exception ex)
            {
                // Fallback data nếu có lỗi
                ViewBag.TotalCount = 0;
                ViewBag.ActiveCount = 0;
                ViewBag.InactiveCount = 0;
                
                TempData["error"] = $"Lỗi khi tải dữ liệu: {ex.Message}";
                return View(new List<KhachHang>());
            }
        }

        // GET: Admin/KhachHangs/Create
        public async Task<IActionResult> Create()
        {
            var taiKhoans = await _taiKhoanService.GetAllTaiKhoanAsync();
            var activeTaiKhoans = taiKhoans.Where(t => t.TrangThai)
                                   .Select(t => new {
                                       TaiKhoanId = t.TaiKhoanId,
                                       TenHienThi = t.UserName
                                   });

            ViewBag.TaiKhoanList = new SelectList(activeTaiKhoans, "TaiKhoanId", "TenHienThi");
            return View();
        }

        // POST: Admin/KhachHangs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhachHang khachHang)
        {
            var taiKhoans = await _taiKhoanService.GetAllTaiKhoanAsync();
            var activeTaiKhoans = taiKhoans.Where(t => t.TrangThai)
                                   .Select(t => new {
                                       TaiKhoanId = t.TaiKhoanId,
                                       TenHienThi = t.UserName
                                   });

            ViewBag.TaiKhoanList = new SelectList(activeTaiKhoans, "TaiKhoanId", "TenHienThi", khachHang.TaiKhoanId);
            if (!ModelState.IsValid) return View(khachHang);

            var success = await _khachHangService.CreateAsync(khachHang);
            var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
            await _thongBaoService.CreateAsync(new ThongBaoDTO
            {
                TieuDe = "Khách hàng mới",
                NoiDung = $"Đã tạo khách hàng \"{khachHang.TenKhachHang}\" (SDT: {khachHang.SDT}).",
                Loai = "KhachHang",
                UserName = tenNhanVien,
                NgayTao = DateTime.Now,
                DaDoc = false
            });
            if (success) return RedirectToAction(nameof(Index));
            return View(khachHang);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var khachHang = await _khachHangService.GetByIdAsync(id);
            if (khachHang == null) return NotFound();
            return View(khachHang);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var khachHang = await _khachHangService.GetByIdAsync(id);
            if (khachHang == null) return NotFound();

            // Lấy danh sách tất cả tài khoản (nếu cần dùng cho các mục khác)
            var taiKhoans = await _taiKhoanService.GetAllTaiKhoanAsync();
            var activeTaiKhoans = taiKhoans.Where(t => t.TrangThai)
                                   .Select(t => new {
                                       TaiKhoanId = t.TaiKhoanId,
                                       TenHienThi = t.UserName
                                   });

            ViewBag.TaiKhoanList = new SelectList(activeTaiKhoans, "TaiKhoanId", "TenHienThi", khachHang.TaiKhoanId);

            // ✅ Lấy tài khoản đã chọn để hiển thị tên trong select2
            if (khachHang.TaiKhoanId != null)
            {
                var taiKhoan = await _taiKhoanService.GetByIdAsync(khachHang.TaiKhoanId.Value);
                ViewBag.SelectedTaiKhoanText = taiKhoan?.UserName; // phải có dòng này!
            }

            return View(khachHang);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, KhachHang model)
        {
            var taiKhoans = await _taiKhoanService.GetAllTaiKhoanAsync();
            var taiKhoanSelectList = taiKhoans.Select(t => new {
                TaiKhoanId = t.TaiKhoanId,
                TenHienThi = t.TrangThai
        ? t.UserName
        : $"{t.UserName} (không hoạt động)"
            });
            ViewBag.TaiKhoanList = new SelectList(taiKhoanSelectList, "TaiKhoanId", "TenHienThi");
            if (id != model.KhachHangId) return BadRequest();
            if (!ModelState.IsValid) return View(model);
            await _khachHangService.UpdateAsync(model.KhachHangId, model);
            var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
            await _thongBaoService.CreateAsync(new ThongBaoDTO
            {
                TieuDe = "Cập nhật khách hàng",
                NoiDung = $"Đã cập nhật thông tin khách hàng \"{model.TenKhachHang}\" (ID: {model.KhachHangId}).",
                Loai = "KhachHang",
                UserName = tenNhanVien,
                NgayTao = DateTime.Now,
                DaDoc = false
            });
            TempData["success"] = "Cập nhật khách hàng thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var khachHang = await _khachHangService.GetByIdAsync(id);
            if (khachHang == null) return NotFound();
            return View(khachHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid KhachHangId)
        {
            await _khachHangService.DeleteAsync(KhachHangId);
            TempData["success"] = "Xóa khách hàng thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}