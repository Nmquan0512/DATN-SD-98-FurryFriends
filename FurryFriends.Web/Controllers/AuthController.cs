using FurryFriends.Web.Models;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using FurryFriends.API.Models;
using LoginRequest = FurryFriends.API.Models.LoginRequest;
using Microsoft.Extensions.Logging;

public class AuthController : Controller
{
    private readonly ITaiKhoanService _taiKhoanService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ITaiKhoanService taiKhoanService, ILogger<AuthController> logger)
    {
        _taiKhoanService = taiKhoanService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult DangNhap()
    {
        var taiKhoanId = HttpContext.Session.GetString("TaiKhoanId");
        if (!string.IsNullOrEmpty(taiKhoanId))
        {
            var role = HttpContext.Session.GetString("Role") ?? "";
            if (role.ToLower().Contains("admin"))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            if (role.ToLower().Contains("nhanvien"))
                return RedirectToAction("Index", "HoaDon", new { area = "Admin" });
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DangNhap(LoginRequest model)
    {
        if (!ModelState.IsValid) return View(model);

        _logger.LogInformation($"Đăng nhập với UserName: {model.UserName}");

        // Thử đăng nhập admin/nhân viên
        var result = await _taiKhoanService.DangNhapAdminAsync(model);
        _logger.LogInformation($"Kết quả đăng nhập admin: {(result != null ? "Thành công" : "Thất bại")}");

        if (result != null)
        {
            HttpContext.Session.SetString("TaiKhoanId", result.TaiKhoanId.ToString());
            HttpContext.Session.SetString("Role", result.Role);
            HttpContext.Session.SetString("HoTen", result.HoTen ?? "");
            if (result.Role != null && result.Role.ToLower().Contains("admin"))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            if (result.Role != null && result.Role.ToLower().Contains("nhanvien"))
                return RedirectToAction("Index", "HoaDon", new { area = "Admin" });
            TempData["Error"] = $"Quyền không xác định: {result.Role}";
            return RedirectToAction("DangNhap");
        }

        // Nếu không phải admin/nhân viên, thử đăng nhập khách hàng
        var khResult = await _taiKhoanService.DangNhapKhachHangAsync(model);
        _logger.LogInformation($"Kết quả đăng nhập khách hàng: {(khResult != null ? "Thành công" : "Thất bại")}");

        if (khResult != null)
        {
            // Lưu session cho khách hàng
            HttpContext.Session.SetString("TaiKhoanId", khResult.TaiKhoanId.ToString());
            HttpContext.Session.SetString("Role", khResult.Role);
            HttpContext.Session.SetString("HoTen", khResult.HoTen ?? "");
            // Báo lỗi không có quyền vào admin
            ViewBag.Error = "Bạn không có quyền truy cập khu vực quản trị.";
            return View(model);
        }

        ViewBag.Error = "Sai tài khoản hoặc mật khẩu.";
        return View(model);
    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("DangNhap");
    }
}