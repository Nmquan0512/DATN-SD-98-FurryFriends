namespace FurryFriends.API.Models
{
    public class Voucher
    {
        public Guid VoucherId { get; set; }
        public string TenVoucher { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public int PhanTram { get; set; }
        public string TrangThai { get; set; }
        public int SoLuong { get; set; }
        public Guid MaTaiKhoan { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }

        public virtual ICollection<HoaDon> HoaDons { get; set; }
    }
}
