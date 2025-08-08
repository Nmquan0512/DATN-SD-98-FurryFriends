using System;
using System.Collections.Generic;

namespace FurryFriends.API.Models.DTO.BanHang
{
    public class SanPhamBanHangDto
    {
        public Guid SanPhamChiTietId { get; set; }
        public string TenSanPham { get; set; }
        public string TenMauSac { get; set; }
        public string TenKichCo { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
        public int SoLuongTon { get; set; }
    }
}