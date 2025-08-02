using FurryFriends.Web.Models;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using FurryFriends.API.Models;
using LoginRequest = FurryFriends.API.Models.LoginRequest;
using Microsoft.Extensions.Logging;

public class KhachHangLoginController : Controller
{
    private readonly ITaiKhoanService _taiKhoanService;
    private readonly ILogger<KhachHangLoginController> _logger;

    public KhachHangLoginController(ITaiKhoanService taiKhoanService, ILogger<KhachHangLoginController> logger)
    {
        _taiKhoanService = taiKhoanService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult DangNhap()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("TaiKhoanId")))
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DangNhap(LoginRequest model)
    {
        if (!ModelState.IsValid) return View(model);

        _logger.LogInformation($"Khách hàng đăng nhập với UserName: {model.UserName}");

        var result = await _taiKhoanService.DangNhapKhachHangAsync(model);
        _logger.LogInformation($"Kết quả đăng nhập khách hàng: {(result != null ? "Thành công" : "Thất bại")}");

        if (result == null)
        {
            ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu.";
            return View(model);
        }

        // Lưu session
        HttpContext.Session.SetString("TaiKhoanId", result.TaiKhoanId.ToString());
        HttpContext.Session.SetString("KhachHangId", result.KhachHangId.ToString()); //sửa ơ đây
        HttpContext.Session.SetString("Role", result.Role);
        HttpContext.Session.SetString("HoTen", result.HoTen ?? "");

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        //return RedirectToAction("DangNhap");
        return RedirectToAction("Index", "Home");
    }
}