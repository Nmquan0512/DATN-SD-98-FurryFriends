using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FurryFriends.Web.Filter
{
    public class AuthorizeAdminAreaAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var area = context.RouteData.Values["area"]?.ToString();
            var role = context.HttpContext.Session.GetString("Role");
            
            // Nếu đang truy cập Area Admin
            if (area?.ToLower() == "admin")
            {
                // Kiểm tra xem có phải admin không
                if (string.IsNullOrEmpty(role) || role.ToLower() != "admin")
                {
                    var controller = (Controller)context.Controller;
                    controller.TempData["Error"] = "Bạn không có quyền truy cập khu vực quản trị. Chỉ admin mới có thể truy cập.";
                    context.Result = new RedirectToActionResult("DangNhap", "Auth", new { area = "" });
                    return;
                }
            }
            
            base.OnActionExecuting(context);
        }
    }
}
