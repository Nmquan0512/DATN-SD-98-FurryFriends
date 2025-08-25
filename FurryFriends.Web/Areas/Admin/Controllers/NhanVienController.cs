using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Filter;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Channels;


namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdminOnly]

    //[AuthorizeAdminOnly]
    public class NhanVienController : Controller
    {
        private readonly INhanVienService _nhanVienService;
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly IChucVuService _chucVuService;
        private readonly IThongBaoService _thongBaoService;
        public NhanVienController(INhanVienService nhanVienService, ITaiKhoanService taiKhoanService, IChucVuService chucVuService, IThongBaoService thongBaoService)
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
            // Lấy tất cả tài khoản và chỉ chọn tài khoản CHƯA được gán
            var taiKhoanChuaPhanLoai = (await _taiKhoanService.GetAllAsync())
                                .Where(t => t.TrangThai && t.NhanVien == null && t.KhachHangId == null)
                                .ToList();

            ViewBag.TaiKhoanId = new SelectList(taiKhoanChuaPhanLoai, "TaiKhoanId", "UserName");
            ViewBag.ChucVuId = new SelectList(await _chucVuService.GetAllAsync(), "ChucVuId", "TenChucVu");
            return View();
        }

        // POST: NhanVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _nhanVienService.AddAsync(nhanVien);
                    TempData["Success"] = "Nhân viên đã được tạo thành công.";
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
            var taiKhoanChuaPhanLoai = (await _taiKhoanService.GetAllAsync())
                                .Where(t => t.TrangThai && t.NhanVien == null && t.KhachHangId == null)
                                .ToList();

            ViewBag.TaiKhoanId = new SelectList(taiKhoanChuaPhanLoai, "TaiKhoanId", "UserName");
            ViewBag.ChucVuId = new SelectList(await _chucVuService.GetAllAsync(), "ChucVuId", "TenChucVu", nhanVien.ChucVuId);

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
            return View(nhanVien);
        }

        // GET: NhanVien/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var nhanVien = await _nhanVienService.GetByIdAsync(id);
            if (nhanVien == null)
                return NotFound();
            var allTaiKhoan = await _taiKhoanService.GetAllAsync();

            // Lấy các tài khoản có thể chọn:
            var taiKhoanOptions = allTaiKhoan
                .Where(t => (t.NhanVien == null && t.KhachHangId == null) || t.TaiKhoanId == nhanVien.TaiKhoanId)
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

        // POST: NhanVien/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, NhanVien nhanVien)
        {
            if (id != nhanVien.NhanVienId)
                return BadRequest("ID không khớp.");

            if (ModelState.IsValid)
            {
                try
                {
                    await _nhanVienService.UpdateAsync(nhanVien);
                    TempData["Success"] = "Nhân viên đã được cập nhật thành công.";
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
            var allTaiKhoan = await _taiKhoanService.GetAllAsync();

            // Lấy các tài khoản có thể chọn:
            var taiKhoanOptions = allTaiKhoan
                .Where(t => (t.NhanVien == null && t.KhachHangId == null) || t.TaiKhoanId == nhanVien.TaiKhoanId)
                .Select(t => new
                {
                    t.TaiKhoanId,
                    DisplayName = t.TrangThai ? t.UserName : $"{t.UserName} (Không hoạt động)"
                })
                .ToList();

            ViewBag.TaiKhoanId = new SelectList(taiKhoanOptions, "TaiKhoanId", "DisplayName", nhanVien.TaiKhoanId);
            ViewBag.ChucVuId = new SelectList(await _chucVuService.GetAllAsync(), "ChucVuId", "TenChucVu", nhanVien.ChucVuId);
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
            return View(nhanVien);
        }

        // GET: NhanVien/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var nhanVien = await _nhanVienService.GetByIdAsync(id);
            if (nhanVien == null)
                return NotFound();
            return View(nhanVien);
        }

        // POST: NhanVien/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _nhanVienService.DeleteAsync(id);
                TempData["Success"] = "Nhân viên đã được xóa thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi: {ex.Message}");
            }
            var nhanVien = await _nhanVienService.GetByIdAsync(id);
            return View(nhanVien);
        }

        // POST: NhanVien/Search
        [HttpPost]
        public async Task<IActionResult> Search(string hoVaTen)
        {
            if (string.IsNullOrWhiteSpace(hoVaTen))
                return RedirectToAction(nameof(Index));

            try
            {
                var nhanViens = await _nhanVienService.FindByHoVaTenAsync(hoVaTen);
                return View("Index", nhanViens);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Index", new List<NhanVien>());
            }
        }
    }
}