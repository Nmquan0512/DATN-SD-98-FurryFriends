using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.API.Models
{
    public class DotGiamGiaSanPham
    {
        [Key]
        public Guid DotGiamGiaSanPhamId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid GiamGiaId { get; set; }

        [Required]
        public Guid SanPhamChiTietId { get; set; }

        [Required]
        public decimal PhanTramGiamGia { get; set; }

        [Required]
        public bool TrangThai { get; set; } = true;

        [Required]
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime NgayCapNhat { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("GiamGiaId")]
        public virtual GiamGia GiamGia { get; set; }

        [ForeignKey("SanPhamChiTietId")]
        public virtual SanPhamChiTiet SanPhamChiTiet { get; set; }
    }
}