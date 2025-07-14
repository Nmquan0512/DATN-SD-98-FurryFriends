using System;

namespace FurryFriends.Web.Models
{
    public class DiaChiKhachHangViewModel
    {
        public Guid DiaChiId { get; set; }
        public string TenDiaChi { get; set; }
        public string ThanhPho { get; set; } // Tỉnh/Thành phố
        public string PhuongXa { get; set; } // Phường/Xã/Thị trấn
        public string MoTa { get; set; }
        public string SoDienThoai { get; set; }
        public bool LaMacDinh { get; set; }
        public string? GhiChu { get; set; }
        public string? ThanhPhoCode { get; set; }
        public string? PhuongXaCode { get; set; }
    }
} 