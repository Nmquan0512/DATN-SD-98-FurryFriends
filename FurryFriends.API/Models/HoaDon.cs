namespace FurryFriends.API.Models
{
    public class HoaDon
    {
        public Guid HoaDonId { get; set; }

        public Guid TaiKhoanId { get; set; }
        public Guid? VoucherId { get; set; }
        public Guid KhachHangId { get; set; }
        public Guid HinhThucThanhToanId { get; set; }

        public string TenCuaKhachHang { get; set; }
        public string SdtCuaKhachHang { get; set; }
        public string EmailCuaKhachHang { get; set; }

        public DateTime NgayTao { get; set; }
        public DateTime? NgayNhanHang { get; set; }

        public decimal TongTienSauKhiGiam { get; set; }
        public string TrangThai { get; set; }
        public string GhiChu { get; set; }

        public virtual Voucher Voucher { get; set; }
        public virtual HinhThucThanhToan HinhThucThanhToan { get; set; }
        public virtual ICollection<HoaDonChiTiet> HoaDonChiTiets { get; set; }
    }
}
