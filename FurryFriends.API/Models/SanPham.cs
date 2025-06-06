using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.API.Models
{
    public class SanPham
    {
        [Key]
        public Guid SanPhamId { get; set; }

        [Required]
        public string TenSanPham { get; set; }

        [Required]
        public Guid TaiKhoanId { get; set; }

        [Required]
        public Guid ThuongHieuId { get; set; }

        [Required]
        public bool TrangThai { get; set; }

        [ForeignKey("TaiKhoanId")]
        public virtual TaiKhoan TaiKhoan { get; set; }

        [ForeignKey("ThuongHieuId")]
        public virtual ThuongHieu ThuongHieu { get; set; }

        // Navigation properties
        public virtual ICollection<DotGiamGiaSanPham> DotGiamGiaSanPhams { get; set; }
        public virtual ICollection<SanPhamChiTiet> SanPhamChiTiets { get; set; }
        public virtual ICollection<SanPhamThanhPhan> SanPhamThanhPhans { get; set; }
        public virtual ICollection<SanPhamChatLieu> SanPhamChatLieus { get; set; }
        public ICollection<GioHangChiTiet> GioHangChiTiets { get; set; }
        public ICollection<HoaDonChiTiet> HoaDonChiTiets { get; set; }

    }

}
