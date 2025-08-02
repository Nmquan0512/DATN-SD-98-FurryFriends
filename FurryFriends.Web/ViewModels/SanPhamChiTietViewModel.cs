namespace FurryFriends.Web.ViewModels
{
    public class SanPhamChiTietViewModel
    {
        public Guid SanPhamChiTietId { get; set; }

        public string TenSanPham { get; set; } = "";
        public string MauSac { get; set; } = "";
        public string KichCo { get; set; } = "";
        public int SoLuongTon { get; set; }
        public decimal GiaBan { get; set; }
        public List<string> DanhSachAnh { get; set; } = new(); // Url ảnh để hiển thị
    }

}
