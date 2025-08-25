using System.Net.Http;
using System.Text;
using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using FurryFriends.Web.Filter;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdminOnly]
    public class KhachHangsController : Controller
    {
        private readonly IKhachHangService _khachHangService;
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly INhanVienService _nhanVienService;
        private readonly IHttpClientFactory _clientFactory;

        public KhachHangsController(IKhachHangService khachHangService, ITaiKhoanService taiKhoanService, INhanVienService nhanVienService, IHttpClientFactory clientFactory)
        {
            _khachHangService = khachHangService;
            _taiKhoanService = taiKhoanService;
            _nhanVienService = nhanVienService;
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
            var khachHangs = await _khachHangService.GetAllAsync();
                
                // Tính toán thống kê - chỉ đếm những khách hàng chưa bị xóa
                var totalCount = khachHangs.Count();
                var activeCount = khachHangs.Count(kh => kh.TrangThai == 1); // 1 = Đang hoạt động
                var inactiveCount = khachHangs.Count(kh => kh.TrangThai == 2); // 2 = Đã khóa
                
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
            // Copy chính xác logic từ NhanVienController
            var allTaiKhoans = await _taiKhoanService.GetAllAsync();
            var taiKhoanChuaPhanLoai = allTaiKhoans
                                        .Where(t => t.KhachHangId == null && t.NhanVien == null)
                                        .ToList();

            Console.WriteLine($"=== KHACH HANG CREATE DEBUG ===");
            Console.WriteLine($"Total TaiKhoans: {allTaiKhoans.Count()}");
            Console.WriteLine($"Filtered TaiKhoans: {taiKhoanChuaPhanLoai.Count()}");
            foreach (var tk in allTaiKhoans)
            {
                Console.WriteLine($"TaiKhoan: {tk.UserName}, KhachHangId: {tk.KhachHangId}, NhanVienId: {tk.NhanVienId}");
            }
            Console.WriteLine($"=== END DEBUG ===");

            ViewBag.TaiKhoanId = new SelectList(taiKhoanChuaPhanLoai, "TaiKhoanId", "UserName");
            return View();
        }

        // POST: Admin/KhachHangs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhachHang khachHang)
        {
            // Xử lý TaiKhoanId từ form
            var taiKhoanIdFromForm = Request.Form["TaiKhoanId"].ToString();
            if (!string.IsNullOrEmpty(taiKhoanIdFromForm) && taiKhoanIdFromForm != "null" && Guid.TryParse(taiKhoanIdFromForm, out var parsedTaiKhoanId))
            {
                khachHang.TaiKhoanId = parsedTaiKhoanId;
            }
            else
            {
                khachHang.TaiKhoanId = null;
            }

            // Validation: Kiểm tra email và số điện thoại trùng lặp
            var existingKhachHangs = await _khachHangService.GetAllAsync();
            
            if (!string.IsNullOrEmpty(khachHang.EmailCuaKhachHang) && 
                existingKhachHangs.Any(kh => kh.EmailCuaKhachHang == khachHang.EmailCuaKhachHang))
            {
                ModelState.AddModelError("EmailCuaKhachHang", "Email này đã được sử dụng bởi khách hàng khác.");
            }
            
            if (!string.IsNullOrEmpty(khachHang.SDT) && 
                existingKhachHangs.Any(kh => kh.SDT == khachHang.SDT))
            {
                ModelState.AddModelError("SDT", "Số điện thoại này đã được sử dụng bởi khách hàng khác.");
            }

            // Validation: Kiểm tra 1 tài khoản chỉ được 1 khách hàng
            if (khachHang.TaiKhoanId.HasValue)
            {
                if (existingKhachHangs.Any(kh => kh.TaiKhoanId == khachHang.TaiKhoanId))
                {
                    ModelState.AddModelError("TaiKhoanId", "Tài khoản này đã được liên kết với khách hàng khác.");
                }
            }

            // Validation: Kiểm tra 1 khách hàng chỉ được 1 tài khoản (đã có TaiKhoanId)
            if (khachHang.TaiKhoanId.HasValue)
            {
                // Không cần kiểm tra vì đây là tạo mới
            }

            var taiKhoanChuaPhanLoai = (await _taiKhoanService.GetAllAsync())
                                        .Where(t => t.KhachHangId == null && t.NhanVien == null)
                                        .ToList();

            ViewBag.TaiKhoanId = new SelectList(taiKhoanChuaPhanLoai, "TaiKhoanId", "UserName", khachHang.TaiKhoanId);
            
            if (!ModelState.IsValid) return View(khachHang);

            var success = await _khachHangService.CreateAsync(khachHang);
            if (success) 
            {
                // Cập nhật tài khoản nếu có liên kết
                if (khachHang.TaiKhoanId.HasValue)
                {
                    try
                    {
                        var taiKhoan = await _taiKhoanService.GetByIdAsync(khachHang.TaiKhoanId.Value);
                        if (taiKhoan != null)
                        {
                            taiKhoan.KhachHangId = khachHang.KhachHangId;
                            taiKhoan.NhanVienId = null; // Clear nhân viên nếu có
                            
                            // Cập nhật trạng thái tài khoản dựa trên trạng thái khách hàng
                            // Trạng thái khách hàng: 1 = Hoạt động, 2 = Đã khóa
                            bool khachHangActive = khachHang.TrangThai == 1;
                            taiKhoan.TrangThai = khachHangActive;
                            
                            await _taiKhoanService.UpdateAsync(taiKhoan);
                            Console.WriteLine($"Updated TaiKhoan link and status: {taiKhoan.UserName}, Active: {khachHangActive}");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log lỗi nhưng không fail toàn bộ operation
                        Console.WriteLine($"Error updating TaiKhoan: {ex.Message}");
                    }
                }
                
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
            
            // Lấy tất cả tài khoản chưa được liên kết và tài khoản hiện tại của khách hàng này
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
            // Xử lý TaiKhoanId từ form
            var taiKhoanIdFromForm = Request.Form["TaiKhoanId"].ToString();
            if (!string.IsNullOrEmpty(taiKhoanIdFromForm) && taiKhoanIdFromForm != "null" && Guid.TryParse(taiKhoanIdFromForm, out var parsedTaiKhoanId))
            {
                model.TaiKhoanId = parsedTaiKhoanId;
            }
            else
            {
                model.TaiKhoanId = null;
            }

            // Validation: Kiểm tra email và số điện thoại trùng lặp (trừ chính nó)
            var existingKhachHangs = await _khachHangService.GetAllAsync();
            
            if (!string.IsNullOrEmpty(model.EmailCuaKhachHang) && 
                existingKhachHangs.Any(kh => kh.EmailCuaKhachHang == model.EmailCuaKhachHang && kh.KhachHangId != model.KhachHangId))
            {
                ModelState.AddModelError("EmailCuaKhachHang", "Email này đã được sử dụng bởi khách hàng khác.");
            }
            
            if (!string.IsNullOrEmpty(model.SDT) && 
                existingKhachHangs.Any(kh => kh.SDT == model.SDT && kh.KhachHangId != model.KhachHangId))
            {
                ModelState.AddModelError("SDT", "Số điện thoại này đã được sử dụng bởi khách hàng khác.");
            }

            // Validation: Kiểm tra 1 tài khoản chỉ được 1 khách hàng
            if (model.TaiKhoanId.HasValue)
            {
                if (existingKhachHangs.Any(kh => kh.TaiKhoanId == model.TaiKhoanId && kh.KhachHangId != model.KhachHangId))
                {
                    ModelState.AddModelError("TaiKhoanId", "Tài khoản này đã được liên kết với khách hàng khác.");
                }
            }

            var allTaiKhoans = await _taiKhoanService.GetAllAsync();
            var taiKhoanChuaPhanLoai = allTaiKhoans
                .Where(t => (t.KhachHangId == null && t.NhanVien == null) || t.TaiKhoanId == model.TaiKhoanId)
                .ToList();

            ViewBag.TaiKhoanId = new SelectList(taiKhoanChuaPhanLoai, "TaiKhoanId", "UserName", model.TaiKhoanId);
            
            if (id != model.KhachHangId) return BadRequest();
            if (!ModelState.IsValid) return View(model);
            
            // Lấy thông tin khách hàng cũ trước khi cập nhật
            var oldKhachHang = await _khachHangService.GetByIdAsync(model.KhachHangId);
            var oldTaiKhoanId = oldKhachHang?.TaiKhoanId;
            
            var success = await _khachHangService.UpdateAsync(model.KhachHangId, model);
            if (success)
            {
                // Cập nhật trạng thái tài khoản liên kết dựa trên trạng thái khách hàng
                if (model.TaiKhoanId.HasValue)
                {
                    try
                    {
                        var taiKhoan = await _taiKhoanService.GetByIdAsync(model.TaiKhoanId.Value);
                        if (taiKhoan != null)
                        {
                            // Cập nhật KhachHangId
                            taiKhoan.KhachHangId = model.KhachHangId;
                            
                            // Cập nhật trạng thái tài khoản dựa trên trạng thái khách hàng
                            // Trạng thái khách hàng: 1 = Hoạt động, 2 = Đã khóa
                            bool khachHangActive = model.TrangThai == 1;
                            taiKhoan.TrangThai = khachHangActive;
                            
                            await _taiKhoanService.UpdateAsync(taiKhoan);
                            Console.WriteLine($"Updated TaiKhoan link and status: {taiKhoan.UserName}, Active: {khachHangActive}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating TaiKhoan: {ex.Message}");
                    }
                }
                
                // Nếu có thay đổi về TaiKhoanId
                if (oldTaiKhoanId != model.TaiKhoanId)
                {
                    // Clear liên kết cũ nếu có
                    if (oldTaiKhoanId.HasValue)
                    {
                        try
                        {
                            var oldTaiKhoan = await _taiKhoanService.GetByIdAsync(oldTaiKhoanId.Value);
                            if (oldTaiKhoan != null)
                            {
                                oldTaiKhoan.KhachHangId = null;
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
                
                TempData["Success"] = "Cập nhật khách hàng thành công!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Error"] = "Cập nhật khách hàng thất bại!";
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật khách hàng.");
                return View(model);
            }
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