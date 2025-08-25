using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FurryFriends.Web.Filter
{
    public class AuthorizeEmployeeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || (role.ToLower() != "admin" && role.ToLower() != "nhanvien"))
            {
                var controller = (Controller)context.Controller;
                controller.TempData["Error"] = "Bạn không có quyền truy cập khu vực này. Vui lòng đăng nhập với tài khoản có quyền phù hợp.";
                context.Result = new RedirectToActionResult("DangNhap", "Auth", new { area = "" });
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}
