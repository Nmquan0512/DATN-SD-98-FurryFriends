namespace FurryFriends.API.Models.DTO
{
    public class GioHangChiTietDTO
    {
        public Guid GioHangChiTietId { get; set; }
        public Guid SanPhamId { get; set; }
        public string TenSanPham { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }

        public string HinhAnh { get; set; } // dòng này thêm nhưng chưa dùng đến
        public Guid SanPhamChiTietId { get; set; }
        public string? AnhSanPham { get; set; }
        public string MauSac { get; set; } = "";
        public string KichCo { get; set; } = "";
    }
}
