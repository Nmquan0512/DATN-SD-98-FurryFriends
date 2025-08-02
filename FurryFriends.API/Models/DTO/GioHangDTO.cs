using Newtonsoft.Json;

namespace FurryFriends.API.Models.DTO
{
    public class GioHangDTO
    {
        [JsonProperty("gioHangId")]
        public Guid GioHangId { get; set; }
        [JsonProperty("gioHangChiTiets")]
        public List<GioHangChiTietDTO> GioHangChiTiets { get; set; }

        // Alias cho ChiTietGioHang
        public List<GioHangChiTietDTO> ChiTietGioHang => GioHangChiTiets;

        // Tính tổng tiền tự động
        public decimal TongTien => GioHangChiTiets?.Sum(ct => ct.ThanhTien) ?? 0;
        public Guid KhachHangId { get; set; }
        public DateTime NgayTao { get; set; }       // Thêm dòng này
        public int TrangThai { get; set; }          // Thêm dòng này
        public decimal TongTienSauGiam { get; set; }
    }

}
