using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.API.Models
{
    public class LichSuTrangThaiHoaDon
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid HoaDonId { get; set; }

        [Required]
        public int TrangThaiCu { get; set; }

        [Required]
        public int TrangThaiMoi { get; set; }

        [Required]
        public DateTime ThoiGianThayDoi { get; set; }

        public string? GhiChu { get; set; }

        public Guid? NhanVienId { get; set; }

        // Navigation properties
        [ForeignKey("HoaDonId")]
        public virtual HoaDon HoaDon { get; set; }

        [ForeignKey("NhanVienId")]
        public virtual NhanVien? NhanVien { get; set; }
    }
}
