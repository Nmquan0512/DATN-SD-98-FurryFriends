using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.API.Models
{
    public class DiaChiKhachHang
    {
        [Key]
        public Guid DiaChiId { get; set; }

        [Required(ErrorMessage = "Tên địa chỉ không được để trống")]
        [MaxLength(200, ErrorMessage = "Tên địa chỉ tối đa 200 ký tự")]
        public string TenDiaChi { get; set; }

        [Required(ErrorMessage = "Thành phố không được để trống")]
        [MaxLength(50)]
        public string ThanhPho { get; set; }

        [Required(ErrorMessage = "Quận/Huyện không được để trống")]
        [MaxLength(50)]
        public string QuanHuyen { get; set; }

        [Required(ErrorMessage = "Phường/Xã không được để trống")]
        [MaxLength(50)]
        public string PhuongXa { get; set; }

        public string MoTa { get; set; }

        public int TrangThai { get; set; }

        [Required]
        public DateTime NgayTao { get; set; }

        [Required]
        public DateTime NgayCapNhat { get; set; }

        [Required]
        public Guid KhachHangId { get; set; }

        [ForeignKey("KhachHangId")]
        public virtual KhachHang? KhachHang { get; set; }
    }
}