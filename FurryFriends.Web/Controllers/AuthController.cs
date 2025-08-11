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
        // Xóa TempData cũ để tránh hiển thị thông báo không mong muốn
        TempData.Clear();
        
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
        if (!ModelState.IsValid) 
        {
            TempData["Error"] = "Vui lòng kiểm tra lại thông tin đăng nhập!";
            return View(model); // Return view instead of redirect
        }

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
            {
                TempData["Success"] = $"Đăng nhập thành công! Xin chào Admin {result.HoTen} 🎉";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            if (result.Role != null && result.Role.ToLower().Contains("nhanvien"))
            {
                TempData["Success"] = $"Đăng nhập thành công! Xin chào {result.HoTen} 🎉";
                return RedirectToAction("Index", "HoaDon", new { area = "Admin" });
            }
            
            TempData["Error"] = $"Quyền không xác định: {result.Role}";
            return View(model); // Return view instead of redirect
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
            
            TempData["Warning"] = "Bạn không có quyền truy cập khu vực quản trị. Vui lòng đăng nhập vào trang khách hàng.";
            return View(model); // Return view instead of redirect
        }

        TempData["Error"] = "Sai tài khoản hoặc mật khẩu. Vui lòng kiểm tra lại!";
        return View(model); // Return view instead of redirect
    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["Success"] = "Đăng xuất thành công! Hẹn gặp lại bạn! 👋";
        return RedirectToAction("DangNhap");
    }
}