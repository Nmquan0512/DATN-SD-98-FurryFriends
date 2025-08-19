using System;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models.DTO.BanHang.Requests
{
    public class ThanhToanRequest
    {
        public Guid HoaDonId { get; set; }
        
        [Required(ErrorMessage = "Hình thức thanh toán là bắt buộc")]
        public Guid HinhThucThanhToanId { get; set; }
        
        public Guid? VoucherId { get; set; }

        // Chỉ dùng khi thanh toán tiền mặt
        [Range(0, double.MaxValue, ErrorMessage = "Tiền khách đưa phải lớn hơn hoặc bằng 0")]
        public decimal TienKhachDua { get; set; } = 0;

        // Thêm thông tin ghi chú thanh toán
        public string? GhiChuThanhToan { get; set; }
    }
}