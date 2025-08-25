using System.Net.Http;
using System.Text;
using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using FurryFriends.Web.Filter;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdminOrEmployee] // ✅ Cho cả Admin và Employee
    public class KhachHangsController : Controller
    {
        private readonly IKhachHangService _khachHangService;
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly INhanVienService _nhanVienService;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IThongBaoService _thongBaoService;

        public KhachHangsController(
            IKhachHangService khachHangService,
            ITaiKhoanService taiKhoanService,
            INhanVienService nhanVienService,
            IHttpClientFactory clientFactory,
            IThongBaoService thongBaoService)
        {
            _khachHangService = khachHangService;
            _taiKhoanService = taiKhoanService;
            _nhanVienService = nhanVienService;
            _clientFactory = clientFactory;
            _thongBaoService = thongBaoService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var khachHangs = await _khachHangService.GetAllAsync();

                var totalCount = khachHangs.Count();
                var activeCount = khachHangs.Count(kh => kh.TrangThai == 1);
                var inactiveCount = khachHangs.Count(kh => kh.TrangThai == 2);

                ViewBag.TotalCount = totalCount;
                ViewBag.ActiveCount = activeCount;
                ViewBag.InactiveCount = inactiveCount;

                return View(khachHangs);
            }
            catch (Exception ex)
            {
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
            var allTaiKhoans = await _taiKhoanService.GetAllAsync();
            var taiKhoanChuaPhanLoai = allTaiKhoans
                .Where(t => t.KhachHangId == null && t.NhanVien == null)
                .ToList();

            ViewBag.TaiKhoanId = new SelectList(taiKhoanChuaPhanLoai, "TaiKhoanId", "UserName");
            return View();
        }

        // POST: Admin/KhachHangs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhachHang khachHang)
        {
            // Gán TaiKhoanId từ form
            var taiKhoanIdFromForm = Request.Form["TaiKhoanId"].ToString();
            if (!string.IsNullOrEmpty(taiKhoanIdFromForm) && taiKhoanIdFromForm != "null" &&
                Guid.TryParse(taiKhoanIdFromForm, out var parsedTaiKhoanId))
            {
                khachHang.TaiKhoanId = parsedTaiKhoanId;
            }
            else
            {
                khachHang.TaiKhoanId = null;
            }

            // Validation email / sđt / tài khoản
            var existingKhachHangs = await _khachHangService.GetAllAsync();

            if (!string.IsNullOrEmpty(khachHang.EmailCuaKhachHang) &&
                existingKhachHangs.Any(kh => kh.EmailCuaKhachHang == khachHang.EmailCuaKhachHang))
            {
                ModelState.AddModelError("EmailCuaKhachHang", "Email này đã được sử dụng.");
            }

            if (!string.IsNullOrEmpty(khachHang.SDT) &&
                existingKhachHangs.Any(kh => kh.SDT == khachHang.SDT))
            {
                ModelState.AddModelError("SDT", "Số điện thoại này đã được sử dụng.");
            }

            if (khachHang.TaiKhoanId.HasValue &&
                existingKhachHangs.Any(kh => kh.TaiKhoanId == khachHang.TaiKhoanId))
            {
                ModelState.AddModelError("TaiKhoanId", "Tài khoản này đã được liên kết.");
            }

            var taiKhoanChuaPhanLoai = (await _taiKhoanService.GetAllAsync())
                .Where(t => t.KhachHangId == null && t.NhanVien == null)
                .ToList();

            ViewBag.TaiKhoanId = new SelectList(taiKhoanChuaPhanLoai, "TaiKhoanId", "UserName", khachHang.TaiKhoanId);

            if (!ModelState.IsValid) return View(khachHang);

            var success = await _khachHangService.CreateAsync(khachHang);
            if (success)
            {
                if (khachHang.TaiKhoanId.HasValue)
                {
                    var taiKhoan = await _taiKhoanService.GetByIdAsync(khachHang.TaiKhoanId.Value);
                    if (taiKhoan != null)
                    {
                        taiKhoan.KhachHangId = khachHang.KhachHangId;
                        taiKhoan.NhanVienId = null;
                        taiKhoan.TrangThai = khachHang.TrangThai == 1;
                        await _taiKhoanService.UpdateAsync(taiKhoan);
                    }
                }

                // 🔔 Thông báo hệ thống
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

                TempData["Success"] = "Tạo khách hàng thành công!";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Tạo khách hàng thất bại!";
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

            var allTaiKhoans = await _taiKhoanService.GetAllAsync();
            var taiKhoanChuaPhanLoai = allTaiKhoans
                .Where(t => (t.KhachHangId == null && t.NhanVien == null) || t.TaiKhoanId == khachHang.TaiKhoanId)
                .ToList();

            ViewBag.TaiKhoanId = new SelectList(taiKhoanChuaPhanLoai, "TaiKhoanId", "UserName", khachHang.TaiKhoanId);

            return View(khachHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, KhachHang model)
        {
            // Gán TaiKhoanId từ form
            var taiKhoanIdFromForm = Request.Form["TaiKhoanId"].ToString();
            if (!string.IsNullOrEmpty(taiKhoanIdFromForm) && taiKhoanIdFromForm != "null" &&
                Guid.TryParse(taiKhoanIdFromForm, out var parsedTaiKhoanId))
            {
                model.TaiKhoanId = parsedTaiKhoanId;
            }
            else
            {
                model.TaiKhoanId = null;
            }

            var existingKhachHangs = await _khachHangService.GetAllAsync();

            if (!string.IsNullOrEmpty(model.EmailCuaKhachHang) &&
                existingKhachHangs.Any(kh => kh.EmailCuaKhachHang == model.EmailCuaKhachHang && kh.KhachHangId != model.KhachHangId))
            {
                ModelState.AddModelError("EmailCuaKhachHang", "Email đã tồn tại.");
            }

            if (!string.IsNullOrEmpty(model.SDT) &&
                existingKhachHangs.Any(kh => kh.SDT == model.SDT && kh.KhachHangId != model.KhachHangId))
            {
                ModelState.AddModelError("SDT", "SĐT đã tồn tại.");
            }

            if (model.TaiKhoanId.HasValue &&
                existingKhachHangs.Any(kh => kh.TaiKhoanId == model.TaiKhoanId && kh.KhachHangId != model.KhachHangId))
            {
                ModelState.AddModelError("TaiKhoanId", "Tài khoản đã được liên kết.");
            }

            var allTaiKhoans = await _taiKhoanService.GetAllAsync();
            var taiKhoanChuaPhanLoai = allTaiKhoans
                .Where(t => (t.KhachHangId == null && t.NhanVien == null) || t.TaiKhoanId == model.TaiKhoanId)
                .ToList();

            ViewBag.TaiKhoanId = new SelectList(taiKhoanChuaPhanLoai, "TaiKhoanId", "UserName", model.TaiKhoanId);

            if (id != model.KhachHangId) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var oldKhachHang = await _khachHangService.GetByIdAsync(model.KhachHangId);
            var oldTaiKhoanId = oldKhachHang?.TaiKhoanId;

            var success = await _khachHangService.UpdateAsync(model.KhachHangId, model);
            if (success)
            {
                if (model.TaiKhoanId.HasValue)
                {
                    var taiKhoan = await _taiKhoanService.GetByIdAsync(model.TaiKhoanId.Value);
                    if (taiKhoan != null)
                    {
                        taiKhoan.KhachHangId = model.KhachHangId;
                        taiKhoan.TrangThai = model.TrangThai == 1;
                        await _taiKhoanService.UpdateAsync(taiKhoan);
                    }
                }

                if (oldTaiKhoanId != model.TaiKhoanId && oldTaiKhoanId.HasValue)
                {
                    var oldTaiKhoan = await _taiKhoanService.GetByIdAsync(oldTaiKhoanId.Value);
                    if (oldTaiKhoan != null)
                    {
                        oldTaiKhoan.KhachHangId = null;
                        await _taiKhoanService.UpdateAsync(oldTaiKhoan);
                    }
                }

                // 🔔 Thông báo hệ thống
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

                TempData["Success"] = "Cập nhật khách hàng thành công!";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Cập nhật khách hàng thất bại!";
            return View(model);
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
