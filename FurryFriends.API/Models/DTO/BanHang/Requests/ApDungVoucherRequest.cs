using System;

namespace FurryFriends.API.Models.DTO.BanHang.Requests
{
    public class ApDungVoucherRequest
    {
        public Guid HoaDonId { get; set; }
        public string MaVoucher { get; set; } // Có thể dùng mã thay vì ID
    }
}