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

        // Đăng nhập/Đăng ký Google (gộp 2 chức năng)
        [HttpGet]
        public IActionResult DangNhapGoogle(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("DangNhapGoogleCallback", "DangKy") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // Callback Google - Gộp đăng nhập và đăng ký
        [HttpGet]
        public async Task<IActionResult> DangNhapGoogleCallback()
        {
            try
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
                var picture = claims?.FirstOrDefault(c => c.Type == "urn:google:picture")?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    TempData["Error"] = "Không thể lấy thông tin email từ Google!";
                    return RedirectToAction("Index");
                }

                // Kiểm tra email đã tồn tại chưa
                var existingKhachHang = await _khachHangService.FindByEmailAsync(email);
                
                if (existingKhachHang != null)
                {
                    // Email đã tồn tại - Đăng nhập
                    var existingTaiKhoan = await _taiKhoanService.FindByUserNameAsync(email);
                    var taiKhoan = existingTaiKhoan.FirstOrDefault();
                    
                    if (taiKhoan != null)
                    {
                        // Lưu session
                        HttpContext.Session.SetString("TaiKhoanId", taiKhoan.TaiKhoanId.ToString());
                        HttpContext.Session.SetString("Role", "KhachHang");
                        HttpContext.Session.SetString("HoTen", existingKhachHang.TenKhachHang);

                        TempData["Success"] = $"Đăng nhập Google thành công! Xin chào {existingKhachHang.TenKhachHang}";
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    // Email chưa tồn tại - Tạo tài khoản mới
                    var khachHang = new KhachHang
                    {
                        TenKhachHang = name ?? email.Split('@')[0],
                        EmailCuaKhachHang = email,
                        NgayTaoTaiKhoan = DateTime.Now,
                        TrangThai = 1
                    };
                    await _khachHangService.AddKhachHangAsync(khachHang);

                    var taiKhoan = new TaiKhoan
                    {
                        UserName = email, // Sử dụng email làm username
                        Password = Guid.NewGuid().ToString(), // Tạo password ngẫu nhiên
                        NgayTaoTaiKhoan = DateTime.Now,
                        TrangThai = true,
                        KhachHangId = khachHang.KhachHangId
                    };
                    await _taiKhoanService.AddAsync(taiKhoan);

                    // Lưu session
                    HttpContext.Session.SetString("TaiKhoanId", taiKhoan.TaiKhoanId.ToString());
                    HttpContext.Session.SetString("Role", "KhachHang");
                    HttpContext.Session.SetString("HoTen", khachHang.TenKhachHang);

                    TempData["Success"] = $"Đăng ký Google thành công! Chào mừng {khachHang.TenKhachHang} đến với FurryFriends!";
                    return RedirectToAction("Index", "Home");
                }

                TempData["Error"] = "Có lỗi xảy ra trong quá trình xử lý!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Đăng nhập Facebook
        [HttpGet]
        public IActionResult DangNhapFacebook(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("DangNhapFacebookCallback", "DangKy") };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        // Callback Facebook - Gộp đăng nhập và đăng ký
        [HttpGet]
        public async Task<IActionResult> DangNhapFacebookCallback()
        {
            try
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

                if (string.IsNullOrEmpty(email))
                {
                    TempData["Error"] = "Không thể lấy thông tin email từ Facebook!";
                    return RedirectToAction("Index");
                }

                // Kiểm tra email đã tồn tại chưa
                var existingKhachHang = await _khachHangService.FindByEmailAsync(email);
                
                if (existingKhachHang != null)
                {
                    // Email đã tồn tại - Đăng nhập
                    var existingTaiKhoan = await _taiKhoanService.FindByUserNameAsync(email);
                    var taiKhoan = existingTaiKhoan.FirstOrDefault();
                    
                    if (taiKhoan != null)
                    {
                        // Lưu session
                        HttpContext.Session.SetString("TaiKhoanId", taiKhoan.TaiKhoanId.ToString());
                        HttpContext.Session.SetString("Role", "KhachHang");
                        HttpContext.Session.SetString("HoTen", existingKhachHang.TenKhachHang);

                        var successMessage = $"<img src='{picture}' style='height:40px;border-radius:50%;margin-right:8px;vertical-align:middle;'> Đăng nhập Facebook thành công! Xin chào {existingKhachHang.TenKhachHang}";
                        TempData["Success"] = successMessage;
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    // Email chưa tồn tại - Tạo tài khoản mới
                    var khachHang = new KhachHang
                    {
                        TenKhachHang = name ?? email.Split('@')[0],
                        EmailCuaKhachHang = email,
                        NgayTaoTaiKhoan = DateTime.Now,
                        TrangThai = 1
                    };
                    await _khachHangService.AddKhachHangAsync(khachHang);

                    var taiKhoan = new TaiKhoan
                    {
                        UserName = email, // Sử dụng email làm username
                        Password = Guid.NewGuid().ToString(), // Tạo password ngẫu nhiên
                        NgayTaoTaiKhoan = DateTime.Now,
                        TrangThai = true,
                        KhachHangId = khachHang.KhachHangId
                    };
                    await _taiKhoanService.AddAsync(taiKhoan);

                    // Lưu session
                    HttpContext.Session.SetString("TaiKhoanId", taiKhoan.TaiKhoanId.ToString());
                    HttpContext.Session.SetString("Role", "KhachHang");
                    HttpContext.Session.SetString("HoTen", khachHang.TenKhachHang);

                    var successMessage = $"<img src='{picture}' style='height:40px;border-radius:50%;margin-right:8px;vertical-align:middle;'> Đăng ký Facebook thành công! Chào mừng {khachHang.TenKhachHang} đến với FurryFriends!";
                    TempData["Success"] = successMessage;
                    return RedirectToAction("Index", "Home");
                }

                TempData["Error"] = "Có lỗi xảy ra trong quá trình xử lý!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
