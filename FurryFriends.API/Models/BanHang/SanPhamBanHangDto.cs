using System;
using System.Collections.Generic;

namespace FurryFriends.API.Models.DTO.BanHang
{
    public class SanPhamBanHangDto
    {
        public Guid SanPhamId { get; set; }
        public string TenSanPham { get; set; }
        public string MaSanPham { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuongTon { get; set; }
        public List<BienTheSanPhamDto> BienThes { get; set; }
    }

    public class BienTheSanPhamDto
    {
        public Guid SanPhamChiTietId { get; set; }
        public string TenMauSac { get; set; }
        public string TenKichCo { get; set; }
        public int SoLuong { get; set; }
    }
}