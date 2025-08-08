using System;
using System.Collections.Generic;

namespace FurryFriends.API.Models.DTO.BanHang
{
    public class HoaDonBanHangDto
    {
        public Guid HoaDonId { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime NgayTao { get; set; }
        public KhachHangDto KhachHang { get; set; }
        public List<ChiTietHoaDonDto> ChiTietHoaDon { get; set; }
        public decimal TongTien { get; set; }
        public decimal TienGiam { get; set; }
        public decimal ThanhTien { get; set; }
        public VoucherDto Voucher { get; set; }
        public HinhThucThanhToanDto HinhThucThanhToan { get; set; }
        public string TrangThai { get; set; }
    }

    public class ChiTietHoaDonDto
    {
        public Guid SanPhamChiTietId { get; set; }
        public string TenSanPham { get; set; }
        public string MauSac { get; set; }
        public string KichCo { get; set; }
        public decimal Gia { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien { get; set; }
    }
}