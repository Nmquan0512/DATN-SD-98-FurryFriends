using System;

namespace FurryFriends.API.Models.DTO.BanHang.Requests
{
    public class ApDungVoucherGioHangRequest
    {
        public Guid KhachHangId { get; set; }
        public Guid VoucherId { get; set; }
    }
} 