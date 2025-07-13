using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.API.Models
{
    public class GioHangChiTiet
    {
        [Key]
        public Guid GioHangChiTietId { get; set; }

        [Required]
        public Guid GioHangId { get; set; }

        [Required]
        public Guid SanPhamId { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải >= 1")]
        public int SoLuong { get; set; }

        [Required(ErrorMessage = "Đơn giá không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá không được âm")]
        public decimal DonGia { get; set; }

        [Required(ErrorMessage = "Thành tiền không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Thành tiền không được âm")]
        public decimal ThanhTien { get; set; }

        [Required]
        public bool TrangThai { get; set; }

        [Required]
        public DateTime NgayTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        [ForeignKey("GioHangId")]
        public virtual GioHang GioHang { get; set; }

        [ForeignKey("SanPhamId")]
        public virtual SanPham SanPham { get; set; }
    }
}