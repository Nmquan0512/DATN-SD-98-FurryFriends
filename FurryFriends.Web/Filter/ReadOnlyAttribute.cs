using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FurryFriends.Web.Filter
{
    public class ReadOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetString("Role");
            if (role?.ToLower() == "nhanvien")
            {
                var controller = (Controller)context.Controller;
                controller.TempData["Warning"] = "Bạn chỉ có quyền xem thông tin. Không thể thực hiện thao tác chỉnh sửa.";
                context.Result = new RedirectToActionResult("Index", context.RouteData.Values["controller"].ToString(), new { area = "Admin" });
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}
