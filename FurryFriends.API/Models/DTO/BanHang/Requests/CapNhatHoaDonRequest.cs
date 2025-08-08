using System;

namespace FurryFriends.API.Models.DTO.BanHang.Requests
{
    public class CapNhatHoaDonRequest
    {
        public Guid HoaDonId { get; set; }
        public string GhiChu { get; set; }
        public Guid? KhachHangId { get; set; } // Cập nhật khách hàng nếu cần
    }
}