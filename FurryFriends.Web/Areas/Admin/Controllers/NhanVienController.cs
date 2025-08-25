using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FurryFriends.Web.Filter;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdminOnly]
    public class NhanVienController : Controller
    {
        private readonly INhanVienService _nhanVienService;
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly IChucVuService _chucVuService;
        private readonly IThongBaoService _thongBaoService;

        public NhanVienController(
            INhanVienService nhanVienService,
            ITaiKhoanService taiKhoanService,
            IChucVuService chucVuService,
            IThongBaoService thongBaoService)
        {
            _nhanVienService = nhanVienService;
            _taiKhoanService = taiKhoanService;
            _chucVuService = chucVuService;
            _thongBaoService = thongBaoService;
        }

        public async Task<IActionResult> Index()
        {
            var nhanViens = await _nhanVienService.GetAllAsync();
            ViewBag.TotalCount = nhanViens.Count();
            ViewBag.ActiveCount = nhanViens.Count(x => x.TrangThai);
            ViewBag.InactiveCount = nhanViens.Count(x => !x.TrangThai);
            return View(nhanViens);
        }

        public async Task<IActionResult> Create()
        {
            var allTaiKhoans = await _taiKhoanService.GetAllAsync();
            var taiKhoanChuaPhanLoai = allTaiKhoans
                                        .Where(t => t.NhanVien == null && t.KhachHangId == null)
                                        .ToList();

            // Dropdown có hiển thị "(Không hoạt động)"
            var taiKhoanOptions = taiKhoanChuaPhanLoai
                .Select(t => new
                {
                    t.TaiKhoanId,
                    DisplayName = t.TrangThai ? t.UserName : $"{t.UserName} (Không hoạt động)"
                })
                .ToList();

            ViewBag.TaiKhoanId = new SelectList(taiKhoanOptions, "TaiKhoanId", "DisplayName");
            ViewBag.ChucVuId = new SelectList(await _chucVuService.GetAllAsync(), "ChucVuId", "TenChucVu");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _nhanVienService.AddAsync(nhanVien);

                    if (nhanVien.TaiKhoanId.HasValue)
                    {
                        try
                        {
                            var taiKhoan = await _taiKhoanService.GetByIdAsync(nhanVien.TaiKhoanId.Value);
                            if (taiKhoan != null)
                            {
                                taiKhoan.NhanVienId = nhanVien.NhanVienId;
                                taiKhoan.KhachHangId = null;
                                taiKhoan.TrangThai = nhanVien.TrangThai;
                                await _taiKhoanService.UpdateAsync(taiKhoan);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error updating TaiKhoan: {ex.Message}");
                        }
                    }

                    TempData["Success"] = "Nhân viên đã được tạo thành công.";

                    // 🔔 Thêm thông báo
                    var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                    await _thongBaoService.CreateAsync(new ThongBaoDTO
                    {
                        TieuDe = "Tạo nhân viên",
                        NoiDung = $"Nhân viên '{nhanVien.HoVaTen}' đã được tạo.",
                        Loai = "NhanVien",
                        UserName = userName,
                        NgayTao = DateTime.Now,
                        DaDoc = false
                    });

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

            var allTaiKhoans = await _taiKhoanService.GetAllAsync();
            var taiKhoanChuaPhanLoai = allTaiKhoans
                                        .Where(t => t.NhanVien == null && t.KhachHangId == null)
                                        .ToList();
            var taiKhoanOptions = taiKhoanChuaPhanLoai
                .Select(t => new
                {
                    t.TaiKhoanId,
                    DisplayName = t.TrangThai ? t.UserName : $"{t.UserName} (Không hoạt động)"
                })
                .ToList();

            ViewBag.TaiKhoanId = new SelectList(taiKhoanOptions, "TaiKhoanId", "DisplayName", nhanVien.TaiKhoanId);
            ViewBag.ChucVuId = new SelectList(await _chucVuService.GetAllAsync(), "ChucVuId", "TenChucVu", nhanVien.ChucVuId);
            return View(nhanVien);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var nhanVien = await _nhanVienService.GetByIdAsync(id);
            if (nhanVien == null) return NotFound();

            var allTaiKhoans = await _taiKhoanService.GetAllAsync();
            var taiKhoanChuaPhanLoai = allTaiKhoans
                .Where(t => (t.NhanVien == null && t.KhachHangId == null) || t.TaiKhoanId == nhanVien.TaiKhoanId)
                .ToList();

            var taiKhoanOptions = taiKhoanChuaPhanLoai
                .Select(t => new
                {
                    t.TaiKhoanId,
                    DisplayName = t.TrangThai ? t.UserName : $"{t.UserName} (Không hoạt động)"
                })
                .ToList();

            ViewBag.TaiKhoanId = new SelectList(taiKhoanOptions, "TaiKhoanId", "DisplayName", nhanVien.TaiKhoanId);
            ViewBag.ChucVuId = new SelectList(await _chucVuService.GetAllAsync(), "ChucVuId", "TenChucVu", nhanVien.ChucVuId);
            return View(nhanVien);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, NhanVien nhanVien)
        {
            if (id != nhanVien.NhanVienId) return BadRequest("ID không khớp.");

            if (ModelState.IsValid)
            {
                try
                {
                    var oldNhanVien = await _nhanVienService.GetByIdAsync(nhanVien.NhanVienId);
                    var oldTaiKhoanId = oldNhanVien?.TaiKhoanId;

                    await _nhanVienService.UpdateAsync(nhanVien);

                    if (nhanVien.TaiKhoanId.HasValue)
                    {
                        try
                        {
                            var taiKhoan = await _taiKhoanService.GetByIdAsync(nhanVien.TaiKhoanId.Value);
                            if (taiKhoan != null)
                            {
                                taiKhoan.NhanVienId = nhanVien.NhanVienId;
                                taiKhoan.TrangThai = nhanVien.TrangThai;
                                await _taiKhoanService.UpdateAsync(taiKhoan);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error updating TaiKhoan: {ex.Message}");
                        }
                    }

                    if (oldTaiKhoanId != nhanVien.TaiKhoanId && oldTaiKhoanId.HasValue)
                    {
                        try
                        {
                            var oldTaiKhoan = await _taiKhoanService.GetByIdAsync(oldTaiKhoanId.Value);
                            if (oldTaiKhoan != null)
                            {
                                oldTaiKhoan.NhanVienId = null;
                                await _taiKhoanService.UpdateAsync(oldTaiKhoan);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error clearing old TaiKhoan link: {ex.Message}");
                        }
                    }

                    TempData["Success"] = "Nhân viên đã được cập nhật thành công.";

                    // 🔔 Thêm thông báo
                    var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                    await _thongBaoService.CreateAsync(new ThongBaoDTO
                    {
                        TieuDe = "Cập nhật nhân viên",
                        NoiDung = $"Nhân viên '{nhanVien.HoVaTen}' đã được chỉnh sửa",
                        Loai = "NhanVien",
                        UserName = userName,
                        NgayTao = DateTime.Now,
                        DaDoc = false
                    });

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                }
            }

            var allTaiKhoans = await _taiKhoanService.GetAllAsync();
            var taiKhoanChuaPhanLoai = allTaiKhoans
                .Where(t => (t.NhanVien == null && t.KhachHangId == null) || t.TaiKhoanId == nhanVien.TaiKhoanId)
                .ToList();

            var taiKhoanOptions = taiKhoanChuaPhanLoai
                .Select(t => new
                {
                    t.TaiKhoanId,
                    DisplayName = t.TrangThai ? t.UserName : $"{t.UserName} (Không hoạt động)"
                })
                .ToList();

            ViewBag.TaiKhoanId = new SelectList(taiKhoanOptions, "TaiKhoanId", "DisplayName", nhanVien.TaiKhoanId);
            ViewBag.ChucVuId = new SelectList(await _chucVuService.GetAllAsync(), "ChucVuId", "TenChucVu", nhanVien.ChucVuId);
            return View(nhanVien);
        }

        // Delete + Search giữ nguyên code 1
        public async Task<IActionResult> Delete(Guid id) { ... }
        [HttpPost, ActionName("Delete")] public async Task<IActionResult> DeleteConfirmed(Guid id) { ... }
        [HttpPost] public async Task<IActionResult> Search(string hoVaTen) { ... }
    }
}
