using FurryFriends.API.Models;
using FurryFriends.Web.Services;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KhachHangsController : Controller
    {
        private readonly IKhachHangService _khachHangService;
        private readonly ILichSuThaoTacService _lichSuService;

        public KhachHangsController(IKhachHangService khachHangService, ILichSuThaoTacService lichSuService)
        {
            _khachHangService = khachHangService;
            _lichSuService = lichSuService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var khachHangs = await _khachHangService.GetAllAsync();
                return View(khachHangs);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(new List<KhachHang>());
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    TempData["error"] = "ID khách hàng không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                var khachHang = await _khachHangService.GetByIdAsync(id);
                if (khachHang == null)
                {
                    TempData["error"] = $"Không tìm thấy khách hàng với ID: {id}";
                    return RedirectToAction(nameof(Index));
                }

                return View(khachHang);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    TempData["error"] = "ID khách hàng không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                var khachHang = await _khachHangService.GetByIdAsync(id); // Để lấy thông tin log
                var success = await _khachHangService.DeleteAsync(id);
                if (success)
                {
                    var log = new LichSuThaoTac
                    {
                        TaiKhoan = User.Identity?.Name,
                        HanhDong = "Xóa khách hàng",
                        NoiDung = $"Đã xóa khách hàng: {khachHang?.TenKhachHang} ({khachHang?.EmailCuaKhachHang})",
                        ThoiGian = DateTime.Now
                    };
                    await _lichSuService.AddLogAsync(log);

                    TempData["success"] = "Xóa khách hàng thành công!";
                }
                else
                {
                    TempData["error"] = "Xóa khách hàng thất bại!";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/KhachHang/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/KhachHang/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhachHang model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var success = await _khachHangService.CreateAsync(model);
                if (success)
                {
                    // ✅ Ghi log thao tác thêm khách hàng
                    var log = new LichSuThaoTac
                    {
                        TaiKhoan = User.Identity?.Name,
                        HanhDong = "Thêm khách hàng",
                        NoiDung = $"Đã thêm khách hàng: {model.TenKhachHang} ({model.EmailCuaKhachHang}, {model.DiemKhachHang}, {model.DiaChiKhachHangs})",
                        ThoiGian = DateTime.Now
                    };
                    await _lichSuService.AddLogAsync(log);

                    TempData["success"] = "Thêm khách hàng thành công!";
                    return RedirectToAction(nameof(Index));
                }

                else
                {
                    TempData["error"] = "Thêm khách hàng thất bại!";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(model);
            }
        }

        // GET: Admin/KhachHang/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var khachHang = await _khachHangService.GetByIdAsync(id);
            if (khachHang == null)
            {
                TempData["error"] = "Không tìm thấy khách hàng";
                return RedirectToAction(nameof(Index));
            }

            return View(khachHang);
        }

        // POST: Admin/KhachHang/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(KhachHang model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var success = await _khachHangService.UpdateAsync(model.KhachHangId, model);
                if (success)
                {
                    var log = new LichSuThaoTac
                    {
                        TaiKhoan = User.Identity?.Name,
                        HanhDong = "Thêm khách hàng",
                        NoiDung = $"Đã thêm khách hàng: {model.TenKhachHang} ({model.EmailCuaKhachHang}, {model.DiemKhachHang}, {model.DiaChiKhachHangs})",
                        ThoiGian = DateTime.Now
                    };
                    await _lichSuService.AddLogAsync(log);

                    TempData["success"] = "Sửa khách hàng thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = "Cập nhật khách hàng thất bại!";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(model);
            }
        }
    }
}

