using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using FurryFriends.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Controllers
{
    [AllowAnonymous] // Quan trọng: Cho phép truy cập công khai
    public class AccountController : Controller
    {
        private readonly ITaiKhoanService _taiKhoanService;

        public AccountController(ITaiKhoanService taiKhoanService)
        {
            _taiKhoanService = taiKhoanService;
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var apiRequest = new ForgotPasswordRequest { Email = model.Email };
                await _taiKhoanService.ForgotPasswordAsync(apiRequest);

                // Lưu email vào session để sử dụng ở trang confirmation
                HttpContext.Session.SetString("ResetPasswordEmail", model.Email);

                return View("ForgotPasswordConfirmation");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // GET: /Account/ResetPassword
        // ĐIỀU CHỈNH: Thêm tham số 'email'
        [HttpGet]
        public IActionResult ResetPassword(string email, string code)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
            {
                // Có thể tạo một trang báo lỗi Error.cshtml thân thiện hơn
                return View("Error", "Đường dẫn đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.");
            }

            // Gán cả email và code vào model
            var model = new ResetPasswordViewModel { Email = email, Code = code };
            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var apiRequest = new ResetPasswordRequest
                {
                    Email = model.Email,
                    Code = model.Code,
                    NewPassword = model.NewPassword
                };
                await _taiKhoanService.ResetPasswordAsync(apiRequest);

                return View("ResetPasswordConfirmation");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }
    }
}