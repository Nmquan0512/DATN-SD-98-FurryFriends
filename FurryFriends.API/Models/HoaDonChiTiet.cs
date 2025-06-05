namespace FurryFriends.API.Models
{
    public class HoaDonChiTiet
    {
        public Guid HoaDonChiTietId { get; set; }
        public Guid SanPhamId { get; set; }
        public Guid HoaDonId { get; set; }
        public int SoLuongSanPham { get; set; }
        public decimal Gia { get; set; }

        public virtual HoaDon HoaDon { get; set; }
    }
}
