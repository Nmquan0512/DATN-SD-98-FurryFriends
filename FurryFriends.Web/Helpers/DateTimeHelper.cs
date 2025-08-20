using System;

namespace FurryFriends.Web.Helpers
{
    public static class DateTimeHelper
    {
        // Helper method để hiển thị thời gian một cách nhất quán
        // Chuyển đổi từ UTC sang giờ Việt Nam (UTC+7)
        public static string FormatDateTime(DateTime dateTime)
        {
            // Chuyển từ UTC sang giờ Việt Nam
            var vietnamTime = dateTime.ToLocalTime();
            return vietnamTime.ToString("dd/MM/yyyy HH:mm");
        }
        
        public static string FormatDateTimeWithSeconds(DateTime dateTime)
        {
            // Chuyển từ UTC sang giờ Việt Nam
            var vietnamTime = dateTime.ToLocalTime();
            return vietnamTime.ToString("dd/MM/yyyy HH:mm:ss");
        }
        
        public static string FormatDateTimeWithLuc(DateTime dateTime)
        {
            // Chuyển từ UTC sang giờ Việt Nam
            var vietnamTime = dateTime.ToLocalTime();
            return vietnamTime.ToString("dd/MM/yyyy 'lúc' HH:mm");
        }
    }
}
