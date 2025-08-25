using Microsoft.AspNetCore.Http;

namespace FurryFriends.Web.Helpers
{
    public static class PermissionHelper
    {
        public static bool IsAdmin(this ISession session)
        {
            var role = session.GetString("Role");
            return !string.IsNullOrEmpty(role) && role.ToLower() == "admin";
        }

        public static bool IsEmployee(this ISession session)
        {
            var role = session.GetString("Role");
            return !string.IsNullOrEmpty(role) && role.ToLower() == "nhanvien";
        }

        public static bool CanEdit(this ISession session)
        {
            return IsAdmin(session);
        }

        public static bool CanCreate(this ISession session)
        {
            return IsAdmin(session);
        }

        public static bool CanDelete(this ISession session)
        {
            return IsAdmin(session);
        }
    }
}
