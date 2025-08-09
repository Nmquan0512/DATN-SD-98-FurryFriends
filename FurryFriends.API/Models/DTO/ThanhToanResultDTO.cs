using System.Text.Json.Serialization;

namespace FurryFriends.API.Models.DTO
{
    public class ThanhToanResultDTO
    {
        [JsonPropertyName("hoaDonId")]
        public Guid HoaDonId { get; set; }

        [JsonPropertyName("tenCuaKhachHang")]
        public string TenCuaKhachHang { get; set; }

        [JsonPropertyName("sdtCuaKhachHang")]
        public string SdtCuaKhachHang { get; set; }

        [JsonPropertyName("emailCuaKhachHang")]
        public string EmailCuaKhachHang { get; set; }

        [JsonPropertyName("diaChiCuaKhachHang")]
        public string DiaChiCuaKhachHang { get; set; }

        [JsonPropertyName("ngayTao")]
        public DateTime NgayTao { get; set; }

        [JsonPropertyName("hinhThucThanhToan")]
        public string HinhThucThanhToan { get; set; }

        [JsonPropertyName("ghiChu")]
        public string GhiChu { get; set; }

        [JsonPropertyName("tongTien")]
        public decimal TongTien { get; set; }
        public decimal TongTienSauKhiGiam { get; set; }

        [JsonPropertyName("chiTietSanPham")]
        public List<HoaDonChiTietOutputDTO> ChiTietSanPham { get; set; }

    }
}
