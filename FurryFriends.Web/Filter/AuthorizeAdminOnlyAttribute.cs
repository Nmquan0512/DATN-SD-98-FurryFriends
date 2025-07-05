using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Filter
{
	public class AuthorizeAdminOnlyAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			var role = context.HttpContext.Session.GetString("Role");

			if (string.IsNullOrEmpty(role))
			{
				// Chưa đăng nhập
				context.Result = new RedirectToActionResult("DangNhap", "Auth", new { area = "" });
				return;
			}

			if (role == "NhanVien")
			{
				// Đăng nhập là nhân viên nhưng cố truy cập trang Admin
				context.HttpContext.Session.SetString("AccessDenied", "Bạn không có quyền truy cập vào chức năng này!");
				context.Result = new RedirectToActionResult("Index", "HoaDon", new { area = "Admin" });
				return;
			}

			// Nếu là Admin => cho đi tiếp
			base.OnActionExecuting(context);
		}
	}
}
