using Microsoft.AspNetCore.Mvc;
using FurryFriends.Web.ViewModels;
using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Facebook;

namespace FurryFriends.Web.Controllers
{
    public class DangKyController : Controller
    {
        private readonly IKhachHangService _khachHangService;
        private readonly ITaiKhoanService _taiKhoanService;

        public DangKyController(IKhachHangService khachHangService, ITaiKhoanService taiKhoanService)
        {
            _khachHangService = khachHangService;
            _taiKhoanService = taiKhoanService;
        }

        // GET: DangKy
        [HttpGet]
        public IActionResult Index()
        {
            return View(new RegisterViewModel());
        }

        // POST: DangKy/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Vui lòng kiểm tra lại thông tin!";
                return View("Index", model);
            }

            // Kiểm tra trùng username/email
            var existingAccounts = await _taiKhoanService.FindByUserNameAsync(model.UserName);
            var existingAccount = existingAccounts.FirstOrDefault();
            if (existingAccount != null)
            {
                ModelState.AddModelError("UserName", "Tài khoản đã tồn tại!");
                return View("Index", model);
            }
            var existingEmail = await _khachHangService.FindByEmailAsync(model.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng!");
                return View("Index", model);
            }

            // 1. Tạo mới KhachHang
            var khachHang = new KhachHang
            {
                TenKhachHang = model.FullName,
                SDT = model.Phone,
                EmailCuaKhachHang = model.Email,
                NgayTaoTaiKhoan = DateTime.Now,
                TrangThai = 1 // Đang hoạt động
            };
            await _khachHangService.AddKhachHangAsync(khachHang);

            // 2. Tạo mới TaiKhoan, liên kết với KhachHang vừa tạo
            var taiKhoan = new TaiKhoan
            {
                UserName = model.UserName,
                Password = model.Password, // Nên mã hóa mật khẩu thực tế
                NgayTaoTaiKhoan = DateTime.Now,
                TrangThai = true,
                KhachHangId = khachHang.KhachHangId
            };
            await _taiKhoanService.AddAsync(taiKhoan);

            TempData["Success"] = "Đăng ký thành công! Chào mừng bạn đến với FurryFriends! ";
            return RedirectToAction("Index");
        }

        // Đăng nhập Google
        [HttpGet]
        public IActionResult DangNhapGoogle(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("DangNhapGoogleCallback", "DangKy") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // Callback Google
        [HttpGet]
        public async Task<IActionResult> DangNhapGoogleCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
            {
                TempData["Error"] = "Đăng nhập Google thất bại!";
                return RedirectToAction("Index");
            }
            var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            // Có thể kiểm tra tài khoản đã tồn tại, nếu chưa thì tạo mới
            TempData["Success"] = $"Đăng nhập Google thành công! Xin chào {name ?? email}";
            return RedirectToAction("Index");
        }

        // Đăng nhập Facebook
        [HttpGet]
        public IActionResult DangNhapFacebook(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("DangNhapFacebookCallback", "DangKy") };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        // Callback Facebook
        [HttpGet]
        public async Task<IActionResult> DangNhapFacebookCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
            {
                TempData["Error"] = "Đăng nhập Facebook thất bại!";
                return RedirectToAction("Index");
            }
            var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var picture = claims?.FirstOrDefault(c => c.Type == "urn:facebook:picture")?.Value;
            // Hiển thị thông báo thành công kèm ảnh đại diện
            TempData["Success"] = $"<img src='{picture}' style='height:40px;border-radius:50%;margin-right:8px;vertical-align:middle;'> Đăng nhập Facebook thành công! Xin chào {(name ?? email)}";
            return RedirectToAction("Index");
        }
    }
}
