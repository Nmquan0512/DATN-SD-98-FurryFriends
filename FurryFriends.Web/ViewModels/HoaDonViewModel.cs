namespace FurryFriends.Web.ViewModels
{
    public class HoaDonViewModel
    {
        public Guid HoaDonId { get; set; }
        public DateTime NgayTao { get; set; }
        public string TenKhachHang { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
        public string SDT { get; set; }
        public decimal TongTien { get; set; }
        public List<ChiTietHoaDonViewModel> ChiTietHoaDon { get; set; }
    }

}
