using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Filter;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Threading.Channels;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdminOnly]
    public class VouchersController : Controller
    {
        private readonly IVoucherService _voucherService;
        private readonly IThongBaoService _thongBaoService;

        public VouchersController(IVoucherService voucherService, IThongBaoService thongBaoService)
        {
            _voucherService = voucherService;
            _thongBaoService = thongBaoService;
        }

        // GET: Admin/Voucher
        public async Task<IActionResult> Index()
        {
            var allVouchers = await _voucherService.GetAllAsync();
            ViewBag.TotalCount = allVouchers.Count();
            ViewBag.ActiveCount = allVouchers.Count(x => x.TrangThai == 1);
            ViewBag.InactiveCount = allVouchers.Count(x => x.TrangThai == 0);
            return View(allVouchers);
        }

        // GET: Admin/Voucher/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var voucher = await _voucherService.GetByIdAsync(id);
            if (voucher == null)
                return NotFound();

            return View(voucher);
        }

        // GET: Admin/Voucher/Create
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: Admin/Voucher/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Voucher voucher)
        {
            if (!ModelState.IsValid)
            {
                return View(voucher);
            }

            voucher.NgayTao = DateTime.Now;

            try
            {
                await _voucherService.CreateAsync(voucher);
                var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
                {
                    TieuDe = "Thêm voucher",
                    NoiDung = $"Voucher '{voucher.TenVoucher}' đã được tạo.",
                    Loai = "Voucher",
                    UserName = userName,
                    NgayTao = DateTime.Now,
                    DaDoc = false
                });

                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                // Phân tích lỗi validation trả về
                var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(ex.Message);

                if (problemDetails?.Errors != null)
                {
                    foreach (var error in problemDetails.Errors)
                    {
                        foreach (var msg in error.Value)
                        {
                            ModelState.AddModelError(error.Key, msg);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Lỗi xác thực.");
                }

                return View(voucher);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Lỗi: {ex.Message}");
                return View(voucher);
            }
        }

        // GET: Admin/Voucher/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var voucher = await _voucherService.GetByIdAsync(id);
            if (voucher == null)
                return NotFound();
            return View(voucher);
        }

        // POST: Admin/Voucher/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Voucher voucher)
        {
            if (!ModelState.IsValid)
            {
                return View(voucher);
            }

            voucher.NgayCapNhat = DateTime.Now;

            try
            {
                await _voucherService.UpdateAsync(id, voucher);
                var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
                {
                    TieuDe = "Cập nhật voucher",
                    NoiDung = $"Voucher '{voucher.TenVoucher}' đã được cập nhật",
                    Loai = "Voucher",
                    UserName = userName,
                    NgayTao = DateTime.Now,
                    DaDoc = false
                });
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(ex.Message);

                if (problemDetails?.Errors != null)
                {
                    foreach (var error in problemDetails.Errors)
                    {
                        foreach (var msg in error.Value)
                        {
                            ModelState.AddModelError(error.Key, msg);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Lỗi xác thực.");
                }

                return View(voucher);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Lỗi: {ex.Message}");
                return View(voucher);
            }
        }

        // POST: /Vouchers/ToggleStatus/{id}
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var voucher = await _voucherService.GetByIdAsync(id);
                if (voucher == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy voucher." });
                }

                // Toggle trạng thái (chuyển từ int sang bool)
                voucher.TrangThai = voucher.TrangThai == 1 ? 0 : 1;
                var updateResult = await _voucherService.UpdateAsync(id, voucher);
                
                if (updateResult)
                {
                    var action = voucher.TrangThai == 1 ? "kích hoạt" : "vô hiệu hóa";
                    var message = $"Voucher '{voucher.TenVoucher}' đã được {action} thành công.";

                    // 🔔 Thêm thông báo
                    var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                    await _thongBaoService.CreateAsync(new ThongBaoDTO
                    {
                        TieuDe = voucher.TrangThai == 1 ? "Kích hoạt voucher" : "Vô hiệu hóa voucher",
                        NoiDung = $"Voucher '{voucher.TenVoucher}' đã được {action}",
                        Loai = "Voucher",
                        UserName = userName,
                        NgayTao = DateTime.Now,
                        DaDoc = false
                    });

                    return Json(new { 
                        success = true, 
                        message = message,
                        newStatus = voucher.TrangThai == 1,
                        statusText = voucher.TrangThai == 1 ? "Đang hoạt động" : "Không hoạt động",
                        statusClass = voucher.TrangThai == 1 ? "bg-success" : "bg-secondary"
                    });
                }

                return Json(new { success = false, message = "Cập nhật trạng thái thất bại!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // GET: Admin/Voucher/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var voucher = await _voucherService.GetByIdAsync(id);
            if (voucher == null)
                return NotFound();

            return View(voucher);
        }

        // POST: Admin/Voucher/DeleteConfirmed/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var voucher = await _voucherService.GetByIdAsync(id);
                if (voucher == null)
                {
                    TempData["Error"] = "Không tìm thấy voucher.";
                    return RedirectToAction(nameof(Index));
                }

                // Xóa mềm - đổi trạng thái thành không hoạt động
                voucher.TrangThai = 0; // 0 = Không hoạt động
                await _voucherService.UpdateAsync(id, voucher);

                TempData["Success"] = "Voucher đã được vô hiệu hóa thành công.";

                // 🔔 Thêm thông báo
                var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
                {
                    TieuDe = "Vô hiệu hóa voucher",
                    NoiDung = $"Voucher '{voucher.TenVoucher}' đã được vô hiệu hóa",
                    Loai = "Voucher",
                    UserName = userName,
                    NgayTao = DateTime.Now,
                    DaDoc = false
                });

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}