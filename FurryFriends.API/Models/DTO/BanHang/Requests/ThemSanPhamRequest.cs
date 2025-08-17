using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models.DTO.BanHang.Requests
{
    public class ThemSanPhamRequest
    {
        [Required]
        public Guid SanPhamChiTietId { get; set; }
        [Range(1, 1000)]
        public int SoLuong { get; set; }
    }

    public class CapNhatSoLuongRequest
    {
        [Range(0, 1000, ErrorMessage = "Số lượng phải là một số không âm.")]
        public int SoLuongMoi { get; set; }
    }

    public class GanKhachHangRequest
    {
        [Required]
        public Guid KhachHangId { get; set; }
    }
}
