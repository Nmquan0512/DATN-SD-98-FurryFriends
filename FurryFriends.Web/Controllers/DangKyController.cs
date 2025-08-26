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
        private readonly ILogger<DangKyController> _logger;

        public DangKyController(IKhachHangService khachHangService, ITaiKhoanService taiKhoanService, ILogger<DangKyController> logger)
        {
            _khachHangService = khachHangService;
            _taiKhoanService = taiKhoanService;
            _logger = logger;
        }

        // GET: DangKy
        [HttpGet]
        public IActionResult Index()
        {
            // Xử lý lỗi từ Google OAuth
            var error = Request.Query["error"].ToString();
            if (!string.IsNullOrEmpty(error))
            {
                switch (error)
                {
                    case "google_auth_failed":
                        ViewBag.Error = "Đăng nhập Google thất bại! Vui lòng thử lại.";
                        break;
                    case "oauth_state_invalid":
                        ViewBag.Error = "Phiên đăng nhập Google đã hết hạn! Vui lòng thử lại.";
                        break;
                    case "google_auth_failed_no_email":
                        ViewBag.Error = "Không thể lấy thông tin email từ Google! Vui lòng thử lại.";
                        break;
                    default:
                        ViewBag.Error = "Đăng nhập Google thất bại! Vui lòng thử lại.";
                        break;
                }
            }
            
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
                return View("Index", model); // Return view with model instead of redirect
            }

            // Kiểm tra trùng username/email
            var existingAccounts = await _taiKhoanService.FindByUserNameAsync(model.UserName);
            var existingAccount = existingAccounts.FirstOrDefault();
            if (existingAccount != null)
            {
                ViewBag.Error = "Tài khoản đã tồn tại! Vui lòng chọn tên đăng nhập khác.";
                return View("Index", model); // Return view with model instead of redirect
            }

            // Kiểm tra trùng số điện thoại
            var existingPhone = await _khachHangService.FindByPhoneAsync(model.Phone);
            if (existingPhone != null)
            {
                ViewBag.Error = "Số điện thoại đã được sử dụng! Vui lòng sử dụng số điện thoại khác.";
                return View("Index", model); // Return view with model instead of redirect
            }

            var existingEmail = await _khachHangService.FindByEmailAsync(model.Email);
            if (existingEmail != null)
            {
                ViewBag.Error = "Email đã được sử dụng! Vui lòng sử dụng email khác.";
                return View("Index", model); // Return view with model instead of redirect
            }

            try
            {
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

                TempData["Success"] = "Đăng ký thành công! Chào mừng bạn đến với FurryFriends! 🎉";
                return View("Index", model); // Return view with success message instead of redirect
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Đã xảy ra lỗi: {ex.Message}. Vui lòng thử lại sau.";
                return View("Index", model); // Return view with model instead of redirect
            }
        }

        // Đăng nhập/Đăng ký Google (gộp 2 chức năng)
        [HttpGet]
        public IActionResult DangNhapGoogle(string returnUrl = "/")
        {
            var redirectUri = Url.Action("ProcessGoogleLogin", "DangKy", null, Request.Scheme, Request.Host.Value);
            
            // Lưu returnUrl vào TempData để sử dụng sau
            TempData["GoogleReturnUrl"] = returnUrl ?? "/";
            
            // Tạo state token và lưu vào session
            var stateToken = Guid.NewGuid().ToString();
            HttpContext.Session.SetString("GoogleOAuthState", stateToken);
            
            var properties = new AuthenticationProperties 
            { 
                RedirectUri = redirectUri,
                Items = { 
                    { "returnUrl", returnUrl ?? "/" },
                    { "state", stateToken }
                },
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };
            
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }



        // Xử lý thông tin Google từ query parameters
        [HttpGet]
        public async Task<IActionResult> ProcessGoogleLogin()
        {
            try
            {
                var email = Request.Query["email"].ToString();
                var name = Request.Query["name"].ToString();
                var picture = Request.Query["picture"].ToString();

                if (string.IsNullOrEmpty(email))
                {
                    TempData["Error"] = "Không tìm thấy thông tin Google! Vui lòng thử lại.";
                    return RedirectToAction("Index");
                }

                // Kiểm tra xem email đã tồn tại trong database chưa
                
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

                        // Commit session để đảm bảo được lưu
                        await HttpContext.Session.CommitAsync();

                        TempData["Success"] = $"Đăng nhập Google thành công! Xin chào {existingKhachHang.TenKhachHang}";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["Error"] = "Tài khoản không tồn tại! Vui lòng liên hệ hỗ trợ.";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    // Email chưa tồn tại - Tạo tài khoản mới
                    var khachHang = new KhachHang
                    {
                        TenKhachHang = name ?? email.Split('@')[0],
                        EmailCuaKhachHang = email,
                        SDT = "0000000000", // Set default phone number
                        NgayTaoTaiKhoan = DateTime.Now,
                        TrangThai = 1
                    };
                    await _khachHangService.AddKhachHangAsync(khachHang);

                    var taiKhoan = new TaiKhoan
                    {
                        UserName = email,
                        Password = Guid.NewGuid().ToString(),
                        NgayTaoTaiKhoan = DateTime.Now,
                        TrangThai = true,
                        KhachHangId = khachHang.KhachHangId
                    };
                    await _taiKhoanService.AddAsync(taiKhoan);

                    // Lưu session
                    HttpContext.Session.SetString("TaiKhoanId", taiKhoan.TaiKhoanId.ToString());
                    HttpContext.Session.SetString("Role", "KhachHang");
                    HttpContext.Session.SetString("HoTen", khachHang.TenKhachHang);

                    // Commit session để đảm bảo được lưu
                    await HttpContext.Session.CommitAsync();

                    TempData["Success"] = $"Đăng ký Google thành công! Chào mừng {khachHang.TenKhachHang} đến với FurryFriends!";
                    return RedirectToAction("Index", "Home");
                }

                TempData["Error"] = "Có lỗi xảy ra trong quá trình xử lý! Vui lòng thử lại.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Đã xảy ra lỗi: {ex.Message}. Vui lòng thử lại sau.";
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
                    TempData["Error"] = "Đăng nhập Facebook thất bại! Vui lòng thử lại.";
                    return RedirectToAction("Index");
                }

                var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims;
                var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var picture = claims?.FirstOrDefault(c => c.Type == "urn:facebook:picture")?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    TempData["Error"] = "Không thể lấy thông tin email từ Facebook! Vui lòng thử lại.";
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
                        SDT = "0000000000", // Set default phone number
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

                TempData["Error"] = "Có lỗi xảy ra trong quá trình xử lý! Vui lòng thử lại.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Đã xảy ra lỗi: {ex.Message}. Vui lòng thử lại sau.";
                return RedirectToAction("Index");
            }
        }
    }
}
