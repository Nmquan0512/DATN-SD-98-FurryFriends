using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
    public class HoaDonChiTiet
    {
        [Key]
        public Guid HoaDonChiTietId { get; set; }

        [Required]
        public Guid SanPhamChiTietId { get; set; }

        [Required]
        public Guid HoaDonId { get; set; }

        [Required(ErrorMessage = "Số lượng sản phẩm là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng sản phẩm phải lớn hơn 0")]
        public int SoLuongSanPham { get; set; }

        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá không được âm")]
        public decimal Gia { get; set; }

        [ForeignKey("HoaDonId")]
        public virtual HoaDon HoaDon { get; set; }

        [ForeignKey("SanPhamChiTietId")]
        public virtual SanPhamChiTiet SanPhamChiTiet { get; set; }

    }
}