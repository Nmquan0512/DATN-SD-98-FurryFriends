using System;

namespace FurryFriends.Web.Helpers
{
    public static class DateTimeHelper
    {
        // Helper method để hiển thị thời gian một cách nhất quán
        // Thời gian trong database đã là local time (Việt Nam), không cần chuyển đổi
        public static string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm");
        }
        
        public static string FormatDateTimeWithSeconds(DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
        }
        
        public static string FormatDateTimeWithLuc(DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy 'lúc' HH:mm");
        }
    }
}
