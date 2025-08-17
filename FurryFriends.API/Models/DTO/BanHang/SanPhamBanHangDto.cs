// File: API/Models/DTO/BanHang/SanPhamBanHangDto.cs

namespace FurryFriends.API.Models.DTO.BanHang
{
    public class SanPhamBanHangDto
    {
        public Guid SanPhamChiTietId { get; set; }
        public string TenSanPham { get; set; }
        public string TenMauSac { get; set; }
        public string TenKichCo { get; set; }

        // <<< THỐNG NHẤT LẠI TÊN THEO YÊU CẦU >>>
        public decimal Gia { get; set; }      // Giá gốc của sản phẩm
        public decimal GiaBan { get; set; }   // Giá bán thực tế (có thể bằng giá gốc hoặc đã giảm)

        public int SoLuongTon { get; set; }
        public string HinhAnh { get; set; }
    }
}