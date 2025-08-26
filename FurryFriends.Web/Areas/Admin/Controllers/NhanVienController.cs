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
            var allTaiKhoans = await _taiKhoanService.GetAllAsync();
            var taiKhoanChuaPhanLoai = allTaiKhoans
                                        .Where(t => t.TrangThai && t.NhanVien == null && t.KhachHangId == null)
                                        .ToList();

            Console.WriteLine($"=== NHAN VIEN CREATE DEBUG ===");
            Console.WriteLine($"Total TaiKhoans: {allTaiKhoans.Count()}");
            Console.WriteLine($"Filtered TaiKhoans: {taiKhoanChuaPhanLoai.Count()}");
            foreach (var tk in allTaiKhoans)
            {
                Console.WriteLine($"TaiKhoan: {tk.UserName}, KhachHangId: {tk.KhachHangId}, NhanVienId: {tk.NhanVienId}");
            }
            Console.WriteLine($"=== END DEBUG ===");

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
                    
                    // Cập nhật tài khoản nếu có liên kết
                    if (nhanVien.TaiKhoanId.HasValue)
                    {
                        try
                        {
                            var taiKhoan = await _taiKhoanService.GetByIdAsync(nhanVien.TaiKhoanId.Value);
                            if (taiKhoan != null)
                            {
                                taiKhoan.NhanVienId = nhanVien.NhanVienId;
                                taiKhoan.KhachHangId = null; // Clear khách hàng nếu có
                                
                                // Cập nhật trạng thái tài khoản dựa trên trạng thái nhân viên
                                taiKhoan.TrangThai = nhanVien.TrangThai;
                                
                                await _taiKhoanService.UpdateAsync(taiKhoan);
                                Console.WriteLine($"Updated TaiKhoan link and status: {taiKhoan.UserName}, Active: {nhanVien.TrangThai}");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log lỗi nhưng không fail toàn bộ operation
                            Console.WriteLine($"Error updating TaiKhoan: {ex.Message}");
                        }
                    }
                    
                    // Tạo thông báo
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

            ViewBag.TaiKhoanId = new SelectList(taiKhoanChuaPhanLoai, "TaiKhoanId", "UserName", nhanVien.TaiKhoanId);
            ViewBag.ChucVuId = new SelectList(await _chucVuService.GetAllAsync(), "ChucVuId", "TenChucVu", nhanVien.ChucVuId);
            return View(nhanVien);
        }

        // GET: NhanVien/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var nhanVien = await _nhanVienService.GetByIdAsync(id);
            if (nhanVien == null)
                return NotFound();
            
            // Lấy tất cả tài khoản chưa được liên kết và tài khoản hiện tại của nhân viên này
            var allTaiKhoans = await _taiKhoanService.GetAllAsync();
            var taiKhoanChuaPhanLoai = allTaiKhoans
                .Where(t => ((t.TrangThai && t.NhanVien == null && t.KhachHangId == null) || t.TaiKhoanId == nhanVien.TaiKhoanId))
                .ToList();

            ViewBag.TaiKhoanId = new SelectList(taiKhoanChuaPhanLoai, "TaiKhoanId", "UserName", nhanVien.TaiKhoanId);
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
                    // Lấy thông tin nhân viên cũ trước khi cập nhật
                    var oldNhanVien = await _nhanVienService.GetByIdAsync(nhanVien.NhanVienId);
                    var oldTaiKhoanId = oldNhanVien?.TaiKhoanId;
                    
                    await _nhanVienService.UpdateAsync(nhanVien);
                    
                    // Cập nhật trạng thái tài khoản liên kết dựa trên trạng thái nhân viên
                    if (nhanVien.TaiKhoanId.HasValue)
                    {
                        try
                        {
                            var taiKhoan = await _taiKhoanService.GetByIdAsync(nhanVien.TaiKhoanId.Value);
                            if (taiKhoan != null)
                            {
                                // Cập nhật NhanVienId
                                taiKhoan.NhanVienId = nhanVien.NhanVienId;
                                
                                // Cập nhật trạng thái tài khoản dựa trên trạng thái nhân viên
                                // Trạng thái nhân viên: true = Hoạt động, false = Không hoạt động
                                taiKhoan.TrangThai = nhanVien.TrangThai;
                                
                                await _taiKhoanService.UpdateAsync(taiKhoan);
                                Console.WriteLine($"Updated TaiKhoan link and status: {taiKhoan.UserName}, Active: {nhanVien.TrangThai}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error updating TaiKhoan: {ex.Message}");
                        }
                    }
                    
                    // Nếu có thay đổi về TaiKhoanId
                    if (oldTaiKhoanId != nhanVien.TaiKhoanId)
                    {
                        // Clear liên kết cũ nếu có
                        if (oldTaiKhoanId.HasValue)
                        {
                            try
                            {
                                var oldTaiKhoan = await _taiKhoanService.GetByIdAsync(oldTaiKhoanId.Value);
                                if (oldTaiKhoan != null)
                                {
                                    oldTaiKhoan.NhanVienId = null;
                                    await _taiKhoanService.UpdateAsync(oldTaiKhoan);
                                    Console.WriteLine($"Cleared old TaiKhoan link: {oldTaiKhoan.UserName}");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error clearing old TaiKhoan link: {ex.Message}");
                            }
                        }
                    }
                    
                    // Tạo thông báo
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
            // Lấy tất cả tài khoản chưa được liên kết và tài khoản hiện tại của nhân viên này
            var allTaiKhoans = await _taiKhoanService.GetAllAsync();
            var taiKhoanChuaPhanLoai = allTaiKhoans
                .Where(t => ((t.TrangThai && t.NhanVien == null && t.KhachHangId == null) || t.TaiKhoanId == nhanVien.TaiKhoanId))
                .ToList();

            ViewBag.TaiKhoanId = new SelectList(taiKhoanChuaPhanLoai, "TaiKhoanId", "UserName", nhanVien.TaiKhoanId);
            ViewBag.ChucVuId = new SelectList(await _chucVuService.GetAllAsync(), "ChucVuId", "TenChucVu", nhanVien.ChucVuId);
            return View(nhanVien);
        }

        // POST: /NhanVien/ToggleStatus/{id}
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var nhanVien = await _nhanVienService.GetByIdAsync(id);
                if (nhanVien == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy nhân viên." });
                }

                // Toggle trạng thái
                nhanVien.TrangThai = !nhanVien.TrangThai;
                await _nhanVienService.UpdateAsync(nhanVien);
                
                // Đồng bộ trạng thái với tài khoản liên kết
                if (nhanVien.TaiKhoanId.HasValue)
                {
                    try
                    {
                        var taiKhoan = await _taiKhoanService.GetByIdAsync(nhanVien.TaiKhoanId.Value);
                        if (taiKhoan != null)
                        {
                            // Đồng bộ trạng thái: nhân viên hoạt động thì tài khoản cũng hoạt động
                            taiKhoan.TrangThai = nhanVien.TrangThai;
                            await _taiKhoanService.UpdateAsync(taiKhoan);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log lỗi nhưng không dừng quá trình
                        Console.WriteLine($"Error updating TaiKhoan status: {ex.Message}");
                    }
                }
                
                var action = nhanVien.TrangThai ? "kích hoạt" : "vô hiệu hóa";
                var message = $"Nhân viên '{nhanVien.HoVaTen}' đã được {action} thành công.";

                // 🔔 Thêm thông báo
                var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
                {
                    TieuDe = nhanVien.TrangThai ? "Kích hoạt nhân viên" : "Vô hiệu hóa nhân viên",
                    NoiDung = $"Nhân viên '{nhanVien.HoVaTen}' đã được {action}",
                    Loai = "NhanVien",
                    UserName = userName,
                    NgayTao = DateTime.Now,
                    DaDoc = false
                });

                return Json(new { 
                    success = true, 
                    message = message,
                    newStatus = nhanVien.TrangThai,
                    statusText = nhanVien.TrangThai ? "Đang hoạt động" : "Không hoạt động",
                    statusClass = nhanVien.TrangThai ? "bg-success" : "bg-secondary"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
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