using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FurryFriends.Web.Services.IService;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace FurryFriends.Web.Filter
{
    public class AccountStatusFilter : ActionFilterAttribute
    {
        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var taiKhoanId = httpContext.Session.GetString("TaiKhoanId");
            
            // Chỉ kiểm tra nếu đã đăng nhập
            if (!string.IsNullOrEmpty(taiKhoanId) && Guid.TryParse(taiKhoanId, out var id))
            {
                try
                {
                    // Lấy service từ DI container
                    var taiKhoanService = httpContext.RequestServices.GetService<ITaiKhoanService>();
                    if (taiKhoanService != null)
                    {
                        var taiKhoan = await taiKhoanService.GetByIdAsync(id);
                        
                        // Kiểm tra trạng thái tài khoản
                        if (taiKhoan != null && !taiKhoan.TrangThai)
                        {
                            // Tài khoản bị khóa - đăng xuất
                            httpContext.Session.Clear();
                            
                            var controller = (Controller)context.Controller;
                            controller.TempData["Error"] = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên để được hỗ trợ.";
                            
                            // Redirect về trang đăng nhập
                            context.Result = new RedirectToActionResult("DangNhap", "KhachHangLogin", new { area = "" });
                            return;
                        }
                        
                        // Kiểm tra trạng thái khách hàng/nhân viên liên kết
                        if (taiKhoan != null)
                        {
                            if (taiKhoan.KhachHang != null && taiKhoan.KhachHang.TrangThai != 1)
                            {
                                // Khách hàng bị khóa
                                httpContext.Session.Clear();
                                var controller = (Controller)context.Controller;
                                controller.TempData["Error"] = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên để được hỗ trợ.";
                                context.Result = new RedirectToActionResult("DangNhap", "KhachHangLogin", new { area = "" });
                                return;
                            }
                            
                            if (taiKhoan.NhanVien != null && !taiKhoan.NhanVien.TrangThai)
                            {
                                // Nhân viên bị khóa
                                httpContext.Session.Clear();
                                var controller = (Controller)context.Controller;
                                controller.TempData["Error"] = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên để được hỗ trợ.";
                                context.Result = new RedirectToActionResult("DangNhap", "KhachHangLogin", new { area = "" });
                                return;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log lỗi nhưng không block request
                    var logger = httpContext.RequestServices.GetService<ILogger<AccountStatusFilter>>();
                    logger?.LogError(ex, "Lỗi khi kiểm tra trạng thái tài khoản");
                }
            }
            
            base.OnActionExecuting(context);
        }
    }
}
