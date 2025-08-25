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
        public string LoaiHoaDon { get; set; }
        
        // ✅ Thêm các trường snapshot khách hàng
        public string TenCuaKhachHang { get; set; }
        public string SdtCuaKhachHang { get; set; }
        public string EmailCuaKhachHang { get; set; }
        
        // ✅ Thêm địa chỉ giao hàng lúc mua
        public string DiaChiGiaoHangLucMua { get; set; }
    }

    public class ChiTietHoaDonDto
    {
        public Guid SanPhamChiTietId { get; set; }
        public string TenSanPham { get; set; }
        public string MauSac { get; set; }
        public string KichCo { get; set; }

        public decimal Gia { get; set; }      // Giá gốc của sản phẩm tại thời điểm thêm
        public decimal GiaBan { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien { get; set; }
        public int SoLuongTon { get; set; }
        public string HinhAnh { get; set; }
    }
}