using System;

namespace FurryFriends.API.Models.DTO.BanHang.Requests
{
    public class ThanhToanRequest
    {
        public Guid HoaDonId { get; set; }
        public Guid HinhThucThanhToanId { get; set; }
        public Guid? VoucherId { get; set; }

        // Chỉ dùng khi thanh toán tiền mặt
        public decimal TienKhachDua { get; set; } = 0;

        // Thêm thông tin ghi chú thanh toán
        public string GhiChuThanhToan { get; set; }
    }
}